﻿using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking.StockTakingControl
{
    /// <summary>
    /// DifferenceReasonControl.xaml 的互動邏輯
    /// </summary>
    public partial class DifferenceReasonControl : UserControl
    {
        public DifferenceReasonControl()
        {
            InitializeComponent();
        }
        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e) {
            var t = sender as TextBox;
            if (e.Key != Key.Decimal) return;
            e.Handled = true;
            if (t != null) t.CaretIndex++;
        }
        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();

            ReasonDataGrid.SelectedItem = (textBox.DataContext as StockTakingProduct);
        }
        private void InputTextbox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            e.Handled = true;
            textBox.Focus();
        }
    }
}
