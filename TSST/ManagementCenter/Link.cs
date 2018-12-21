using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    class Link
    {
       public int nodeA;
       public int nodeB;

        int id;
        public int cost;

        string status;

        int numberOfSlots;
        int numberOfUsedSlots;
        //bool[] usedSlots;
        Array usedSlots;

        public Link(int id, int nodeA, int nodeB, int numberOfSlots, int cost, string status)
        {
            this.id = id;
            this.numberOfSlots = numberOfSlots;
            this.nodeA = nodeA;
            this.nodeB = nodeB;

            this.status = status;

            this.cost = cost;

            usedSlots = Array.CreateInstance(typeof(bool), this.numberOfSlots);
            for(int i=0;i<numberOfSlots;i++)
            {
                usedSlots.SetValue(false, i);
            }
        }

        /// <summary>
        /// rezeruwuje lub zwalnie oznaczenie czy szczelina jest zajeta
        /// </summary>
        /// <param name="startSlot"></param>
        /// <param name="amountOfSlots"></param>
        /// <param name="status"></param>
        public void SetSlots(int startSlot, int amountOfSlots, bool status)
        {
            for (int i = startSlot-1; i < amountOfSlots; i++)
            {
                usedSlots.SetValue(status, i);
            }
        }
    }
}
