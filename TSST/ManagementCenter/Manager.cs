﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
            Console.WriteLine("Połączono z adresem: " + IP);

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


                try
                {
                    SwitchingActions.possibleWindow = (bool[])DeserializeFromStream(auxtrim);

                    Console.WriteLine("Deserializacja");
                }
                catch
                {
                    // Console.WriteLine("Nie udalo sie deserializacja");
                }
                try
                {
                    string receivedMessage = encoding.GetString(auxtrim);

                    Console.WriteLine();
                    Console.Write(this.GetTimestamp() + " : ");
                    Console.WriteLine("Manager otrzymal wiadomosc: " + receivedMessage);
                    try
                    {
                        SwitchingActions.Action(receivedMessage, this);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Zła akcja: " + receivedMessage);
                        Console.WriteLine(ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd odkodowywania");


                }



                buffer = new byte[1024];
                mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endRemote,
                    new AsyncCallback(MessageCallback), buffer);

            }
            catch (Exception ex)
            {

            }
        }



        public static object DeserializeFromStream(byte[] receivedData)
        {
            MemoryStream stream = new MemoryStream(receivedData);
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object objectType = formatter.Deserialize(stream);
            return objectType;
        }

        public void Send(string message)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] sending = new byte[1024];
            sending = enc.GetBytes(message);

            mySocket.Send(sending);

            if (!message.Contains("ping"))
            {
                Console.Write(this.GetTimestamp() + " : ");
                Console.WriteLine("Manager wyslal wiadomość na adres "+myIp+" o treści: " + message);
            }
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
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(5000);
                    Send("ping");
                }
                catch
                {
                    Console.Write(this.GetTimestamp() + " : ");
                    Console.WriteLine("\nWęzeł: " + number + "  jest nieaktywny");

                    ////zamiast wywalac ustawiamy ze jest wylaczony
                    //Program.nodes.Find(x => x.number == number).isAlive=false;

                    //wersja z wywalniem noda
                    var item = Program.nodes.SingleOrDefault(x => x.number == number);
                    Program.nodes.Remove(item);

                    AgentSwitchingAction.NodeIsDead(number);
                    break;
                }
            }

        }

        public string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

    }
}
