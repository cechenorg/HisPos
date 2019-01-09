﻿using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl
{
    /// <summary>
    /// DatabaseControl.xaml 的互動邏輯
    /// </summary>
    public partial class DatabaseControl : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Inner Class -----
        public class ConnectionData
        {
            public ConnectionData(string iPAddr, string port, string account, string password)
            {
                IPAddr = iPAddr;
                Port = port;
                Account = account;
                Password = password;
            }

            public string IPAddr { get; set; }
            public string Port { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }

            public bool ConnectionPass { get; set; } = false;

            public override string ToString()
            {
                return $"Data Source={IPAddr},{Port};Persist Security Info=True;User ID={Account};Password={Password}";
            }
        }
        #endregion

        #region ----- Define Variables -----

        enum ConnectionTarget
        {
            LOCAL = 1
        }

        private ConnectionData localConnection;
        public ConnectionData LocalConnection
        {
            get { return localConnection; }
            set
            {
                localConnection = value;
                NotifyPropertyChanged("LocalConnection");
            }
        }

        public bool IsDataChanged { get; set; } = false;

        BackgroundWorker LocalConnectionWorker = new BackgroundWorker();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public DatabaseControl()
        {
            InitializeComponent();
            DataContext = this;
            InitConnectioData();

            CheckConnection(ConnectionTarget.LOCAL);
        }

        #region ----- Init Data -----
        private void InitConnectioData()
        {
            Regex reg = new Regex(@"Data Source=([0-9.]*),([0-9]*);Persist Security Info=True;User ID=([a-zA-Z0-9]*);Password=([a-zA-Z0-9]*)");
            Match match;
            
            string localConnection = Properties.Settings.Default.SQL_local;
            match = reg.Match(localConnection);
            LocalConnection = new ConnectionData(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value);
            LocalPasswordBox.Password = LocalConnection.Password;

            LocalConnectionWorker.WorkerSupportsCancellation = true;
        }

        #endregion

        #region ----- Data Changed -----
        private void DataHasChanged()
        {
            IsDataChanged = true;

            UpdateDataChangedUi();
        }

        private void UpdateDataChangedUi()
        {
            if (IsDataChanged)
            {
                ChangedLabel.Content = "已修改";
                ChangedLabel.Foreground = Brushes.Red;

                CancelBtn.IsEnabled = true;
                ConfirmBtn.IsEnabled = true;
            }
            else
            {
                ChangedLabel.Content = "未修改";
                ChangedLabel.Foreground = Brushes.DimGray;

                CancelBtn.IsEnabled = false;
                ConfirmBtn.IsEnabled = false;
            }
        }

        private void ClearDataChangedStatus()
        {
            IsDataChanged = false;

            UpdateDataChangedUi();
        }

        #endregion

        #region ----- Check Connection -----
        private void CheckConnection(ConnectionTarget connectionTarget, bool showError = false)
        {
            if(!CheckConnectionFormat(connectionTarget, showError))
                return;

            DatabaseConnection connection;
            BackgroundWorker currentWorker;

            switch (connectionTarget)
            {
                case ConnectionTarget.LOCAL:
                    connection = null;/// new DatabaseConnection(LocalConnection.ToString());
                    currentWorker = LocalConnectionWorker;
                    break;
                default:
                    return;
            }

            if (currentWorker.IsBusy)
            {
                currentWorker.CancelAsync();
                currentWorker = new BackgroundWorker();
            }

            bool connectionState = false;
            
            currentWorker.DoWork += (s, o) =>
            {
                connectionState = true;/// connection.CheckConnection();
            };

            currentWorker.RunWorkerCompleted += (s, o) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateConnectionMessage(connectionTarget, connectionState);
                }));
            };

            WaitingConnectionUi(connectionTarget);

            currentWorker.RunWorkerAsync();
        }

        private bool CheckConnectionFormat(ConnectionTarget connectionTarget, bool showError)
        {
            Regex IPReg = new Regex(@"[0-9]{1,3}[.][0-9]{1,3}[.][0-9]{1,3}[.][0-9]{1,3}");
            Match IPMatch;

            switch (connectionTarget)
            {
                case ConnectionTarget.LOCAL:
                    IPMatch = IPReg.Match(LocalConnection.IPAddr);
                    break;
                default:
                    return false;
            }

            if (IPMatch.Success) return true;

            if (showError)
            {
                MessageWindow.ShowMessage("IP 格式錯誤!", MessageType.ERROR);
                
            }

            return false;
        }

        private void ResetConnectionStatus(ConnectionTarget connectionTarget)
        {
            switch (connectionTarget)
            {
                case ConnectionTarget.LOCAL:
                    if(LConnectingStack.Visibility == Visibility.Visible) return;
                    LSuccessStack.Visibility = Visibility.Collapsed;
                    LConnectingStack.Visibility = Visibility.Hidden;
                    LErrorStack.Visibility = Visibility.Collapsed;

                    localConnection.ConnectionPass = false;
                    break;
                default:
                    return;
            }
        }

        private void WaitingConnectionUi(ConnectionTarget connectionTarget)
        {
            switch (connectionTarget)
            {
                case ConnectionTarget.LOCAL:
                    LSuccessStack.Visibility = Visibility.Collapsed;
                    LConnectingStack.Visibility = Visibility.Visible;
                    LErrorStack.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void UpdateConnectionMessage(ConnectionTarget connectionTarget, bool connectionState)
        {
            switch (connectionTarget)
            {
                case ConnectionTarget.LOCAL:
                    if (connectionState)
                    {
                        LSuccessStack.Visibility = Visibility.Visible;
                        LConnectingStack.Visibility = Visibility.Collapsed;
                        LErrorStack.Visibility = Visibility.Collapsed;

                        localConnection.ConnectionPass = true;
                    }
                    else
                    {
                        LSuccessStack.Visibility = Visibility.Collapsed;
                        LConnectingStack.Visibility = Visibility.Collapsed;
                        LErrorStack.Visibility = Visibility.Visible;

                        localConnection.ConnectionPass = false;
                    }
                    break;
            }
        }

        #endregion

        #region ----- Service Function -----
        private bool IsNumbers(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return true;
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;

            return false;
        }
        private bool IsKeyAvailable(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return true;
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;
            if (key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right || key == Key.OemPeriod || key == Key.Decimal) return true;

            return false;
        }
        #endregion

        private void Textbox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is null) return;

            Control control = sender as Control;

            ConnectionTarget connectionTarget = (ConnectionTarget)Int16.Parse(control.Tag.ToString());
            ResetConnectionStatus(connectionTarget);

            DataHasChanged();
        }
        
        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            PasswordBox passwordBox = sender as PasswordBox;

            ConnectionTarget connectionTarget = (ConnectionTarget)Int16.Parse(passwordBox.Tag.ToString());
            ResetConnectionStatus(connectionTarget);

            DataHasChanged();
        }

        private void CheckConnection_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            Button button = sender as Button;

            ConnectionTarget connectionTarget = (ConnectionTarget)Int16.Parse(button.Tag.ToString());

            switch (connectionTarget)
            {
                case ConnectionTarget.LOCAL:
                    LocalConnection.Password = LocalPasswordBox.Password;
                    break;
            }

            CheckConnection(connectionTarget, true);
        }

        private void InitConnection_Click(object sender, RoutedEventArgs e)
        {
            ResetConnection();
        }

        public void ResetConnection()
        {
            InitConnectioData();

            CheckConnection(ConnectionTarget.LOCAL);

            ClearDataChangedStatus();
        }

        public void ConfirmConnectionChange_Click(object sender, RoutedEventArgs e)
        {
            LocalConnection.Password = LocalPasswordBox.Password;
            Properties.Settings.Default.SQL_local = LocalConnection.ToString();

            Properties.Settings.Default.Save();

            SaveConnectionToFile();

            ClearDataChangedStatus();
        }

        private void SaveConnectionToFile()
        {
            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            string leftLines = "";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                fileReader.ReadLine();
                fileReader.ReadLine();

                leftLines = fileReader.ReadToEnd();
            }

            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine("L " + Properties.Settings.Default.SQL_local);
                fileWriter.WriteLine("G " + Properties.Settings.Default.SQL_global);

                fileWriter.Write(leftLines);
            }
        }
    }
}
