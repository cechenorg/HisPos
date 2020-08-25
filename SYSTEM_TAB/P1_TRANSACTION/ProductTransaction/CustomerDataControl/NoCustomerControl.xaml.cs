using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl
{
    /// <summary>
    /// NoCustomerControl.xaml 的互動邏輯
    /// </summary>
    public partial class NoCustomerControl : UserControl
    {
        private bool isAnonymous = true;
        private string cusID;

        public NoCustomerControl()
        {
            InitializeComponent();
        }

        public string ReturnCusID() 
        {
            if (isAnonymous)
            {
                return "0";
            }
            else 
            {
                return cusID;
            }
        }

        public void ClearView() 
        {
            lbName.Content = "";
            lbGender.Content = "";
            lbBirthDay.Content = "";
            lbCellphone.Content = "";
            lbTelephone.Content = "";
            tbAddress.Text = "";
            tbNote.Text = "";

            isAnonymous = true;
        }

        private void FillInCustomerData(DataTable result) 
        {
            cusID = result.Rows[0]["PosCus_Uid"].ToString();

            lbName.Content = result.Rows[0]["PosCus_Name"].ToString();
            lbGender.Content = result.Rows[0]["PosCus_Gender"].ToString();
            lbBirthDay.Content = result.Rows[0]["PosCus_Birthday"].ToString();
            lbCellphone.Content = result.Rows[0]["PosCus_Cellphone"].ToString();
            lbTelephone.Content = result.Rows[0]["PosCus_Telephone"].ToString();
            tbAddress.Text = result.Rows[0]["PosCus_Address"].ToString();
            tbNote.Text = result.Rows[0]["PosCus_Note"].ToString();
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddNewCustomerWindow acw = new AddNewCustomerWindow();
            acw.ShowDialog();
        }

        private void btnClearCustomer_Click(object sender, RoutedEventArgs e)
        {
            ClearView();
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                bool isCell = tb.Text.StartsWith("09");
                if (isCell)
                {
                    parameters.Add(new SqlParameter("PosCus_Cellphone", tb.Text));
                    parameters.Add(new SqlParameter("PosCus_Telephone", DBNull.Value));
                }
                else 
                {
                    parameters.Add(new SqlParameter("PosCus_Cellphone", DBNull.Value));
                    parameters.Add(new SqlParameter("PosCus_Telephone", tb.Text));
                }
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerQuery]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                if (result.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("查無資料！", MessageType.ERROR);
                }
                else 
                {
                    isAnonymous = false;
                    FillInCustomerData(result);
                }
            }
        }

        
    }
}
