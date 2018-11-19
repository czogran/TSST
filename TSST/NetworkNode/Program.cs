using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
        /// <summary>
        /// Mapuje adres ip
        /// </summary>
        /// 
        private static string[] ArgToIP(string arg)
        {
            int id = int.Parse(arg);

            id = 2 * id + 10;

            return new string[] { "127.0.0." + (id - 1).ToString(), "127.0.0." + id.ToString() };
        }

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);

            string[] ips = ArgToIP(args[0]);
            Console.WriteLine(ips[0]);
            Console.WriteLine(ips[1]);

            Port port = new Port(ips[0], 10000);
            Agent agent = new Agent(ips[1], 10000);
            SwitchingMatrix switchingMatrix = new SwitchingMatrix();
            
            agent.Listen();
            Console.WriteLine(agent.receivedData);
            switchingMatrix.SetPortTable(agent.receivedData);
            switchingMatrix.PrintPortTable();
            agent.SendData("127.0.0.30", CLI.confirmation);
            
            //testy
            Thread thread1 = new Thread(new ThreadStart(port.Execute));
            Thread thread2 = new Thread(new ThreadStart(agent.Execute));
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();
        }
    }
}
