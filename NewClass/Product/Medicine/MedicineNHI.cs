using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.NewClass.Product.Medicine
{
    public class MedicineNHI:Medicine,IDeletable
    {
        public MedicineNHI() : base(){}
        public MedicineNHI(DataRow r) : base(r) {
            Ingredient = r.Field<string>("Med_Ingredient");
            SideEffect = r.Field<string>("Med_SideEffect");
            Indication = r.Field<string>("Med_Indication");
            ATCCode = r.Field<string>("Med_ATC");
            SingleCompound = r.Field<string>("Med_SingleCompound");
            Form = r.Field<string>("Med_Form");
            ControlLevel = r.Field<byte?>("Med_Control");
            Note = r.Field<string>("Med_NhiNote");
            Warning = r.Field<string>("Med_Warning"); 
			 
        }
        public MedicineNHI(ProductStruct p) : base(p) { }
        private string ingredient;//成分
        public string Ingredient
        {
            get => ingredient;
            set
            {
                if (ingredient != value)
                {
                    Set(() => Ingredient, ref ingredient, value);
                }
            }
        }
        private string sideEffect;//副作用
        public string SideEffect
        {
            get => sideEffect;
            set
            {
                if (sideEffect != value)
                {
                    Set(() => SideEffect, ref sideEffect, value);
                }
            }
        }
        private string indication;//適應症
        public string Indication
        {
            get => indication;
            set
            {
                if (indication != value)
                {
                    Set(() => Indication, ref indication, value);
                }
            }
        }
        private string atcCode;
        public string ATCCode
        {
            get => atcCode;
            set
            {
                if (atcCode != value)
                {
                    Set(() => ATCCode, ref atcCode, value);
                }
            }
        }
        private string singleCompound;//單複方
        public string SingleCompound
        {
            get => singleCompound;
            set
            {
                if (singleCompound != value)
                {
                    Set(() => SingleCompound, ref singleCompound, value);
                }
            }
        }
        private string form;//劑型
        public string Form
        {
            get => form;
            set
            {
                if (form != value)
                {
                    Set(() => Form, ref form, value);
                }
            }
        }
        private string note;//健保注意事項
        public string Note
        {
            get => note;
            set
            {
                if (note != value)
                {
                    Set(() => Note, ref note, value);
                }
            }
        }
        private string warning;//用藥注意事項
        public string Warning
        {
            get => warning;
            set
            {
                if (warning != value)
                {
                    Set(() => Warning, ref warning, value);
                }
            }
        }
        private string source;
        public string Source
        {
            get => source;
            set
            {
                Set(() => Source, ref source, value);
            }
        }
        
    }
}
