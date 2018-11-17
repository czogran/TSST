using System;
using System.CodeDom;
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
        /// <summary>
        /// Sockecik
        /// </summary>
        public Socket socket;

        /// <summary>
        /// Lista adresów wszystkich węzłów
        /// </summary>
        public Dictionary<int, string> networkNodeIPAddresses = new Dictionary<int, string>();
        
        /// <summary>
        /// Adres końca portu
        /// </summary>
        public IPEndPoint ipEndPoint;

        /// <summary>
        /// Przysłana od agenta odpowiedź
        /// </summary>
        private string response;
        
        /// <summary>
        /// Adres portu
        /// </summary>
        private const string address = "127.0.0.30";

        /// <summary>
        /// Nr portu
        /// </summary>
        private int port;

        /// <summary>
        /// parser
        /// </summary>
        public XMLParser parser;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="port">Nr portu</param>
        public Manager(int port)
        {
            parser = new XMLParser();
            //TODO: nołdy są na razie hardkodowane, dodać je w batchu
            networkNodeIPAddresses.Add(1, "127.0.0.10");
            networkNodeIPAddresses.Add(2, "127.0.0.3");
            this.port = port;
            ipEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
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
                CreateSocket();

                socket.Connect(receiver, port);
                socket.Send(ASCIIEncoding.ASCII.GetBytes(data));
                Console.WriteLine("SENDING");

                Close();
            }
            catch(Exception e)
            {
                Console.Write(e.ToString());
            }
        }
        
        /// <summary>
        /// Nasłuchuje czy przychodzą dane
        /// </summary>
        public void Listen()
        {
            try
            {
                socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.IP);
                socket.Bind(ipEndPoint);
                
                socket.Listen(1);
                Console.WriteLine("LISTENING");

                Socket handler = socket.Accept();
                response = ReceiveData(handler);
                Console.WriteLine(response);
                
                socket.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Inicjalizacja działania sieci
        /// </summary>
        public void Init()
        {
            Dictionary<int, string> nodeFiles = parser.ReadXml("config.xml");

            foreach (KeyValuePair<int, string> ipAddress in networkNodeIPAddresses)
            {
                SendData(ipAddress.Value, nodeFiles[ipAddress.Key]);
                Listen(); 
            }

            SendData("127.0.0.1", nodeFiles[0]);
            Listen();
            Console.WriteLine("Pliki konfiguracyjne poprawnie wysłane");
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

        /// <summary>
        /// Tworzy socketa
        /// </summary>
        private void CreateSocket()
        {
            socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.IP);
        }

        /// <summary>
        /// Zamyka socketa
        /// </summary>
        private void Close()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
