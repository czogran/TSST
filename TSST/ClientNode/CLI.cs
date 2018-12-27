﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientNode
{
    class CLI
    {
        internal static void ConnectedAgent()
        {
            Console.WriteLine("Połączono agenta");
        }

        internal static void Promt()
        {
            Console.WriteLine();
            Console.WriteLine("KOMENDY:");
            Console.WriteLine("zestaw polaczenie,po dwukropku id odbiorcy://connection:");
            Console.WriteLine("wybierz odbiorce, po dwukropku numer odbiorcy://client:");
            Console.WriteLine("po wybraniu odbiorcy zwykle pisanie jest wysylaniem");
            Console.WriteLine("usun polaczenie, po dwukropku numer polaczenia://usun:");
            Console.WriteLine("spamuj odbiorce wpisz://send  teraz kliknij enter i wiadomosc ktora chcesz spamowac");
            Console.WriteLine();

        }
        internal static void SwitchCommands(Port clientNode)
        {
            while (true)
            {
                string message = Console.ReadLine();
                if (message.Contains("//client:"))
                {
                    try
                    {
                        clientNode.client = Int32.Parse(message.Substring(9));
                        Console.WriteLine("Odbiorcą jest klient: " + clientNode.client);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Zła komenda, spróbuj ponownie.");
                    }

                }
                else if (message.Contains("//send"))
                {
                    message = Console.ReadLine();
                    for (int i = 0; i < 10; i++)
                    {
                        clientNode.Send(message, clientNode.client);
                        Thread.Sleep(500);
                    }
                }
                else if (message.Contains("//connection:"))
                {
                    try
                    {
                        int connection = Int32.Parse(message.Substring(13));
                        Console.WriteLine("Prosze o zestawienie polaczenia z klijentem:" + connection);
                        clientNode.SendCommand(message);

                      
                    }
                    catch
                    {
                        Console.WriteLine("Zła komenda, spróbuj ponownie.");
                    }
                }
                else
                {
                    clientNode.Send(message, clientNode.client);
                }
            }
        }
    }
}
