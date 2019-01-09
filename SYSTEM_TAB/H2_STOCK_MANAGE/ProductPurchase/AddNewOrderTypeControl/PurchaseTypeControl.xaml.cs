using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase.AddNewOrderTypeControl
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

            WareHouseCombo.SelectedIndex = 0;
        }

        private void RadioButton_TargetOnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton is null || Con1 is null) return;

            switch (radioButton.Tag)
            {
                case "2":
                    Con1.IsEnabled = false;

                    Con2.IsChecked = true;
                    break;
                case "5":
                    Con1.IsEnabled = true;
                    
                    Con1.IsChecked = true;
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
                MessageWindow.ShowMessage("請輸入廠商名稱", MessageType.ERROR, true);
                
                return AddOrderType.ERROR;
            }

            if (WareHouseCombo.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請輸入庫存名稱", MessageType.ERROR, true);
                
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

        public AutoCompleteFilterPredicate<object> ManFilter
        {
            get
            {
                return (searchText, obj) =>
                    !((obj as Manufactory)?.Id is null) && (((Manufactory)obj).Id.ToLower().Contains(searchText.ToLower())
                                                                || ((Manufactory)obj).Name.ToLower().Contains(searchText.ToLower()));
            }
        }
    }
}
