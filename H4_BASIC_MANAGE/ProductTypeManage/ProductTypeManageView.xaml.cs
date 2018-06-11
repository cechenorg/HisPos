using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using His_Pos.Class.ProductType;

namespace His_Pos.ProductTypeManage
{
    /// <summary>
    /// ProductTypeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTypeManageView : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<ProductTypeManageMaster> typeManageMasters = new ObservableCollection<ProductTypeManageMaster>();

        public ObservableCollection<ProductTypeManageMaster> TypeManageMasters
        {
            get { return typeManageMasters; }
            set
            {
                typeManageMasters = value;
                NotifyPropertyChanged("TypeManageMasters");
            }
        }

        private ObservableCollection<ProductTypeManageDetail> typeManageDetails = new ObservableCollection<ProductTypeManageDetail>();

        public ObservableCollection<ProductTypeManageDetail> TypeManageDetails
        {
            get { return typeManageDetails; }
            set
            {
                typeManageDetails = value;
                NotifyPropertyChanged("TypeManageDetails");
            }
        }

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                NotifyPropertyChanged("Products");
            }
        }

        public ProductTypeManageView()
        {
            InitializeComponent();

            DataContext = this;

            InitTypes();
            InitProducts();
        }

        private void InitProducts()
        {
            Products = ProductDb.GetProductTypeManageProducts();
        }

        private void InitTypes()
        {
            TypeManageMasters.Clear();
            TypeManageDetails.Clear();

            ProductDb.GetProductTypeManage(TypeManageMasters, TypeManageDetails);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
