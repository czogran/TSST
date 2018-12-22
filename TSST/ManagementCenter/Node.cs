using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ManagementCenter
{
    /// <summary>
    /// bardzo wazne, jako nody beda liczeni tez klijenci
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

        //uzywany do obliczen, czy juz jest na sciezce
        public bool connected; 

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
    }
}
