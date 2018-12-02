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
            Console.WriteLine("client"+args[0]);
            //System.Threading.Thread.Sleep(10000);
            Port p = new Port();
        
            p.CreateSocket("127.0.10."+args[0], 11003);
            p.Connect("127.0.11."+args[0], 11004);
            //p.Send("siema");
            //string test;
            //test=Console.ReadLine();
           
            //Thread t2 = new Thread(() => p.Connect("127.0.0.4", 11004));
            //t2.Start();
            Thread t1 = new Thread(new ThreadStart(p.SendThread));
            t1.Start();

            // Thread new ThreadStart
            // var t = new Thread((p.listen));
            // static Object obj = new Object();
            // t.Start();

            //  p.send("do bani<EOF>");
            //Console.ReadKey();
            //p.send("nie nawidze lcpsow<EOF>");
            // p.send("chranic laby<EOF>");
            //p.send("end");
            //p.close();

        }
    }
}

