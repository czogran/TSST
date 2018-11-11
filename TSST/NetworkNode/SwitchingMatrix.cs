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
        private Dictionary<int, int> portTable;

        public SwitchingMatrix()
        {
            portTable = new Dictionary<int, int>();
        }
        
        
        /// <summary>
        /// Przepisanie portów do słownika z XMLa
        /// </summary>
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        public void SetPortTable(string filePath)
        {
            XMLParser xml = new XMLParser();
            xml.ReadXml(filePath);

            foreach (KeyValuePair<int, int> kvp in xml.portTable)
            {
                portTable.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Wypisanie słownika na konsolę
        /// </summary>
        public void PrintPortTable()
        {
            foreach (KeyValuePair<int, int> kvp in portTable)
            {
                Console.WriteLine(string.Format("Port_in = {0}, Port_out = {1}", kvp.Key, kvp.Value));
            }
        }
    }
}
