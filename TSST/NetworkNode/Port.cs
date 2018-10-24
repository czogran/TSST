using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Port
    /// </summary>
    class Port
    {
        IPAddress ip;
        byte[] bytes = new byte[1024];
        Socket listener;
        IPEndPoint endPoint;
        public static string data = null;

        public Port(String address, int port)
        {
            ip = IPAddress.Parse(address);
            endPoint = new IPEndPoint(ip, port);
            listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(endPoint);
                listener.Listen(10);
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    Console.WriteLine("cable connected");

                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            Console.WriteLine(data);
                            byte[] msg = Encoding.ASCII.GetBytes("data<EOF>");
                            handler.Send(msg);
                            data = null;
                        }
                        if(data=="end")
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            Console.WriteLine("Text received : {0}", data);
                            handler.Close();
                            break;
                        }
                    }
                    Console.WriteLine("test");

             
                    Console.WriteLine("Text received : {0}", data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

    }

}


/*
   public static string address = "192.168.0.10";
        public static string data = null;

        public static void listen()
        {
            IPAddress ip = IPAddress.Parse(address);
            byte[] bytes = new byte[1024];
            IPEndPoint end = new IPEndPoint(ip, 1100);
            Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(end);
                listener.Listen(10);
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    Console.WriteLine("Text received : {0}", data);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


        }

 */
