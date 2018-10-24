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
        private Socket socket;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Port()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        /// <summary>
        /// Wysyła strumień bitów
        /// </summary>
        /// <param name="server">adres serwera</param>
        /// <param name="port">numer portu</param>
        /// <param name="data">dane do wysłania</param>
        public void SendData(string server, int port, string data)
        {
            socket.Connect(server, port);
            socket.Send(ASCIIEncoding.ASCII.GetBytes(data));
            Console.WriteLine("SENDING");
        }
        
        /// <summary>
        /// Nasłuchuje czy przychodzą dane
        /// </summary>
        /// <param name="server">adres serwera</param>
        /// <param name="port">numer portu</param>
        public void Listen(string server, int port)
        {            
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1));
            socket.Listen(1);
            Console.WriteLine("LISTENING");
            
            socket = socket.Accept();
            string result = ReceiveData();
            Console.WriteLine(result);
        }
        
        /// <summary>
        /// Zwraca otrzymane dane
        /// </summary>
        /// <returns>odebrany string</returns>
        private string ReceiveData()
        {
            string result ="";
            int bytes = 0;
            Byte[] buffer = new Byte[256];

            bytes = socket.Receive(buffer, buffer.Length, 0);
            result = Encoding.ASCII.GetString(buffer, 0, bytes);
            
            return result;
        }
    }
}
