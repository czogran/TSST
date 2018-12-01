using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
{
    class Switch
    {
        public static string message="0";
        public static string messageNode = "0";
        public static ObservableCollection<string> collection = new ObservableCollection<string>();
        public static ObservableCollection<string> agentCollection = new ObservableCollection<string>();
        public static BlockingCollection<ObservableCollection<string>> nodeCollection = new BlockingCollection<ObservableCollection<string>>();
        public static BlockingCollection<ObservableCollection<string>> clientCollection = new BlockingCollection<ObservableCollection<string>>();

        public static BlockingCollection<int> data = new BlockingCollection<int>();

        public static string SwitchBufer(string message)
        {
            if (message.Contains("port")) 
            {
                return "node";
            }
            else
                return "client";
        }
        static int testCounter = 0;
        public static int SwitchNodes(string message)
        {
           // testCounter++;
            return 1;
        }
        public static int SwitchClients(string message)
        {
            return 1;
        }
    }
}
