using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{

    class PathAlgorithm
    {
        public static Path dijkstra(List<Node>nodes, List<Link>links, int start, int end, bool direction)
        {
            Path path;
            //wezly przez ktore ma przejsc
            List<Node> nodesOnPath=new List<Node>();
            //linki przez ktore ma przejsc
            List<Link> linksOnPath = new List<Link>();

            //ustawiana na start na bardzo duzo liczbe
            double cheapestPath; ;
            //zmienna do obliczen, mowi ktory node jest w tej chwili rozwazany
            int nodeNumber=0;

            int actualNode=start;
            nodes[start - 1].previousNode = start;
            nodes[start - 1].costToGetHere = 0;

            for (int n = 0; n < nodes.Capacity; n++) //zmienione z petli nisekonczonej na przypadek gdy jakis wezel jest niepoloczony
            {
                //cout << "\n" << aktualny_nr<<"   "<<end<<"\n";
                for (int i = 0; i < links.Capacity; i++)
                {
                    if (links[i].nodeA == actualNode)
                    {
                        if (nodes[links[i].nodeB - 1].costToGetHere > links[i].cost + nodes[actualNode - 1].costToGetHere)          // (wezel[krawendz[i].wezel_b - 1].wejscie == 0 &(wezel[krawendz[i].wezel_b - 1].koszt == 0 || wezel[krawendz[i].wezel_b - 1].koszt > krawendz[i].dlugosc + wezel[aktualny_nr - 1].koszt))
                        {
                            nodes[links[i].nodeB - 1].costToGetHere = nodes[i].costToGetHere + nodes[actualNode - 1].costToGetHere;
                            nodes[links[i].nodeB - 1].previousNode = links[i].nodeA;
                        }
                    }

                }
                //ustawiana na start na maksymalna wartosc int
                cheapestPath = 2147483647;
                for (int i=0;i<nodes.Capacity;i++)
                {
                    if(nodes[i].connected==false && nodes[i].costToGetHere<cheapestPath)
                    {
                        
                        cheapestPath = nodes[i].costToGetHere;
                        //indeksuje od zera, by sie nie powalilo
                        nodeNumber = i;
                    }
                }
               
               // nodes[nodeNumber].connected = true;
               // actualNode = nodeNumber;
            }

                    return path;
        }
    }
}
