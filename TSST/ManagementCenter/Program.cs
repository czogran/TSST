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
            CLI.ClientNum(args[1]);
            int nodeAmount = int.Parse(args[0]);
            //string port;
            //string agent;

          
            int id;

            List<string> port = new List<string>();
            List<string> agent = new List<string>();
            List<Manager> manager = new List<Manager>();
            List<Manager> managerClient = new List<Manager>();

            Manager managerCloud = new Manager();
            managerCloud.CreateSocket("127.0.0.1", 11001);
            managerCloud.Connect("127.0.0.2", 11001);
       //     managerCloud.Send(XML.StringCableLinks());
            System.Threading.Thread.Sleep(100);
            managerCloud.Send("nodes:" + args[0]);
            System.Threading.Thread.Sleep(100);
       
            managerCloud.Send("clients:" + args[1]);
            System.Threading.Thread.Sleep(100);

            //manager create clients
            for (int i = 1; i <= Int32.Parse(args[1]); i++)
            {
                managerClient.Add(new Manager());
                CLI.CreateClientAgent(i);
               
                managerClient[i - 1].CreateSocket("127.0.13." + i.ToString(), 11001);
                managerClient[i - 1].Connect("127.0.12."+i.ToString(), 11001);
            }

                //managerCloud.Send("clients:" );


                for (int i=1;i<=nodeAmount;i++)
               {
                   id = 2 * i + 10;
                   port.Add ( "127.0.0." + (id - 1).ToString());
                   agent.Add( "127.0.0." + id .ToString());
                 
               }
            
            //miejsce na dodatkowe ustalanie polaczen
            int socket = 100;
           //XML.SetName("default5.xml");

            for (int i = 1; i <= nodeAmount; i++)
            {
                manager.Add(new Manager());
                CLI.CreateNodeAgent(agent[i - 1]);
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
                    
                    for (int i = 1; i <= Int32.Parse(args[1]); i++)
                    {
                        Console.WriteLine("try");
                        try
                        {
                            managerClient[i - 1].Send(XML.StringClients());
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
           
          
               // Console.WriteLine(XML.StringNode(1));

        }
    }
}
