using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage {
    public class CustomerManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        #region -----Define Command-----  
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        #endregion
        #region ----- Define Variables -----
        public bool btnCancelEnable;
        public bool BtnCancelEnable
        {
            get { return btnCancelEnable; }
            set { Set(() => BtnCancelEnable, ref btnCancelEnable, value); }
        }
        public bool btnSubmitEnable;
        public bool BtnSubmitEnable
        {
            get { return btnSubmitEnable; }
            set { Set(() => BtnSubmitEnable, ref btnSubmitEnable, value); }
        }
        public string changeText;
        public string ChangeText
        {
            get { return changeText; }
            set { Set(() => ChangeText, ref changeText, value); }
        }
        public Brush changeForeground;
        public Brush ChangeForeground
        {
            get { return changeForeground; }
            set { Set(() => ChangeForeground, ref changeForeground, value); }
        }
        
        public Customers customerCollection;
        public Customers CustomerCollection
        {
            get { return customerCollection; }
            set { Set(() => CustomerCollection, ref customerCollection, value); }
        }
        public Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { Set(() => Customer, ref customer, value); }
        }
        #endregion
        public CustomerManageViewModel() {
            InitData(); 
        }
        #region Action

        #endregion
        
        #region Function
        public void InitData()
        {
            MainWindow.ServerConnection.OpenConnection();
            CustomerCollection = new Customers();
            CustomerCollection.Init();
            MainWindow.ServerConnection.CloseConnection();
            if (CustomerCollection.Count > 0)
                Customer = CustomerCollection[0];
        }
        private void DataChanged()
        {
              
            ChangeText = "已修改";
            ChangeForeground = Brushes.Red;

            BtnCancelEnable = true;
            BtnSubmitEnable = true;
        }

        private void InitDataChanged()
        {
            ChangeText = "未修改";
            ChangeForeground = Brushes.Black; 
            BtnSubmitEnable = false;
            BtnSubmitEnable = false;
        }
        #endregion
    }
}
