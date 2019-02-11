using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
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

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseLogin")
                    Close();
            });
        }

        private void UserName_OnKeyUp(object sender, KeyEventArgs e)
        
            {
            if (e.Key == Key.Enter)
                Password.Focus();
        }
        
    }
}