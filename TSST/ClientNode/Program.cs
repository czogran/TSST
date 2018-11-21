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
        /// Przypisuje klientom adresy IP
        /// </summary>
        /// <param name="a1">liczba networknode</param>
        /// <param name="a2">liczba clientnode</param>
        /// <returns>Para adresów IP klienta</returns>
        private static string[] ArgToIP(string a1, string a2)
        {
            int id1 = int.Parse(a1);
            int id2 = int.Parse(a2);

            int id3 = 2*(id1 + id2)+10;

            return new [] { "127.0.0." + (id3 - 1), "127.0.0." + id3 };
        }
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        static void Main(string[] args)
        {
            var ips = ArgToIP(args[0], args[1]);

            Port port = new Port(ips[0], ips[1], 10000);
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
