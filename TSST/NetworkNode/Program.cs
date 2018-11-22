using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Główna klasa programu
    /// </summary>
    class Program
    {
        public static int number;
        private static string[] ArgToIP(string arg)
        {
            int id = int.Parse(arg);
            number = id;
            id = 2 * id + 10;

            return new string[] { "127.0.0." + (id - 1).ToString(), "127.0.0." + id.ToString() };
        }

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        
        static void Main(string[] args)
        {


            Console.WriteLine("node"+args[0]);
            //Console.WriteLine("node number "+args[0]);

            string[] ips = ArgToIP(args[0]);
            Console.WriteLine("port: "+ips[0]+"end");
            Console.WriteLine("agent: "+ips[1]+"end");

            Port port = new Port();
            Agent agent = new Agent();
            port.CreateSocket(ips[0], 11001);
            //port.CreateSocket(ips[0], 11002);
            agent.CreateSocket(ips[1], 11001);

            port.Connect();
            agent.Connect();

            Thread threadPort = new Thread(new ThreadStart(port.SendThread));
            Thread threadAgent = new Thread(new ThreadStart(agent.ComputingThread));

            threadAgent.Start();
            threadPort.Start();
            Console.ReadLine();

            /*
            Port p = new Port();
            p.CreateSocket("127.0.0.5", 11005);
            p.Connect();
           // Thread t2 = new Thread(p.Connect);
            //t2.Start();
            Thread t1 = new Thread(new ThreadStart(p.SendThread));

            t1.Start();
            Console.Read();
            */

            /* string message;
             Console.WriteLine("Node");
             Label.setMask();
             //int la = 13 + 8388608;
             try
             {
                 Label.SetLabel(1111, 4, 1, 253);
             }
             catch(InvalidOperationException ex)
             {
                 Console.WriteLine(ex.ToString());
             }


             Label.GetLabel("aaaaaa<label>" + Label.label + "</label>ddddd<label>22</label>dd");
             // Label.SetLabelID(100);
             try
             {
                 Label.SetLabelS(0);
                 Label.SetTTL(10);
                 Label.DecreaseTTL();
                 Label.SetTC(5);
                 Label.SetLabelID(12);
             }
             catch (InvalidOperationException ex)
             {
                 Console.WriteLine(ex.ToString());
             }

             message =Label.push("a<port>22</port><label>13</label>addddddd", 9);
             Label.Swap(message, 11);
             Label.SwapPort(message, 11122);
             message =Label.pop(message);
             Label.Swap(message, 10);
             //Label.SetLabelID(222);
             //Label.SetTTL(10);

            // Label.GetLabel("aaaaaa<label>" + Label.label + "</label>ddddd<label>22</label>dd");

             //Label.GetPort("a<port>22</port>aaaaa < label > 13 </ label > ddddddd");
            // Port port = new Port("127.0.0.2", 11002);
             Console.Read();*/
        }
        
    }
}
