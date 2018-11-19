﻿using System;
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
          

            XMLCreator.CreateXML("test1.xml");

            XMLCreator.SetName("test1.xml");
            XMLCreator.AddNode(1);
            XMLCreator.AddClient(2, 11);
            XMLCreator.AddClient(4, 44);
            XMLCreator.AddClient(5, 66);
            XMLCreator.AddNode(2);
            XMLCreator.AddMatrix(1, 1);
            XMLCreator.AddMatrix(2, 2);
            XMLCreator.AddLabel(1, 1, "push", 2, 11, 14, 12);
            XMLCreator.AddLabel(2, 2, "pop", 11, 33);
            XMLCreator.ChangeLabelPort(1, 1, 2, 222);
            XMLCreator.AddLink(1, 11, 22, "on");
            XMLCreator.AddLink(2, 21, 12, "off");
            XMLCreator.AddLink(3, 21, 11, "on");
            XMLCreator.ChangeLinkStatus(2, "on");
            //XMLCreator.RemoveNode(2);
            XMLCreator.RemoveClient(4);
            XMLCreator.RemoveLink(2);
            // XMLCreator.RemoveLabel(2, 2, 11);
            XMLCreator.ChangeLabelLabelIn(2, 2, 11, 15);
            XMLCreator.ChangeLabelPort(2, 2, 15, 111);
            XMLCreator.ChangeLabelPush(2, 2, 15, 10);
            XMLCreator.ChangeLabelSwap(2, 2, 15, 20);
            XMLCreator.ChangeLabelAcction(2, 2, 15, "push");

            Manager manager = new Manager(10000);



           // manager.Init();
        }
    }
}
