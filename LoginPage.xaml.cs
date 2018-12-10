﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.Service;
using Microsoft.SqlServer.Dts.Runtime;
using Application = System.Windows.Application;

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

        private bool IsConnectionDataValid()
        {
            CheckSettingFiles();

            DbConnection localConnection = new DbConnection(Properties.Settings.Default.SQL_local);
            if (!localConnection.CheckConnection()) return false;

            return true;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            User user = PersonDb.CheckUserPassword(UserName.Text, Password.Password);

            if (!user.Id.Equals(""))
            {
                try
                {
                    CheckDBVersion();
                    SyncNewProductDataFromSingde();
                }
                catch (Exception ex)
                {
                    MessageWindow messageWindow = new MessageWindow(ex.Message, MessageType.ERROR);
                    messageWindow.ShowDialog();
                }

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

        private void SyncNewProductDataFromSingde()
        {
            Microsoft.SqlServer.Dts.Runtime.Application app = new Microsoft.SqlServer.Dts.Runtime.Application();
            Microsoft.SqlServer.Dts.Runtime.Package package = null;

            try
            {
                Regex reg = new Regex(@"Data Source=([0-9.]*,[0-9]*);Persist Security Info=True;User ID=[a-zA-Z0-9]*;Password=[a-zA-Z0-9]*");
                Match match = reg.Match(Properties.Settings.Default.SQL_local);

                package = app.LoadPackage(Directory.GetCurrentDirectory() + @"\SSIS_Package\SyncProductDataFromSingdePackage.dtsx", null);
                Variables myVars = package.Variables;
                
                myVars["LocalConn"].Value = $"Data Source={match.Groups[1].Value};Initial Catalog=HIS_POS_DB;Persist Security Info=True;User ID=singde;Password=city1234;";

                DTSExecResult results = package.Execute(null, myVars, null, null, null);

                if (results == DTSExecResult.Failure)
                {
                    string errorMessage = "";

                    foreach (DtsError error in package.Errors)
                    {
                        errorMessage += error.ErrorCode.ToString() + error.Description.ToString() + "\n";
                    }

                    string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\packageError.txt";

                    try
                    {
                        using (TextWriter fileWriter = new StreamWriter(filePath))
                        {
                            fileWriter.WriteLine(errorMessage);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageWindow messageWindow = new MessageWindow(exception.Message, MessageType.ERROR);
                messageWindow.ShowDialog();
            }
            finally
            {
                package.Dispose();
                package = null;
            }
        }

        private void CheckDBVersion()
        {
            string versionId = FunctionDb.GetSystemVersionId();
            if (!versionId.Equals(Assembly.GetExecutingAssembly().GetName().Version.ToString()))
            {
                Regex reg = new Regex(@"Data Source=([0-9.]*,[0-9]*);Persist Security Info=True;User ID=[a-zA-Z0-9]*;Password=[a-zA-Z0-9]*");
                Match match = reg.Match(Properties.Settings.Default.SQL_local);
                
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                startInfo.FileName = "SQLPackage\\sqlpackage.exe";
                startInfo.Arguments = $@"/a:Publish /sf:""SQLPackage\\ServerDb.dacpac"" /tsn:{match.Groups[1].Value} /tu:singde /tp:city1234 /tdn:HIS_POS_DB /p:""IncludeCompositeObjects = True"" /p:""BlockOnPossibleDataLoss = False"" /p:""DropObjectsNotInSource = True"" /p:""DoNotDropObjectType = Permissions"" /p:""DoNotDropObjectType = DatabaseRoles""  /p:""DoNotDropObjectType = Logins"" /p:""DoNotDropObjectType = ServerRoles""";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                FunctionDb.UpdateSystemVersionId();
            }
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