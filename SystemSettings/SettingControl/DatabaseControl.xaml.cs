using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.Service;

namespace His_Pos.SystemSettings.SettingControl
{
    /// <summary>
    /// DatabaseControl.xaml 的互動邏輯
    /// </summary>
    public partial class DatabaseControl : UserControl
    {
        #region ----- Define Struct -----
        public struct ConnectionData
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

            public override string ToString()
            {
                return $"Data Source={IPAddr},{Port};Persist Security Info=True;User ID={Account};Password={Password}";
            }
        }
        #endregion

        #region ----- Define Variables -----

        enum ConnectionTarget
        {
            LOCAL = 1,
            GLOBAL = 2
        }

        public ConnectionData LocalConnection { get; set; }
        public ConnectionData GlobalConnection { get; set; }

        public bool IsDataChanged { get; set; } = false;
        #endregion

        public DatabaseControl()
        {
            InitializeComponent();
            DataContext = this;
            InitConnectioData();

            CheckConnection(ConnectionTarget.LOCAL);
            CheckConnection(ConnectionTarget.GLOBAL);
        }

        #region ----- Init Data -----

        private void InitConnectioData()
        {
            Regex reg = new Regex(@"Data Source=([0-9.]*),([0-9]*);Persist Security Info=True;User ID=([a-zA-Z0-9]*);Password=([a-zA-Z0-9]*)");
            Match match;
            
            string localConnection = Properties.Settings.Default.SQL_local;
            match = reg.Match(localConnection);
            LocalConnection = new ConnectionData(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value);
            
            string globalConnection = Properties.Settings.Default.SQL_global;
            match = reg.Match(globalConnection);
            GlobalConnection = new ConnectionData(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value);
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

        #endregion

        #region ----- Check Connection -----
        private void CheckConnection(ConnectionTarget connectionTarget)
        {
            DbConnection connection;

            switch (connectionTarget)
            {
                case ConnectionTarget.LOCAL:
                    connection = new DbConnection(LocalConnection.ToString());
                    break;
                case ConnectionTarget.GLOBAL:
                    connection = new DbConnection(GlobalConnection.ToString());
                    break;
                default:
                    return;
            }

            bool connectionState = false;

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (s, o) =>
            {
                connectionState = connection.CheckConnection();
            };

            backgroundWorker.RunWorkerCompleted += (s, o) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateConnectionMessage(connectionTarget, connectionState);
                }));
            };

            WaitingConnectionUi(connectionTarget);

            backgroundWorker.RunWorkerAsync();
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
                case ConnectionTarget.GLOBAL:
                    GSuccessStack.Visibility = Visibility.Collapsed;
                    GConnectingStack.Visibility = Visibility.Visible;
                    GErrorStack.Visibility = Visibility.Collapsed;
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
                    }
                    else
                    {
                        LSuccessStack.Visibility = Visibility.Collapsed;
                        LConnectingStack.Visibility = Visibility.Collapsed;
                        LErrorStack.Visibility = Visibility.Visible;
                    }
                    break;
                case ConnectionTarget.GLOBAL:
                    if (connectionState)
                    {
                        GSuccessStack.Visibility = Visibility.Visible;
                        GConnectingStack.Visibility = Visibility.Collapsed;
                        GErrorStack.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        GSuccessStack.Visibility = Visibility.Collapsed;
                        GConnectingStack.Visibility = Visibility.Collapsed;
                        GErrorStack.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        #endregion

        private void Textbox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            DataHasChanged();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            DataHasChanged();
        }

        private void CheckConnection_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            Button button = sender as Button;

            ConnectionTarget connectionTarget = (ConnectionTarget)Int16.Parse(button.Tag.ToString());

            CheckConnection(connectionTarget);
        }
    }
}
