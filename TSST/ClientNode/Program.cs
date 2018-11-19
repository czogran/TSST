using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientNode
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
            Console.WriteLine(args[0]);
            
            Port port = new Port("127.0.0.3", "127.0.0.12", 10000);
            XMLParser parser = new XMLParser();
            
            port.Listen();
            port.connectedPortNumber = parser.ReadXml(port.receivedData);
            Console.WriteLine(port.connectedPortNumber);
            port.SendData("127.0.0.30", CLI.confirmation);
            
            Thread t1 = new Thread(new ThreadStart(port.SenderLoop));
            Thread t2 = new Thread(new ThreadStart(port.ListenerLoop));
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
        }
    }
}
