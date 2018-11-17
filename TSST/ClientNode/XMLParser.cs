using System;
using System.Xml;

namespace ClientNode
{
    public class XMLParser
    {
        /// <summary>
        /// Odczyt pliku XML i zapis portów do słownika
        /// <param name="filePath">ścieżka do pliku konfiguracyjnego</param>
        /// </summary>
        public int ReadXml(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(filePath);

            return Int32.Parse(doc.SelectSingleNode("port_out").InnerText);
        }
    }
}