using GalaSoft.MvvmLight;
using System.Data;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot
{
    public class PrescriptionDetailMedicineRepot : ObservableObject
    {
     
        public string Id { get; set; }
        public string Name { get; set; }

        public double PaySelfValue { get; set; }
        public double MedicinePrice { get; set; }

        public double Price
        {
            get => IsPaySelf ? PaySelfValue : MedicinePrice;
        }
        public double MedUse { get; set; }

        public double Profit
        {
            get => IsPaySelf ? PaySelfValue + MedUse : MedicinePrice + MedUse;
        }

        public bool IsPaySelf { get; set; }
    }
}