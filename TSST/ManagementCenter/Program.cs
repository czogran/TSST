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

        public static List<Link> links;
        // tu w ich dodawaniu bedzie jeden wielki cheat bo bedzie szlo ono po wezlach jakie sa w linkach
        public static List<Node> nodes;

        public static List<Manager> manager;
        public static List<Manager> managerClient;

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
            Console.WriteLine("MANAGER");
            CLI.ClientNum(args[1]);
            int nodeAmount = int.Parse(args[0]);
            int clientAmount = int.Parse(args[1]);
          
            int id;

            List<string> port = new List<string>();
            List<string> agent = new List<string>();
             manager = new List<Manager>();
             managerClient = new List<Manager>();
            // List<Manager> manager = new List<Manager>();
            //List<Manager> managerClient = new List<Manager>();

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
         


            for (int i=1;i<=nodeAmount;i++)
               {
                   id = 2 * i + 10;
                   port.Add ( "127.0.1." +i.ToString());
                   agent.Add( "127.0.3." + i.ToString());
                 
               }
            
            //miejsce na dodatkowe ustalanie polaczen
            //XML.SetName("default5.xml");
            CLI.NodeNum(nodeAmount);
            for (int i = 1; i <= nodeAmount; i++)
            {
                manager.Add(new Manager(i));
                
              //  socket += i;
                manager[i - 1].CreateSocket("127.0.4." + i.ToString(), 11001);
                manager[i - 1].Connect(agent[i - 1], 11001);
                Thread threadPing = new Thread(new ThreadStart(manager[i - 1].PingThread));
             //   threadPing.Start();
            }
            for (int i = 1; i <= nodeAmount; i++)
            {
                manager.Add(new Manager(i));
                Thread threadPing = new Thread(new ThreadStart(manager[i - 1].PingThread));
                threadPing.Start();
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
                    CLI.ConfigureLinkConnections(managerCloud);
                   // CLI.Configure(nodeAmount, manager, clientAmount, managerClient, managerCloud);
                }
                else if (choose=="2")
                {
                    //wysylanie konfiguracji do klijenta
                    
                    CLI.ConfigureClients(clientAmount, managerClient);
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


            // Console.WriteLine(XML.StringNode(1));
            Console.ReadLine();
        }
    }
}
