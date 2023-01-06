using His_Pos.NewClass.Accounts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.FromSourceWindow;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher
{
    /// <summary>
    /// AccountVoucher.xaml 的互動邏輯
    /// </summary>
    public partial class AccountVoucher : UserControl
    {
        public AccountVoucher()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid?.SelectedItem is null) return;

            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }

        private void DebitGrid_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            DebitTotalAmount.Content = ((ObservableCollection<JournalDetail>)DebitGrid.ItemsSource).Sum(S=>S.JouDet_Amount);
        }

        private void CreditGrid_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CreditTotalAmount.Content = ((ObservableCollection<JournalDetail>)CreditGrid.ItemsSource).Sum(S => S.JouDet_Amount);
        }

        private void CreditGrid_TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if(((JournalDetail)textBox.DataContext).JouDet_Amount == 0)
            {
                ((JournalDetail)textBox.DataContext).JouDet_Amount = (int)DebitTotalAmount.Content - (int)CreditTotalAmount.Content;
            }
        }

        private void DebitGrid_TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (((JournalDetail)textBox.DataContext).JouDet_Amount == 0)
            {
                ((JournalDetail)textBox.DataContext).JouDet_Amount = (int)CreditTotalAmount.Content - (int)DebitTotalAmount.Content;
            }
        }


        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var currentRow = textBox.BindingGroup;
            JournalDetail currentDetail = (JournalDetail)currentRow.Items[0];
            AccountVoucherViewModel currentViewModel = (AccountVoucherViewModel)DataContext;

            if (currentViewModel.CurrentVoucher != null && currentViewModel.CurrentVoucher.JouMas_Status.Equals("T"))
            {
                return;
            }
            if (currentDetail is null || currentDetail.Account is null)
            {
                return;
            }

            var fromSourceWindow = new His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.FromSourceWindow.FromSourceWindow();
            fromSourceWindow.ShowDialog();
            if((bool)fromSourceWindow.DialogResult)
            {
                DataTable table = ((FromSourceViewModel)fromSourceWindow.DataContext).SoureTable;
                if (table != null && table.Columns.Contains("IsCheck"))
                {
                    DataRow[] selectRow = table.Select("IsCheck = true");
                    foreach (DataRow dr in selectRow)
                    {
                        JournalDetail detail = new JournalDetail();
                        detail.Accounts = ((JournalDetail)currentRow.Items[0]).Accounts;
                        detail.Account = ((JournalDetail)currentRow.Items[0]).Account;
                        currentRow.Items.Add(detail);
                    }
                }
            }
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox cob = (ComboBox)sender;
            cob.Background = Brushes.White;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cob = (ComboBox)sender;
            cob.Background = Brushes.Transparent;
        }
    }
}
