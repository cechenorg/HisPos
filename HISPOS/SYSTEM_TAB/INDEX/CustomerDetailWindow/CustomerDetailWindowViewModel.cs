using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.CustomerDetailPrescription;
using His_Pos.NewClass.Prescription.CustomerDetailPrescription.CustomerDetailPrescriptionMedicine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using His_Pos.InfraStructure;

namespace His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow
{
    public class CustomerDetailWindowViewModel : ViewModelBase
    {
        #region Var

        private List<string> prescriptionCaseString;

        public List<string> PrescriptionCaseString
        {
            get => prescriptionCaseString;
            set
            {
                Set(() => PrescriptionCaseString, ref prescriptionCaseString, value);
            }
        }

        private string prescriptionCaseSelectItem;

        public string PrescriptionCaseSelectItem
        {
            get => prescriptionCaseSelectItem;
            set
            {
                Set(() => PrescriptionCaseSelectItem, ref prescriptionCaseSelectItem, value);
                PrescriptionDetailViewSource.Filter += AdjustTypeFilter;
            }
        }

        private CollectionViewSource prescriptionDetailViewSource;

        private CollectionViewSource PrescriptionDetailViewSource
        {
            get => prescriptionDetailViewSource;
            set
            {
                Set(() => PrescriptionDetailViewSource, ref prescriptionDetailViewSource, value);
            }
        }

        private ICollectionView prescriptionDetailView;

        public ICollectionView PrescriptionDetailView
        {
            get => prescriptionDetailView;
            private set
            {
                Set(() => PrescriptionDetailView, ref prescriptionDetailView, value);
            }
        }

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

        #endregion Var

        public CustomerDetailWindowViewModel(int cusID)
        {
            CustomerData = CustomerService.GetCustomerByCusId(cusID);
            CustomerDetailPrescriptionCollection.GetDataByID(cusID);
            if (CustomerDetailPrescriptionCollection.Count > 0)
            {
                CustomerDetailPrescriptionSelectedItem = CustomerDetailPrescriptionCollection[0];
            }
            PrescriptionDetailViewSource = new CollectionViewSource { Source = CustomerDetailPrescriptionCollection };
            PrescriptionDetailView = PrescriptionDetailViewSource.View;
            PrescriptionDetailViewSource.Filter += AdjustTypeFilter;
            PrescriptionCaseString = new List<string>() { "全部", "調劑", "登錄", "預約" };
            PrescriptionCaseSelectItem = PrescriptionCaseString[0];
            SaveCommand = new RelayCommand(SaveAction);
            ShowMedicinesDetailCommand = new RelayCommand(ShowMedicinesDetailAction);
        }

        #region Action

        private void ShowMedicinesDetailAction()
        {
            if (CustomerDetailPrescriptionSelectedItem is null) return;
            CustomerDetailPrescriptionMedicines.GetDataByID(CustomerDetailPrescriptionSelectedItem.ID, CustomerDetailPrescriptionSelectedItem.TypeName);
        }

        private void SaveAction()
        {
            CustomerData.Save();
            MessageWindow.ShowMessage("存檔成功", Class.MessageType.SUCCESS);
        }

        private void AdjustTypeFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is CustomerDetailPrescription src))
                e.Accepted = false;

            e.Accepted = false;
            CustomerDetailPrescription customerDetailPrescription = (CustomerDetailPrescription)e.Item;
            if (customerDetailPrescription.TypeName == PrescriptionCaseSelectItem)
                e.Accepted = true;
            else if (PrescriptionCaseSelectItem == "全部")
                e.Accepted = true;
        }

        #endregion Action
    }
}