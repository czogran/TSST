using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ManagementCenter
{
    /// <summary>
    /// informacje o wezlach sa tu przechowywane
    /// bardzo wazne, jako nody beda liczeni tez klijenci w algorytmie sciezek
    /// </summary>
    class Node
    {
        public int number;
        //Array ports;
        List<int> ports;

        //koszt dotaracia do tego wezla, potrzebna do dijxtry
        public int costToGetHere;

        //wezel ktory jest poprzedni na sciezce
       public int previousNode;

        //z ktorego linku wychodzi z wezla
        public Link outputLink;

        //z ktorego linku wchodzi do wezla
        public Link inputLink;

        //uzywany do obliczen, czy juz jest na sciezce
        public bool connected;

        public List<Tuple<int, int>> connections;

        public Node(int number,List<int> ports)
        {
            this.number = number;
            this.ports = ports;

            
            connected = false;
            //maksymalny koszt jaki moze miec int
            costToGetHere = 2147483647;
        }

        public Node(int number)
        {
            this.number = number;

            connected = false;
            //maksymalny koszt jaki moze miec int
            costToGetHere = 2147483647;
        }

        public static void ResestConnectionStatus(List<Node> nodes)
        {
            foreach(Node node in nodes)
            {
                node.connected = false;
                node.costToGetHere = 2147483647;
            }
        }
    }
}
