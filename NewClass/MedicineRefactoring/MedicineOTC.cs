using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.Medicine.MedBag;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class MedicineOTC : Medicine
    {
        public MedicineOTC() : base() { }
        public MedicineOTC(DataRow r) : base(r)
        {

        }

        public override MedBagMedicine CreateMedBagMedicine(bool isSingle)
        {
            return new MedBagMedicine(this,isSingle);
        }
    }
}
