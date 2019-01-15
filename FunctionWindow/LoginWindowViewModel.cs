﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Database;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Employee;

namespace His_Pos.FunctionWindow
{
    public class LoginWindowViewModel : ViewModelBase
    {
        #region ----- Define Command -----
        public RelayCommand<object> LoginCommand { get; set; }
        public RelayCommand LeaveCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public string Account { get; set; }
        private bool isAccountValid = false;
        public bool IsAccountWrong {
            get { return isAccountValid; }
            set { Set(() => IsAccountWrong, ref isAccountValid, value); }
        }
        #endregion

        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<object>(LoginAction);
            LeaveCommand = new RelayCommand(LeaveAction);

            if (!IsConnectionDataValid())
            {
                InitConnectionWindow initConnectionWindow = new InitConnectionWindow();
            }
        }

        #region ----- Define Actions -----
        private void LoginAction(object sender)
        {
            Employee user = Employee.Login(Account, (sender as PasswordBox)?.Password);
            if (user != null)
            {
                //LoadingWindow loadingWindow = new LoadingWindow();
                //loadingWindow.GetNecessaryData(user);
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                Messenger.Default.Send(new NotificationMessage("CloseLogin"));
            }
            else
            {
                IsAccountWrong = true;
            }
        }

        private void LeaveAction()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseLogin"));
        }
        #endregion

        #region ----- Define Functions -----
        private bool IsConnectionDataValid()
        {
            CheckSettingFiles();
            SQLServerConnection localConnection = new SQLServerConnection();
            return localConnection.CheckConnection();
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
            #endregion
        }
    }
}
