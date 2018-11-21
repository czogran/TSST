﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CableCloud
{

    class SocketCloud
    { 
        Socket mySocket;
        Socket listeningSocket;

    byte[] buffer;

    public SocketCloud()
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
        mySocket= mySocket.Accept();
        Console.WriteLine("connected");
    
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

            Console.WriteLine("FROM CLIENT: "+receivedMessage);
                lock(Switch.collection)
                {
                    Switch.collection.Add(receivedMessage);
                }

            buffer = new byte[1024];
            mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
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
    class SocketCloud
    {
        public Socket node;
        byte[] bytes = new byte[1024];


        public SocketCloud(string ip,int port)
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
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.  
                node = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    node.Connect(remoteEP);
                   Console.WriteLine("connected to node");

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
        public static string  send(string tekst,SocketCloud sender)
        {
            string get=null;
            try
            {
                tekst = "to node<EOF>";
                byte[] bytes = new byte[1024];
                byte[] msg = Encoding.ASCII.GetBytes(tekst);


                // Send the data through the socket.  
                int bytesSent = sender.node.Send(msg);//SocketCloud.node.Send(msg);

                Console.WriteLine("test send");
                // Receive the response from the remote device.  
                int bytesRec = sender.node.Receive(bytes);

                Console.WriteLine("Echoed test send = {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));
                get = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                // Release the socket.  

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

            return get;
        }
        public void close()
        {
            node.Shutdown(SocketShutdown.Both);
            node.Close();
        }

    }
}
*/