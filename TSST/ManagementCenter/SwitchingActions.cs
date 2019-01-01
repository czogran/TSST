﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    class SwitchingActions
    {
        /// <summary>
        /// przelaczanie tego co ma zrobic manager
        /// w zaleznosci co do niego doszlo
        /// </summary>
        /// <param name="message"></param>
        /// <param name="manager"></param>
        internal static void Action(string message, Manager manager)
        {
            if (message.Contains("connection:"))
            {
                AddConnection(message);
            }
            else if (message.Contains("delete"))
            {
                DeleteConnection(message);
            }
        }


        /// <summary>
        /// gdy zdechnie wezel by od nowa zrekonfigurowac polaczenia i 
        /// rozsyla info wezlom co byly na poprzedniej sciezce by wywlaily co maja na nia
        /// potem wysyla klijentowi nowe info jak ma wysylac
        /// i wezlom co sa na tej nowej sciezce
        /// </summary>
        /// <param name="id"></param>
        internal static void NodeIsDead(int id)
        {
            string message1;
            var toReconfigure = Program.paths.FindAll(x => x.nodes.Contains(x.nodes.Find(y => y.number == id)));
            foreach (Path path in toReconfigure)
            {
                path.ResetSlotReservation();
            }
                foreach (Path path in toReconfigure)
            {
                System.Threading.Thread.Sleep(100);
                path.ResetSlotReservation();
                message1 = "remove:" + path.nodes.Last().number + path.nodes[0].number;
                foreach (Node node in path.nodes)
                {
                    if (node.number <= 80 && node.number != id)
                    {
                        lock (Program.manager)
                        {
                            try
                            {
                                Program.manager[node.number - 1].Send(message1);
                            }
                            catch
                            {
                                Console.WriteLine("Nie udalo sie automatycznie usunac wpisow");
                            }
                        }
                    }
                }

                Path pathForFunction;
                lock (Program.nodes)
                {
                    lock (Program.links)
                    {
                        pathForFunction = PathAlgorithm.dijkstra(Program.nodes, Program.links,path.nodes.Last().number, path.nodes.First().number,false);
                    }

                }

                if (pathForFunction.pathIsSet == true)
                {
                    lock (Program.paths)
                    {
                        try
                        {
                            //jezeli udalo sie zestawic nowe polaczenie to jest podmieniane
                            Program.paths[Program.paths.FindIndex(x => x == path)] = pathForFunction;
                            Console.WriteLine("Zamienilem sciezke");
                        }
                        catch
                        {
                            Console.WriteLine("Nie udalo sie zamienic sciezki");
                        }
                    }

               

                    var xml = new XMLeon(path.xmlName);

                    //rozeslanie informacji do klijenta wysylajacego o zmianie sciezki
                      var targetClient = pathForFunction.nodes.First().number - 80;
                      message1 = "replace:<start_slot>" + pathForFunction.startSlot + "</start_slot><target_client>" +targetClient + "</target_client>";
                          
                      try
                      {
                                
                       Program.managerClient[path.nodes.Last().number - 80 - 1].Send(message1);
                      }
                      catch(Exception ex)
                      {
                       Console.WriteLine("Nie udalo sie wyslac sciezki do klijenta, ex: "+ex.ToString());
                      }
                    //koniec rozsylo do klijenta

                    //taka indkesacja, bo bierzemy od konca i nie potrzebujemy do odbiorcy niczego wysylac
                    for (int i = pathForFunction.nodes.Count - 1; i >= 1; i--)
                    {
                        if (pathForFunction.nodes[i].number < 80)
           
                        {

                            message1 = xml.StringNode(pathForFunction.nodes[i].number);
                            Console.WriteLine(message1);
                            try
                            {
                                Program.manager[pathForFunction.nodes[i].number - 1].Send(message1);
                            }
                            catch
                            {
                                Console.WriteLine("Nie udalo sie wyslac sciezki do wezla");
                            }
                        }
                    }
                }
                else
                {
                    lock (Program.paths)
                    {
                        try
                        {
                            //jezeli nie udalo sie zestawic polaczenia to jest ono wywalane z listy polaczen
                            Program.paths.Remove(Program.paths.Find(x => x == path));
                        }
                        catch
                        {
                            Console.WriteLine("Nie udalo sie wywalic sciezki");
                        }
                    }
                }
            }
        }
    
   


        /// <summary>
        /// gdy client prosi o usuniecie polaczenia
        /// sluzy do usuniecia sciezki z listy gdy jest o to prosba i rozeslania wiadomosci do wezlow by wywalily je ze swojej pamieci
        /// </summary>
        /// <param name="message"></param>
        static void DeleteConnection(string message)
        {
            Console.WriteLine("Prosba o usuniecie polaczenia");
            int askingClient, targetClient;
            int start, end;


            end = message.IndexOf("<port>");
            //9 stad ze //delete: konczy sie na 9 znaku
            targetClient = Int32.Parse(message.Substring(9, end - 9));
            Console.WriteLine("target client" + targetClient);
            start = message.IndexOf("<my_id>") + 7;
            end = message.IndexOf("</my_id>");
            askingClient = Int32.Parse(message.Substring(start, end - start));

            Console.WriteLine("asking client" + askingClient);

            string id = (askingClient + 80).ToString() + (targetClient + 80).ToString();
            Path p;
            try

            {
             p = Program.paths.Find(x => x.id == id);
                Console.WriteLine(p.id);
                try
                {
                    //zwalnianie linkow
                    p.ResetSlotReservation();
                    foreach(Node node in p.nodes)
                    {
                        lock(Program.manager)
                        {
                            if (node.number < 80)
                            {
                                string message1 = "remove:" + p.nodes.Last().number + p.nodes[0].number;
                                Program.manager[node.number - 1].Send(message1);
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("nie udalo sie wyslac prosb o usuniecie wpisow");
                }

                try
                {

                    lock (Program.paths)
                    {

                        Program.paths.Remove(p);
                       // Console.WriteLine("cc");

                    }
                }
                catch
                {
                    Console.WriteLine("Nie udało sie usunac ");
                }
            }
            catch
            {
                Console.WriteLine("nie znaleziono takiej sciezki do usuniecia");
            }
            Console.WriteLine("Polecenie usuniecia sciezki obsluzone");
        }


        /// <summary>
        /// gdy klijent prosi o zestawienie polaczenua jest wywolywana ta funkcja
        /// wywoluje ona ustawianie sciezki
        /// i potem rozsyla istotne informacje dalej
        /// </summary>
        /// <param name="message"></param>
        static void AddConnection(string message)
        {
            Console.WriteLine("Prosba o zestawienie polaczenia");
            int askingClient, targetClient;
            int start, end;

            XMLeon xml;

            end = message.IndexOf("<port>");
            //13 stad ze //connection: konczy sie na 13 znaku
            targetClient = Int32.Parse(message.Substring(13, end - 13));
            Console.WriteLine("target client" + targetClient);
            start = message.IndexOf("<my_id>") + 7;
            end = message.IndexOf("</my_id>");
            askingClient = Int32.Parse(message.Substring(start, end - start));

            Console.WriteLine("asking client" + askingClient);


            Path path;
            lock (Program.nodes)
            {
                lock (Program.links)
                {
                    //plus 80 bo taka glupia konwencje dalem ze klijenty to nody o numerach od 80 dla algo
                    path = PathAlgorithm.dijkstra(Program.nodes, Program.links, askingClient + 80, targetClient + 80, false);          
                }

            }

            if (path.pathIsSet == true)
            {
                lock (Program.paths)
                {
                    try
                    {
                        Program.paths.Add(path);
                    }
                    catch
                    {
                        Console.WriteLine("Nie no");
                    }
                }


                xml = new XMLeon(path.xmlName);
                //taka indkesacja, bo bierzemy od konca i nie potrzebujemy do odbiorcy niczego wysylac
                for (int i = path.nodes.Count - 1; i >= 1; i--)
                {
                    if (path.nodes[i].number > 80)
                    {
                        string message1;
                        if (path.pathIsSet == true)
                        {
                            message1 = "<start_slot>" + path.startSlot + "</start_slot><target_client>" + targetClient + "</target_client>";
                        }
                        else
                        {
                            message1 = "zabraklo slotow";
                        }
                        try
                        {
                            Program.managerClient[path.nodes[i].number - 80 - 1].Send(message1);
                        }
                        catch
                        {
                            Console.WriteLine("Nie udalo sie wyslac sciezki do klijenta");
                        }
                    }
                    else
                    {

                        string message1 = xml.StringNode(path.nodes[i].number);
                        Console.WriteLine(message1);
                        try
                        {
                            Program.manager[path.nodes[i].number - 1].Send(message1);
                        }
                        catch
                        {
                            Console.WriteLine("Nie udalo sie wyslac sciezki do wezla");
                        }
                   
                    }
                }
            }
            Console.WriteLine("Zakonczona obsluga zadania connection");
        }
    }
}
