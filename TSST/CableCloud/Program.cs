using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

            cc.Listen();
            cc.SetPortTable(cc.receivedData);
            cc.PrintPortTable();
            cc.SendData("127.0.0.30", 10000, "potwierdzam");
            
            while (true)
            {
                //TODO: sprawdzać w tablicy dokąd wysłać dalej i zrobić tam senddata
                cc.Listen();
            }
        }
    }
}
