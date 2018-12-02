using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static string message="0";
       static BlockingCollection<string> dataItems = new BlockingCollection<string>(100);
        public static ObservableCollection<string> sendCollection = new ObservableCollection<string>();
        public static ObservableCollection<string> computingCollection = new ObservableCollection<string>();

        public static ObservableCollection<string> agentCollection = new ObservableCollection<string>();


        //public static void EnableCollectionSynchronization(System.Collections.IEnumerable collection, object lockObject);


       public static void ComputeThread()
        {
            lock (computingCollection)
            {
                computingCollection.CollectionChanged += Compute;
            }
        }

        private static void Compute(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string content= computingCollection.Last();
            int num;
            string toSend;
            try
            {
                lock(computingCollection)
                {
                    
                    num = Program.number;
                    
                    num = 100 * (num + 10) + num + 11;
                    if ((Program.number) == 3)
                        num = 1392;
                    toSend=Label.SwapPort(content,num );

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

       
        static void a()
        {
            
            dataItems.Add("333");
            dataItems.Add("2222");
            dataItems.Take(1);

        }
        public event EventHandler ThresholdReached;

        protected  virtual void OnThresholdReached(EventArgs e)
        {
            EventHandler handler = ThresholdReached;
            if (message != "0")
            {
                handler(this, e);
            }
        }

    }
}
