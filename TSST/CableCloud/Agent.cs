using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CableCloud
{
    class Agent
    {
        Socket mySocket;
        Socket listeningSocket;

        EndPoint endRemote, endLocal;
        byte[] buffer;

        List<NodeCloud> node = new List<NodeCloud>();
        List<ClientCloud> client = new List<ClientCloud>();


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
           // CLI.ConnectedAgent();
            buffer = new byte[30240];

            mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
        new AsyncCallback(MessageCallback), buffer);
        }

        private void MessageCallback(IAsyncResult result)
        {
            try
            {
                byte[] receivedData = new byte[30240];
                receivedData = (byte[])result.AsyncState;

                ASCIIEncoding encoding = new ASCIIEncoding();

                int i = receivedData.Length - 1;
                while (receivedData[i] == 0)
                    --i;

                byte[] auxtrim = new byte[i + 1];
                Array.Copy(receivedData, auxtrim, i + 1);

                string receivedMessage = encoding.GetString(auxtrim);

                Console.WriteLine("Od Agenta: " + receivedMessage);
                lock (global::CableCloud.Switch.agentCollection)
                {
                    global::CableCloud.Switch.agentCollection.Add(receivedMessage);
                }
                buffer = new byte[30240];

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
            lock (global::CableCloud.Switch.agentCollection)
            {
                string s = Switch.agentCollection.Last();
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
            lock (Switch.agentCollection)
            {
                Switch.agentCollection.CollectionChanged += SwitchAction;
            }
        }
       
        public void SwitchAction(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {


            if (Switch.agentCollection.Last().Contains("cloud"))
            {
                File.WriteAllText("myLinks.xml", Switch.agentCollection.Last());
                lock(Switch.linkDictionary)
                {
                    Switch.linkDictionary.Clear();
                    Switch.FillDictionary();
                }
            }
            else if (Switch.agentCollection.Last().Contains("nodes"))
            {
                int start = Switch.agentCollection.Last().IndexOf("nodes");
                string nodes = Switch.agentCollection.Last().Substring(6);
                lock (Program.nodeAmount)
                {
                    Program.nodeAmount = nodes;
                    Switch.data.Add(Int32.Parse(nodes));
                  
                    string localIP;
                    int localHost;
                    int remoteID;
                    for (int i = 1; i <= Int32.Parse(nodes); i++)
                    {
                        //int i = 1;
                        localHost = 150 + i;
                        node.Add(new NodeCloud(i));
                        localIP = "127.0.2." +i.ToString();
                        node[i - 1].CreateSocket(localIP, 11001);

                      //  remoteID = 2 * i + 10;//+ (remoteID - 1).ToString()
                        //Console.WriteLine("127.0.2." + i.ToString());
                        node[i - 1].Connect("127.0.1." + i.ToString(), 11001);
                        Thread threadNode = new Thread(new ThreadStart(node[i - 1].SendThread));
                        threadNode.Start();
                    }

                    // Console.WriteLine("nodesAmount:" + Program.nodeAmount);
                } 

            }
            else if(Switch.agentCollection.Last().Contains("clients"))
            {

                string clients = Switch.agentCollection.Last().Substring(8);
            }
        }
    }
}
