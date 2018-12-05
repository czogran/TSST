using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
{
    class CLI
    {
        internal static void ClientsCount(string clients)
        {
            Console.WriteLine($"Znalezniono {clients} klientów");
        }

        internal static void ClientConnected()
        {
            Console.WriteLine($"Połączono z klientem");
        }

        internal static void ConnectedAgent()
        {
            Console.WriteLine("Połączono Agenta");
        }
    }
}
