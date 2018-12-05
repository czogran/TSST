using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    /// <summary>
    /// CLI programu
    /// </summary>
    class CLI
    {
        /// <summary>
        /// Treść komendy wysłania prośby o tablicę routingu
        /// format: table id_routera.
        /// </summary>
        private const string printRoutingTableCommand = "table";

        /// <summary>
        /// Printuje się po wpisaniu nieprawidłowej komendy
        /// </summary>
        private const string invalidCommand = "Nierozpoznana komenda";

        /// <summary>
        /// Printuje się po podaniu nieprawidłowych parametrów
        /// </summary>
        private const string invalidParameters = "Nieodpowiednie parametry";

        /// <summary>
        /// Wypisuje błąd podczas wysyłania
        /// </summary>
        public static void PrintError()
        {
            Console.WriteLine("Błąd podczas wysyłania");
        }

        /// <summary>
        /// Powiadamia że wysłało XMLa
        /// </summary>
        /// <param name="name">nazwa xmla</param>
        /// <param name="port">nr portu</param>
        public static void PrintSentXML(string name, int port)
        {
            Console.WriteLine($"{DateTime.Now} Wysyłam plik XML: {name} do węzła {port}");
        }

        /// <summary>
        /// Printuje treść otrzymanej wiadomości
        /// </summary>
        /// <param name="message">treść wiadomości</param>
        public static void PrintReceivedMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now} {message}");
        }

        /// <summary>
        /// Informuje o wysłaniu wszystkich plików konfiguracyjnych
        /// </summary>
        public static void PrintConfigFilesSent()
        {
            Console.WriteLine("Pliki konfiguracyjne poprawnie wysłane");
        }

        /// <summary>
        /// Gruba sprawa, nie umiem jeszcze zaimplementować. Proszę o propozycje
        /// </summary>
        public static void PrintRoutingTable()
        {

        }

        internal static void NodeNum(int nodeAmount)
        {
            Console.WriteLine($"Liczba węzłów: {nodeAmount}");
        }


        /// <summary>
        /// Waliduje wpisane bazgroły sprawdzając czy to komenda
        /// </summary>
        /// <param name="line">wpisana w konsolę linia</param>
        /// <returns>Czy komenda jest poprawna</returns>
        public bool ValidateCommand(string line)
        {
            var command = line.IndexOf(" ") > -1
                ? line.Substring(0, line.IndexOf(" "))
                : line;

            bool isCorrect;

            switch (command)
            {
                case printRoutingTableCommand:
                    isCorrect = ValidateTableCommand(line);
                    break;
                default:
                    Console.WriteLine(invalidCommand);
                    isCorrect = false;
                    break;
            }

            return isCorrect;
        }

        internal static void Prompt()
        {
            Console.WriteLine("Dostępne komendy:");
            Console.WriteLine("[1] Konfiguracja");
            Console.WriteLine("[2] Naprawa");
        }

        /// <summary>
        /// Waliduje komendę podania tablicy routingu
        /// format: table id_routera
        /// </summary>
        /// <param name="line">wpisana linia</param>
        /// <returns>czy komenda jest poprawna</returns>
        private bool ValidateTableCommand(string line)
        {
            string[] command = line.Split(' ');
            if (command.Length > 2)
            {
                Console.WriteLine(invalidParameters);
                return false;
            }
            else if (int.TryParse(command[1], out int x))
            {
                return true;
            }
            else
            {
                Console.WriteLine(invalidParameters);
                return false;
            }
        }

        internal static void RequestXML()
        {
            Console.WriteLine("Podaj plik XML:");
        }

        public static void ClientNum(string args)
        {
            Console.WriteLine($"Liczba klientów: {args}");
        }

        public static void CreateClientAgent(int c)
        {
            Console.WriteLine($"Tworze agenta dla klienta {c}");
        }


        public static void CreateNodeAgent(string v)
        {
            Console.WriteLine($"Tworze agenta dla węzła {v}");
        }
    }
}