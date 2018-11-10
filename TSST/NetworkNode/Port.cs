using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace NetworkNode
{
    /// <summary>
    /// Port
    /// </summary>
    class Port
    {
        /// <summary>
        /// Sockecik do komunikacji z chmurką
        /// </summary>
        public Socket socket;

        /// <summary>
        /// Adres końca portu
        /// </summary>
        public IPEndPoint ipEndPoint;

        public string test;    ///do usuniecia

        /// <summary>
        /// Adres portu
        /// </summary>
        private string address;

        /// <summary>
        /// Nr portu
        /// </summary>
        private int port;

        /// <summary>
        /// Słownik portów wejścia/wyjścia
        /// </summary>
        private Dictionary<int, int> portTable;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="address">adres portu</param>
        /// <param name="port">Nr portu</param>
        public Port(string address, int port)
        {
            this.address = address;
            this.port = port;
            ipEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            portTable = new Dictionary<int, int>();
        }

        /// <summary>
        /// Głowna pętla portu. Trzeba tak zrobić bo wątek potrzebuje delegata tej metody
        /// </summary>
        public void Execute()
        {
            while (true)
            {
                Listen();
                SendData("127.0.0.1", test);
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
