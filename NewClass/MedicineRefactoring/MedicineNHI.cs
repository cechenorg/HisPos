using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.Medicine.MedBag;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class MedicineNHI : Medicine
    {
        public MedicineNHI(DataRow r) : base(r)
        {
            
        }

        private string atcCode;
        public string ATCCode
        {
            get => atcCode;
            set
            {
                Set(() => ATCCode, ref atcCode, value);
            }
        }

        private string singleCompound;//單複方
        public string SingleCompound
        {
            get => singleCompound;
            set
            {
                Set(() => SingleCompound, ref singleCompound, value);
            }
        }

        private string form;//劑型
        public string Form
        {
            get => form;
            set
            {
                Set(() => Form, ref form, value);
            }
        }

        private string note;//健保注意事項
        public string Note
        {
            get => note;
            set
            {
                Set(() => Note, ref note, value);
            }
        }
        private string warning;//用藥注意事項
        public string Warning
        {
            get => warning;
            set
            {
                Set(() => Warning, ref warning, value);
            }
        }

        public override MedBagMedicine CreateMedBagMedicine(bool isSingle)
        {
            return new MedBagMedicine(this, isSingle);
        }
    }
}
