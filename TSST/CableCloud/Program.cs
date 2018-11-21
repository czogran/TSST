using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Console.WriteLine("cable1");

           

            SocketCloud p = new SocketCloud();
            p.CreateSocket("127.0.0.4", 11004);
            //p.Connect("127.0.0.3",11003);
            //p.Send("siema");
            //string test;
            //test=Console.ReadLine();
            p.Connect();
            //Thread t2 = new Thread( p.Connect);
           // t2.Start();
            Thread t1 = new Thread(new ThreadStart(p.SendThread));
            t1.Start();

            CableCloud c = new CableCloud();
            c.CreateSocket("127.0.0.6", 11006);
            c.Connect("127.0.0.5", 11005);
            //Thread t3 = new Thread(() => c.Connect("127.0.0.5", 11005));
            //t3.Start();
            Thread t4 = new Thread(new ThreadStart(c.SendThread));
            t4.Start();


            //  CableCloud.listen();\

            //SocketCloud socket1 = new SocketCloud("127.0.0.2", 11002);
            //CableCloud.connect(socket1);
            //Thread t = new Thread(new ThreadStart (SocketCloud.connect));
            // Thread t = new Thread(new ThreadStart(SocketCloud.connect));
            // Thread t = new Thread(new ThreadStart(SocketCloud.connect));
            //t.Start();


            // socket2 = new SocketCloud("127.0.0.4", 11004);
            //socket1.send("asdadad<EOF>");
            //socket2.send("dttttttttt<EOF>");
            // socket1.send("end");
            // socket2.send("end");
            //socket1.close();
            // socket2.close();



            //SocketCloud socket2 = new SocketCloud("127.0.0.1", 12001);


        }
    }
}
