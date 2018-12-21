using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
usi

namespace ManagementCenter
{
    class Node
    {
        int number;
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
            costToGetHere =(int) Math.Pow(2, 31);
        }
    }
}
