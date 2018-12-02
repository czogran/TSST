using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
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
            Console.WriteLine("MANAGER");
            Console.WriteLine("liczba klijentow: "+args[1]);
            int nodeAmount = int.Parse(args[0]);
            //string port;
            //string agent;

            XML.CreateXML("test1.xml");
            XML.SetName("test1.xml");
            //nowa konwencja nazwy powyrzej 80 znacza ze chodzi o klijenta
            XML.AddLink(9111, 81, 1, "on");
            XML.AddLink(1112, 1, 2, "on");
            XML.AddLink(1213, 2,3, "on");
            XML.AddLink(1392, 3, 82, "on");

            int id;

            List<string> port = new List<string>();
            List<string> agent = new List<string>();
            List<Manager> manager = new List<Manager>();

            Manager managerCloud = new Manager();
            managerCloud.CreateSocket("127.0.0.1", 11001);
            managerCloud.Connect("127.0.0.2", 11001);
       //     managerCloud.Send(XML.StringCableLinks());
            System.Threading.Thread.Sleep(100);
            managerCloud.Send("nodes:" + args[0]);
            System.Threading.Thread.Sleep(100);
       
            managerCloud.Send("clients:" + args[1]);
            System.Threading.Thread.Sleep(100);


            //managerCloud.Send("clients:" );

         
               for (int i=1;i<=nodeAmount;i++)
               {
                   id = 2 * i + 10;
                   port.Add ( "127.0.0." + (id - 1).ToString());
                   agent.Add( "127.0.0." + id .ToString());
                   XML.AddNode(i,port[i-1],agent[i-1]);
                  // XML.AddMatrix(10+i, i);
               }
            XML.AddMatrix(9111, 1);
            XML.AddMatrix(1112, 2);
            XML.AddMatrix(1213, 3);
           
            XML.AddLabel(1, 9111, "push", 0, 1112, 0, 1);
            XML.AddLabel(2, 1112, "swap", 1, 1213, 2);
            XML.AddLabel(3, 1213, "pop", 2, 1392);

            //miejsce na dodatkowe ustalanie polaczen
            int socket = 100;
           //XML.SetName("default5.xml");

            for (int i = 1; i <= nodeAmount; i++)
            {
                manager.Add(new Manager());
                Console.WriteLine("tworze agenta dlanoda" + agent[i - 1]);
                socket += i;
                manager[i - 1].CreateSocket("127.0.0." + socket, 11001);
                manager[i - 1].Connect(agent[i - 1], 11001);
            }
            string choose;
            while (true)
            {
                Console.WriteLine("JEZELI CHCESZ SKONFIGUROWAC SIEC WYBIERZ 1");
                Console.WriteLine("JEZELI CHCESZ NAPRAWIC SIEC WYBIERZ 2");
                choose = Console.ReadLine();
                if ( choose== "1")
                {
                    Console.WriteLine("PODADAJ XML");
                    string name = "false";
                    XML.SetName(name);
                    while (XML.Test() != true)
                    {
                        name = Console.ReadLine();
                        XML.SetName(name);
                    }

                    for (int i = 1; i <= nodeAmount; i++)
                    { 
                        try
                        {
                            manager[i - 1].Send(XML.StringNode(i));
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    managerCloud.Send(XML.StringCableLinks());
                }
                else if (choose=="2")
                {
                    Console.WriteLine("PODADAJ XML");
                    string name = "false";
                    XML.SetName(name);
                    while (XML.Test() != true)
                    {
                        name = Console.ReadLine();
                        XML.SetName(name);
                    }

                    for (int i = 1; i <= nodeAmount; i++)
                    {
                        try
                        {
                            manager[i - 1].Send(XML.StringNode(i));
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }
                }
           
            /*XML.AddNode(1,"1","1");
            XML.AddClient(2,11);
            XML.AddClient(4,44);
            XML.AddClient(5,66);
            XML.AddNode(2,"2","3");
            XML.AddMatrix(1, 1);
            XML.AddMatrix(2, 2);
            XML.AddLabel(1, 1, "push", 2, 11,14, 12);
            XML.AddLabel(2, 2, "pop", 11, 33);
            XML.ChangeLabelPort(1, 1, 2,222);
            XML.AddLink(1, 11, 22, "on");
            XML.AddLink(2, 21, 12, "off");
            XML.AddLink(3,21,11,"on");
            XML.ChangeLinkStatus(2, "on");
            //XML.RemoveNode(2);
            XML.RemoveClient(4);
            XML.RemoveLink(2);
            // XML.RemoveLabel(2, 2, 11);
            XML.ChangeLabelLabelIn(2, 2, 11, 15);
            XML.ChangeLabelPort(2, 2, 15, 111);
            XML.ChangeLabelPush(2, 2, 15, 10);
            XML.ChangeLabelSwap(2, 2, 15, 20);
            XML.ChangeLabelAcction(2, 2, 15, "push");*/
            Console.ReadLine();
               // Console.WriteLine(XML.StringNode(1));

        }
    }
}
