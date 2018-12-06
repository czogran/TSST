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
        Console.WriteLine("Połączono z chmurą");
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
               
                Console.WriteLine("Otrzymana wiadomosc na porcie:" + Label.GetPort(receivedMessage) + "\n" + receivedMessage);
                lock (SwitchingMatrix.computingCollection)
                {
                    if (!receivedMessage.Contains("<path>"))
                    {
                        receivedMessage = Label.SetPath(receivedMessage, Program.number);
                    }
                    else
                    {
                        receivedMessage = Label.AddToPath(receivedMessage, Program.number);
                    }
                    SwitchingMatrix.computingCollection.Add(receivedMessage);
                }


            buffer = new byte[1024];

            mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(MessageCallback), buffer);

        }
        catch (Exception ex)
        {
                Console.WriteLine(ex.ToString());
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

                }
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






  