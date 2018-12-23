using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NetworkNode
{
    class XMLParser
    {
        /// <summary>
        /// robi stringa z pliku konfiguracyjnego
        /// 
        /// </summary>
        /// <returns></returns>
        public static string StringNode()
        {
            XmlDocument xmlDefault = new XmlDocument();
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);

            string file;
            string readXML;
            int start, end;
            xmlDefault.Load("myNode" + Program.number + ".xml");
            xmlDefault.WriteTo(tx);
            readXML = sw.ToString();
            try
            {
                start = readXML.IndexOf("<node id=\"" + Program.number + "\">");

                end = readXML.IndexOf("</node>", start);
                file = readXML.Substring(start, end - start);
                file = file + "</node>";
                return file;
            }
            catch (Exception ex)
            {
                Console.WriteLine("nie ma wezlow, ex:" + ex.ToString());
                return null;
            }
        }
            /// <summary>
            /// zwraca dla danego wezla zawartosc dla danego portu
            /// </summary>
            /// <param name="number">numer portu ktorego szukamy</param>
            /// <returns></returns>
            public static string StringMatrix(int number)
            {
                XmlDocument xmlDefault = new XmlDocument();
                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);

                string file;
                string readXML;
                int start, end;
                xmlDefault.Load("myNode" + Program.number + ".xml");
                xmlDefault.WriteTo(tx);
                readXML = sw.ToString();
                try
                {
                    //znajduje gdzie jest poczatek w xml informacji o danym numerze
                    start = readXML.IndexOf("<matrix_entry num=\"" + number + "\">");
                    
                   //szuka konca tej informacji, szuka poczynajac od miejsca gdzie sie zaczela (start)
                    end = readXML.IndexOf("</matrix_entry>", start);

                    file = readXML.Substring(start, end - start);
                    //niestety wycinajac tracimy znacznik konca informacji, wiec go teraz dodajemy
                    file = file + "</matrix_entry>";
                    return file;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("nie ma matrixa, ex:" + ex.ToString());
                      //jezeli cos sie pochrzanilo  to zwracamy info ze nie ma takiego portu
                    return "nie ma takiego portu";
                }

            }



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
