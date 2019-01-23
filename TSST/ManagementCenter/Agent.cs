using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
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
        string myIp;
        public void CreateSocket(string IP, int port)
        {
           
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
                {
                    Console.WriteLine();
                    Console.Write(this.GetTimestamp() + " : ");
                    Console.WriteLine("Agent odebral wiadomosc: "+receivedMessage);

                    //   Console.WriteLine("Odebrano od agenta wiadomość o treści " + receivedMessage);


                    //Console.WriteLine("Od Agenta:\n " + receivedMessage);
                }

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
            lock (mySocket)
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sending = new byte[1024];
                sending = enc.GetBytes(message);

                mySocket.Send(sending);

                Console.Write(this.GetTimestamp() + " : ");
                Console.WriteLine("Agent wyslal wiadomość do "+myIp+" o treści: " + message);
            }
        }

        /// <summary>
        /// wielki cheat wysylam potem znacznik, ktory mowi co bedzie w  wiadomosci
        /// bo inaczej nie umiem
        /// </summary>
        /// <param name="stream"></param>
        public void Send(MemoryStream stream)
        {
            lock (mySocket)
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sending = new byte[1024];
                sending = enc.GetBytes("possible_window");
                // byte[] c = sending.Concat();


                mySocket.Send(stream.ToArray());
                mySocket.Send(sending);

                Console.Write(this.GetTimestamp() + " : ");
                Console.WriteLine("Wysłano wiadomość o treści " + "possible_window");
            }
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

            if (AgentSwitchingAction.agentCollection.Last().Contains("error"))
            {
                try
                {
                    Send(AgentSwitchingAction.agentCollection.Last());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                AgentSwitchingAction.AgentAction(AgentSwitchingAction.agentCollection.Last(), this);
            }
        }

        public void disconnect_Click()
        {
            mySocket.Disconnect(true);
            mySocket.Close();
        }

        public string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

    }

}
