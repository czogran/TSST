using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    /// <summary>
    /// Zarządza węzłami
    /// </summary>
    class Manager
    {
        Socket mySocket;

        EndPoint endRemote;
        byte[] buffer;

        string myIp;
        int myport;

        public int number;

        public Manager()
        {
        }

        public Manager(int number)
        {
            this.number = number;
        }


        public void CreateSocket(string IP, int port)
        {
            
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
            mySocket.Connect(endRemote);
            buffer = new byte[1024];
            Console.WriteLine("polaczono z adresem: " + IP);

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

                Console.WriteLine(receivedMessage);

                SwitchingActions.Action(receivedMessage, this);

                buffer = new byte[1024];
                mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endRemote,
                    new AsyncCallback(MessageCallback), buffer);

            }
            catch (Exception ex)
            {

            }
        }
        public void Send(string message)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] sending = new byte[1024];
            sending = enc.GetBytes(message);

            mySocket.Send(sending);
        }

        public void disconnect_Click()
        {
            mySocket.Disconnect(true);
            mySocket.Close();
        }


        /// <summary>
        /// jest wywolywany jako watek i sprawdza czy wezly jeszcze zyja
        /// jak jakis zdechnie catch powinnien to wylapac
        /// i wtedy powinny zostac wycofane stare wpisy
        /// ustawione nowe
        /// i rozeslane
        /// </summary>
        public void PingThread()
        {
            while(true)
            {
                try
                {
                    System.Threading.Thread.Sleep(5000);
                    Send("ping");                   
                }
                catch
                {
                    Console.WriteLine("\nWezel:" + number + "  is dead");

                    //zamiast wywalac ustawiamy ze jest wylaczony
                    Program.nodes.Find(x => x.number == number).isAlive=false;
                    //var item =Program.nodes.SingleOrDefault(x => x.number== number);
                    //Program.nodes.Remove(item);

                    SwitchingActions.NodeIsDead(number);
                    break;
                }
            }
           
        }

        
    }
}
