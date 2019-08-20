using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    public class PrescriptionViewModel : ViewModelBase
    {
        #region ----- Define Variables -----
        private ProductRegisterPrescriptions productRegisterPrescriptionCollection;
        
        public ProductRegisterPrescriptions ProductRegisterPrescriptionCollection
        {
            get { return productRegisterPrescriptionCollection; }
            set { Set(() => ProductRegisterPrescriptionCollection, ref productRegisterPrescriptionCollection, value); }
        }
        #endregion

        private void ReloadProductPrescription()
        {
            ProductRegisterPrescriptionCollection = ProductRegisterPrescriptions.GetRegisterPrescriptionsByID(Medicine.ID, SelectedWareHouse.ID);
        }
    }
}
