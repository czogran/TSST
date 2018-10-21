using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// klasa do obslugi wszystkich operacji na etykietach
    /// zrodlo-https://www.juniper.net/documentation/en_US/junos/topics/concept/mpls-labels-operations.html
    /// @author Pawel
    /// </summary>
    class Label
    {
        /// <summary>
        /// dodaje etykiete na gore stosu
        /// </summary>
        public void push()
        {
        }
        /// <summary>
        /// zdejmuje gorna etykiete
        /// </summary>
        public void pop()
        {
        }
        /// <summary>
        /// zamienia gorna etykiete na stosie
        /// </summary>
        public void swap()
        {
        }
        /// <summary>
        /// dodawanie wielu etykiet do pakietu, max 3, ta operacja rowna sie parokrotnemu pushowaniu
        /// </summary>
        public void multiplePush()
        {
        }
        /// <summary>
        /// zamienia gorna etykiete a nastepnie dodaje nowa na wierzch
        /// </summary>
        public void swapAndPush()
        {
        }


    }


    
}
