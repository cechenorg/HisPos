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
using His_Pos.NewClass.Report.Accounts;
using System.Threading;
using His_Pos.FunctionWindow;
using His_Pos.Class;
using System.ComponentModel;
using System.Text.RegularExpressions;

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
                CreditTotalAmount.Content = ((ObservableCollection<JournalDetail>)CreditGrid.ItemsSource).Sum(S => S.JouDet_Amount);
            }
        }

        private void DebitGrid_TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (((JournalDetail)textBox.DataContext).JouDet_Amount == 0)
            {
                ((JournalDetail)textBox.DataContext).JouDet_Amount = (int)CreditTotalAmount.Content - (int)DebitTotalAmount.Content;
                DebitTotalAmount.Content = ((ObservableCollection<JournalDetail>)DebitGrid.ItemsSource).Sum(S => S.JouDet_Amount);
            }
        }


        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var currentRow = textBox.BindingGroup;
            JournalDetail currentDetail = (JournalDetail)currentRow.Items[0];
            AccountVoucherViewModel currentViewModel = (AccountVoucherViewModel)DataContext;

            if (currentViewModel.CurrentVoucher is null)
            {
                return;
            }
            if (currentDetail is null || currentDetail.Account is null)
            {
                return;
            }

            bool isWriteOff = currentDetail.Account.AcctWriteOff;
            if (!isWriteOff)
                return;

            DataTable sourceTable = AccountsDb.GetSourceData(currentDetail, DateTime.Today);

            if (sourceTable != null && sourceTable.Rows.Count > 0)
            {
                foreach (JournalDetail item in currentViewModel.CurrentVoucher.DebitDetails)
                {
                    if (item != currentDetail)
                    {
                        if (!string.IsNullOrEmpty(item.JouDet_WriteOffID))
                        {
                            DataRow[] drs = sourceTable.Select(string.Format("JouDet_ID = '{0}' And JouDet_Number = '{1}' And JouDet_SourceID = '{2}'", item.JouDet_WriteOffID, item.JouDet_WriteOffNumber, item.JouDet_SourceID));
                            if (drs != null && drs.Count() > 0)
                            {
                                foreach (DataRow dr in drs)
                                {
                                    sourceTable.Rows.Remove(dr);
                                }
                            }
                        }
                    }
                }

                DataColumn dc = new DataColumn("IsChecked", typeof(bool));
                sourceTable.Columns.Add(dc);
                sourceTable.Columns["IsChecked"].DefaultValue = false;
                for (int i = 0; i < sourceTable.Rows.Count; i++)
                {
                    sourceTable.Rows[i]["IsChecked"] = false;
                }

                FromSourceWindow.FromSourceWindow fromSourceWindow = new FromSourceWindow.FromSourceWindow(sourceTable);
                fromSourceWindow.ShowDialog();
                bool isSubmit = ((FromSourceViewModel)fromSourceWindow.DataContext).IsSubmit;
                if (isSubmit)
                {
                    DataTable table = ((FromSourceViewModel)fromSourceWindow.DataContext).SelectTable;
                    if (table != null && table.Columns.Contains("IsChecked"))
                    {
                        DataRow[] selectRow = table.Select("IsChecked = true");
                        int i = (((JournalDetail)textBox.DataContext).JouDet_Type == "D") ?
                            currentViewModel.CurrentVoucher.DebitDetails.Max(m => m.JouDet_Number) + 1 : currentViewModel.CurrentVoucher.CreditDetails.Max(m => m.JouDet_Number) + 1;
                        bool updCurrentRow = true;
                        foreach (DataRow dr in selectRow)
                        {
                            if(updCurrentRow)
                            {
                                int index = 0;
                                if (((JournalDetail)textBox.DataContext).JouDet_Type == "D")
                                {
                                    index = currentViewModel.CurrentVoucher.DebitDetails.IndexOf(currentDetail);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_Amount = Convert.ToInt32(dr["JouDet_Amount"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_SourceID = Convert.ToString(dr["JouDet_SourceID"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_Source = Convert.ToString(dr["JouDet_Source"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_WriteOffID = Convert.ToString(dr["JouDet_ID"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_WriteOffNumber = Convert.ToInt32(dr["JouDet_Number"]);
                                    if (string.IsNullOrEmpty(currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_Memo))
                                    {
                                        currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_Memo = Convert.ToString(dr["JouDet_Memo"]);
                                    }
                                }
                                else
                                {
                                    index = currentViewModel.CurrentVoucher.CreditDetails.IndexOf(currentDetail);
                                    currentViewModel.CurrentVoucher.CreditDetails[index].JouDet_Amount = Convert.ToInt32(dr["JouDet_Amount"]);
                                    currentViewModel.CurrentVoucher.CreditDetails[index].JouDet_SourceID = Convert.ToString(dr["JouDet_SourceID"]);
                                    currentViewModel.CurrentVoucher.CreditDetails[index].JouDet_Source = Convert.ToString(dr["JouDet_Source"]);
                                    currentViewModel.CurrentVoucher.CreditDetails[index].JouDet_WriteOffID = Convert.ToString(dr["JouDet_ID"]);
                                    currentViewModel.CurrentVoucher.CreditDetails[index].JouDet_WriteOffNumber = Convert.ToInt32(dr["JouDet_Number"]);
                                    if (string.IsNullOrEmpty(currentViewModel.CurrentVoucher.CreditDetails[index].JouDet_Memo))
                                    {
                                        currentViewModel.CurrentVoucher.CreditDetails[index].JouDet_Memo = Convert.ToString(dr["JouDet_Memo"]);
                                    }
                                }
                                updCurrentRow = false;
                            }
                            else
                            {
                                JournalDetail detail = new JournalDetail();
                                detail.Accounts = ((JournalDetail)currentRow.Items[0]).Accounts;
                                detail.Account = ((JournalDetail)currentRow.Items[0]).Account;
                                detail.JouDet_ID = currentViewModel.CurrentVoucher.JouMas_ID;
                                detail.JouDet_Number = i;
                                detail.JouDet_Type = ((JournalDetail)textBox.DataContext).JouDet_Type;
                                detail.JouDet_Amount = Convert.ToInt32(dr["JouDet_Amount"]);
                                detail.JouDet_Source = "StoOrd_ID";
                                detail.JouDet_SourceID = Convert.ToString(dr["JouDet_SourceID"]);
                                detail.JouDet_WriteOffID = Convert.ToString(dr["JouDet_ID"]);
                                detail.JouDet_WriteOffNumber = Convert.ToInt32(dr["JouDet_Number"]);
                                detail.JouDet_Memo = Convert.ToString(dr["JouDet_Memo"]);
                                if (((JournalDetail)textBox.DataContext).JouDet_Type == "D")
                                {
                                    currentViewModel.CurrentVoucher.DebitDetails.Add(detail);
                                }
                                else
                                {
                                    currentViewModel.CurrentVoucher.CreditDetails.Add(detail);
                                }
                                    
                                i++;
                            }
                        }
                    }
                    currentViewModel.CurrentVoucher.DebitTotalAmount = (int)currentViewModel.CurrentVoucher.DebitDetails.Sum(s => s.JouDet_Amount);
                    currentViewModel.CurrentVoucher.CreditTotalAmount = (int)currentViewModel.CurrentVoucher.CreditDetails.Sum(s => s.JouDet_Amount);
                    currentViewModel.ResetRowNo();
                }
            }
            else
            {
                MessageWindow.ShowMessage("此科目沒有沖帳來源!", MessageType.WARNING);
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
        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            IEnumerable<JournalAccount> Accounts = AccountsDb.GetJournalAccount("傳票作業");
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

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                TextBox box = (TextBox)sender;
                JournalDetail item = (JournalDetail)box.DataContext;
                item.JouDet_WriteOffID = string.Empty;
                item.JouDet_WriteOffNumber = 0;
                item.JouDet_SourceID = string.Empty;
                item.JouDet_Source = string.Empty;
            }
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            if (!box.IsDropDownOpen)
            {
                e.Handled = true;
            }
        }

        private void DeleteDot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            JournalDetail detail = (JournalDetail)image.DataContext;
            AccountVoucherViewModel viewModel = new AccountVoucherViewModel();
            viewModel = (AccountVoucherViewModel)DataContext;
            viewModel.DeleteDetailAction(detail.JouDet_Type == "D" ? "0" : "1");
        }
    }
}
