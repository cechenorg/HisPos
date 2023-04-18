using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    /// <summary>
    /// TransferControl.xaml 的互動邏輯
    /// </summary>
    public partial class NormalControl : UserControl
    {
        public NormalControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void Amount_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((TextBox)sender).SelectAll();
            ((TextBox)sender).Focus();
        }

        private void Amount_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void Amount_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            textBox.Focus();
            ((TextBox)sender).SelectAll();
        }

        private void tb_New_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            textBox.Focus();
            ((TextBox)sender).SelectAll();
        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ShowRecordDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridRow data = (DataGridRow)sender;
            AccountsDetailReports currentItem = (AccountsDetailReports)data.Item;
            MainWindow.Instance.AddNewTab("傳票作業");
            AccountVoucherViewModel currentTab = (AccountVoucherViewModel)((ViewModelMainWindow)MainWindow.Instance.DataContext).SelectedTab;
            currentTab.ShowOrderDetailByOrderID(currentItem.ID);
        }
    }
}