using His_Pos.Extention;
using His_Pos.NewClass.Accounts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.LedgerWindow
{
    /// <summary>
    /// LedgerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LedgerWindow : Window
    {
        public LedgerWindow()
        {
            InitializeComponent();
        }
        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            IEnumerable<JournalAccount> Accounts = ((LedgerViewModel)DataContext).Accounts;
            ICollectionView itemsViewOriginal = CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            bool isFilter = itemsViewOriginal.CanFilter;
            if (isFilter)
            {
                List<JournalAccount> accounts = new List<JournalAccount>();
                if (string.IsNullOrEmpty(cmb.Text))
                {
                    cmb.ItemsSource = Accounts;
                }
                else
                {
                    foreach (JournalAccount account in Accounts)
                    {
                        if (account.AcctFullName.Contains(cmb.Text))
                        {
                            accounts.Add(account);
                        }
                    }
                    cmb.ItemsSource = accounts;
                }
            }
            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
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

        private void MaskedTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaskedTextBox textBox = (MaskedTextBox)sender;
            if (textBox != null)
            {
                textBox.Text = DateTimeFormatExtention.ToTaiwanDateTime(DateTime.Today);
            }
        }
    }
}
