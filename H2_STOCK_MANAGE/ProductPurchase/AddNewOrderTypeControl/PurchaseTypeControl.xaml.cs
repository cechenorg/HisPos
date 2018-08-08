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
    /// PurchaseTypeControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseTypeControl : UserControl
    {
        private ObservableCollection<Manufactory> manufactoryAutoCompleteCollection;

        public PurchaseTypeControl()
        {
            InitializeComponent();
        }

        public PurchaseTypeControl(ObservableCollection<Manufactory> manufactoryAutoCompleteCollection)
        {
            this.manufactoryAutoCompleteCollection = manufactoryAutoCompleteCollection;
        }

        private void RadioButton_TargetOnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton is null || Con1 is null) return;

            switch (radioButton.Tag)
            {
                case "2":
                    Con1.IsEnabled = false;
                    Con2.IsEnabled = true;
                    Con3.IsEnabled = true;
                    Con4.IsEnabled = true;
                    Con2.IsChecked = true;
                    break;
                case "5":
                    Con1.IsEnabled = true;
                    Con2.IsEnabled = true;
                    Con3.IsEnabled = true;
                    Con4.IsEnabled = true;
                    break;
            }
        }

        private void ManufactoryAuto_OnDropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;

            if (autoCompleteBox is null || autoCompleteBox.SelectedItem is null) return;

            Manufactory = autoCompleteBox.SelectedItem as Manufactory;
        }

        private void ManufactoryAuto_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TargetManufactory.IsChecked = true;
        }
    }
}
