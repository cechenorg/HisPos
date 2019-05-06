using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.CustomerDetailPrescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow {
    public class CustomerDetailWindowViewModel : ViewModelBase {
        #region Var
        private Customer customerData = new Customer();
        public Customer CustomerData
        {
            get => customerData;
            set
            {
                Set(() => CustomerData, ref customerData, value);
            }
        }
        private CustomerDetailPrescriptions customerDetailPrescriptionCollection = new CustomerDetailPrescriptions();
        public CustomerDetailPrescriptions CustomerDetailPrescriptionCollection
        {
            get => customerDetailPrescriptionCollection;
            set
            {
                Set(() => CustomerDetailPrescriptionCollection, ref customerDetailPrescriptionCollection, value);
            }
        }
        public RelayCommand SaveCommand { get; set; }
        #endregion
        public CustomerDetailWindowViewModel(int cusID) { 
            CustomerData = CustomerData.GetCustomerByCusId(cusID);
            CustomerDetailPrescriptionCollection.GetDataByID(cusID);
            SaveCommand = new RelayCommand(SaveAction);

        }
        #region Action
        private void SaveAction() {
            CustomerData.Save();
            MessageWindow.ShowMessage("存檔成功",Class.MessageType.SUCCESS);
        }
        #endregion
    }
}
