using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("client");

            Port p = new Port();
            // Thread new ThreadStart
           // var t = new Thread((p.listen));
           // static Object obj = new Object();
           // t.Start();

            p.send("do bani<EOF>");
            Console.ReadKey();
            p.send("nie nawidze lcpsow<EOF>");
               p.send("chranic laby<EOF>");
            p.send("end");
             p.close();

        }
    }
}

