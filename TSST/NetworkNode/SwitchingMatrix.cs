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
        /// label_in, label_out, port_out
        /// </summary>
        private Dictionary<int, Tuple<int, int>> portTable;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public SwitchingMatrix()
        {
            portTable = new Dictionary<int, Tuple<int, int>>();
        }
        
        
        /// <summary>
        /// Przepisanie portów do słownika z XMLa
        /// </summary>
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        public void SetPortTable(string filePath)
        {
            XMLParser xml = new XMLParser();
            xml.ReadXml(filePath);

            foreach (KeyValuePair<int, Tuple<int,int>> kvp in xml.portTable)
            {
                portTable.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Wypisanie słownika na konsolę
        /// </summary>
        public void PrintPortTable()
        {
            foreach (KeyValuePair<int, Tuple<int,int>> kvp in portTable)
            {
                Console.WriteLine("Label_in = {0}, Label_out = {1}, Port_out = {2}", kvp.Key, kvp.Value.Item1, kvp.Value.Item2);
            }
        }
    }
}
