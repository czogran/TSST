using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
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
            CableCloud cc = new CableCloud();
            //testy
            cc.Listen();
            cc.SendData("127.0.0.4", 10000, cc.test);
            cc.Listen();
            cc.SendData("127.0.0.3", 10000, cc.test);

            for (int i = 0; i < 3; i++)
            {
                cc.Listen();
                cc.SendData("127.0.0.3", 10000, cc.test);
            }

            //tu będzie piękna pętla w której będzie chodził program
//            while (true)
//            {
//                
//            }
        }
    }
}
