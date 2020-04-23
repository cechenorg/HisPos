using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.SetPrices
{
    /// <summary>
    /// SetPricesWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SetPricesWindow : Window, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        private string proId;
        private double retailPrice;
        private double memberPrice;
        private double employeePrice;
        private double specialPrice;

        public string ProID
        {
            get { return proId; }
            set
            {
                proId = value;
            }
        }
        public double RetailPrice
        {
            get { return retailPrice; }
            set
            {
                retailPrice = value;
                NotifyPropertyChanged(nameof(RetailPrice));
            }
        }
        public double MemberPrice
        {
            get { return memberPrice; }
            set
            {
                memberPrice = value;
                NotifyPropertyChanged(nameof(MemberPrice));
            }
        }
        public double EmployeePrice
        {
            get { return employeePrice; }
            set
            {
                employeePrice = value;
                NotifyPropertyChanged(nameof(EmployeePrice));
            }
        }
        public double SpecialPrice
        {
            get { return specialPrice; }
            set
            {
                specialPrice = value;
                NotifyPropertyChanged(nameof(SpecialPrice));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                //MessageWindow.ShowMessage("1111", MessageType.SUCCESS);
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public SetPricesWindow(string proid, double retailprice, double memberprice, double employeeprice, double specialprice)
        {
            InitializeComponent();
            DataContext = this;

            ProID = proid;
            RetailPrice = retailprice;
            MemberPrice = memberprice;
            EmployeePrice = employeeprice;
            SpecialPrice = specialprice;            
        }
        #region ----- Define Functions -----
        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = ProductDetailDB.SetPrices(ProID, RetailPrice, MemberPrice, EmployeePrice, SpecialPrice);

            if (dataTable?.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                DialogResult = true;
                MessageWindow.ShowMessage("更新成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("更新失敗 請稍後再試", MessageType.ERROR);
            }
            Close();
        }
        #endregion

    }
}
