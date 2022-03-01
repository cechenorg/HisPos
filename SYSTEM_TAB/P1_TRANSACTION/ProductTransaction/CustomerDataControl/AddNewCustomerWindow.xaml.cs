using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl
{
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string s)
        {
            msg = s;
        }

        private string msg;

        public string Message
        {
            get { return msg; }
        }
    }

    /// <summary>
    /// AddNewCustomerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewCustomerWindow : Window
    {
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public bool IsTelephone(string str_telephone)
        {
            return Regex.IsMatch(str_telephone, @"\d{2,3}\d{3,4}\d{4}");
        }

        public bool IsCellphone(string str_handset)
        {
            return Regex.IsMatch(str_handset, @"^09[0-9]{8}$");
        }

        public static bool ValidateDateTime(string datetime, string format)
        {
            if (datetime == null || datetime.Length == 0)
            {
                return false;
            }
            try
            {
                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                dtfi.FullDateTimePattern = format;
                DateTime dt = DateTime.ParseExact(datetime, "F", dtfi);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public AddNewCustomerWindow()
        {
            InitializeComponent();
        }

        private void FocusNext(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                MoveFocus(request);
            }
        }

        private string GetBirthday()
        {
            if (tbYear.Text == "" && tbMonth.Text == "" && tbDay.Text == "")
            {
                return "";
            }

            int.TryParse(tbYear.Text, out int year);
            int.TryParse(tbMonth.Text, out int month);
            int.TryParse(tbDay.Text, out int day);
            string yearStr = (year + 1911).ToString();
            string dateStr = yearStr + month.ToString("00") + day.ToString("00");

            bool isDate = ValidateDateTime(dateStr, "yyyyMMdd");
            if (isDate)
            {
                return dateStr;
            }
            else
            {
                return "ERROR";
            }
        }

        private string GetGender()
        {
            if (rbMale.IsChecked == true)
            {
                return "男";
            }
            else
            {
                return "女";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string birthday = GetBirthday();
            string tel = tbTelPhoneCode.Text + tbTelPhone.Text;

            if (tbCellPhone.Text == "" && tbTelPhone.Text == "")
            {
                MessageWindow.ShowMessage("手機 / 市話 必擇一填寫！", MessageType.ERROR);
                return;
            }
            if (!IsCellphone(tbCellPhone.Text) && tbCellPhone.Text != "")
            {
                MessageWindow.ShowMessage("手機號碼格式錯誤！", MessageType.ERROR);
                return;
            }
            if (!IsTelephone(tel) && tel != "")
            {
                MessageWindow.ShowMessage("家電號碼格式錯誤！", MessageType.ERROR);
                return;
            }
            if (birthday == "ERROR")
            {
                MessageWindow.ShowMessage("生日格式錯誤！", MessageType.ERROR);
                return;
            }

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (tbCellPhone.Text == "")
            {
                parameters.Add(new SqlParameter("PosCus_Cellphone", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("PosCus_Cellphone", tbCellPhone.Text));
            }
            if (tbTelPhone.Text == "")
            {
                parameters.Add(new SqlParameter("PosCus_Telephone", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("PosCus_Telephone", tbTelPhone.Text));
            }
            parameters.Add(new SqlParameter("PosCus_Name", tbName.Text));
            parameters.Add(new SqlParameter("PosCus_Birthday", birthday));
            parameters.Add(new SqlParameter("PosCus_Gender", GetGender()));
            parameters.Add(new SqlParameter("PosCus_Address", tbAddress.Text));
            parameters.Add(new SqlParameter("PosCus_Note", tbNote.Text));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerInsert]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("新增成功！", MessageType.SUCCESS);
                RaiseCustomEvent(this, new CustomEventArgs(result.Rows[0].Field<int>("ID").ToString()));
                Close();
            }
            else if (result.Rows[0].Field<string>("RESULT").Equals("SAME"))
            {
                MessageWindow.ShowMessage("該電話號碼已登錄會員！", MessageType.WARNING);
            }
            else
            {
                MessageWindow.ShowMessage("新增失敗！", MessageType.ERROR);
            }
        }

        private void tbCellPhone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbTelPhoneCode.Focus(); }
        }

        private void tbTelPhoneCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbTelPhone.Focus(); }
        }

        private void tbTelPhone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbName.Focus(); }
        }

        private void tbName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbYear.Focus(); }
        }

        private void tbYear_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbMonth.Focus(); }
        }

        private void tbMonth_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbDay.Focus(); }
        }

        private void tbDay_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbAddress.Focus(); }
        }

        private void tbAddress_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbNote.Focus(); }
        }
    }
}