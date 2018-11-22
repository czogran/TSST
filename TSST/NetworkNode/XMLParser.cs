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
        /// label_in, label_out, port_out
        /// </summary>
        public Dictionary<int, Tuple<int, int>> portTable;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public XMLParser()
        {
            portTable = new Dictionary<int, Tuple<int, int>>();
        }

        /// <summary>
        /// Odczyt pliku XML i zapis portów do słownika
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        /// </summary>
        public void ReadXml(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(filePath);

            foreach (XmlNode node in doc.SelectNodes("matrix_entry"))
            {
                var label_in = Int32.Parse(node.SelectSingleNode("label_in").InnerText);
                var label_out = Int32.Parse(node.SelectSingleNode("label_out").InnerText);
                var port_out = Int32.Parse(node.SelectSingleNode("port_out").InnerText);

                portTable.Add(label_in, new Tuple<int, int>(label_out, port_out));
            }
        }
    }
}
