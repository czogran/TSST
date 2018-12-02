using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    class Port
    {

    

    Socket mySocket;
    Socket listeningSocket;

    EndPoint endRemote, endLocal;
    byte[] buffer;

    public Port()
    {
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
    public void Connect()
    {
       

       

        mySocket.Listen(10);
        mySocket = mySocket.Accept();
        Console.WriteLine("connected Cloud");
        //mySocket.Accept();
        //mySocket.BeginAccept(AcceptCallback, mySocket);
        buffer = new byte[1024];

            
                mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
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

            Console.WriteLine("FROM Cloud: " + receivedMessage);
            lock(SwitchingMatrix.computingCollection)
                {
                    SwitchingMatrix.computingCollection.Add( receivedMessage + Program.number);
                }


            buffer = new byte[1024];

            mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(MessageCallback), buffer);

        }
        catch (Exception ex)
        {
                Console.WriteLine("Message callback execption");
        }
    }
        private static int counter = 0;
    public void Send(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {

           // if (counter == 0)
            //{
                lock (SwitchingMatrix.sendCollection)
                {
                string s = SwitchingMatrix.sendCollection.Last();
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sending = new byte[1024];
                sending = enc.GetBytes(s);

                mySocket.Send(sending);

                    //SwitchingMatrix.collection.Move(SwitchingMatrix.collection.IndexOf(SwitchingMatrix.collection.First()), SwitchingMatrix.collection.IndexOf(SwitchingMatrix.collection.Last()));
                   // Console.WriteLine("taaa "+SwitchingMatrix.collection[0]);
                    //SwitchingMatrix.collection.Remove(SwitchingMatrix.collection.Last());
                }
            //    counter = 1;
           // }
          //  else
            //    counter = 0;
    }
    public void disconnect_Click()
    {
        mySocket.Disconnect(true);
        mySocket.Close();
    }
    public void SendThread()
    {
            lock (SwitchingMatrix.sendCollection)
            {
                SwitchingMatrix.sendCollection.CollectionChanged += Send;
                
              
               // SwitchingMatrix.collection.
            }
    }

}
}






    /*
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
