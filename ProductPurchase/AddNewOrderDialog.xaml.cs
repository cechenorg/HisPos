﻿using His_Pos.Class;
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
        public AddOrderType AddOrderType;
        public Manufactory Manufactory;
        ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection;
        public bool ConfirmButtonClicked = false;
        public AddNewOrderDialog( ObservableCollection<Manufactory> manufactoryAutoCompleteCollection)
        {
            InitializeComponent();

            ManufactoryAutoCompleteCollection = manufactoryAutoCompleteCollection;
            ManufactoryAuto.ItemsSource = ManufactoryAutoCompleteCollection;
            ManufactoryAuto.ItemFilter = ManufactoryFilter;
        }
        private void ConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            List<RadioButton> radioButtons = TargetGrid.Children.OfType<RadioButton>().ToList();
            int taget = Int16.Parse(radioButtons.Single(r => r.GroupName == "target" && r.IsChecked == true).Tag.ToString());

            if (taget == 5 && Manufactory is null)
            {
                MessageWindow messageWindow = new MessageWindow("請輸入廠商名稱", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }

            radioButtons = ConditionGrid.Children.OfType<RadioButton>().ToList();
            int condition = Int16.Parse(radioButtons.Single(r => r.GroupName == "condition" && r.IsChecked == true).Tag.ToString());

            AddOrderType = (AddOrderType) (taget * condition);

            ConfirmButtonClicked = true;
            this.Close();
        }

        public AutoCompleteFilterPredicate<object> ManufactoryFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as Manufactory).Id is null) ? true : (obj as Manufactory).Id.Contains(searchText)
                                                               || (obj as Manufactory).Name.Contains(searchText);
            }
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
