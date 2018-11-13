using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace CableCloud
{
    /// <summary>
    /// Chmura kablowa ze wszystkimi socketami
    /// </summary>
    class CableCloud
    {
        /// <summary>
        /// Sockecik
        /// </summary>
        public Socket socket;

        /// <summary>
        /// Adres końca portu receivera
        /// </summary>
        public IPEndPoint ipEndPoint;

        /// <summary>
        /// Adres
        /// </summary>
        private const string addresss = "127.0.0.1";

        /// <summary>
        /// Nr portu
        /// </summary>
        private const int port = 10000;

        public string test; //do usuniecia

        /// <summary>
        /// Słownik portów wejścia/wyjścia
        /// </summary>
        private Dictionary<int, int> portTable;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public CableCloud()
        {
            ipEndPoint = new  IPEndPoint(IPAddress.Parse(addresss), port);
            portTable = new Dictionary<int, int>();
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

                socket.Listen(2);
                Console.WriteLine("LISTENING");

                Socket handler = socket.Accept();
                string result = ReceiveData(handler);
                test = result+"0";
                Console.WriteLine(result);
                
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        /// <summary>
        /// Wysyła strumień bitów
        /// </summary>
        /// <param name="receiverAddress">adres na który wysłane będą dane</param>
        /// <param name="data">dane do wysłania</param>
        public void SendData(string receiverAddress, int receiverPort, string data)
        {
            try
            {
                CreateSocket();

                socket.Connect(receiverAddress, receiverPort);
                socket.Send(ASCIIEncoding.ASCII.GetBytes(data));
                Console.WriteLine("SENDING");

                Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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

        /// <summary>
        /// Przepisanie portów do słownika z XMLa
        /// </summary>
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        public void SetPortTable(string filePath)
        { 
            XMLParser xml = new XMLParser();
            xml.ReadXml(filePath);
            
            foreach (KeyValuePair<int, int> kvp in xml.portTable)
            {
                portTable.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Wypisanie słownika na konsolę
        /// </summary>
        public void PrintPortTable()
        {
            foreach (KeyValuePair<int, int> kvp in portTable)
            {
                Console.WriteLine(string.Format("Port_in = {0}, Port_out = {1}", kvp.Key, kvp.Value));
            }
        }

    }
}
