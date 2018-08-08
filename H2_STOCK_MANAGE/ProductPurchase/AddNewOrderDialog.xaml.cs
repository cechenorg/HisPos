using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.H2_STOCK_MANAGE.ProductPurchase.AddNewOrderTypeControl;
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
using System.Windows.Shapes;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// AddNewOrderDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewOrderDialog : Window, INotifyPropertyChanged
    {
        private PurchaseTypeControl purchaseTypeControl;
        private ReturnTypeControl returnTypeControl;

        private UserControl currentControl;

        public UserControl CurrentControl
        {
            get
            {
                return currentControl;
            }
            set
            {
                currentControl = value;
                NotifyPropertyChanged("CurrentControl");
            }
        }

        public AddOrderType AddOrderType;
        public Manufactory Manufactory;

        public bool ConfirmButtonClicked = false;
        public AddNewOrderDialog( ObservableCollection<Manufactory> manufactoryAutoCompleteCollection)
        {
            InitializeComponent();
            DataContext = this;

            purchaseTypeControl = new PurchaseTypeControl(manufactoryAutoCompleteCollection);
            returnTypeControl = new ReturnTypeControl(manufactoryAutoCompleteCollection);

            CurrentControl = purchaseTypeControl;
        }
        private void ConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            //List<RadioButton> radioButtons = TargetGrid.Children.OfType<RadioButton>().ToList();
            //int taget = Int16.Parse(radioButtons.Single(r => r.GroupName == "target" && r.IsChecked == true).Tag.ToString());

            //if (taget == 5 && Manufactory is null)
            //{
            //    MessageWindow messageWindow = new MessageWindow("請輸入廠商名稱", MessageType.ERROR);
            //    messageWindow.ShowDialog();
            //    return;
            //}

            //radioButtons = ConditionGrid.Children.OfType<RadioButton>().ToList();
            //int condition = Int16.Parse(radioButtons.Single(r => r.GroupName == "condition" && r.IsChecked == true).Tag.ToString());

            //AddOrderType = (AddOrderType) (taget * condition);

            ConfirmButtonClicked = true;
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void NewOrderType_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            switch(radioButton.Tag.ToString())
            {
                case "P":
                    CurrentControl = purchaseTypeControl;
                    break;
                case "R":
                    CurrentControl = returnTypeControl;
                    break;
            }
        }
    }
}
