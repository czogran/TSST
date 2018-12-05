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
using System.Xml;

namespace ClientNode
{
    class Agent
    {
        Socket mySocket;
        byte[] buffer;
        public  ObservableCollection<string> agentCollection = new ObservableCollection<string>();
        //kluczem jest klijent z jakim sie laczymy, a wartoscia jego adres, port wyjsciowey
        public static Dictionary<int, Tuple<string,int> >clientDictioinary = new Dictionary<int, Tuple<string,int>>();


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
            Console.WriteLine("connected Agent");
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

                Console.WriteLine("FROM Agent: " + receivedMessage);
                lock(agentCollection)
                {
                    agentCollection.Add(receivedMessage);
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
            lock (agentCollection)
            {
                string s = agentCollection.Last();
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

            lock (agentCollection)
            {
               agentCollection.CollectionChanged += SwitchAction;
            }
        }
       
        public  void SwitchAction(object sender, NotifyCollectionChangedEventArgs e)//(string message)
        {


            if (agentCollection.Last().Contains("client"))
            {
                File.WriteAllText("myClient" + Program.number + ".xml", agentCollection.Last());
                clientDictioinary.Clear();
                FillDictionary();
            }
        }

        public void FillDictionary()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load("myClient" + Program.number + ".xml");
            int id;
            string address;
            int portOut;
            XmlNode node1;
            foreach (XmlNode client in doc.SelectNodes("clients/client"))
            {
                    id = Int32.Parse(client.Attributes["id"].Value);
                   

                    node1 = client.SelectSingleNode("address");
                    address = node1.InnerText;
                    node1 = client.SelectSingleNode("port_out");
                    portOut =Int32.Parse( node1.InnerText);
                //var t = new Tuple<address, portOut>;
                clientDictioinary.Add(id, new Tuple<string,int>(address, portOut) );
                Console.WriteLine("uzupelniam slownik klijentow o klijenta:"+id);
            }
             
            

        }
    }
}

