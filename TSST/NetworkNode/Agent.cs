using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Obsługuje polecenia z centrum zarządzania
    /// </summary>
    class Agent
    {
        Socket mySocket;
        byte[] buffer;

        public Agent()
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
            CLI.ConnectedAgent();
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

                //Console.WriteLine("FROM Agent: " + receivedMessage);
                lock (SwitchingMatrix.agentCollection)
                {
                    SwitchingMatrix.agentCollection.Add(receivedMessage );
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


        public void Send(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {
            lock (SwitchingMatrix.agentCollection)
            {
                string s = SwitchingMatrix.agentCollection.Last();
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

        public void ComputingThread()
        {
            
            lock (SwitchingMatrix.agentCollection)
            {
                 SwitchingMatrix.agentCollection.CollectionChanged += SwitchAction;
            }
        }
        public void AgentInfo()
        {
            lock (SwitchingMatrix.agentCollection)
            {
                SwitchingMatrix.agentCollection.CollectionChanged += SwitchAction;
            }
        }
        public void SwitchAction(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {
           

            if (SwitchingMatrix.agentCollection.Last().Contains("node"))
            {
                File.WriteAllText("myNode"+Program.number+".xml", SwitchingMatrix.agentCollection.Last());
                SwitchingMatrix.portDictionary.Clear();
                SwitchingMatrix.labelZeroDictionary.Clear();
                SwitchingMatrix.FillDictionary();
            }
        }
    }
}


