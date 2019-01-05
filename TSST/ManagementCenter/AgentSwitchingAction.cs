using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ManagementCenter
{
    /// <summary>
    /// obsluguje polecenia i wiadomosci jakie dostal agent
    /// </summary>
    class AgentSwitchingAction
    {
        internal static ObservableCollection<string> agentCollection = new ObservableCollection<string>();

        internal static void AgentAction(string message, Manager manager)
        {
            //jezeli ma wyslac jeszcze dalej
            if (message.Contains("subsubnetwork"))
            {
                
            }

            //jezeli jest na samym dole hierarchi to nie ma juz wewnatrz podsicei
            else if (message.Contains("subnetwork"))
            {
                ConnectSubnetwork(message);
                Program.isTheBottonSub = true;
            }
            else if(message.Contains("connection"))
            {
                //jezeli jest to juz najnizsza podsiec to na jej poziomie juz konfigurujemy
                if(Program.isTheBottonSub==true)
                {
                    //format wiadomosci
                    //connection:<port_in>port</port_in><port_out>port<port_out>
                    int start, end;
                    int portIn, portOut;
                    int startNode, endNode;
                    start = message.IndexOf("<port_in>")+9;
                    end = message.IndexOf("</port_in>");
                    portIn = Int32.Parse(message.Substring(start, end - start));

                    start = message.IndexOf("<port_out>") + 10;
                    end = message.IndexOf("</port_out>");
                    portOut = Int32.Parse(message.Substring(start, end - start));

                    startNode = portIn % 100-10;
                    endNode = portOut / 100 - 10;

                    Console.WriteLine("port_in:" + portIn + "  port_out:" + portOut);
                    Console.WriteLine("start node:" + startNode + "  end node:" + endNode);
                }
                //TODO
                //w przeciwnym razie slemy nizej, czyli jak sa podspodem jeszcze inne polaczenia bedziemy musieli slac nizej
                else
                {

                }
            }

        }


        static void ConnectSubnetwork(string message)
        {
            Console.WriteLine("Konfiguracja podsieci");
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Niepoprawny format wiadomosci, ex:" + ex.ToString());
            }
            XMLeonSubnetwork eonXml = new XMLeonSubnetwork(xmlDoc);
            string linkFile = eonXml.GetLinkFile();
            XMLParser xml = new XMLParser(linkFile);
            Program.links = xml.GetLinks();
            lock (Program.managerCloud)
            {
                Program.managerCloud.Send(XML.StringCableLinks(linkFile));

            }
            CLI.PrintConfigFilesSent();

            //laczenie sie z wezlami w podsieci
            lock (Program.managerNodes)
            {
                int i = 0;
                foreach (Node node in Program.nodes)
                {
                    Program.managerNodes.Add(new Manager());
                    Program.managerNodes[i].CreateSocket("127.0.4." + node.number, 11001);
                    while (true)
                    {
                        try
                        {
                            Program.managerNodes[i].Connect("127.0.3." + node.number, 11001);
                            break;
                        }
                        catch
                        {

                        }
                    }
                    i++;
                }


            }
        }


    }
}
