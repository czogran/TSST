using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
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
            Port port = new Port("127.0.0.3", 10000);
            port.SendData("127.0.0.1", "dupa");
            port.Listen();

            for (int i = 0; i < 3; i++)
            {
                port.SendData("127.0.0.1", port.test);
                port.Listen();
            }
            
            //tu będzie piękna pętla w której będzie chodził program
//            while (true)
//            {
//                
//            }
        }
    }
}
