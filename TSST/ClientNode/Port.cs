using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace ClientNode
{
    /// <summary>
    /// Port
    /// </summary>
    class Port
    {
        /// <summary>
        /// Sockecik do słuchania
        /// </summary>
        public Socket listener;

        /// <summary>
        /// Sockecik do wysyłania
        /// </summary>
        public Socket sender;

        /// <summary>
        /// Otrzymana wiadomość
        /// </summary>
        public string receivedData;

        /// <summary>
        /// Numer kabla, do którego podłączony jest host
        /// </summary>
        public int connectedPortNumber;

        /// <summary>
        /// Adres końca portu listenera
        /// </summary>
        private IPEndPoint listenerIpEndPoint;

        /// <summary>
        /// Adres końca portu sendera
        /// </summary>
        private IPEndPoint senderIpEndPoint;

        /// <summary>
        /// Adres listenera
        /// </summary>
        private string listenerAddress;

        /// <summary>
        /// Adres sendera
        /// </summary>
        private string senderAddress;

        /// <summary>
        /// Nr portu
        /// </summary>
        private int port;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="listenerAddress">adres listenera</param>
        /// <param name="senderAddress">adres sendera</param>
        /// <param name="port">Nr portu</param>
        public Port(string listenerAddress, string senderAddress, int port)
        {
            this.listenerAddress = listenerAddress;
            this.senderAddress = senderAddress;
            this.port = port;
            listenerIpEndPoint = new IPEndPoint(IPAddress.Parse(listenerAddress), port);
            senderIpEndPoint = new IPEndPoint(IPAddress.Parse(senderAddress), port);
        }
        
        /// <summary>
        /// Nasłuchuje czy przychodzą dane
        /// </summary>
        public void Listen()
        {
            try
            {
                listener = new Socket(listenerIpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.IP);
                listener.Bind(listenerIpEndPoint);
                    
                listener.Listen(1);
                Console.WriteLine("LISTENING");
    
                Socket handler = listener.Accept();
                receivedData = ReceiveData(handler);
                Console.WriteLine(receivedData);
                    
                listener.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Pętla do wątku sendera
        /// </summary>
        public void SenderLoop()
        {
            while (true)
            {
                var message = Console.ReadLine();
                SendData("127.0.0.1", message);
            }
        }

        public void ListenerLoop()
        {
            while (true)
            {
                Listen();
            }
        }
        
        /// <summary>
        /// Wysyła strumień bitów
        /// </summary>
        /// <param name="receiver">adres na który wysłane będą dane</param>
        /// <param name="data">dane do wysłania</param>
        public void SendData(string receiver, string data)
        {
            try
            {
                CreateSender();

                sender.Connect(receiver, port);
                sender.Send(ASCIIEncoding.ASCII.GetBytes(data));

                Close();
            }
            catch(Exception e)
            {
                Console.Write(e);
            }
        }
        
        /// <summary>
        /// Zwraca otrzymane dane
        /// </summary>
        /// <returns>odebrany string</returns>
        private string ReceiveData(Socket handler)
        {
            string result ="";
            int bytes = 0;
            Byte[] buffer = new Byte[256];

            bytes = handler.Receive(buffer, buffer.Length, 0);
            result = Encoding.ASCII.GetString(buffer, 0, bytes);
            
            return result;
        }

        private void CreateSender()
        {
            sender = new Socket(senderIpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.IP);
        }

        /// <summary>
        /// Zamyka sendera
        /// </summary>
        private void Close()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
