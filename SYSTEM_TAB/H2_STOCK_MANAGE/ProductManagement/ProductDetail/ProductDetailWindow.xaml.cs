using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

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
            Messenger.Default.Send(new NotificationMessage(this, "CloseProductTabs"));
        }

        public static void ActivateProductDetailWindow()
        {
            Instance.Activate();
        }
    }
}