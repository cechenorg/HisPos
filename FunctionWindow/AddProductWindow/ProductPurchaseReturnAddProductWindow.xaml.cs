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
using System.Windows.Shapes;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// ProductPurchaseReturnAddProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseReturnAddProductWindow : Window
    {
        public ProductPurchaseReturnAddProductWindow(string searchString, AddProductEnum addProductEnum)
        {
            InitializeComponent();

            DataContext = new AddProductViewModel(searchString, addProductEnum);
        }
    }
}
