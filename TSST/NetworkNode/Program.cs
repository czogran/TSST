using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Główna klasa programu
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">Nieużywane</param>
        static void Main(string[] args)
        {
            string message;
            Console.WriteLine("Node");
            Label.setMask();
            //int la = 13 + 8388608;
            try
            {
                Label.SetLabel(1111, 4, 1, 253);
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine(ex.ToString());
            }


            Label.GetLabel("aaaaaa<label>" + Label.label + "</label>ddddd<label>22</label>dd");
            // Label.SetLabelID(100);
            try
            {
                Label.SetLabelS(0);
                Label.SetTTL(10);
                Label.DecreaseTTL();
                Label.SetTC(5);
                Label.SetLabelID(12);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            message =Label.push("a<port>22</port><label>13</label>addddddd", 9);
            Label.Swap(message, 11);
            Label.SwapPort(message, 11122);
            message =Label.pop(message);
            Label.Swap(message, 10);
            //Label.SetLabelID(222);
            //Label.SetTTL(10);
           
           // Label.GetLabel("aaaaaa<label>" + Label.label + "</label>ddddd<label>22</label>dd");

            //Label.GetPort("a<port>22</port>aaaaa < label > 13 </ label > ddddddd");
           // Port port = new Port("127.0.0.2", 11002);
            Console.Read();
        }
    }
}
