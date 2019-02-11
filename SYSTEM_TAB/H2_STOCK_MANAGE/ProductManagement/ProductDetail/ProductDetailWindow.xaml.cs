﻿using System;
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
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail
{
    /// <summary>
    /// ProductDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProductDetailWindow : Window
    {
        private static ProductDetailWindow Instance { get; set; }

        public ProductDetailWindow()
        {
            InitializeComponent();
            Show();
        }

        public static void ShowProductDetailWindow()
        {
            if (Instance is null)
                Instance = new ProductDetailWindow();

            Instance.Activate();
        }
        private void ProductDetailWindow_OnClosed(object sender, EventArgs e)
        {
            Instance = null;
        }
    }
}