using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.PresControl
{
    public class PrescriptionViewModel : ViewModelBase
    {
        #region ----- Define Variables -----
        private string medicineID;
        private string wareHouseID;
        private ProductRegisterPrescriptions productRegisterPrescriptionCollection;
        
        public ProductRegisterPrescriptions ProductRegisterPrescriptionCollection
        {
            get { return productRegisterPrescriptionCollection; }
            set { Set(() => ProductRegisterPrescriptionCollection, ref productRegisterPrescriptionCollection, value); }
        }
        #endregion

        public void ReloadData(string medID, string wareID)
        {
            medicineID = medID;
            wareHouseID = wareID;

            ProductRegisterPrescriptionCollection = ProductRegisterPrescriptions.GetRegisterPrescriptionsByID(medicineID, wareHouseID);
        }
    }
}
