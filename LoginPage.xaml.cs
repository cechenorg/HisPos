using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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

            if (!IsConnectionDataValid())
            {
                InitConnectionWindow initConnectionWindow = new InitConnectionWindow();
                initConnectionWindow.ShowDialog();
            }
        }

        private static void CheckSettingFiles()
        {
            string folderPath = "C:\\Program Files\\HISPOS";

            bool folderExist = Directory.Exists(folderPath);

            if (!folderExist)
                Directory.CreateDirectory(folderPath);

            string filePath = folderPath + "\\settings.singde";

            bool fileExist = File.Exists(filePath);

            if (!fileExist)
            {
                File.Create(filePath).Dispose();

                using (TextWriter fileWriter = new StreamWriter(filePath))
                {
                    fileWriter.WriteLine("L Data Source=,;Persist Security Info=True;User ID=;Password=");
                    fileWriter.WriteLine("G Data Source=,;Persist Security Info=True;User ID=;Password=");

                    fileWriter.WriteLine("M ");
                    fileWriter.WriteLine("Rc ");
                    fileWriter.WriteLine("Rp ");
                }

                Properties.Settings.Default.SQL_local =
                    "Data Source=,;Persist Security Info=True;User ID=;Password=";
                Properties.Settings.Default.SQL_global =
                    "Data Source=,;Persist Security Info=True;User ID=;Password=";

                Properties.Settings.Default.MedBagPrinter = "";
                Properties.Settings.Default.ReceiptPrinter = "";
                Properties.Settings.Default.ReportPrinter = "";

                Properties.Settings.Default.Save();
            }
        }

        private bool IsConnectionDataValid()
        {
            CheckSettingFiles();

            DbConnection localConnection = new DbConnection(Properties.Settings.Default.SQL_local);
            if (!localConnection.CheckConnection()) return false;
            
            DbConnection globalConnection = new DbConnection(Properties.Settings.Default.SQL_global);
            if (!globalConnection.CheckConnection()) return false;

            return true;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            User user = PersonDb.CheckUserPassword(UserName.Text, Password.Password);

            if (!user.Id.Equals(""))
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