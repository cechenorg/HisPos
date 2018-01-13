using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using His_Pos.Class;

namespace His_Pos
{
    /// <summary>
    /// LoginPage.xaml 的互動邏輯
    /// </summary>
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
            Height = SystemParameters.PrimaryScreenHeight * 0.85;
            Width = Height * 0.77;
            UserName.Focus();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            User userLogin = new User();
            string[] auth = new string[2];
            userLogin.Id = UserName.Text;
            userLogin.Password = Password.Text;
            if (CheckAccount(ref userLogin, ref auth))
            {
                userLogin.Authority.HisFeatures = auth.ToList();
                LoadingWindow loadingWindow = new LoadingWindow();
                loadingWindow.Show();
                loadingWindow.GetMedicineData(userLogin);
                Close();
            }
            else
            {
                ErrorStack.Visibility = Visibility.Visible;
            }
        }

        private void UpdateUserAuthPos(List<string> authArray) {
            int value = 0;
            foreach (string auth in authArray) {
                value += (int)(LoginAuth)Enum.Parse(typeof(LoginAuth),auth);
            }
            List<SqlParameter> listparam = new List<SqlParameter>();
            SqlParameter sqlEmpId= new SqlParameter("@EMPID", MainWindow.CurrentUser.Id);
            SqlParameter sqlAuth = new SqlParameter("@AUTH", value);
            listparam.Add(sqlEmpId);
            listparam.Add(sqlAuth);
            DbConnection dbConn = new DbConnection(Properties.Settings.Default.SQL_local);
            dbConn.ExecuteProc( "[POSHIS_Test].[DBO].[UPDATEUSERAUTH_POS]", listparam);
        }

        private bool CheckAccount(ref User Login, ref string[] authArray)
        {
            bool isPass = false;
            LoginAuth auth;
            try
            {
                List<SqlParameter> listparam = new List<SqlParameter>();
                SqlParameter sqlAccount = new SqlParameter("@ACCOUNT", Login.Id);
                SqlParameter sqlPassword = new SqlParameter("@PASSWORD", Login.Password);
                listparam.Add(sqlAccount);
                listparam.Add(sqlPassword);
                DbConnection dbConn = new DbConnection(Properties.Settings.Default.SQL_global);
                DataTable table = dbConn.ExecuteProc("[HIS_POS_DB].[GET].[CHECKLOGIN]", listparam);
                if (table.Rows.Count != 0){
                    auth = (LoginAuth) table.Rows[0]["EMPAUT_POS"];
                    authArray = auth.ToString().Split(',');
                    Login.Id = table.Rows[0]["EMP_ID"].ToString();
                    Login.IcNumber = table.Rows[0]["EMP_IDNUM"].ToString();
                    Login.Name = table.Rows[0]["EMP_NAME"].ToString();
                    isPass = true;
                }
            }
            catch (Exception ex)
            {
                DbConnection dbConn = new DbConnection(Properties.Settings.Default.SQL_local);
                dbConn.Log("His", "CheckAccount", ex.Message);
            }
            return isPass;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login_Click(sender, e);
        }
    }
}
