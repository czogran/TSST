using System;
using System.Collections.Generic;
using System.Linq;
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
            Port port = new Port();
            port.SendData("127.0.0.1", 1, "dupa");
        }
    }
}
