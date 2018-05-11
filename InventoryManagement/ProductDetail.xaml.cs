using His_Pos.ViewModel;
using MenuUserControl;
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
using His_Pos.AbstractClass;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// ProductDetail.xaml 的互動邏輯
    /// </summary>
    public partial class ProductDetail : Window
    {
        //((ViewModelProductDetailWindow)DataContext).AddTabCommandAction("OTC");

        public ProductDetail()
        {
            InitializeComponent();
        }
     
        public void AddNewTab( Product newProduct)
        {
            
        }
    }
}
