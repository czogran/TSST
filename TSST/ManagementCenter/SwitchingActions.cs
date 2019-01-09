using System;
using System.Collections.Concurrent;
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
        /// z zwiazku z tym ze zakladam ze komp jest szybszy od nas
        /// zakaldam ze na raz bedzie zestawiana tylko jedna sciezka
        /// co bardzo ulatwia sprawe
        ///ale w zupelnosci nie emuluje prawdziwego zachowania sie sieci
        /// </summary>
        internal static Path pathToCount;

        /// <summary>
        /// jakie jest mozliwe okno dla jakiejs podsieci, ktora je przesyla
        /// i jest w rzucane w ta zmienna
        /// a nizej aktualizowana jest sciezka
        /// i zliaczane czy doszlo to info od wszystkich podsieci
        /// </summary>
        public static bool[] possibleWindow;




        /// <summary>
        /// zlicza ile wiadomosci possible window przyszlo
        /// by jak doszlo od wszystkich pod sieci stwierdzic ze 
        ///albo mozna rezultat wyslac wyzej
        ///albo juz mozna wybrac okno
        /// </summary>
        static int messageCounterPossibleWindow;

        static int messageCounterPossibleReservation;

        static int amountOfSlots;
        static int[] window;
        /// <summary>
        /// przechowuje informacje o startowym i koncowym kliencie
        /// data[0] = askingClient;
        /// data[1] = targetClient;
        /// </summary>
        static int[] data;

        /// <summary>
        /// przelaczanie tego co ma zrobic manager
        /// w zaleznosci co do niego doszlo
        /// </summary>
        /// <param name="message"></param>
        /// <param name="manager"></param>
        internal static void Action(string message, Manager manager)
        {
            
            //jezeli ma zostac polaczenie w podsieci czyl
            if (message.Contains("subconection"))
            {
                Console.WriteLine("Prosba o zestawienie polaczenia w podsieci");
            }
            //ta wiadomosc moze przyjsc tylko do glownego managera
            //poniewaz do tych mniejszych zakladamy ze nie moga byc podlaczeni klijenci
            else if (message.Contains("connection:"))
            {
                //zerujemy licznik
                messageCounterPossibleWindow = 0;
                messageCounterPossibleReservation = 0;

                //data[0] = askingClient;
                //data[1] = targetClient;
                data = GetStartAndEndNode(message);

                    lock (Program.nodes)
                    {
                        lock (Program.links)
                        {
                            pathToCount = PathAlgorithm.dijkstra(Program.nodes, Program.links, data[0] + 80, data[1] + 80, false);
                        }
                    }
                SendToSubnetworks(pathToCount);             
            }
            //klijent prosi o usuniecie podsieci
            else if (message.Contains("delete"))
            {
                data = GetStartAndEndNode(message);
                string pathId= (data[0] + 80).ToString() + (data[1] + 80).ToString();
                pathToCount= Program.paths.Find(x => x.id == pathId);
                SendSubToDeleteConnection(pathToCount);
                pathToCount.ResetSlotReservation();
                lock(Program.paths)
                {
                    Program.paths.Remove(pathToCount);
                }

            }
           
            else if(message.Contains("lenght"))
            {
                int lenght = GetLenght(message);
                pathToCount.lenght += lenght;
            }
            else if(message.Contains("possible_window"))
            {
                pathToCount.ChangeWindow(possibleWindow);
                messageCounterPossibleWindow++;

                //jezeli jest to najwyza sciezka i doszly juz wszystkie wiadomosci
                //minus 2 jest, bo na samej gorze sa jeszcze klijenci ich nie uwzgledniamy
                if (Program.isTheTopSub && messageCounterPossibleWindow==pathToCount.nodes.Count-2)
                {
                     amountOfSlots = PathAlgorithm.AmountNeededSlots(pathToCount.lenght);
                    //returnWindow= new int[2] {startSlot,maxWindow };
                     window = pathToCount.FindMaxWindow();

                    bool isReservationPossible = pathToCount.IsReservingWindowPossible(amountOfSlots, window[0]);

                    if(isReservationPossible)
                    {
                        SendAskIfReservationIsPossible(window[0], amountOfSlots);
                    }
                    //to trzeba zrobic jakies inne polecania na zestawianie sciezko na okolo
                    else
                    {
                        Console.WriteLine("Nie mozna zestawic sciezki");
                    }
                 
                }
            }

            else if (message.Contains("possible_path"))
            {
                messageCounterPossibleReservation++;
                //jezeli jest na samym szczycie by wyslal nizej zadnia
                if(messageCounterPossibleReservation==pathToCount.nodes.Count-2 && Program.isTheTopSub==true)
                {

                    pathToCount.ReserveWindow(amountOfSlots, window[0]);

                    SendSubToReserveWindow(window[0], amountOfSlots);
                    //data[1] target client
                    SendClientsToReserveWindow(window[0], data[1]);


                    
                    XMLeon xml = new XMLeon("path" + data[0] + data[1] + ".xml", XMLeon.Type.nodes);
                    pathToCount.xmlName = "path" + data[0] + data[1] + ".xml";
                    xml.CreatePathXML(pathToCount);
                    
                    //dodawania sciezki do listy sciezek 
                    lock(Program.paths)
                    {
                        Program.paths.Add(pathToCount);
                    }
    
                }
            }    
        }



        static void SendClientsToReserveWindow(int startSlot, int targetClient)
        {
            //sprawdzic czy indeksowanie OK jest
            for (int i =  SwitchingActions.pathToCount.nodes.Count - 1; i >= 1; i--)
            {
                if (SwitchingActions.pathToCount.nodes[i].number > 80)
                {
                    string message1;
                    if (SwitchingActions.pathToCount.pathIsSet == true)
                    {
                        message1 = "<start_slot>" + SwitchingActions.pathToCount.startSlot + "</start_slot><target_client>" + targetClient + "</target_client>";
                    }
                    else
                    {
                        message1 = "zabraklo slotow";
                    }
                    try
                    {
                        Program.managerClient[SwitchingActions.pathToCount.nodes[i].number - 80 - 1].Send(message1);
                    }
                    catch
                    {
                        Console.WriteLine("Nie udalo sie wyslac sciezki do klijenta");
                    }
                }
            }
            }

        static void SendSubToReserveWindow(int startSlot, int amountOfSlots)
        {
            //sprawdzic czy indeksowanie OK jest
            for (int i = pathToCount.nodes.Count - 1; i >= 0; i--)
            {
                if (Program.isTheBottonSub == false && pathToCount.nodes[i].number < 80)
                {
                    string message1 = "reserve:<start_slot>" + startSlot + "</start_slot><amount>" + amountOfSlots + "</amount>";
                    lock (Program.subnetworkManager)
                    {
                        Program.subnetworkManager.Find(x => x.number == pathToCount.nodes[i].number).Send(message1);
                        Console.WriteLine("Wysylam do podsieci prosbe by zarezerwowala okna:" + pathToCount.nodes[i].number);
                    }
                }
            }
        }


        static void SendAskIfReservationIsPossible(int startSlot, int amountOfSlots)
        {
            //sprawdzic czy indeksowanie OK jest
            for (int i = pathToCount.nodes.Count - 1; i >= 0; i--)
            {
                if (Program.isTheBottonSub == false && pathToCount.nodes[i].number < 80)
                {
                    string message1 = "check:<start_slot>"+startSlot+"</start_slot><amount>"+amountOfSlots+"</amount>";
                    lock (Program.subnetworkManager)
                    {
                        Program.subnetworkManager.Find(x => x.number == pathToCount.nodes[i].number).Send(message1);
                        Console.WriteLine("Wysylam pytanie do podsieci czy sa w stanie zarezerwowac okna:" + pathToCount.nodes[i].number);
                    }
                }
            }
        }


        /// <summary>
        /// bierze dlugosc z wiadomosci ktora doszla od podsieci
        /// format wiadomosci
        /// <lenght>i tu jest dlugosc</lenght>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static int GetLenght(string message)
        {
            int lenght;
            int start, end;

            start = message.IndexOf("<lenght>")+8;
            end = message.IndexOf("</lenght>");

            lenght = Int32.Parse(message.Substring(start, end - start));

            return lenght;
        }

        static int[] GetStartAndEndNode(string message)
        {
            int[] result = new int[2];
            int askingClient, targetClient;
            int start, end;

            start = message.IndexOf("<target_client>") + 15;
            end = message.IndexOf("</target_client>");
           // end = message.IndexOf("<port>");
           //13 stad ze //connection: konczy sie na 13 znaku
           //targetClient = Int32.Parse(message.Substring(13, end - 13));
           targetClient = Int32.Parse(message.Substring(start, end - start));

            Console.WriteLine("target client" + targetClient);
            start = message.IndexOf("<my_id>") + 7;
            end = message.IndexOf("</my_id>");
            askingClient = Int32.Parse(message.Substring(start, end - start));
            result[0] = askingClient;
            result[1] = targetClient;

            return result;
        }

        /// <summary>
        /// wysylanie podsieciom wiadomosi o to jaka maja sciezeke zestawic
        /// </summary>
        /// <param name="path"></param>
        public static void SendToSubnetworks(Path path)
        {
            if (path.endToEnd == true)
            {

                //sprawdzic czy indeksowanie OK jest
                for (int i = path.nodes.Count - 1; i >= 0; i--)
                {
                    if (Program.isTheBottonSub == false && path.nodes[i].number < 80)
                    {
                        string message1 = "connection<port_in>" + path.nodes[i].inputLink.id + "</port_in><port_out>" + path.nodes[i].outputLink.id + "</port_out>";
                        lock (Program.subnetworkManager)
                        {
                            Program.subnetworkManager.Find(x => x.number == path.nodes[i].number).Send(message1);
                            Console.WriteLine("Wysylam zadanie do podsieci:" + path.nodes[i].number);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// wysyla zadanie podsieciom jaka sciezke maja wywalic
        /// </summary>
        /// <param name="path"></param>
        public static void SendSubToDeleteConnection(Path path)
        {
            for (int i = path.nodes.Count - 1; i >= 0; i--)
            {
                if (Program.isTheBottonSub == false && path.nodes[i].number < 80)
                {
                    string message1 = "delete<port_in>" + path.nodes[i].inputLink.id + "</port_in><port_out>" + path.nodes[i].outputLink.id + "</port_out>";
                    lock (Program.subnetworkManager)
                    {
                        Program.subnetworkManager.Find(x => x.number == path.nodes[i].number).Send(message1);
                        Console.WriteLine("Wysylam zadanie do podsieci:" + path.nodes[i].number);
                    }
                }
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
                        lock (Program.managerNodes)
                        {
                            try
                            {
                                Program.managerNodes[node.number - 1].Send(message1);
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
                                Program.managerNodes[pathForFunction.nodes[i].number - 1].Send(message1);
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
                        lock(Program.managerNodes)
                        {
                            if (node.number < 80)
                            {
                                string message1 = "remove:" + p.nodes.Last().number + p.nodes[0].number;
                                Program.managerNodes[node.number - 1].Send(message1);
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


        
    }
}
