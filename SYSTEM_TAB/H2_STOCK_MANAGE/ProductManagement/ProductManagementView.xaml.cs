﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement
{
    /// <summary>
    /// ProductManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductManagementView : UserControl
    {
        public ProductManagementView()
        {
            InitializeComponent();
        }

        private void SearchTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
        }

        private void SearchTextbox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            //e.Handled = true;
            textBox.Focus();
        }

        private void btnMed_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.DimGray;
            ConRB.Visibility = Visibility.Visible;
            IceRB.Visibility = Visibility.Visible;
            StopRB.Visibility = Visibility.Visible;
            ZeroRB.Visibility = Visibility.Visible;
            MedBagAmt.Visibility = Visibility.Visible;
            ErrorAmt.Visibility = Visibility.Hidden;
            tbMedBagAmt.Visibility = Visibility.Visible;
            tbErrorAmt.Visibility = Visibility.Hidden;
            MedGroup.Visibility = Visibility.Visible;
            OTCGroup.Visibility = Visibility.Collapsed;

        }

        private void btnTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnMed.Background = Brushes.Transparent;
            btnMed.Foreground = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnTrade.Background = Brushes.DimGray;
            ConRB.Visibility = Visibility.Collapsed;
            IceRB.Visibility = Visibility.Collapsed;
            ZeroRB.Visibility = Visibility.Collapsed;
            MedBagAmt.Visibility = Visibility.Visible;
            ErrorAmt.Visibility = Visibility.Hidden;
            tbMedBagAmt.Visibility = Visibility.Visible;
            tbErrorAmt.Visibility = Visibility.Hidden;
            OTCGroup.Visibility = Visibility.Visible;
            MedGroup.Visibility = Visibility.Collapsed;
        }
    }
}
