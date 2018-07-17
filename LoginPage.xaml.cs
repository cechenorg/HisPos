using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.Service;

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
            User user = PersonDb.CheckUserPassword(UserName.Text, Password.Password);

            if (user.Id != null)
            {
                var loadingWindow = new LoadingWindow();
                loadingWindow.Show();
                loadingWindow.GetNecessaryData(user);
                Close();
            }
            else
            {
                ErrorStack.Visibility = Visibility.Visible;
                Password.Password = "";
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login_Click(sender, e);
        }

        private void UserName_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Password.Focus();
        }
    }
}