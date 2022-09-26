using His_Pos.NewClass.Accounts;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.Service;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountsManage
{
    /// <summary>
    /// AdditionalCashFlowManage.xaml 的互動邏輯
    /// </summary>
    public partial class AccountsManage : UserControl
    {
        public AccountsManage()
        {
            InitializeComponent();
        }

        private void StartDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                EndDate.Focus();
                EndDate.SelectionStart = 0;
            }
        }

        private void EndDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void tb_Value_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            ICollectionView itemsViewOriginal = CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            bool isFilter = itemsViewOriginal.CanFilter;
            if (isFilter)
            {
                itemsViewOriginal.Filter = (o) =>
                {
                    AccountsAccount account = (AccountsAccount)o;
                    if (string.IsNullOrEmpty(cmb.Text))
                    {
                        return true;
                    }
                    else
                    {
                        if (account.ID.StartsWith(cmb.Text) || account.AccountName.Contains(cmb.Text))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                };
            }
            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }
    }
}