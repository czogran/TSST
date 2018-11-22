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
        /// Konstruktor
        /// </summary>
        public XMLParser()
        {

        }

        /// <summary>
        /// Odczyt pliku XML i zapis portów do słownika
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        /// </summary>
        public Dictionary<int, string> ReadXml(string filePath)
        {
            Dictionary<int, string> configText = new Dictionary<int, string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode node in doc.SelectNodes("config/nodes/node"))
            {
                int id = int.Parse(node.Attributes["id"].Value);
                configText.Add(id, node.InnerXml);
            }

            foreach (XmlNode node in doc.SelectNodes("config/clients/client"))
            {
                int id = int.Parse(node.Attributes["id"].Value);
                configText.Add(id, node.InnerXml);
            }

            foreach (XmlNode node in doc.SelectNodes("config/cable_cloud"))
            {
                //chmura nie potrzebuje id, więc daję arbitralnie 0
                configText.Add(0, node.InnerXml);
            }
            return configText;
        }
    }
}
