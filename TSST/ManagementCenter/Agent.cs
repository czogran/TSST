using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
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
            buffer = new byte[30240];

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
                if (receivedMessage != "ping")
                    Console.WriteLine("Od Agenta:\n " + receivedMessage);
                lock (AgentSwitchingAction.agentCollection)
                {
                    AgentSwitchingAction.agentCollection.Add(receivedMessage);
                }
                buffer = new byte[30240];

                mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    new AsyncCallback(MessageCallback), buffer);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Message callback execption:" + ex.ToString());
            }
        }

        /// <summary>
        /// wysyla z powrotem do agenta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       // public void Send(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        public void Send(string message)
        {
          
        }






        /// </summary>
        public void ComputingThread()
        {

            lock (AgentSwitchingAction.agentCollection)
            {
                AgentSwitchingAction.agentCollection.CollectionChanged += SwitchAction;
            }
        }

        /// <summary>
        /// sluzy do przelaczania akcji w zaleznosci od otrzymanej wiadomosci
        ///docelowo powinna zostac wywalona do innej klasy, by to ladnie zrobic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SwitchAction(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {

        }
            public void disconnect_Click()
        {
            mySocket.Disconnect(true);
            mySocket.Close();
        }

        /// <summary>
        ///  
        /// </summary>
       
        
    }
    
}
