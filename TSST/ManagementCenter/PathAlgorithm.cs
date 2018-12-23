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
            //sciezka ktorej szukamy
            Path path=new Path();
            
            //wezly przez ktore ma przejsc
            List<Node> nodesOnPath=new List<Node>();
            //linki przez ktore ma przejsc
            List<Link> linksOnPath = new List<Link>();

            //ustawiana na start na bardzo duzo liczbe
            double cheapestPath; ;
            //zmienna do obliczen, mowi ktory node jest w tej chwili rozwazany
            int nodeNumber=0;

            int actualNode=start;
            int previousNode;

            // nodes.Find(x => x.number==start);
            int index;
            int index2;
            index=nodes.IndexOf(nodes.Find(x => x.number == start));
                nodes[index].previousNode = start;
                nodes[index].costToGetHere = 0;
            nodes[index].connected = true;


            for (int n = 0; n < nodes.Count; n++) //zmienione z petli nisekonczonej na przypadek gdy jakis wezel jest niepoloczony
            {
                //cout << "\n" << aktualny_nr<<"   "<<end<<"\n";
                for (int i = 0; i < links.Count; i++)
                {
                    if (links[i].nodeA == actualNode)
                    {
                        index = nodes.IndexOf(nodes.Find(x => x.number == links[i].nodeB));
                      //  Console.WriteLine(links[i].nodeB);

                        index2 = nodes.IndexOf(nodes.Find(x => x.number == actualNode));
                        if (nodes[index].costToGetHere > links[i].cost + nodes[index2].costToGetHere)          // (wezel[krawendz[i].wezel_b - 1].wejscie == 0 &(wezel[krawendz[i].wezel_b - 1].koszt == 0 || wezel[krawendz[i].wezel_b - 1].koszt > krawendz[i].dlugosc + wezel[aktualny_nr - 1].koszt))
                        {
                            nodes[index].costToGetHere = links[i].cost + nodes[index2].costToGetHere;
                            nodes[index].previousNode = links[i].nodeA;
                            nodes[index].inputLink = links[i];
                           // Console.WriteLine(links[i].nodeA);
                        }
                    }

                }
                //ustawiana na start na maksymalna wartosc int
                cheapestPath = 2147483647;
                for (int i=0;i<nodes.Count;i++)
                {
                    if(nodes[i].connected==false && nodes[i].costToGetHere < cheapestPath)
                    {
                        
                        cheapestPath = nodes[i].costToGetHere;
                        //indeksuje od zera, by sie nie powalilo
                        nodeNumber = nodes[i].number;
                    }
                }
                //zaznaczamy, ze najtanszy 
                index = nodes.IndexOf(nodes.Find(x => x.number == nodeNumber));
                nodes[index].connected = true;
                actualNode = nodeNumber;
                // Console.WriteLine(actualNode);

                if (actualNode == end)
                {
                    Console.WriteLine("Znalazlem sciezke");
                    //ustawiamy tutaj ze sciezka zostala znaleziona
                    path.endToEnd = true;
                    break;
                }
            }
            if (path.endToEnd == true)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    index = nodes.IndexOf(nodes.Find(x => x.number == actualNode));
                    //Console.WriteLine(nodes[index].number);
                    path.nodes.Add(nodes[index]);

                    path.ChangeWindow(nodes[index].inputLink);
                    path.hops++;
                    path.lenght += nodes[index].inputLink.lenght;

                    
                    //cofamy sie po sciezce
                    actualNode = nodes[index].previousNode;

                    //tu przypisujemy wyjscia nodow, wyjscie aktualnego jest wejsciem poprzedniego
                    index2 = nodes.IndexOf(nodes.Find(x => x.number == actualNode));
                    nodes[index2].outputLink = nodes[index].inputLink;

                    if (actualNode == start)
                    {
                        //jest tu dodawanie, bo cofanie sie po sciezce jest przed ifem
                        index = nodes.IndexOf(nodes.Find(x => x.number == actualNode));
                        path.nodes.Add(nodes[index]);

                        
                        //jak ostatni to nie bedzie mial poprzedniego wiec raczej z tad tego tu nie bedzie
                        //  path.ChangeWindow(nodes[index].previusLink);
                        path.hops++;

                        int[] pathWindow =  path.FindMaxWindow();
                        Console.WriteLine("Window start: "+pathWindow[0]+"Window Size:"+pathWindow[1]);
                        Console.WriteLine("Hops:"+path.hops);
                        break;

                    }
                }
                for (int i = 0; i < path.nodes.Count; i++)
                {
                    Console.WriteLine(path.nodes[i].number);

                    try
                    {
                        Console.WriteLine("  InputLink: " + path.nodes[i].inputLink.id);
                    }
                    catch(Exception ex)
                    {  }
                    try
                    {
                        Console.WriteLine("  OutputLink: " + path.nodes[i].outputLink.id);
                    }
                    catch (Exception ex)
                    { }


                }
                Console.WriteLine("Path Lenght:" + path.lenght);
                AmountNeededSlots(path.lenght);
               // XMLeon.CreatePathXMLs(path);
            }
            else
            {
                Console.WriteLine("Nie udalo sie zestawic sciezki");
            }
            
                    return path;
        }

        /// <summary>
        /// przy
        /// </summary>
        /// <param name="lengthOfPath"></param>
        /// <returns></returns>
        static int AmountNeededSlots(int lengthOfPath)
        {
            int amountNeeded=0;

            //granice sa przyjete przezemnie arbitralnie
           if(lengthOfPath<10)
            {
                amountNeeded = 1;
                Console.WriteLine("Wykorzystana modulacja: 16QAM");
                Console.WriteLine("Ilosc potrzebnych szczelin:" + 1);
            }
           else if(lengthOfPath<20)
            {
                amountNeeded = 2;
                Console.WriteLine("Wykorzystana modulacja: 8QAM");
                Console.WriteLine("Ilosc potrzebnych szczelin :" + 2);
            }
           else if(lengthOfPath<30)
            {
                amountNeeded = 3;
                Console.WriteLine("Wykorzystana modulacja: QPSK");
                Console.WriteLine("Ilosc potrzebnych szczelin :" + 3);
            }
           else
            {
                amountNeeded = 4;
                Console.WriteLine("Wykorzystana modulacja: BSK");
                Console.WriteLine("Ilosc potrzebnych szczelin :" + 4);
            }


            return amountNeeded;
        }

    }
}
