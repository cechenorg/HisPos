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
using His_Pos.Class;
using His_Pos.Class.Product;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// ProductDetail.xaml 的互動邏輯
    /// </summary>
    public partial class ProductDetail : Window
    {
        public static Product NewProduct;

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
        }
     
        public void AddNewTab(Product newProduct)
        {
            NewProduct = newProduct;

            ((ViewModelProductDetailWindow)DataContext).AddTabCommandAction(new NewProductTab(NewProduct.Id,(NewProduct is InventoryMedicine)? SearchType.MED : SearchType.OTC));
        }
    }
}
