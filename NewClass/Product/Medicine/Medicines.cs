using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines() { }

        public int CountMedicinePoint()
        {
            var medicinePoint = this.Where(m => m is MedicineNHI med && !med.PaySelf)
                .Sum(med => med.NHIPrice * med.Amount);
            return (int)Math.Round(medicinePoint, 0, MidpointRounding.AwayFromZero);
        }

        public int CountSelfPay()
        {
            var selfPay = this.Where(m => m.PaySelf).Sum(m=>m.TotalPrice);
            return (int)Math.Ceiling(selfPay);
        }
    }
}
