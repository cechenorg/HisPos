using System;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement.MedControl
{
    /// <summary>
    /// MedicineHistoryPriceWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineHistoryPriceWindow : Window
    {
        public MedicineHistoryPriceWindow(string medId)
        {
            InitializeComponent();
        }

        private void MedicineHistoryPriceWindow_OnDeactivated(object sender, EventArgs e)
        {
            Close();
        }
    }
}
