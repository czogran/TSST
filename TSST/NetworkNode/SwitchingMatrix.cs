using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Pole komutacyjne
    /// </summary>
    class SwitchingMatrix
    {
        /// <summary>
        /// Słownik portów wejścia/wyjścia
        /// </summary>
        private Dictionary<int, int> portTable;

        public SwitchingMatrix()
        {
            portTable = new Dictionary<int, int>();
        }
    }
}
