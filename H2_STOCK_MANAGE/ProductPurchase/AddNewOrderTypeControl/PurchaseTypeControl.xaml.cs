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
using His_Pos.Class;
using His_Pos.Class.Manufactory;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.AddNewOrderTypeControl
{
    /// <summary>
    /// PurchaseTypeControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseTypeControl : UserControl
    {
        public Manufactory SelectedManufactory { get; set; }

        public WareHouse SelectedWareHouse { get; set; }

        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection { get; }

        public ObservableCollection<WareHouse> WareHouseComboCollection { get; }

        public PurchaseTypeControl(ObservableCollection<Manufactory> manufactoryAutoCompleteCollection, ObservableCollection<WareHouse> wareHouseComboCollection)
        {
            InitializeComponent();
            DataContext = this;

            ManufactoryAutoCompleteCollection = manufactoryAutoCompleteCollection;
            WareHouseComboCollection = wareHouseComboCollection;
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

            SelectedManufactory = autoCompleteBox.SelectedItem as Manufactory;
        }

        internal AddOrderType GetOrderType()
        {
            List<RadioButton> radioButtons = TargetGrid.Children.OfType<RadioButton>().ToList();
            int taget = Int16.Parse(radioButtons.Single(r => r.GroupName == "target" && r.IsChecked == true).Tag.ToString());

            if (taget == 5 && SelectedManufactory is null)
            {
                MessageWindow messageWindow = new MessageWindow("請輸入廠商名稱", MessageType.ERROR);
                messageWindow.ShowDialog();
                return AddOrderType.ERROR;
            }

            if (WareHouseCombo.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請輸入庫存名稱", MessageType.ERROR);
                messageWindow.ShowDialog();
                return AddOrderType.ERROR;
            }

            SelectedWareHouse = (WareHouse)WareHouseCombo.SelectedItem;

            radioButtons = ConditionGrid.Children.OfType<RadioButton>().ToList();
            int condition = Int16.Parse(radioButtons.Single(r => r.GroupName == "condition" && r.IsChecked == true).Tag.ToString());

            return (AddOrderType)(taget * condition);
        }

        private void ManufactoryAuto_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TargetManufactory.IsChecked = true;
        }
    }
}
