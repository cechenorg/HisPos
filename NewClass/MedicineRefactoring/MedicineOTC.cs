using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Product.Medicine.MedBag;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class MedicineOTC : Medicine
    {
        public MedicineOTC() : base() { }
        public MedicineOTC(DataRow r) : base(r)
        {
            CanEdit = true;
            IsBuckle = true;
        }

        public MedicineOTC(CooperativePrescription.Item m) : base(m)
        {

        }

        public MedicineOTC(Item m) : base(m)
        {
        }

        public override MedBagMedicine CreateMedBagMedicine(bool isSingle)
        {
            return new MedBagMedicine(this,isSingle);
        }
    }
}
