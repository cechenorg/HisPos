﻿using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    /// <summary>
    /// BalanceSheetView.xaml 的互動邏輯
    /// </summary>
    public partial class ClosingWorkView : UserControl
    {
        public ClosingWorkView()
        {
            InitializeComponent();
        }

        private void KeyIn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            textBox.Focus();
            ((TextBox)sender).SelectAll();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OTC_CASH.IsChecked != true || OTC_CARD.IsChecked != true || OTC_TICKET.IsChecked != true || OTC_CASHTICKET.IsChecked != true || OTHER_CASH.IsChecked != true || PAY_CASH.IsChecked != true || MED_CASH.IsChecked != true || PREPAY_CASH.IsChecked != true || PREPAY_CARD.IsChecked != true || RETURN_PRECASH.IsChecked != true || RETURN_PRECARD.IsChecked != true)
            {
                MessageWindow.ShowMessage("請確認所有項目！", MessageType.ERROR);
                return;
            }
        }
    }
}