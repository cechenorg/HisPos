using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.PresControl
{
    public class PrescriptionViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand ExportCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private string medicineID;
        private string wareHouseID;
        private ProductRegisterPrescriptions productRegisterPrescriptionCollection;

        public ProductRegisterPrescriptions ProductRegisterPrescriptionCollection
        {
            get { return productRegisterPrescriptionCollection; }
            set { Set(() => ProductRegisterPrescriptionCollection, ref productRegisterPrescriptionCollection, value); }
        }

        #endregion ----- Define Variables -----

        public void ReloadData(string medID, string wareID)
        {
            medicineID = medID;
            wareHouseID = wareID;

            ProductRegisterPrescriptionCollection = ProductRegisterPrescriptions.GetRegisterPrescriptionsByID(medicineID, wareHouseID);
            ExportCommand = new RelayCommand(ExportAction);
        }

        #region ----- Define Actions -----

        private void ExportAction()
        {
            ExportPresHistoryWindow exportPresHistoryWindow = new ExportPresHistoryWindow(medicineID, int.Parse(wareHouseID));
            exportPresHistoryWindow.ShowDialog();
        }

        #endregion ----- Define Actions -----
    }
}