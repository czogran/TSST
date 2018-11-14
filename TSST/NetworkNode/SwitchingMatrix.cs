using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Pole komutacyjne
    /// </summary>
    class SwitchingMatrix
    {
        /// <summary>
        /// Słownik portów wejścia/wyjścia
        /// </summary>
        private Dictionary<int, Tuple<int, string>> portTable;

        public SwitchingMatrix()
        {
            portTable = new Dictionary<int, Tuple<int, string>>();
        }
        
        
        /// <summary>
        /// Przepisanie portów do słownika z XMLa
        /// </summary>
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        /// <param name="ipNodeAddress">adres ip węzła</param>
        public void SetPortTable(string filePath, string ipNodeAddress)
        {
            XMLParser xml = new XMLParser();
            xml.ReadXml(filePath);
            Tuple<int, string> tuple;

            foreach (KeyValuePair<int, int> kvp in xml.portTable)
            {
                tuple = Tuple.Create<int, string>(kvp.Value, ipNodeAddress);
                portTable.Add(kvp.Key, tuple);
            }
        }

        /// <summary>
        /// Wypisanie słownika na konsolę
        /// </summary>
        public void PrintPortTable()
        {
            foreach (KeyValuePair<int, Tuple<int, string>> kvp in portTable)
            {
                Console.WriteLine(string.Format("Port_in = {0}, Port_out = {1}, Node_ip = {2}", kvp.Key, kvp.Value.Item1, kvp.Value.Item2));
            }
        }
    }
}
