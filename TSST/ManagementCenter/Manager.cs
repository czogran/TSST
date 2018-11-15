﻿using System;
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
        private List<string> networkNodeIPAddresses;
        
        /// <summary>
        /// Adres końca portu
        /// </summary>
        public IPEndPoint ipEndPoint;
        
        public string test;    ///do usuniecia

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
            List<string> node_configs = parser.ReadXml("config.xml");
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
        public string Listen()
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
            return "";
        }


        public string NodeInitWait()
        {
            try
            {
                socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.IP);
                socket.Bind(ipEndPoint);

                socket.Listen(1);
                Console.WriteLine("LISTENING");

                Socket handler = socket.Accept();
                string result = ReceiveData(handler);
                test = result + "0";
                Console.WriteLine(result);

                socket.Close();
                var endPoint = (IPEndPoint)handler.RemoteEndPoint;

                return endPoint.Address.ToString();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return "";
        }

        public void InitNodes()
        {
            for (int i = 0; i < parser.config_text.Count; i++)
            {
                string x = NodeInitWait();
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
    }
}
