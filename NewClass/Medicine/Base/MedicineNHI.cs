using His_Pos.NewClass.Medicine.MedBag;
using His_Pos.Service;
using System.Data;

namespace His_Pos.NewClass.Medicine.Base
{
    public class MedicineNHI : Medicine
    {
        public MedicineNHI(DataRow r) : base(r)
        {
            Ingredient = r.Field<string>("Med_Ingredient");
            SideEffect = r.Field<string>("Med_SideEffect");
            Indication = r.Field<string>("Med_Indication");
            if (NewFunction.CheckDataRowContainsColumn(r, "Med_ATC"))
                ATCCode = r.Field<string>("Med_ATC");
            if (NewFunction.CheckDataRowContainsColumn(r, "Med_SingleCompound"))
                SingleCompound = r.Field<string>("Med_SingleCompound");
            Form = r.Field<string>("Med_Form");
            ControlLevel = r.Field<byte?>("Med_Control");
            Note = r.Field<string>("Med_NhiNote");
            Warning = r.Field<string>("Med_Warning");
            IsBuckle = true;
            CanEdit = true;
            MostPricedID = r.Field<string>("MostPricedID");
        }

        public MedicineNHI()
        {
        }

        private int? controlLevel;

        public int? ControlLevel
        {
            get => controlLevel;
            set
            {
                Set(() => ControlLevel, ref controlLevel, value);
            }
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

        public bool MostPriced => !string.IsNullOrEmpty(MostPricedID);
        private string mostPricedID;

        public string MostPricedID
        {
            get => mostPricedID;
            set
            {
                Set(() => MostPricedID, ref mostPricedID, value);
            }
        }

        public override MedBagMedicine CreateMedBagMedicine(bool isSingle)
        {
            return new MedBagMedicine(this, isSingle);
        }
    }
}