﻿using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    /// <summary>
    /// MedPointControl.xaml 的互動邏輯
    /// </summary>
    public partial class MedPointControl : UserControl
    {
        public MedPointControl()
        {
            InitializeComponent();
        }

        private void Amount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            textBox.Focus();
            ((TextBox)sender).SelectAll();
        }

        private void Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Amount_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}