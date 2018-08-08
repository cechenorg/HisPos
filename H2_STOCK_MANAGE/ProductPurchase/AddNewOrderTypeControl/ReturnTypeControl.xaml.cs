using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using His_Pos.Class.Manufactory;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.AddNewOrderTypeControl
{
    /// <summary>
    /// ReturnTypeControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnTypeControl : UserControl
    {
        private ObservableCollection<Manufactory> manufactoryAutoCompleteCollection;

        public ReturnTypeControl()
        {
            InitializeComponent();
        }

        public ReturnTypeControl(ObservableCollection<Manufactory> manufactoryAutoCompleteCollection)
        {
            this.manufactoryAutoCompleteCollection = manufactoryAutoCompleteCollection;
        }
    }
}
