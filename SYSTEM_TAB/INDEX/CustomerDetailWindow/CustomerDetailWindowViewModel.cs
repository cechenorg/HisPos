using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.CustomerDetailPrescription;
using His_Pos.NewClass.Prescription.CustomerDetailPrescription.CustomerDetailPrescriptionMedicine;
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
        private CustomerDetailPrescription customerDetailPrescriptionSelectedItem = new CustomerDetailPrescription();
        public CustomerDetailPrescription CustomerDetailPrescriptionSelectedItem
        {
            get => customerDetailPrescriptionSelectedItem;
            set
            {
                Set(() => CustomerDetailPrescriptionSelectedItem, ref customerDetailPrescriptionSelectedItem, value);
            }
        }
        private CustomerDetailPrescriptionMedicines customerDetailPrescriptionMedicines = new CustomerDetailPrescriptionMedicines();
        public CustomerDetailPrescriptionMedicines CustomerDetailPrescriptionMedicines
        {
            get => customerDetailPrescriptionMedicines;
            set
            {
                Set(() => CustomerDetailPrescriptionMedicines, ref customerDetailPrescriptionMedicines, value);
            }
        }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ShowMedicinesDetailCommand { get; set; }
        #endregion
        public CustomerDetailWindowViewModel(int cusID) { 
            CustomerData = Customer.GetCustomerByCusId(cusID);
            CustomerDetailPrescriptionCollection.GetDataByID(cusID);
            if (CustomerDetailPrescriptionCollection.Count > 0)
            {
                CustomerDetailPrescriptionSelectedItem = CustomerDetailPrescriptionCollection[0];
                CustomerDetailPrescriptionMedicines.GetDataByID(CustomerDetailPrescriptionSelectedItem.ID);
            }

            SaveCommand = new RelayCommand(SaveAction);
            ShowMedicinesDetailCommand = new RelayCommand(ShowMedicinesDetailAction);
        }
        #region Action
        private void ShowMedicinesDetailAction() {
            CustomerDetailPrescriptionMedicines.GetDataByID(CustomerDetailPrescriptionSelectedItem.ID);
        }
        private void SaveAction() {
            CustomerData.Save();
            MessageWindow.ShowMessage("存檔成功",Class.MessageType.SUCCESS);
        }
        #endregion
    }
}
