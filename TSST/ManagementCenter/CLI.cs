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


        /// <summary>
        /// opcje dostepnych polecen
        /// </summary>
        internal static void Prompt()
        {
            Console.WriteLine("Dostępne komendy:");
            Console.WriteLine("[\"esc\"] Wyjdz z komendy");
            Console.WriteLine("[1] Konfiguracja polaczen");
            Console.WriteLine("[2] Konfiguracja klijentow");
            Console.WriteLine("[3] Zczytaj wezel");
            Console.WriteLine("[4] Zczytaj port wezla");
        }

        /// <summary>
        /// wysyla klijentom info na jaki port maja wysylac
        /// </summary>
        /// <param name="clientAmount"></param>
        /// <param name="managerClient"></param>
        public static void ConfigureClients(int clientAmount,List<Manager> managerClient)
        {
            CLI.RequestXML();
            string name;
            do
            {
                name = Console.ReadLine();

                if (name == "esc")
                {
                    break;
                }
                XML.SetName(name);
            } while (XML.Test() != true);
            if (name != "esc")
            {
                List<int> portOut = XMLeon.GetClientPortOut(name);
                for (int i = 0; i < clientAmount; i++)
                {
                    try
                    {
                        managerClient[i].Send("port_out:"+portOut[i]);
                        Console.WriteLine("Wysylam info o porcie:" + portOut[i]);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Blad wysylania informacji o porcie");
                    }
                }
            }
        } 

        /// <summary>
        /// wysyla chmurze xml z linkami
        /// </summary>
        /// <param name="managerCloud"></param>
        public static void ConfigureLinkConnections(Manager managerCloud)
        {
            CLI.RequestXML();
            string name;
            do
            {
                name = Console.ReadLine();

                if (name == "esc")
                {
                    break;
                }
                XML.SetName(name);
            } while (XML.Test() != true);

            if (name != "esc")

            {
                XMLParser xml = new XMLParser(name);
                Program.links = xml.GetLinks();
                managerCloud.Send(XML.StringCableLinks());
                CLI.PrintConfigFilesSent();
            }
        }

        /// <summary>
        /// konfiguracja sieci w mpls
        /// </summary>
        public static void Configure(int nodeAmount, List<Manager> manager,int clientAmount, List<Manager> managerClient,Manager managerCloud)
        {
            CLI.RequestXML();
            string name;
            do
            {
                name = Console.ReadLine();
               
                if (name == "esc")
                {
                    break;
                }
                XML.SetName(name);
            } while (XML.Test() != true);
            if (name != "esc")
            {
                for (int i = 1; i <= nodeAmount; i++)
                {
                    try
                    {
                        manager[i - 1].Send(XML.StringNode(i));
                    }
                    catch (Exception ex)
                    {

                    }
                }

                for (int i = 1; i <= clientAmount; i++)
                {
                    try
                    {
                        managerClient[i - 1].Send(XML.StringClients());
                    }
                    catch (Exception ex)
                    {

                    }
                }
                managerCloud.Send(XML.StringCableLinks());
                CLI.PrintConfigFilesSent();
            }
        }
        /// <summary>
        /// naprawa sieci
        /// </summary>
        public static void Fix(int nodeAmount,List<Manager> manager)
        {
            CLI.RequestXML();
            string name;
            do
            {

               name = Console.ReadLine();

                if (name == "esc")
                {
                    break;
                }

                XML.SetName(name);
            } while (XML.Test() != true);
            if (name != "esc")
            {

                for (int i = 1; i <= nodeAmount; i++)
                {
                    try
                    {
                        manager[i - 1].Send(XML.StringNode(i));
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        /// <summary>
        /// prosi wezel o wpis z niego
        /// </summary>
        public static void GetNodeFromNode (List<Manager> manager)
        {
            Console.WriteLine("Podaj wezla od ktorego chcesz pobrac plik konfiguracyjny");
            int number;
            string input;
            while (true)
            {
                try
                {
                    input = Console.ReadLine();
                    if (input == "esc")
                    {
                        break;
                    }
                    number = Int32.Parse(input);
                    manager[number - 1].Send("DAWAJ CONFIGA FRAJERZE,get_config");
                    break;
                }
                catch(Exception ex)
                {                 
                    Console.WriteLine("Podaj poprawny numer");
                }
            }
        }


        /// <summary>
        /// gdy chcemy uzyskac od noda info co siedzi u niego w danym porcie wywolujemy to
        /// wrzuca ta funkcja zdarzenie- wysylanie z info co chcemy od noda
        /// </summary>
        /// <param name="manager"></param>
        public static void GetMatrixFromNode(List<Manager> manager)
        {
            Console.WriteLine("Podaj wezla od ktorego chcesz pobrac dane portu");
            int nodeNumber, matrixNumber;
            string input;
            while (true)
            {
                try
                {
                    nodeNumber = Int32.Parse(Console.ReadLine());
                    try
                    {
                        Console.WriteLine("Podaj id portu");
                        input = Console.ReadLine();
                        if (input == "esc")
                        {
                            break;
                        }
                        matrixNumber = Int32.Parse(input);
                        manager[nodeNumber - 1].Send("<get_matrix>"+matrixNumber+"</get_matrix>");
                        break;
                    }
                    catch(Exception ex)
                    {

                    }

                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Podaj poprawny numer");
                }
            }
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