using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.WareHouse;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
{
    /// <summary>
    /// PurchaseReturnReportView.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseReturnReportView
    {
        public PurchaseReturnReportView()
        {
            InitializeComponent();
        }
        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            ICollectionView itemsViewOriginal = CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            int dcSwitch = CobWare.SelectedIndex;
            if (cmb.Name.Equals("CobMan"))
            {
                dcSwitch = CobMan.SelectedIndex;
            }

            bool isFilter = itemsViewOriginal.CanFilter;
            if (isFilter)
            {
                itemsViewOriginal.Filter = (o) =>
                {
                    if (cmb.Name.Equals("CobWare"))
                    {
                        WareHouse ware = (WareHouse)o;
                        if (string.IsNullOrEmpty(cmb.Text))
                        {
                            return true;
                        }
                        else
                        {
                            if (ware.ID.StartsWith(cmb.Text) || ware.Name.Contains(cmb.Text))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Manufactory manufactory = (Manufactory)o;
                        if (string.IsNullOrEmpty(cmb.Text))
                        {
                            return true;
                        }
                        else
                        {
                            if (manufactory.ID.StartsWith(cmb.Text) || manufactory.Name.Contains(cmb.Text))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                };
            }
            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }
    }
}