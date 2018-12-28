using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    /// <summary>
    /// klasa przechowujaca sciezke pomiedzy dwoma klijentami
    /// </summary>
    class Path
    {
        /// <summary>
        /// bool mowiacy o tym czy sie udalo pociagnac sciezke od jednego konca do drugiego
        /// </summary>
        public bool endToEnd;

        //czy udalo sie zarezerowac szczeliny
        public bool pathIsSet;

        /// <summary>
        /// lista linkow znajdujaca sie na sciezce
        /// </summary>
        public List<Link> connection;
        public List<Node> nodes;

        /// <summary>
        ///numer slotu poczatkowego
        /// </summary>
        public int startSlot;

        /// <summary>
        /// numer konicowego slotu
        /// </summary>
        public int endSlot;

        /// <summary>
        /// lista linkow ktore najbardziej beda zmniejszaly okno, do obliczen, jak sie nie uda zestawic sciezki by wywalic najgorszy
        ///i sprobowac znalezc nowa sciezke bez niego
        /// </summary>
        public List<Link> worstLinks;


        /// <summary>
        /// ktore szczeliny sa dostepne
        /// </summary>
        public bool[] possibleWindow;

        /// <summary>
        /// przez ile wezlow idzie sciezka
        /// </summary>
        public int hops;

        /// <summary>
        /// okresla jak dluga jest sciezka
        /// </summary>
        public int lenght;
        
        /// <summary>
        /// jak sie nazywa xml, ktory przechowuje ta sciezke
        /// </summary>
        public string xmlName;

        public Path()
        {
            //domyslnie sciezka nie jest znaleziona;
            endToEnd = false;
            connection = new List<Link>();
            nodes=new List<Node>();
            //wiecej szczelin nie bedziemy mieli, nie chce mi sie bawic i na bruta daje ich z zapasem
            possibleWindow = new bool[100];
            for(int i=0;i< possibleWindow.Length;i++)
            {
                possibleWindow[i] = true;
            }
            hops = 0;
            lenght = 0;
        }
    

        public Path(List<Link> connection, List<Node> nodes)
        {
            this.connection = connection;
            this.nodes = nodes;
         
        }

        /// <summary>
        /// elimnuje szczeliny na polaczeniu gdy sa juz one zajeta na jakims linku
        /// </summary>
        /// <param name="link"></param>
        public void ChangeWindow(Link link)
        {
            for (int i = 0; i < link.usedSlots.Length; i++)
            {
                //zmieniamy gdy tylko jestesmy w obszarze dostepnych okien
                if (possibleWindow[i] == true)
                {
                    //jezeli szczelina jest niezajeta to znaczy ze bedzie dostepna do okna, stad zaprzeczenie
                    possibleWindow[i] = !link.usedSlots[i];
                }
            }
            //gdy skonczy sie ilosc szczelin w swiatlowodzie na reszcie sie juz nie da zestawic polaczenia
            //nie podoba mi sie tu indeksownie
            for (int i = link.usedSlots.Length ; i < possibleWindow.Length; i++)
            {
                possibleWindow[i] = false;
            }
        }


        /// <summary>
        /// sluzy do znalezienia maksymalnego okna na szlaku, by w ogole stwierdzic czy zestawienie polaczenia
        /// na tym szlaku jest mozliwe
        /// </summary>
        /// <returns></returns>  
        public int[]  FindMaxWindow()
        {
            int maxWindow = 0;
            int startSlot=0;
            int actualWindow = 0;
            for(int i=0;i<possibleWindow.Length;i++)
            {
                if(possibleWindow[i]==true)
                {
                    if (actualWindow == 0)
                    {
                        //walone indeksowanie, sloty od 1 a wszytskie te tablice od 0
                        startSlot = i + 1;
                    }
                    actualWindow++;
                    if(actualWindow>maxWindow)
                    {
                        maxWindow = actualWindow;
                    }
                }
                else
                {
                    actualWindow = 0;
                }
            }
            Console.WriteLine("Max Window: " + maxWindow + "  Start Slot: " + startSlot);
            int[] returnWindow= new int[2] {startSlot,maxWindow };
            return returnWindow;
        }

        /// <summary>
        /// sprawdza czy da sie zarezerwoac sciezke
        /// i jak sie da to ustawia odpowiednie szczeliny na zajete
        /// </summary>
        /// <param name="neededSlots"></param>
        /// <param name="startWindow"></param>
        /// <param name="maxWindow"></param>
        /// <returns></returns>
        public bool ReserveWindow(int neededSlots, int startWindow, int maxWindow)
        {
            if (neededSlots > maxWindow)
            {
                pathIsSet = false;
                Console.WriteLine("Zbyt male okno");
                return false;
            }
            else
            {
                pathIsSet = true;
                startSlot = startWindow;
                endSlot = startWindow + neededSlots;


                //dla kazdego linku na sciezce rezerwujemy szczeliny
                foreach (Link link in connection)
                {
                    link.SetSlots(startWindow, endSlot, true);
                }
                return true;
            }
        }

    }
}
