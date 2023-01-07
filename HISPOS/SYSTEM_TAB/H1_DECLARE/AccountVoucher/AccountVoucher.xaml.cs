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
            DataTable sourceTable = AccountsDb.GetSourceData(currentDetail);
            if(sourceTable != null && sourceTable.Rows.Count > 0)
            {
                DataColumn dc = new DataColumn("IsChecked", typeof(bool));
                sourceTable.Columns.Add(dc);
                sourceTable.Columns["IsChecked"].DefaultValue = false;
                for (int i = 0; i < sourceTable.Rows.Count; i++)
                {
                    sourceTable.Rows[i]["IsChecked"] = false;
                }
                var fromSourceWindow = new His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.FromSourceWindow.FromSourceWindow(sourceTable);
                fromSourceWindow.ShowDialog();
                
                if ((bool)fromSourceWindow.DialogResult)
                {
                    DataTable table = ((FromSourceViewModel)fromSourceWindow.DataContext).SoureTable;
                    if (table != null && table.Columns.Contains("IsChecked"))
                    {
                        DataRow[] selectRow = table.Select("IsChecked = true");
                        int i = (((JournalDetail)textBox.DataContext).JouDet_Type == "D") ?
                            currentViewModel.CurrentVoucher.DebitDetails.Count + 1 : currentViewModel.CurrentVoucher.CreditDetails.Count + 1;
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
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_Source = "StoOrd_ID";
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_WriteOffID = Convert.ToString(dr["JouDet_ID"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_WriteOffNumber = Convert.ToInt32(dr["JouDet_Number"]);
                                }
                                else
                                {
                                    index = currentViewModel.CurrentVoucher.CreditDetails.IndexOf(currentDetail);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_Amount = Convert.ToInt32(dr["JouDet_Amount"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_SourceID = Convert.ToString(dr["JouDet_SourceID"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_Source = "StoOrd_ID";
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_WriteOffID = Convert.ToString(dr["JouDet_ID"]);
                                    currentViewModel.CurrentVoucher.DebitDetails[index].JouDet_WriteOffNumber = Convert.ToInt32(dr["JouDet_Number"]);
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
                                currentViewModel.CurrentVoucher.DebitDetails.Add(detail);
                                i++;
                            }
                        }
                    }
                    currentViewModel.CurrentVoucher.DebitTotalAmount = (int)currentViewModel.CurrentVoucher.DebitDetails.Sum(s => s.JouDet_Amount);
                    currentViewModel.CurrentVoucher.CreditTotalAmount = (int)currentViewModel.CurrentVoucher.CreditDetails.Sum(s => s.JouDet_Amount);
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
    }
}
