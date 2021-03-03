using GalaSoft.MvvmLight;
using System.Data;
using System.Linq;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    [ZeroFormattable]
    public class PrescriptionCase : ObservableObject
    {
        private readonly string[] freeCopaymentCases = { "007", "11", "12", "13", "14", "15", "16", "19", "C1" };

        public PrescriptionCase()
        {
        }

        public PrescriptionCase(DataRow r)
        {
            ID = r.Field<string>("PreCase_ID");
            Name = r.Field<string>("PreCase_Name");
            FullName = r.Field<string>("PreCase_FullName");
        }

        [Index(0)]
        public virtual string ID { get; set; } = string.Empty;

        [Index(1)]
        public virtual string Name { get; set; } = string.Empty;

        private string fullName = string.Empty;

        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => FullName, ref fullName, value);
            }
        }

        public bool FreeCopayment()
        {
            //007山地離島就醫/戒菸免收
            //11牙醫一般
            //12牙醫急診
            //13牙醫門診
            //14牙醫資源不足方案
            //15牙周統合照護
            //16牙醫特殊專案
            //19牙醫其他專案
            //C1論病計酬
            return freeCopaymentCases.Contains(ID);
        }
    }
}