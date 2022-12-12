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
    }
}
