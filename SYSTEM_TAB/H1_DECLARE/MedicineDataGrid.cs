using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE
{
    public class ControlSpecificCustomAutomationPeer : ItemsControlAutomationPeer
    {
        public ControlSpecificCustomAutomationPeer(ItemsControl owner)
            : base(owner)
        {
        }

        protected override string GetNameCore()
        {
            return "";
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new CustomDummyItemAutomationPeer(item, this);
        }
    }

    public class CustomDummyItemAutomationPeer : ItemAutomationPeer
    {
        public CustomDummyItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
            : base(item, itemsControlAutomationPeer)
        {
        }

        protected override string GetNameCore()
        {
            if (Item == null)
                return "";

            return Item.ToString() ?? "";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return System.Windows.Automation.Peers.AutomationControlType.Text;
        }

        protected override string GetClassNameCore()
        {
            return "Dummy";
        }
    }
    public class MedicineDataGrid : DataGrid
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ControlSpecificCustomAutomationPeer(this);
        }
    }
}
