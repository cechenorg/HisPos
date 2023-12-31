﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace His_Pos.SYSTEM_TAB.H5_ATTEND.ClockInSearch
{
    /// <summary>
    /// ClockInSearchView.xaml 的互動邏輯
    /// </summary>
    public partial class ClockInSearchView : UserControl, INotifyPropertyChanged
    {
        public ClockInSearchView()
        {
            InitializeComponent();
            _ = Account.Focus();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Account_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(Account.Text))
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        Password.Focus();
                        break;
                }
            }
        }
        private void PassWord_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty((sender as System.Windows.Controls.PasswordBox)?.Password))
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        AddClockIn.Focus();
                        break;
                }
            }
        }
    }
}
