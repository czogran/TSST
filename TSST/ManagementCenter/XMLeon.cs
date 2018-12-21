using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ManagementCenter
{
    class XMLeon
    { 
    
        //lista funkcji:
        //AddNode
        //StringNode -testnac jeszcze
        //StringCableLinks- testnac
        //StringClients- testnac- te funkcje kiedy dzialaly
        //AddClient
        //AddMatrix
        //AddConnection
        //RemoveConnection
        //AddLink
        //ChangeLinkStatus
        //RemoveNode
        //RemoveClient-testnac
        //RemoveLink



        /// <summary>
        /// plik na ktorym zasowamy
        /// </summary>
        XmlDocument xmlDoc;
        ///nazwa dokumentu na ktorym pracujemy
        string name;

        /// <summary>
        /// typy plikow jakie morzemy utworzyc
        /// </summary>
        public enum Type { nodes, cable_cloud, clients };

        /// <summary>
        /// pomysl taki by bylodzielny xml na wezly, klijenty i lacza
        /// wazna by name podawac z koncowka xml
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">czy lacze, wezly, </param>
        /// 
        public XMLeon(string name, Type type)
        {
            this.name = name;
            xmlDoc = new XmlDocument();
            XmlNode config = xmlDoc.CreateElement("config");
            XmlNode nodes = xmlDoc.CreateElement(type.ToString());

            config.AppendChild(nodes);

            xmlDoc.AppendChild(config);
            xmlDoc.Save(name);
        }


        public void AddNode(int id, string addressForCloud, string agent)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);

            XmlNode node = xmlDoc.CreateElement("node");
            XmlNode cablePort = xmlDoc.CreateElement("cable_port");
            cablePort.InnerText = addressForCloud;

            XmlNode Agent = xmlDoc.CreateElement("agent");
            Agent.InnerText = agent;

            XmlAttribute attribute = xmlDoc.CreateAttribute("id");
            attribute.Value = id.ToString();
            node.Attributes.Append(attribute);
            node.AppendChild(cablePort);
            node.AppendChild(Agent);
            XmlNode addTo = xmlDoc.DocumentElement.SelectSingleNode("nodes");
            addTo.AppendChild(node);
            xmlDoc.Save(name);
        }



        public string StringNode(int id)
        {
            XmlDocument xmlDefault = new XmlDocument();
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);

            string file;
            string readXML;
            int start, end;
            xmlDefault.Load(name);
            xmlDefault.WriteTo(tx);
            readXML = sw.ToString();
            try
            {
                start = readXML.IndexOf("<node id=\"" + id + "\">");

                end = readXML.IndexOf("</node>", start);
                file = readXML.Substring(start, end - start);
                file = file + "</node>";
                return file;
            }
            catch(Exception ex)
            {
                Console.WriteLine("nie ma wezlow, ex:"+ex.ToString());
                return null;
            }
        }

        public string StringCableLinks()
        {
            XmlDocument xmlDefault = new XmlDocument();
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);

            string file;
            string readXML;
            int start, end;
            xmlDefault.Load(name);
            xmlDefault.WriteTo(tx);
            readXML = sw.ToString();
            try
            {
                start = readXML.IndexOf("<cable_cloud");

                end = readXML.IndexOf("</cable_cloud>", start);
                file = readXML.Substring(start, end - start);
                file = file + "</cable_cloud>";
                return file;
            }
            catch(Exception ex)
            {
                Console.WriteLine("StringCableLinks:nie ma cable clouda, ex:" + ex.ToString());
                return null;
            }
        }
        public string StringClients()
        {
            XmlDocument xmlDefault = new XmlDocument();
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);

            string file;
            string readXML;
            int start, end;
            xmlDefault.Load(name);
            xmlDefault.WriteTo(tx);
            readXML = sw.ToString();
            try
            {
                start = readXML.IndexOf("<clients");

                end = readXML.IndexOf("</clients>", start);
                file = readXML.Substring(start, end - start);
                file = file + "</clients>";
                return file;
            }
            catch(Exception ex)
            {
                Console.WriteLine("StringClients: nie ma klijenta, ex:" + ex.ToString());
                return null;
            }
        }

        public void AddClient(int id, string clientAddress, int port_out)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);
            XmlNode client = xmlDefault.CreateElement("client");
            XmlAttribute attribute = xmlDefault.CreateAttribute("id");
            attribute.Value = id.ToString();
            client.Attributes.Append(attribute);
            XmlNode address = xmlDefault.CreateElement("address");
            address.InnerText = clientAddress;
            client.AppendChild(address);
            XmlNode port = xmlDefault.CreateElement("port_out");
            port.InnerText = port_out.ToString();
            client.AppendChild(port);
            try
            {
                XmlNode addTo = xmlDefault.DocumentElement.SelectSingleNode("clients");
                addTo.AppendChild(client);
                xmlDefault.Save(name);
            }
            catch(Exception ex)
            {
                Console.WriteLine("AddClient, zly klijent, ex:" + ex.ToString());
            }
        }


        public void AddMatrix(int num, int nodeNumber)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);
            XmlNode matrix = xmlDefault.CreateElement("matrix_entry");
            XmlAttribute attribute = xmlDefault.CreateAttribute("num");
            attribute.Value = num.ToString();
            matrix.Attributes.Append(attribute);

            try
            {
                XmlNode addTo = xmlDefault.DocumentElement.SelectSingleNode("//node[@id=" + nodeNumber + "]");
                addTo.AppendChild(matrix);
                xmlDefault.Save(name);
            }
            catch(Exception ex)
            {
                Console.WriteLine("AddMatrix, ex:" + ex.ToString());
            }
        }
        public void AddConnection(int node, int matrix, int startSlot, int numberOfSlots, int portOut)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);

            XmlNode connection = xmlDefault.CreateElement("connection");
            XmlNode startSlotNode = xmlDefault.CreateElement("start_slot");
            startSlotNode.InnerText = startSlot.ToString();

            XmlNode numberSlotsNode = xmlDefault.CreateElement("number_of_slots");
            numberSlotsNode.InnerText = numberOfSlots.ToString();

            XmlNode portNode = xmlDefault.CreateElement("port_out");
            portNode.InnerText = portOut.ToString();


            connection.AppendChild(startSlotNode);
            connection.AppendChild(numberSlotsNode);
            connection.AppendChild(portNode);
            try
            {
                XmlNode addTo = xmlDefault.DocumentElement.SelectSingleNode("//node[@id=" + node + "]/matrix_entry[@num=" + matrix + "]");
                addTo.AppendChild(connection);
                xmlDefault.Save(name);
            }
            catch(Exception ex)
            {
                Console.WriteLine("AddConnection, node:"+node+" matrix:"+matrix+" ex:" + ex.ToString());
            }
        }

        /// <summary>
        /// zdejmuje polaczenie w danym wezle, szuka po pierwszej szczelinie
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="matrix"></param>
        /// <param name="startSlot"></param>
        public void RemoveConnection(int nodeNumber, int matrixNumber, int startSlot)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);
            try
            {
                XmlNode rootConnection = xmlDefault.DocumentElement.SelectSingleNode("//node[@id=" + nodeNumber + "]/matrix_entry[@num=" + matrixNumber + "]");
                 XmlNodeList nodeList = rootConnection.SelectNodes("connection");

                foreach (XmlNode child in nodeList )
                {
                   
                    if (Int32.Parse(child.SelectSingleNode("start_slot").InnerText) == startSlot)
                    {
                        XmlNode parent = child.ParentNode;
                      //  XmlNode grandParent = parent.ParentNode;
                        parent.RemoveChild(child);
                        xmlDefault.Save(name);
                        break;
                    }

                }
            }
            catch (System.Xml.XmlException e)
            {
                Console.WriteLine("RemoveConnection ex:" + e.ToString());
            }

           
           
         
        }


        public void AddLink(int id, int nodeA, int nodeB, string status, int cost, int numberOfSlots)
        {
            
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);

            XmlNode aNode= xmlDefault.CreateElement("node_a");
            aNode.InnerText = nodeA.ToString();
            XmlNode bNode = xmlDefault.CreateElement("node_b");
            bNode.InnerText = nodeB.ToString();

            //do standrdu wyznaczamy wartosc
            nodeA += 10;
            nodeB += 10;

            XmlNode port = xmlDefault.CreateElement("port");
            XmlAttribute attribute = xmlDefault.CreateAttribute("id");
            attribute.InnerText = id.ToString();
            port.Attributes.Append(attribute);
            XmlNode statusType = xmlDefault.CreateElement("status");
            statusType.InnerText = status;
            XmlNode linkIn = xmlDefault.CreateElement("port_in");
            linkIn.InnerText = nodeA.ToString() + nodeB.ToString();
            XmlNode linkOut = xmlDefault.CreateElement("port_out");
            linkOut.InnerText = nodeB.ToString() + nodeA.ToString();

            XmlNode costLink = xmlDefault.CreateElement("cost");
            costLink.InnerText =cost.ToString();
            XmlNode slots = xmlDefault.CreateElement("slots_amount");
            slots.InnerText = numberOfSlots.ToString();

            port.AppendChild(aNode);
            port.AppendChild(bNode);

            port.AppendChild(statusType);
            port.AppendChild(linkIn);
            port.AppendChild(linkOut);
            port.AppendChild(costLink);
            port.AppendChild(slots);

            try
            {
                XmlNode addTo = xmlDefault.DocumentElement.SelectSingleNode("cable_cloud");
                addTo.AppendChild(port);
                xmlDefault.Save(name);
            }
            catch(Exception ex)
            {
                Console.WriteLine("AddLink, ex:" + ex.ToString());
            }
        }

        public void ChangeLinkStatus(int portId, string status)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);

            try
            {
                XmlNode addTo = xmlDefault.DocumentElement.SelectSingleNode("cable_cloud/port[@id=" + portId + "]/status");
                addTo.InnerText = status;
                xmlDefault.Save(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChangeLinkStatus ex:" + ex.ToString());
            }
        }


        public void RemoveNode(int id)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);
            // XmlNode parent //= xmlDefault.DocumentElement.SelectSingleNode("nodes");
            try
            {
                XmlNode child = xmlDefault.DocumentElement.SelectSingleNode("//node[@id=" + id + "]");
                XmlNode parent = child.ParentNode;
                parent.RemoveChild(child);
                xmlDefault.Save(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RemoveNode, ex:" + ex.ToString());
            }

        }
        public  void RemoveClient(int id)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);
            try
            {
                XmlNode child = xmlDefault.DocumentElement.SelectSingleNode("//client[@id=" + id + "]");
                XmlNode parent = child.ParentNode;
                parent.RemoveChild(child);
                xmlDefault.Save(name);
            }
            catch(Exception ex)
            {
                Console.WriteLine("RemoveClient, ex:" + ex.ToString());
            }
        }

        public  void RemoveLink(int id)
        {
            XmlDocument xmlDefault = new XmlDocument();
            xmlDefault.Load(name);
            try
            {
                XmlNode child = xmlDefault.DocumentElement.SelectSingleNode("//cable_cloud/port[@id=" + id + "]");
                XmlNode parent = child.ParentNode;
                parent.RemoveChild(child);
                xmlDefault.Save(name);
            }
            catch(Exception ex)
            {
                Console.WriteLine("RemoveLink, ex:" + ex.ToString());
            }
        }

    }
}
