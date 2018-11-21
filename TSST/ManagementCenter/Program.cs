using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
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
            XML.CreateXML("test1.xml");

            XML.SetName("test1.xml");
            XML.AddNode(1);
            XML.AddClient(2,11);
            XML.AddClient(4,44);
            XML.AddClient(5,66);
            XML.AddNode(2);
            XML.AddMatrix(1, 1);
            XML.AddMatrix(2, 2);
            XML.AddLabel(1, 1, "push", 2, 11,14, 12);
            XML.AddLabel(2, 2, "pop", 11, 33);
            XML.ChangeLabelPort(1, 1, 2,222);
            XML.AddLink(1, 11, 22, "on");
            XML.AddLink(2, 21, 12, "off");
            XML.AddLink(3,21,11,"on");
            XML.ChangeLinkStatus(2, "on");
            //XML.RemoveNode(2);
            XML.RemoveClient(4);
            XML.RemoveLink(2);
            // XML.RemoveLabel(2, 2, 11);
            XML.ChangeLabelLabelIn(2, 2, 11, 15);
            XML.ChangeLabelPort(2, 2, 15, 111);
            XML.ChangeLabelPush(2, 2, 15, 10);
            XML.ChangeLabelSwap(2, 2, 15, 20);
            XML.ChangeLabelAcction(2, 2, 15, "push");
        }
    }
}
