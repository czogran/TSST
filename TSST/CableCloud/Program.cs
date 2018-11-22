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
                Console.WriteLine("num"+num);
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
            ClientCloud p = new ClientCloud();
            p.CreateSocket("127.0.0.4", 11004);
            p.Connect();
            //Thread t2 = new Thread( p.Connect);
           // t2.Start();
            Thread t1 = new Thread(new ThreadStart(p.SendThread));
            t1.Start();
            /*
NodeCloud c = new NodeCloud(1);
            c.CreateSocket("127.0.0.151", 11001);
            c.Connect("127.0.0.11", 11001);
            //Thread t3 = new Thread(() => c.Connect("127.0.0.5", 11005));
            //t3.Start();
            Thread t4 = new Thread(new ThreadStart(c.SendThread));
            t4.Start();

*/
            //  CableCloud.listen();\

            //SocketCloud socket1 = new SocketCloud("127.0.0.2", 11002);
            //CableCloud.connect(socket1);
            //Thread t = new Thread(new ThreadStart (SocketCloud.connect));
            // Thread t = new Thread(new ThreadStart(SocketCloud.connect));
            // Thread t = new Thread(new ThreadStart(SocketCloud.connect));
            //t.Start();


            // socket2 = new SocketCloud("127.0.0.4", 11004);
            //socket1.send("asdadad<EOF>");
            //socket2.send("dttttttttt<EOF>");
            // socket1.send("end");
            // socket2.send("end");
            //socket1.close();
            // socket2.close();



            //SocketCloud socket2 = new SocketCloud("127.0.0.1", 12001);


        }
    }
}
