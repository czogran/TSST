using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementCenter
{
    class AgentSwitchingAction
    {
        internal static ObservableCollection<string> agentCollection = new ObservableCollection<string>();

        internal static void AgentAction(string message, Manager manager)
        {
            //jezeli ma wyslac jeszcze dalej
            if (message.Contains("subsubstring"))
            {
                
            }
            //jezeli jest na samym dole hierarchi to nie ma juz wewnatrz podsicei
            else if (message.Contains("substring"))
            {
               
            }

        }

    }
}
