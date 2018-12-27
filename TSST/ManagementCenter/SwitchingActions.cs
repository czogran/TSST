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
        internal static ObservableCollection<Tuple <int,string>> managerNodeCollection = new ObservableCollection<Tuple<int,string>>();
        internal static ObservableCollection<Tuple<int, string>> managerClientCollection = new ObservableCollection<Tuple<int, string>>();



        internal static void Action(string message, Manager manager)
        {
            if (message.Contains("connection:"))
            {
                Console.WriteLine("Prosba o zestawienie polaczenia");
                int askingClient, targetClient;
                int start, end;

                XMLeon xml;

                end = message.IndexOf("<port>");
                targetClient = Int32.Parse(message.Substring(13, end - 13));
                Console.WriteLine("target client" + targetClient);
                start = message.IndexOf("<my_id>") + 7;
                end = message.IndexOf("</my_id>");
                askingClient = Int32.Parse(message.Substring(start, end - start));

                Console.WriteLine("asking client" + askingClient);

                //tylko do testow
                /*  List<Node> nodesList = new List<Node>();
                  nodesList.Add(new Node(81));
                  nodesList.Add(new Node(82));
                  nodesList.Add(new Node(83));
                  nodesList.Add(new Node(84));
                  nodesList.Add(new Node(1));
                  nodesList.Add(new Node(2));
                  nodesList.Add(new Node(3));
                  nodesList.Add(new Node(4));
                  nodesList.Add(new Node(5));
                  nodesList.Add(new Node(6));
                  nodesList.Add(new Node(7));
                  nodesList.Add(new Node(8));
                  nodesList.Add(new Node(9));
                  nodesList.Add(new Node(10));
                  nodesList.Add(new Node(11));*/
                Path path;
                lock (Program.nodes)
                {
                  //  Console.WriteLine("aaaaaaaaaaaaaa");
                    lock (Program.links)
                    {
                       path = PathAlgorithm.dijkstra(Program.nodes, Program.links, askingClient + 80, targetClient + 80, false);
                       
                    }
                    
                }
                xml = new XMLeon(path.xmlName);
                //taka indkesacja, bo bierzemy od konca i nie potrzebujemy do odbiorcy niczego wysylac
                for(int i =path.nodes.Count-1; i>=1;i--)
                {
                    if(path.nodes[i].number>80)
                    {
                        // string message1 = "addConnection:start_slot" + path.startSlot + "/start_slot" + "target_client" + targetClient + "/target_client";
                        //string message1 = "aaaaaaaaa111111111111111111111111111111111111aaaaaaaaaaa";
                        string message1 = "<start_slot>" + path.startSlot + "</start_slot><target_client>"+targetClient+"</target_client>";
                        //var tup = new Tuple<int,string>(path.nodes[i].number-80,message1);
                        //string message1 = "<start_slot>" + path.startSlot + "</start_slot><target_client>"+targetClient+"</target_client>";
                       //string message1="aaaaa<start_slot>" + path.startSlot + "</start_slot>aaaaa<target_client>"+targetClient+"</target_client>aaa";
                        // managerClientCollection.Add(tup);
                       try
                        {
                            Program.managerClient[path.nodes[i].number - 80-1].Send(message1);
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
                //path.nodes;
               // var tup=new Tuple<path.>
            }
        }
    }
}
