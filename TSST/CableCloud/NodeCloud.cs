using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
{
    class NodeCloud
    {

        Socket mySocket;

        EndPoint endRemote, endLocal;
        byte[] buffer;
        int id;
        public NodeCloud(int id)
        {
            this.id = id;
            Switch.nodeCollection.Add(new ObservableCollection<string>());
        }

        public void CreateSocket(string IP, int port)
        {
            string myIp;
            int myport;
            myIp = IP;
            myport = port;

            IPAddress ipAddress = IPAddress.Parse(myIp);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, myport);

            mySocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            mySocket.Bind(localEndPoint);
        }
        public void Connect(string IP, int port)
        {
            string toIp = IP;
            int toPort;
            toPort = port;

            IPAddress ipAddress = IPAddress.Parse(toIp);

            endRemote = new IPEndPoint(ipAddress, toPort);
            //mySocket.Bind(endRemote);
            mySocket.Connect(endRemote);
            buffer = new byte[1024];

            mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endRemote,
                new AsyncCallback(MessageCallback), buffer);
        }
        private void MessageCallback(IAsyncResult result)
        {

            try
            {

                byte[] receivedData = new byte[1024];
                receivedData = (byte[])result.AsyncState;

                ASCIIEncoding encoding = new ASCIIEncoding();

                int i = receivedData.Length - 1;
                while (receivedData[i] == 0)
                    --i;

                byte[] auxtrim = new byte[i + 1];
                Array.Copy(receivedData, auxtrim, i + 1);

                string receivedMessage = encoding.GetString(auxtrim);

                Console.WriteLine("FROM NODE: " + receivedMessage);
                int nextNode = Switch.SwitchNodes(receivedMessage);
                /* lock (Switch.nodeCollection.ElementAt(nextNode-1))
                 {
                     //if (id < 5)
                     //{
                     //nastepny element
                     try
                     {
                         Switch.nodeCollection.ElementAt(id).Add(receivedMessage);
                     }
                     catch(Exception ex)
                     {
                         Console.WriteLine("za daleko");
                     }
                      //  }
                 }*/
                Switch.SwitchBufer(receivedMessage);


                buffer = new byte[1024];
                mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endRemote,
                    new AsyncCallback(MessageCallback), buffer);

            }
            catch (Exception ex)
            {

            }
        }
        public void Send(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {


            lock (Switch.nodeCollection.ElementAt(id - 1))
            {
                string s = Switch.nodeCollection.ElementAt(id - 1).Last();
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sending = new byte[1024];
                sending = enc.GetBytes(s);

                mySocket.Send(sending);

            }
        }
            public void disconnect_Click()
        {
            mySocket.Disconnect(true);
            mySocket.Close();
        }
        public void SendThread()
        {
            lock (Switch.nodeCollection.ElementAt(id - 1))
            {
                Switch.nodeCollection.ElementAt(id - 1).CollectionChanged += Send;
            }
        }
    }
}










    /*
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
                   // SocketCloud socket = new SocketCloud();

                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {

                            //   byte[] msg = Encoding.ASCII.GetBytes("data");
                            //handler.Send(msg);
                            Console.WriteLine("Text received2 : {0}", data);
                            //get = socket.Send(data, socket);
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
*/