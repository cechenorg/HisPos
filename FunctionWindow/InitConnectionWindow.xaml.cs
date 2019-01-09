﻿using System.Windows;
using His_Pos.Class;

namespace His_Pos.FunctionWindow
{
    /// <summary>
    /// InitConnectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InitConnectionWindow : Window
    {
        public InitConnectionWindow()
        {
            InitializeComponent();
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            DatabaseControl.ConfirmConnectionChange_Click(sender, e);

            if(DatabaseControl.LocalConnection.ConnectionPass)
                Close();
            else
            {
                MessageWindow.ShowMessage("連線失敗 請確認資料是否正確", MessageType.ERROR);
                
            }
        }
    }
}
