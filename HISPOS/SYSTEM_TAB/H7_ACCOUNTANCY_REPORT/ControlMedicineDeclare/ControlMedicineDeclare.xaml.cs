﻿using His_Pos.Service;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare
{
    /// <summary>
    /// ControlMedicineDeclare.xaml 的互動邏輯
    /// </summary>
    public partial class ControlMedicineDeclare : UserControl
    {
        public ControlMedicineDeclare()
        {
            InitializeComponent();
        }

        private void DateMaskedTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }
    }
}