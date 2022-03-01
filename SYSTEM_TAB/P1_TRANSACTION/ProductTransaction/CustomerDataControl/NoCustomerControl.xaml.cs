using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        private static string cusID = "0";

        public string CusID
        {
            get { return cusID; }
            set { cusID = value; }
        }

        public NoCustomerControl()
        {
            InitializeComponent();
        }

        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                if (obj is T)
                    yield return obj as T;

                foreach (DependencyObject child in LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>())
                    foreach (T c in FindLogicalChildren<T>(child))
                        yield return c;
            }
        }

        public void ClearView()
        {
            tbSearch.Text = "";

            lbName.Content = "";
            lbGender.Content = "";
            lbBirthDay.Content = "";
            lbCellphone.Content = "";
            lbTelephone.Content = "";
            tbAddress.Text = "";
            tbNote.Text = "";

            cusID = "0";
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

            foreach (DataGridTemplateColumn column in FindLogicalChildren<DataGridTemplateColumn>(Application.Current.MainWindow))
            {
                int count = 0;
                count++;
            }
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddNewCustomerWindow acw = new AddNewCustomerWindow();
            acw.RaiseCustomEvent += new EventHandler<CustomEventArgs>(acw_RaiseCustomEvent);
            acw.ShowDialog();
        }

        private void acw_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", int.Parse(e.Message)));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetCustomerByID]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("查無資料！", MessageType.ERROR);
            }
            else
            {
                FillInCustomerData(result);
            }
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
                    parameters.Add(new SqlParameter("Cus_Cellphone", tb.Text));
                    parameters.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
                }
                else
                {
                    parameters.Add(new SqlParameter("Cus_Cellphone", DBNull.Value));
                    parameters.Add(new SqlParameter("Cus_Telephone", tb.Text));
                }
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerQuery]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                if (result.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("查無資料！", MessageType.ERROR);
                }
                else
                {
                    FillInCustomerData(result);
                }
            }
        }
    }
}