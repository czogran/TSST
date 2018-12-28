using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CableCloud
{
    /// <summary>
    /// Główna klasa programu
    /// </summary>
    class Program
    {
        public static String nodeAmount = "0";
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        static void Main(string[] args)
        {
            Console.WriteLine("CABLE CLOUD");

            //utworzenie agenta
            Agent agent = new Agent();
            agent.CreateSocket("127.0.0.2", 11001);
            agent.Connect();
            Thread threadAgent = new Thread(new ThreadStart(agent.ComputingThread));
            threadAgent.Start();

            //wezly
            List<NodeCloud> node = new List<NodeCloud>();
            string localIP;
            int localHost;
            int remoteID;
            //lock (nodeAmount)
            //{
                int num = Int32.Parse(nodeAmount);

                /*for (int i = 1; i <= Switch.data.ElementAt(0); i++)
                {
                //int i = 1;
                    localHost = 150 + i;
                    node.Add(new NodeCloud(i));
                    localIP = "127.0.0." + localHost.ToString();
                    node[i - 1].CreateSocket(localIP, 11001);

                    remoteID = 2 * i + 10;//+ (remoteID - 1).ToString()
                    Console.WriteLine("127.0.0." + (remoteID - 1).ToString());
                    node[i - 1].Connect("127.0.0." + (remoteID - 1).ToString(), 11001);
                    Thread threadNode = new Thread(new ThreadStart(node[i - 1].SendThread));
                    threadNode.Start();
                }*/
            //}
            
            //klijenci
            ClientCloud p = new ClientCloud(1);
            p.CreateSocket("127.0.11.1", 11001);
            p.Connect();
            //Thread t2 = new Thread( p.Connect);
           // t2.Start();
            Thread t1 = new Thread(new ThreadStart(p.SendThread));
            t1.Start();


            ClientCloud p1 = new ClientCloud(2);
            p1.CreateSocket("127.0.11.2", 11001);
            p1.Connect();
            Thread t2 = new Thread(new ThreadStart(p1.SendThread));
            t2.Start();

            ClientCloud p2 = new ClientCloud(3);
            p2.CreateSocket("127.0.11.3", 11001);
            p2.Connect();
            Thread t3 = new Thread(new ThreadStart(p2.SendThread));
            t3.Start();

            //te read line musi byc inaczej wszystko sie konczy
          
            ClientCloud p3 = new ClientCloud(4);
            p3.CreateSocket("127.0.11.4", 11001);
            p3.Connect();
            Thread t4 = new Thread(new ThreadStart(p3.SendThread));
            t4.Start();


            ClientCloud p4 = new ClientCloud(5);
            p4.CreateSocket("127.0.11.5", 11001);
            p4.Connect();
            Thread t5 = new Thread(new ThreadStart(p3.SendThread));
            t5.Start();


            ClientCloud p5 = new ClientCloud(6);
            p3.CreateSocket("127.0.11.6", 11001);
            p3.Connect();
            Thread t6 = new Thread(new ThreadStart(p3.SendThread));
            t6.Start();


            //te read line musi byc inaczej wszystko sie konczy
            Console.ReadLine();



        }
    }
}
