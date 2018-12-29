using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                if(receivedMessage!="ping")
                     Console.WriteLine("Od Agenta:\n " + receivedMessage);
                lock (SwitchingMatrix.agentCollection)
                {
                    SwitchingMatrix.agentCollection.Add(receivedMessage );
                }
                buffer = new byte[30240];

                mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    new AsyncCallback(MessageCallback), buffer);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Message callback execption:"+ex.ToString());
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
            lock (SwitchingMatrix.agentCollection)
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] sending = new byte[1024];
                sending = enc.GetBytes(message);

                mySocket.Send(sending);

            }
        }



        public void disconnect_Click()
        {
            mySocket.Disconnect(true);
            mySocket.Close();
        }

        /// <summary>
        ///  
        /// </summary>
        public void ComputingThread()
        {
          
            lock (SwitchingMatrix.agentCollection)
            {
                 SwitchingMatrix.agentCollection.CollectionChanged += SwitchAction;
            }
        }
      
        public void SwitchAction(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {

            lock (SwitchingMatrix.agentCollection)
            {
                if (SwitchingMatrix.agentCollection.Last().Contains("node"))
                {
                    XMLParser.AddConnection("myNode" + Program.number + ".xml", SwitchingMatrix.agentCollection.Last());
                    // File.WriteAllText("myNode" + Program.number + ".xml", SwitchingMatrix.agentCollection.Last());
                   // File.AppendAllText("myNode" + Program.number + ".xml", SwitchingMatrix.agentCollection.Last());


                    //SwitchingMatrix.portDictionary.Clear();
                    //SwitchingMatrix.labelZeroDictionary.Clear();
                    // SwitchingMatrix.FillDictionary();
                    SwitchingMatrix.FillEonDictionary();
                }
                else if (SwitchingMatrix.agentCollection.Last().Contains("get_config"))
                {
                    //jezeli w wiadomosci jest polecenie by dac konfiguracje wezla, wysyla do wiadomosci zawartosc swojego xml-a
                    Send(XMLParser.StringNode());
                }
                //gdy agent chce informacje co siedzi w danym porcie
                else if (SwitchingMatrix.agentCollection.Last().Contains("get_matrix"))
                {
                    int start = SwitchingMatrix.agentCollection.Last().IndexOf("<get_matrix>");
                    int end = SwitchingMatrix.agentCollection.Last().IndexOf("</get_matrix>");
                    int matrix = Int32.Parse(SwitchingMatrix.agentCollection.Last().Substring(start + 12, end - start - 12));
                    //Console.WriteLine(matrix);
                    Send(XMLParser.StringMatrix(matrix));
                }
                else if(SwitchingMatrix.agentCollection.Last().Contains("ping"))
                {

                }
                else
                {
                    Send(SwitchingMatrix.agentCollection.Last());
                }
            }
        }



        //zdaje sie ze smiec
        public void AgentInfo()
        {
            lock (SwitchingMatrix.agentCollection)
            {
                SwitchingMatrix.agentCollection.CollectionChanged += SwitchAction;
            }
        }
    }
}


