using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CableCloud
{

    class ClientCloud
    { 
        Socket mySocket;
        Socket listeningSocket;
        int id;
    byte[] buffer;
        
    public ClientCloud(int id)
    {
            Switch.clientCollection.Add(new ObservableCollection<string>());
            this.id = id;
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
        mySocket= mySocket.Accept();
            CLI.ClientConnected();
    
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
                

            Console.WriteLine("Wiadomość od klienta: "+receivedMessage);
           
                //tu jest funkcja switchujaca
                Switch.SwitchBufer(receivedMessage);

                buffer = new byte[1024];
            mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(MessageCallback), buffer);

        }
        catch (Exception ex)
        {

        }
    }
  
    public void disconnect_Click()
    {
        mySocket.Disconnect(true);
        mySocket.Close();
    }

    public void SendThread()
        {


            //Console.ReadLine();
           lock (Switch.clientCollection.ElementAt(id - 1))
            {
                Switch.clientCollection.ElementAt(id - 1).CollectionChanged += Send;
            }
            //Console.ReadLine();
            
        }
        
        private void Send(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("wysylam do klijenta");
            lock (Switch.clientCollection.ElementAt(id - 1))
            {
                string s = Switch.clientCollection.ElementAt(id - 1).Last();
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sending = new byte[1024];
                sending = enc.GetBytes(s);

                mySocket.Send(sending);

            }
        }
    }
}
   