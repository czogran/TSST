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

            //klucz port, potem byl zdaje sie ze label
        public static Dictionary<int, Dictionary<uint,Label>> portDictionary = new Dictionary<int, Dictionary<uint, Label>>();
        public static Dictionary<int, Dictionary<string, Label>> labelZeroDictionary = new Dictionary<int, Dictionary<string, Label>>();

        //pierwszy int to port wejsciowy, drugi to start slot, trzeci to port wyjsciowy
        public static Dictionary<int, Dictionary<int, int>> eonDictionary = new Dictionary<int, Dictionary<int, int>>();
        //pierwszy to start slot, drugi to port wyjsciowy
        static Dictionary<int, int> switchingDictionary = new Dictionary<int, int>();




        public static MultiKeyDictionary<int, Dictionary<int, int>> dictionary =new MultiKeyDictionary<int, Dictionary<int, int>>();
        /// <summary>
        /// uzupelnia eono wy slownik
        /// </summary>
        public static void FillEonDictionary()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("myNode" + Program.number + ".xml");
            int inPort;
            int outPort;
            int startSlot;

            XmlNode node1;

            //to w sumie mozna wywalic do parsera, bo tam jest tego miejsce zgodnie z konwencja
            foreach (XmlNode nodePort in doc.SelectNodes("node/matrix_entry"))
            {

                inPort = Int32.Parse(nodePort.Attributes["num"].Value);
                foreach (XmlNode nodeConnection in nodePort.SelectNodes("connection"))
                {
                    node1 = nodeConnection.SelectSingleNode("start_slot");
                    startSlot = Int32.Parse(node1.InnerText);

                    node1 = nodeConnection.SelectSingleNode("port_out");
                    outPort = Int32.Parse(node1.InnerText); 

                    switchingDictionary.Add(startSlot, outPort);
                    Console.WriteLine(inPort + "   " + startSlot + "   " + outPort);
                }
                if(!eonDictionary.ContainsKey(inPort))
                {
                    eonDictionary.Add(inPort, switchingDictionary);
                }
               
                

            }
            Console.WriteLine("Dodalem wpisy sciezki");
        }


        /// <summary>
        /// do mpls, bedzie do wywalki
        /// </summary>
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
                string address;
                int chooseDictionary = 0;
            foreach (XmlNode nodePort in doc.SelectNodes("node/matrix_entry"))
            {
                 Dictionary<uint, Label> labelDictionary = new Dictionary<uint, Label>();
                Dictionary<string, Label> labelZeroDictionaryHelp = new Dictionary<string, Label>();

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
                   

                    if (labelIn == 0)
                    {
                        node1 = nodeLabel.SelectSingleNode("address");
                        address = node1.InnerText;
                        label = new Label(address,push,acction,outPort);
                        labelZeroDictionaryHelp.Add(address, label);
                    }
                    else
                    {
                        label = new Label(swap, push, acction, outPort);
                        labelDictionary.Add(labelIn, label);
                    }

                }
                labelZeroDictionary.Add(inPort, labelZeroDictionaryHelp);

                portDictionary.Add(inPort, labelDictionary);
            }
            Console.WriteLine("Uzupełniłem słownik\n");

        }

        /// <summary>
        /// watek co tam trzyma to by to sie piknie switchowalo
        /// </summary>
        public static void ComputeThread()
        {
            lock (computingCollection)
            {
                computingCollection.CollectionChanged += ComputingEon;
               // computingCollection.CollectionChanged += Compute;
            }
        }

        /// <summary>
        /// switchowanie eonowe
        /// tylko zmiana portow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ComputingEon(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string content;
            int inPort;
            int startSlot;
            int outPort;

            lock(computingCollection)
            {
                content = computingCollection.Last();

                inPort = Label.GetPort(content);
                startSlot = Label.GetStartSlot(content);
                outPort = eonDictionary[inPort][startSlot];

                content = Label.SwapPort(content, outPort);

                lock(sendCollection)
                {
                    sendCollection.Add(content);
                }


            }
        }

        //z mpls zdaje sie
        static Label label;
        static string content;
        private static void Compute(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

          
               
            int num;
            string toSend;
            int inPort;
            uint labelIn;
            uint labelInId;
            string address;
           
            try
            {
                lock(computingCollection)
                {
                    content = computingCollection.Last();
                    address = Label.GetAddress(content);
                    inPort =Label.GetPort(content);
                   
                    //Console.WriteLine("inPort:" + inPort);
                    labelIn = Label.GetLabel(content);
                    labelInId = Label.ID;

                    //Console.WriteLine("inLabel Id:" + labelInId);
                    if (labelIn==0)
                    {

                        label =labelZeroDictionary[inPort][address];
                        //Console.WriteLine("znaleziono w slowniku label ZERO");

                        if (label.action=="push")
                        {
                            //Console.WriteLine("wykonuję operację push");
                            Label.SetLabel(label.IDpush, 0, 1, 0);
                            content = Label.Push(content,Label.label);
                        }
                        
                    }
                    else
                    {
                        label = portDictionary[inPort][labelInId];
                        //Console.WriteLine("znaleziono w slowniku");

                        switch (label.action)
                            {
                            case "push":
                                Label.SetLabel(label.IDswap, 0, Label.S, 0);
                                content = Label.Swap(content, Label.label);
                                Label.SetLabel(label.IDpush, 0, 0, 0);
                                content = Label.Push(content, Label.label);
                                break;
                            case "swap":
                                Label.SetLabel(label.IDswap, 0, Label.S, 0);
                                content=Label.Swap(content, Label.label);
                                break;
                            case "pop":
                                content=Label.Pop(content);
                                //0 znaczy ze nie ma labela
                                if(Label.GetLabel(content) != 0)
                                {
                                    Label.GetLabel(content);
                                    Label.SetLabel(label.IDswap, 0, Label.S, 0);
                                    content = Label.Swap(content, Label.label);
                                }
                                //Console.WriteLine("tu jestem");
                                break;

                        }
                    }
                 

                    toSend = Label.SwapPort(content, label.portOut);
                    lock (sendCollection)
                    {
                        sendCollection.Add(toSend);
                        CLI.PrintSentMessage(Label.GetPort(toSend).ToString());
                    }
                }
              //  content.IndexOf("label");
            }
            catch (Exception ex)
            {

            }
            
        }
    }

    public class MultiKeyDictionary<T1, T2>
    {
    }

   
}
