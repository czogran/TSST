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

                Console.WriteLine(receivedMessage);


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
            sending = enc.GetBytes(message+"<port>9111</port>");

            mySocket.Send(sending);
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
                if (message == "end")
                {
                    disconnect_Click();
                    break;
                }    
                else
                    Send(message);
            }
            }
    }
}
    /*
    /// <summary>
    /// Port
    /// </summary>
    class Port
    {
        public static Socket sender;
        byte[] bytes = new byte[1024];

        public Port()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //  IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPAddress ipAddress = IPAddress.Parse("127.0.0.3");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11003);

                // Create a TCP/IP  socket.  
                sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);
                    Console.WriteLine("connected" );


                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
       public void send(string tekst)
        {
            try
            {
 
                byte[] msg = Encoding.ASCII.GetBytes(tekst);
           
                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);
                Console.WriteLine("test");
           
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

           
        }
        public void listen()
        {
            string data=null;
            int bytesRec = sender.Receive(bytes);
            Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
            while (true)
            {
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data.IndexOf("<EOF>") > -1)
                {

                    byte[] msg = Encoding.ASCII.GetBytes("data");

                    break;
                }             
            }
        }
        public void close()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
}
    }
   
    */