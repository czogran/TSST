using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ManagementCenter
{
    class XMLParser
    {
        /// <summary>
        /// Słownik portów wejścia/wyjścia
        /// </summary>
        public List<string> config_text;
        public int node_num;
        /// <summary>
        /// Konstruktor
        /// </summary>
        public XMLParser()
        {
            config_text = new List<string>();
        }

        /// <summary>
        /// Odczyt pliku XML i zapis portów do słownika
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        /// </summary>
        public List<string> ReadXml(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode node in doc.SelectNodes("config/nodes"))
            {
                config_text.Add(node.InnerXml);
                
            }
            return config_text;
        }
    }
}
