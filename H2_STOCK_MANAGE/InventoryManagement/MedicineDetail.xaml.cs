using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.ProductPurchaseRecord;
using System.ComponentModel;
using His_Pos.Class.StockTakingOrder;
using His_Pos.H2_STOCK_MANAGE.InventoryManagement;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// MedicineDetail.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineDetail : UserControl, INotifyPropertyChanged
    {
        public InventoryMedicine InventoryMedicine;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public MedicineDetail()
        {
            InitializeComponent();
            DataContext = this;

            InventoryMedicine = (InventoryMedicine)ProductDetail.NewProduct;
            ProductDetail.NewProduct = null;
        }
        
    }
}
