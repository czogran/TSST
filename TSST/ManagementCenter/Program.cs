using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManagementCenter
{
    /// <summary>
    /// Główna klasa programu
    /// </summary>
    class Program
    {
        public static int amountOfnodes;
        public static int amountOfclients;
        public static int amountOfSubnetworks;

        public static List<Link> links;
        // tu w ich dodawaniu bedzie jeden wielki cheat bo bedzie szlo ono po wezlach jakie sa w linkach
        public static List<Node> nodes;
        public static List<Path> paths=new List<Path>();
        public static List<Subnetwork> subnetworksList = new List<Subnetwork>();


        public static List<Manager> manager;
        public static List<Manager> managerClient= new List<Manager>();
        public static Manager managerCloud;


        public static List<Manager> subnetworkManager;

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        static void Main(string[] args)
        {
            //w tescie sie tworzy te sieci i inne xml, wiec raz odpalic
            //a potem zakomentowac
            //i znowu skompilowac

            //Tests.TestXML();
            Console.WriteLine("MANAGER:"+args[0]);
            subnetworkManager = new List<Manager>();

            amountOfnodes = Int32.Parse(args[1]);
            amountOfclients = Int32.Parse(args[2]);
            amountOfSubnetworks = Int32.Parse(args[3]);




            XMLeonSubnetwork test = new XMLeonSubnetwork("test.xml","client.xml","links.xml");

            List<int> links = new List<int>();
            List<int> nodes = new List<int>();
            List<int> links1 = new List<int>();
            List<int> nodes1 = new List<int>();
            links.Add(1);
            links.Add(3);
            nodes.Add(4);

            test.AddSubnetwork(3,"11111", "linkxd");
            test.AddSubnetwork(2, "1.1.1.1",links1 ,nodes1);
            test.AddSubSubNetwork(2,4, "3.3.3", links, nodes);
            test.AddSubnetwork(3, "33333", links, nodes);

            test.GetSubnetworks();

         
            Agent agent;

            if (args[0]=="1")
            {
                Console.WriteLine("Centralny Manager");
                managerCloud = new Manager();
                managerCloud.CreateSocket("127.0.0.1", 11001);

               // System.Threading.Thread.Sleep(100);
                managerCloud.Connect("127.0.0.2", 11001);


                  for(int i=2; i<=amountOfSubnetworks;i++)
                  {
                      subnetworkManager.Add(new Manager(i));
                      subnetworkManager[i - 2].CreateSocket("127.0.21." + i, 11001);
                      subnetworkManager[i-2].Connect("127.0.20." + i, 11001);

                  }

                for (int i = 1; i <= amountOfclients; i++)
                {
                    managerClient.Add(new Manager());
                    Console.WriteLine("robie managera dla clienta" + "127.0.12." + i.ToString());

                    managerClient[i - 1].CreateSocket("127.0.13." + i.ToString(), 11001);
                    managerClient[i - 1].Connect("127.0.12." + i.ToString(), 11001);
                }
            }
            else
            {
                agent = new Agent();

                agent.CreateSocket("127.0.20."+args[0].ToString(),11001);
               Thread thread=new Thread( new ThreadStart(agent.Connect));
                thread.Start();

            }


            //string do ktorego wczytujemy polecenie odbiorcy
            string choose;
            while (true)
            {
                //wypisanie polecen
                CLI.Prompt();

                choose = Console.ReadLine();
                if ( choose== "1")
                {
                    CLI.ConfigureSubnetworks();
                   // CLI.ConfigureLinkConnections(managerCloud);
                   // CLI.Configure(nodeAmount, manager, clientAmount, managerClient, managerCloud);
                }
                else if (choose=="2")
                {
                    //wysylanie konfiguracji do klijenta
                    
                   // CLI.ConfigureClients(clientAmount, managerClient);
                   // CLI.Fix(nodeAmount, manager);
                }
                //prosba o konfiguracje wezla
                else if (choose =="3")
                {
                   
                    {
                        CLI.GetNodeFromNode(manager);
                    }

                }
                //prosba o konfiguracje konkretnego portu konkretnego wezla
                else if (choose=="4")
                {
                    CLI.GetMatrixFromNode(manager);
                }
            }

        }
    }
}
