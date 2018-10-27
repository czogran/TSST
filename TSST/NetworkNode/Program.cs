using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
            port.Listen();
            port.SendData("127.0.0.1", port.test);
            
            //tu będzie piękna pętla w której będzie chodził program
//            while (true)
//            {
//                
//            }
        }
    }
}
