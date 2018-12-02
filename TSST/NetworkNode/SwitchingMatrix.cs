using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NetworkNode
{
    /// <summary>
    /// Pole komutacyjne
    /// </summary>
    class SwitchingMatrix
    {
        public static ObservableCollection<string> sendCollection = new ObservableCollection<string>();
        public static ObservableCollection<string> computingCollection = new ObservableCollection<string>();

        public static ObservableCollection<string> agentCollection = new ObservableCollection<string>();


        //public static void EnableCollectionSynchronization(System.Collections.IEnumerable collection, object lockObject);

        public static Dictionary<int, Dictionary<uint,Label>> portDictionary = new Dictionary<int, Dictionary<uint, Label>>();

        public static void FillDictionary()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load("myNode" + Program.number + ".xml");
            int inPort;
            int outPort;
            uint swap;
            uint push;
            string acction;
            XmlNode node1;
            uint labelIn;
            string labelInString;
            Label label;
            foreach (XmlNode nodePort in doc.SelectNodes("node/matrix_entry"))
            {
                 Dictionary<uint, Label> labelDictionary = new Dictionary<uint, Label>();

                 inPort = Int32.Parse(nodePort.Attributes["num"].Value);
                foreach (XmlNode nodeLabel in nodePort.SelectNodes("label_in"))
                {
                    labelInString=nodeLabel.Attributes["label"].Value;
                    labelIn = UInt32.Parse(labelInString);

                    node1=nodeLabel.SelectSingleNode("acction");
                    acction = node1.InnerText;
                    node1 = nodeLabel.SelectSingleNode("swap");
                    swap = UInt32.Parse(node1.InnerText);
                    node1 = nodeLabel.SelectSingleNode("push");
                    push = UInt32.Parse(node1.InnerText);
                    node1 = nodeLabel.SelectSingleNode("port");
                    outPort = Int32.Parse(node1.InnerText);
                    label = new Label(swap, push, acction, outPort);
                    labelDictionary.Add(labelIn, label);
                  
                }
                portDictionary.Add(inPort, labelDictionary);
                Console.WriteLine("cos uzupelniam");
            }
          
        }
        public static void ComputeThread()
        {
            lock (computingCollection)
            {
                computingCollection.CollectionChanged += Compute;
            }
        }

        static Label label;
        static string content;
        private static void Compute(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            content = computingCollection.Last();
            int num;
            string toSend;
            int inPort;
            uint labelIn;
           
            try
            {
                lock(computingCollection)
                {
                   
                    inPort=Label.GetPort(content);
                   
                    Console.WriteLine("inPort:" + inPort);
                    labelIn = Label.GetLabel(content);
                    Console.WriteLine("inLabel:" + labelIn);
                    if (labelIn==0)
                    {

                        label = portDictionary[inPort][labelIn];
                        Console.WriteLine("znaleziono w slowniku");

                        if (label.action=="push")
                        {
                            Label.SetLabel(label.IDpush, 0, 0, 0);
                            content = Label.Push(content,Label.label);
                        }
                        
                    }
                    else
                    {
                        label = portDictionary[inPort][labelIn];
                        Console.WriteLine("znaleziono w slowniku");

                        switch (label.action)
                            {
                            case "push":
                                break;
                            case "swap":
                                Label.SetLabel(label.IDswap, 0, 0, 0);
                                content=Label.Swap(content, Label.label);
                                break;
                            case "pop":
                                content=Label.Pop(content);
                                Console.WriteLine("tu jestem");
                                break;

                        }
                    }
                 
                    /* num = Program.number;

                     num = 100 * (num + 10) + num + 11;
                     if ((Program.number) == 3)
                         num = 1392;
                     toSend=Label.SwapPort(content,num );
                     */
                    toSend = Label.SwapPort(content, label.portOut);
                    lock (sendCollection)
                    {
                        sendCollection.Add(toSend);
                    }
                }
              //  content.IndexOf("label");
            }
            catch (Exception ex)
            {

            }
            
        }

       
        

    }
}
