using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ManagementCenter
{
    /// <summary>
    /// obsluguje polecenia i wiadomosci jakie dostal agent
    /// </summary>
    class AgentSwitchingAction
    {
        internal static ObservableCollection<string> agentCollection = new ObservableCollection<string>();

        /// <summary>
        /// ma przechowywac polecenia jakie otrzymal od managera wyzej
        /// przychodzi polecenie i go tu zapisuje
        /// i potem jak idzie <order> tu polecenie </order> to wie ktorego polecenia sie to tyczy
        /// </summary>
        internal static BlockingCollection<string> requestCollection = new BlockingCollection<string>();

        static int[] messageData;

        internal static void AgentAction(string message, Manager manager,Agent agent)
        {
            //jezeli ma wyslac jeszcze dalej
            if (message.Contains("subsubnetwork"))
            {
                
            }

            //jezeli jest na samym dole hierarchi to nie ma juz wewnatrz podsicei
            else if (message.Contains("subnetwork"))
            {
                ConnectSubnetwork(message);
                Program.isTheBottonSub = true;
            }
            else if(message.Contains("connection"))
            {
                //dodajemy to do polecen jakie do Nas doszly
                requestCollection.Add(message);

                

                //jezeli jest to juz najnizsza podsiec to na jej poziomie juz konfigurujemy
                if(Program.isTheBottonSub==true)
                {
                    //format wiadomosci
                    //connection:<port_in>port</port_in><port_out>port<port_out>

                   messageData = GetStartAndEndNode(message);


                   var path= PathAlgorithm.dijkstra(Program.nodes, Program.links, messageData[0], messageData[1], false);
                    if(path.endToEnd)
                    {
                        //by byla tylko jedna sciezka ta globalna na ktorej pracujemy

                        SwitchingActions.pathToCount = path;
                        Console.WriteLine("Istnieje polaczenie EndToEnd");

                        //tu dodajemy do sciezki port na ktorej mamy z niej wyjechac i na ktory mamy wjechac
                        Link inPort= new Link(messageData[2]);
                        Link outPort = new Link(messageData[3]);
                        path.nodes.First().outputLink = outPort;
                        path.nodes.Last().inputLink = inPort;
                        //path.nodes.Last().outputLink = outPort;
                        //path.nodes.First().inputLink = inPort;

                        // XmlSerializer serializer = new XmlSerializer(typeof(bool[]));
                        //    bool[] a = new[] { true, false };
                        //  int[] numbers = new[] { 0, 1, 2, 3 };
                        // XmlSerializer serializer = new XmlSerializer(typeof(int[]));

                        string message1 = "<lenght>" + path.lenght + "</lenght>";

                        // string message1="<order>"+message+"</order><lenght>"+path.lenght+"</lenght>";
                        agent.Send(message1);

                        MemoryStream stream = new MemoryStream();
                        IFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, path.possibleWindow);

                       

                        agent.Send(stream);

                        //
                        //System.IO.MemoryStream stream = SerializeToStream(path);
                        // serializer.Serialize(numbers);
                        //var myString = serializer.Deserialize(numbers);
                    }

                }
                //TODO
                //w przeciwnym razie slemy nizej, czyli jak sa podspodem jeszcze inne polaczenia bedziemy musieli slac nizej
                else
                {

                }
            }
            else if(message.Contains("check"))
            {
                int[] data;
                data = GetStartAndAmountOfSlots(message);
                bool res;
                res=SwitchingActions.pathToCount.IsReservingWindowPossible(data[1], data[0]);
                if(res==true)
                {
                  //  string message1= "possible_path:"+agent.
                    //wysylamy info 
                    agent.Send("possible_path");
                }
                //jezeli nie bedzie mozliwa reserwacja trzeba bedzie to wyslac wyzej
                else
                {
                    Console.WriteLine("Nie uda sie zarezerwowac slotow");
                }
            }
            else if(message.Contains("reserve"))
            {
                int[] data;
                data = GetStartAndAmountOfSlots(message);
                SwitchingActions.pathToCount.ReserveWindow(data[1],data[0]);
                XMLeon xml = new XMLeon("path" + messageData[0] + messageData[1] + ".xml", XMLeon.Type.nodes);
                SwitchingActions.pathToCount.xmlName = ("path" + messageData[0] + messageData[1] + ".xml");
                xml.CreatePathXML(SwitchingActions.pathToCount);

                if (Program.isTheBottonSub == true)
                {
                    foreach(Manager nod in Program.managerNodes)
                    {
                        Console.WriteLine(nod.number);
                    }
                    foreach(Node node in SwitchingActions.pathToCount.nodes)
                    {
                        string message1 = xml.StringNode(node.number);
                        Console.WriteLine(message1);
                        try
                        {
                            Console.WriteLine(node.number);
                            Program.managerNodes.Find(x=>x.number==node.number).Send(message1);
                        }
                        catch
                        {
                            Console.WriteLine("Nie udalo sie wyslac sciezki do wezla");
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns>start slot, amountOfSlots</returns>
        static int[] GetStartAndAmountOfSlots(string message)
        {
            int[] result= new int[2];


            int start, end;
            
            int startSlot, amountOfSlots;
            start = message.IndexOf("<start_slot>") + 12;
            end = message.IndexOf("</start_slot>");
            startSlot = Int32.Parse(message.Substring(start, end - start));

            start = message.IndexOf("<amount>") + 8;
            end = message.IndexOf("</amount>");
            amountOfSlots = Int32.Parse(message.Substring(start, end - start));

            result[0] = startSlot;
            result[1] = amountOfSlots;
            return result;
        }

        /// <summary>
        /// pobiera z wiadomosci info o rzadanym polaczeniu
        /// </summary>
        /// <param name="message"></param>
        /// <returns>
        /// 0 startNode
        /// 1 endNode
        /// 2 portIn
        /// 3 portOut
        /// </returns>
        static int[] GetStartAndEndNode(string message)
        {
            int[] result=new int[4];
            int start, end;
            int portIn, portOut;
            int startNode, endNode;
            start = message.IndexOf("<port_in>") + 9;
            end = message.IndexOf("</port_in>");
            portIn = Int32.Parse(message.Substring(start, end - start));

            start = message.IndexOf("<port_out>") + 10;
            end = message.IndexOf("</port_out>");
            portOut = Int32.Parse(message.Substring(start, end - start));

            startNode = portIn % 100 - 10;
            endNode = portOut / 100 - 10;

            result[0] = startNode;
            result[1] = endNode;
            result[2] = portIn;
            result[3] = portOut;

            Console.WriteLine("port_in:" + portIn + "  port_out:" + portOut);
            Console.WriteLine("start node:" + startNode + "  end node:" + endNode);

            return result;
        }

        /// <summary>
        /// odpowiada za polaczenie sie z tymi wszystkimi wezlami na samy dole
        /// gdy juz wiemy ze nasz manger jest tym najnizszym managerem
        /// 
        /// </summary>
        /// <param name="message"></param>
        static void ConnectSubnetwork(string message)
        {
            Console.WriteLine("Konfiguracja podsieci");
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Niepoprawny format wiadomosci, ex:" + ex.ToString());
            }
            XMLeonSubnetwork eonXml = new XMLeonSubnetwork(xmlDoc);
            string linkFile = eonXml.GetLinkFile();
            XMLParser xml = new XMLParser(linkFile);
            Program.links = xml.GetLinks();
            lock (Program.managerCloud)
            {
                Program.managerCloud.Send(XML.StringCableLinks(linkFile));

            }
            CLI.PrintConfigFilesSent();

            //laczenie sie z wezlami w podsieci
            lock (Program.managerNodes)
            {
                int i = 0;
                foreach (Node node in Program.nodes)
                {
                    Program.managerNodes.Add(new Manager(node.number));
                    Program.managerNodes[i].CreateSocket("127.0.4." + node.number, 11001);
                    while (true)
                    {
                        try
                        {
                            Program.managerNodes[i].Connect("127.0.3." + node.number, 11001);
                            break;
                        }
                        catch
                        {

                        }
                    }
                    i++;
                }


            }
        }


    }
}
