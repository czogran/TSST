using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


namespace ClientNode
{
    class Port
    {
        Socket mySocket;

        EndPoint endRemote, endLocal;
        byte[] buffer;
        int client;

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

                //usuniecie informacji charakterystycznej
               // receivedMessage=receivedMessage.Substring(receivedMessage.IndexOf("<address>"));

                Console.WriteLine(receivedMessage.Substring(0,receivedMessage.IndexOf("<address>")));


                buffer = new byte[1024];
                mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endRemote,
                    new AsyncCallback(MessageCallback), buffer);

            }
            catch (Exception ex)
            {

            }
        }
        public void Send(string message,int idOfNodeWeAreSendingTo)
        {
            try
            {
                int port;
                string address;
                lock (Agent.clientDictioinary)
                {
                    //bierzemy adres docelowego
                    var tuple = Agent.clientDictioinary[idOfNodeWeAreSendingTo];
                    
                    address = tuple.Item1;
                    
                    tuple = Agent.clientDictioinary[Program.number];
                    port = tuple.Item2;

                }

                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sending = new byte[1024];
                sending = enc.GetBytes(message + "<address>" + address + "</address>"+ "<port>" +port.ToString()+"</port>");

                mySocket.Send(sending);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Nie udalo sie wyslac:" + ex.ToString());
            }
        }
        public void disconnect_Click()
        {
            mySocket.Disconnect(true);
            mySocket.Close();
        }
        public void SendThread()
            {
            while (true)
                {
                string message = Console.ReadLine();
                 if (message.Contains("//client:"))
                {
                    try
                    {
                        client = Int32.Parse(message.Substring(9));
                        Console.WriteLine("Odbiorca jest klijent: " + client);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("zla komenda");
                    }

                }
                 else
                {
                    Send(message, client);
                }
              
            }
            }
    }
}
   