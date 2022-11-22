using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.WareHouse;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    /// <summary>
    /// ProductPurchaseRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseRecordView : UserControl
    {
        public ProductPurchaseRecordView()
        {
            InitializeComponent();
        }

        private void StartDate_OnKeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;

            if (maskedTextBox is null) return;

            if (e.Key == Key.Enter)
            {
                EndDate.Focus();
                EndDate.Select(0, 0);
            }
        }

        private void StartDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PresetToday(sender);
        }

        private void EndDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PresetToday(sender);
        }
        private void PresetToday(object sender)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox is null) return;
            if (maskedTextBox.Text == "---/--/--")
            {
                DateTime dt = DateTime.Today;
                TaiwanCalendar tc = new TaiwanCalendar();
                string year = tc.GetYear(dt).ToString().PadLeft(3, '0');
                string month = tc.GetMonth(dt).ToString().PadLeft(2, '0');
                string day = tc.GetDayOfMonth(dt).ToString().PadLeft(2, '0');
                string today = string.Format("{0}/{1}/{2}", year, month, day);
                maskedTextBox.Text = today;
            }
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