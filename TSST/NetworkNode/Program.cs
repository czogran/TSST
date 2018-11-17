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
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        static void Main(string[] args)
        {
            Port port = new Port("127.0.0.4", 10000);
            Agent agent = new Agent("127.0.0.10", 10000);
            SwitchingMatrix switchingMatrix = new SwitchingMatrix();
            
            agent.Listen();
            Console.WriteLine(agent.receivedData);
            switchingMatrix.SetPortTable(agent.receivedData);
            switchingMatrix.PrintPortTable();
            agent.SendData("127.0.0.30", "potwierdzam");
            
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
