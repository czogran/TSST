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
        
        public List<Link> connection;
        public List<Node> nodes;
        int startSlotCounted;
        int amountOfSlots;

        public Path()
        {
            connection = new List<Link>();
            nodes=new List<Node>();
        }
    

        public Path(List<Link> connection, List<Node> nodes,    int startSlotCounted,    int amountOfSlots)
        {
            this.connection = connection;
            this.nodes = nodes;
            this.startSlotCounted = startSlotCounted;
            this.amountOfSlots = amountOfSlots;
        }
    }
}
