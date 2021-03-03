using His_Pos.NewClass.Cooperative.CooperativeInstitution;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Medicine.MedBag;
using System.Data;

namespace His_Pos.NewClass.Medicine.Base
{
    public class MedicineOTC : Medicine
    {
        public MedicineOTC() : base()
        {
        }

        public MedicineOTC(DataRow r) : base(r)
        {
            IsBuckle = true;
            CanEdit = true;
        }

        public MedicineOTC(CooperativePrescription.Item m) : base(m)
        {
        }

        public MedicineOTC(Item m) : base(m)
        {
        }

        public override MedBagMedicine CreateMedBagMedicine(bool isSingle)
        {
            return new MedBagMedicine(this, isSingle);
        }
    }
}