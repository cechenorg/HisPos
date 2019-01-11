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
        }
         
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UserName_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Password.Focus();
        }
        
    }
}