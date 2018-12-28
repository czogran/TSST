﻿using System;
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
            Console.WriteLine("czas zaczac dijstre");
            //sciezka ktorej szukamy
            Path path=new Path();

            //resetowanie statusu nodow z informacji ze sa na sciezce, z poprzedniego zestawiania
            Node.ResestConnectionStatus(nodes);
           
            //ustawiana na start na bardzo duzo liczbe
            double cheapestPath; ;

            //zmienna do obliczen, mowi ktory node jest w tej chwili rozwazany
            int nodeNumber=0;

            //w ktorym aktualnie wezle jestesmy
            int actualNode=start;
            
            //ilosc slotow jaka bedzie potrzebna dla danej sciezki
            int amountOfSlots;

            //na jakim slocie zacznie sie okno i jaka jest jego maksymalna wielkosc
            int[] window;

            //zmienne sluzace do indeksowania po nodach
            int index;
            int index2;


            //ustawienie wartosci startowego noda
            //znalezienie jego indeksu
            index=nodes.IndexOf(nodes.Find(x => x.number == start));
            //chyba chodzi o to, ze sam jest poprzednikiem siebie
            nodes[index].previousNode = start;
            //dotarcie do pierwszego nic nie kosztuje
            nodes[index].costToGetHere = 0;
            //i jest juz na sciezce
            nodes[index].connected = true;


            for (int n = 0; n < nodes.Count; n++) //zmienione z petli nisekonczonej na przypadek gdy jakis wezel jest niepoloczony
            {
                for (int i = 0; i < links.Count; i++)
                {
                    if (links[i].nodeA == actualNode)
                    {
                        index = nodes.IndexOf(nodes.Find(x => x.number == links[i].nodeB));

                        index2 = nodes.IndexOf(nodes.Find(x => x.number == actualNode));
                        if (nodes[index].costToGetHere > links[i].cost + nodes[index2].costToGetHere)          // (wezel[krawendz[i].wezel_b - 1].wejscie == 0 &(wezel[krawendz[i].wezel_b - 1].koszt == 0 || wezel[krawendz[i].wezel_b - 1].koszt > krawendz[i].dlugosc + wezel[aktualny_nr - 1].koszt))
                        {
                            nodes[index].costToGetHere = links[i].cost + nodes[index2].costToGetHere;
                            nodes[index].previousNode = links[i].nodeA;
                            nodes[index].inputLink = links[i];
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
                    path.nodes.Add(nodes[index]);
                    path.connection.Add(nodes[index].inputLink);

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
                amountOfSlots=AmountNeededSlots(path.lenght);
                window= path.FindMaxWindow();
                path.ReserveWindow(amountOfSlots,window[0],window[1]);
                XMLeon xml = new XMLeon("path" + start + end + ".xml", XMLeon.Type.nodes);
                path.xmlName = ("path" + start + end + ".xml");
                xml.CreatePathXML(path);
            }
            else
            {
                Console.WriteLine("Nie udalo sie zestawic sciezki");
            }           
                    return path;
        }

        /// <summary>
        /// w zaleznosci od dlugosci sciezki 
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