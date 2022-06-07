using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using System;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail
{
    /// <summary>
    /// ProductDetail.xaml 的互動邏輯
    /// </summary>
    public partial class ProductDetail : Window
    {
        public static ProductDetail Instance;

        public double StartTop => (SystemParameters.WorkArea.Height - this.Height) / 2;
        public double StartLeft => (SystemParameters.WorkArea.Width - this.Width) / 2;

        public double WindowWidth => SystemParameters.WorkArea.Width * 0.8;
        public double WindowHeight => SystemParameters.WorkArea.Height * 0.8;

        public struct NewProductTab
        {
            public NewProductTab(string id, SearchType type)
            {
                Id = id;
                Type = type;
            }

            public string Id { get; set; }
            public SearchType Type { get; set; }
        }

        public ProductDetail()
        {
            InitializeComponent();

            Title = "商品詳細資料管理";
            // WindowState = WindowState.Maximized;

            Instance = this;
        }

       

        private void ProductDetail_OnClosed(object sender, EventArgs e)
        {
            Instance = null;
            ((ViewModelProductDetailWindow)DataContext).ItemCollection.Clear();
        }
    }
}