using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    class SwitchingActions
    {
        //chyba sa do wywalki, TODO sprawdzic to
        internal static ObservableCollection<Tuple <int,string>> managerNodeCollection = new ObservableCollection<Tuple<int,string>>();
        internal static ObservableCollection<Tuple<int, string>> managerClientCollection = new ObservableCollection<Tuple<int, string>>();


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
            else if(message.Contains("delete"))
            {
                DeleteConnection(message);
            }
        }

        /// <summary>
        /// sluzy do usuniecia sciezki z listy i rozeslania wiadomosci do wezlow by wywalily je ze swojej pamieci
        /// </summary>
        /// <param name="message"></param>
        static void DeleteConnection(string message)
        {
            Console.WriteLine("Prosba o usuniecie polaczenia");
            int askingClient, targetClient;
            int start, end;

            XMLeon xml;

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
           
            }
            xml = new XMLeon(path.xmlName);
            //taka indkesacja, bo bierzemy od konca i nie potrzebujemy do odbiorcy niczego wysylac
            for (int i = path.nodes.Count - 1; i >= 1; i--)
            {
                if (path.nodes[i].number > 80)
                {
                    // string message1 = "addConnection:start_slot" + path.startSlot + "/start_slot" + "target_client" + targetClient + "/target_client";
                    //string message1 = "aaaaaaaaa111111111111111111111111111111111111aaaaaaaaaaa";
                    string message1;
                    if (path.pathIsSet == true)
                    {
                        message1 = "<start_slot>" + path.startSlot + "</start_slot><target_client>" + targetClient + "</target_client>";
                    }
                    else
                    {
                        message1 = "zabraklo slotow";
                    }
                    //var tup = new Tuple<int,string>(path.nodes[i].number-80,message1);
                    //string message1 = "<start_slot>" + path.startSlot + "</start_slot><target_client>"+targetClient+"</target_client>";
                    //string message1="aaaaa<start_slot>" + path.startSlot + "</start_slot>aaaaa<target_client>"+targetClient+"</target_client>aaa";
                    // managerClientCollection.Add(tup);
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
                    //var tup = new Tuple<int, string>(path.nodes[i].number - 80, message1);
                    // managerClientCollection.Add(tup);

                }
            }
            Console.WriteLine("Zakonczona obsluga zadania connection");
        }
    }
}
