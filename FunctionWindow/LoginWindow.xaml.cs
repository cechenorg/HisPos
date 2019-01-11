using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input; 
using His_Pos.NewClass.Person;
using His_Pos.Database;

namespace His_Pos.FunctionWindow
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

            if (!IsConnectionDataValid()) {
                InitConnectionWindow initConnectionWindow = new InitConnectionWindow();
            } 
        }
         
        private bool IsConnectionDataValid()
        {
            CheckSettingFiles(); 
            SQLServerConnection localConnection = new SQLServerConnection(); 
            return localConnection.CheckConnection();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login();
        } 
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login();
        }

        private void UserName_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Password.Focus();
        }
        private void Login() {
            Employee user = Employee.Login(UserName.Text, Password.Password);
            if (user != null)
            {
                LoadingWindow loadingWindow = new LoadingWindow();
                loadingWindow.GetNecessaryData(user);
                Close();
            }
            else
            {
                ErrorStack.Visibility = Visibility.Visible;
                Password.Password = string.Empty;
            }
        }
        private static void CheckSettingFiles() {
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
            else
            {
                using (StreamReader fileReader = new StreamReader(filePath))
                {
                    Regex connReg = new Regex(@"[LG] Data Source=([0-9.]*),([0-9]*);Persist Security Info=True;User ID=([a-zA-Z0-9]*);Password=([a-zA-Z0-9]*)");
                    Regex printReg = new Regex(@"[MR][cp]* (.*)");
                    Match match;

                    match = connReg.Match(fileReader.ReadLine());
                    Properties.Settings.Default.SQL_local =
                        $"Data Source={match.Groups[1].Value},{match.Groups[2].Value};Persist Security Info=True;User ID={match.Groups[3].Value};Password={match.Groups[4].Value}";

                    match = connReg.Match(fileReader.ReadLine());
                    Properties.Settings.Default.SQL_global =
                        $"Data Source={match.Groups[1].Value},{match.Groups[2].Value};Persist Security Info=True;User ID={match.Groups[3].Value};Password={match.Groups[4].Value}";

                    match = printReg.Match(fileReader.ReadLine());
                    Properties.Settings.Default.MedBagPrinter = match.Groups[1].Value;

                    match = printReg.Match(fileReader.ReadLine());
                    Properties.Settings.Default.ReceiptPrinter = match.Groups[1].Value;

                    match = printReg.Match(fileReader.ReadLine());
                    Properties.Settings.Default.ReportPrinter = match.Groups[1].Value;

                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}