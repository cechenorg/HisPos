using His_Pos.Class;
using His_Pos.Class.Manufactory;
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
using System.Windows.Shapes;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// AddNewOrderDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewOrderDialog : Window
    {
        public AddOrderType addOrderType;
        ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection;
        public bool ConfirmButtonClicked = false;
        public AddNewOrderDialog( ObservableCollection<Manufactory> manufactoryAutoCompleteCollection)
        {
            InitializeComponent();

            ManufactoryAutoCompleteCollection = manufactoryAutoCompleteCollection;
            ManufactoryAuto.ItemsSource = ManufactoryAutoCompleteCollection;
        }

        private void ChangeAddOrderType(object radioButton, RoutedEventArgs e)
        {
            addOrderType = (AddOrderType)Int16.Parse(((RadioButton)radioButton).Tag.ToString());
        }

        private void ConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            ConfirmButtonClicked = true;
            this.Close();
        }
    }
}
