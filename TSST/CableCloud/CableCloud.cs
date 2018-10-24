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
    /// Chmura kablowa ze wszystkimi socketami
    /// </summary>
    class CableCloud
    {
        //public static Socket listener;//= new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        CableCloud()
        {
           // listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        }

        public static void connect(SocketCloud socket)
        {

            //string address,int port
            int port = 11003 ;
            string address = "127.0.0.3";//, 11002
            IPAddress ip = IPAddress.Parse(address);
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            string data = null;
            string get = null;
            Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            byte[] bytes = new byte[1024];

            try
            {
                listener.Bind(endPoint);
                listener.Listen(10);
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    Console.WriteLine("client connected");

                    data = null;

                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {

                            //   byte[] msg = Encoding.ASCII.GetBytes("data");
                            //handler.Send(msg);
                            Console.WriteLine("Text received2 : {0}", data);
                            get = SocketCloud.send(data, socket);
                            handler.Shutdown(SocketShutdown.Both);
                            //  Console.WriteLine("Text received2 : {0}", data);
                            handler.Close();
                            data = null;
                            break;
                        }
                        if (data == "end")
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            Console.WriteLine("Text received : {0}", data);
                            handler.Close();
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }


}
