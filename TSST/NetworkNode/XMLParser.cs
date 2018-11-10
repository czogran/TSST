using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NetworkNode
{
    class XMLParser
    {
        /// <summary>
        /// Słownik portów wejścia/wyjścia
        /// </summary>
        public Dictionary<int, int> portTable;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public XMLParser()
        {
            portTable = new Dictionary<int, int>();
        }

        /// <summary>
        /// Odczyt pliku XML i zapis portów do słownika
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        /// </summary>
        public void ReadXml(string filePath)
        {
            int port_in, port_out;
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode node in doc.SelectNodes("config_file/port"))
            {
                port_in = Int32.Parse(node.SelectSingleNode("port_in").InnerText);
                port_out = Int32.Parse(node.SelectSingleNode("port_out").InnerText);

                portTable.Add(port_in, port_out);
            }
        }
    }
}
