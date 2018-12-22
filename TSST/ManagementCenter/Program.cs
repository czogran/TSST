using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    /// <summary>
    /// Główna klasa programu
    /// </summary>
    class Program
    {
        //do testownia xml
        static void Main(string[] args)
        {
            XMLeon client = new XMLeon("client.xml", XMLeon.Type.clients);
            client.AddClient(1, "127.0.0.1", 11);
            client.AddClient(2, "111", 33);
            client.AddClient(3, "127.0.0.1", 11);
            client.AddClient(4, "111", 33);
            client.AddClient(5, "127.0.0.1", 11);
            client.AddClient(6, "111", 33);
            client.RemoveClient(3);


            XMLeon nodes = new XMLeon("nodes.xml", XMLeon.Type.nodes);
            nodes.AddNode(1, "111", "3333");
            nodes.AddNode(2, "111", "3333");
            nodes.AddNode(3, "111", "3333");

            nodes.AddNode(4, "111", "3333");
            nodes.AddNode(5, "111e", "3333");
            nodes.AddNode(6, "11q1", "3333");
            nodes.AddNode(7, "11p1", "3333");
            nodes.RemoveNode(3);

            nodes.AddMatrix(3, 2);
            nodes.AddMatrix(3, 4);
            nodes.AddMatrix(11, 1);
            nodes.AddMatrix(13, 2);
            nodes.AddMatrix(23, 2);
            nodes.AddMatrix(3, 5);
            nodes.AddMatrix(23, 6);
            nodes.AddMatrix(93, 2);
            nodes.AddMatrix(31, 1);
            nodes.AddMatrix(3, 1);

            nodes.AddConnection(1, 11, 2, 4, 11);
            nodes.AddConnection(2, 93, 15, 4, 11);
            nodes.AddConnection(2, 13, 2, 4, 11);
            nodes.AddConnection(2, 13, 5, 4, 11);
            nodes.AddConnection(2, 13, 9, 4, 11);
            nodes.AddConnection(2, 13, 11, 4, 11);

            nodes.RemoveConnection(2, 13, 2);
            nodes.RemoveConnection(1, 11, 2);

            XMLeon links = new XMLeon("links.xml", XMLeon.Type.cable_cloud);

            links.AddLink(9111, 81, 1, "on", 1, 14);
            links.AddLink(9211, 82, 1, "on", 1, 14);

            links.AddLink(1112, 1, 2, "on", 1, 14);
            links.AddLink(1114, 1, 4, "on", 22, 14);

            links.AddLink(1215, 2, 5, "on", 22, 14);
            links.AddLink(1213, 2, 3, "on", 1, 14);

            links.AddLink(1316, 3, 6, "on", 1, 14);
            links.AddLink(1314, 3, 4, "on", 19, 14);

            links.AddLink(1417,4 , 7, "on", 22, 14);
            links.AddLink(1411, 4, 1, "on", 1, 14);

            links.AddLink(1516, 5, 6, "on", 22, 14);
            links.AddLink(1518, 5, 8, "on", 1, 14);

            links.AddLink(1617, 6, 7, "on", 22, 14);
            links.AddLink(1615, 6, 5, "on", 1, 14);

            links.AddLink(1719, 7, 9, "on", 22, 14);

            links.AddLink(1819, 8, 9, "on", 22, 14);

            links.AddLink(1920, 9, 10, "on", 22, 14);

            links.AddLink(2093, 10, 83, "on", 22, 14);
            links.AddLink(2094, 10, 84, "on", 22, 14);


            // links.AddLink(9111, 81, 1, "on", 22, 14);
            //links.AddLink(1112, 1, 2, "on", 22, 14);
            // links.AddLink(1213, 2, 3, "on", 22, 14);
            // links.AddLink(1392, 2, 82, "on", 22, 14);

            /* links.AddLink(1191, 1, 81, "on", 22, 14);
             links.AddLink(1211, 2, 1, "on", 22, 14);
             links.AddLink(1312, 3, 2, "on", 22, 14);
             links.AddLink(9213, 82, 3, "on", 22, 14);*/
            // links.RemoveLink(1115);
            //  links.ChangeLinkStatus(1112, "off");

            XMLParser test = new XMLParser("nodes.xml");
            //test.GetNodePorts(2);

            XMLParser test1 = new XMLParser("links.xml");
            //  test1.GetLinks();




            List<Link> linksList = test1.GetLinks();
            List<Node> nodesList = new List<Node>();
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
            nodesList.Add(new Node(11));

            PathAlgorithm.dijkstra(nodesList, linksList, 81, 83, false);

            Console.Read();
        }
    }
}

       /* /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        static void Main(string[] args)
        {
            Console.WriteLine("MANAGER");
            CLI.ClientNum(args[1]);
            int nodeAmount = int.Parse(args[0]);
            //string port;
            //string agent;

          
            int id;

            List<string> port = new List<string>();
            List<string> agent = new List<string>();
            List<Manager> manager = new List<Manager>();
            List<Manager> managerClient = new List<Manager>();

            Manager managerCloud = new Manager();
            managerCloud.CreateSocket("127.0.0.1", 11001);
            managerCloud.Connect("127.0.0.2", 11001);
       //     managerCloud.Send(XML.StringCableLinks());
            System.Threading.Thread.Sleep(100);
            managerCloud.Send("nodes:" + args[0]);
            System.Threading.Thread.Sleep(100);
       
            managerCloud.Send("clients:" + args[1]);
            System.Threading.Thread.Sleep(100);

            //manager create clients
            for (int i = 1; i <= Int32.Parse(args[1]); i++)
            {
                managerClient.Add(new Manager());
                Console.WriteLine("robie managera dla clienta"+ "127.0.12." + i.ToString());
               
                managerClient[i - 1].CreateSocket("127.0.13." + i.ToString(), 11001);
                managerClient[i - 1].Connect("127.0.12."+i.ToString(), 11001);
            }
         
            //managerCloud.Send("clients:" );


            for (int i=1;i<=nodeAmount;i++)
               {
                   id = 2 * i + 10;
                   port.Add ( "127.0.1." +i.ToString());
                   agent.Add( "127.0.3." + i.ToString());
                 
               }
            
            //miejsce na dodatkowe ustalanie polaczen
            int socket = 100;
            //XML.SetName("default5.xml");
            CLI.NodeNum(nodeAmount);
            for (int i = 1; i <= nodeAmount; i++)
            {
                manager.Add(new Manager());
                
              //  socket += i;
                manager[i - 1].CreateSocket("127.0.0.4" + i.ToString(), 11001);
                manager[i - 1].Connect(agent[i - 1], 11001);
            }
            string choose;
            while (true)
            {
                CLI.Prompt();

                choose = Console.ReadLine();
                if ( choose== "1")
                {
                    CLI.RequestXML();
                    do
                    {
                        var name = Console.ReadLine();
                        XML.SetName(name);
                    } while (XML.Test() != true);

                    for (int i = 1; i <= nodeAmount; i++)
                    { 
                        try
                        {
                            manager[i - 1].Send(XML.StringNode(i));
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    
                    for (int i = 1; i <= Int32.Parse(args[1]); i++)
                    {
                        try
                        {
                            managerClient[i - 1].Send(XML.StringClients());
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    managerCloud.Send(XML.StringCableLinks());
                    CLI.PrintConfigFilesSent();
                }
                else if (choose=="2")
                {
                    CLI.RequestXML();
                    do
                    {
                        var name = Console.ReadLine();
                        XML.SetName(name);
                    } while (XML.Test() != true);

                    for (int i = 1; i <= nodeAmount; i++)
                    {
                        try
                        {
                            manager[i - 1].Send(XML.StringNode(i));
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }
                }


            // Console.WriteLine(XML.StringNode(1));
            Console.ReadLine();
        }
    }
}
*/