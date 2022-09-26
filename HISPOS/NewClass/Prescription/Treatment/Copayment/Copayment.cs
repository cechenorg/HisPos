using GalaSoft.MvvmLight;
using System.Data;
using System.Linq;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    [ZeroFormattable]
    public class Copayment : ObservableObject
    {
        private readonly string[] freeIDList = { "006", "001", "002", "003", "004", "005", "007", "008", "009", "914", "I22" };
        private readonly string[] idList = { "003", "004", "007", "009", "I22", "001", "002", "005", "006", "008", "902", "903", "906", "907" };
        private readonly string[] administrativeAssistanceIDList = { "003", "004", "005", "006", "901", "902", "903", "904", "905", "906" };

        public Copayment()
        {
        }

        public Copayment(DataRow r)
        {
            Id = r.Field<string>("Cop_ID");
            Name = r.Field<string>("Cop_Name");
            FullName = r.Field<string>("Cop_FullName");
        }

        [Index(0)]
        public virtual string Id { get; set; } = string.Empty;

        [Index(1)]
        public virtual string Name { get; set; } = string.Empty;

        private string fullName = string.Empty;

        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => Id, ref fullName, value);
            }
        }

        public bool IsFree()
        {
            #region IDDescription

            //006.001~009(除006).801.802.901.902.903.904
            //006勞保被人因職業傷害或疾病門診者
            //001重大傷病
            //002分娩
            //003低收入戶
            //004榮民
            //005結核病患至指定之醫療院所就醫者
            //007山地離島就醫/戒菸免收
            //008經離島醫院診所轉至台灣本門及急救者
            //009其他免負擔
            //I22免收

            #endregion IDDescription

            return freeIDList.Contains(Id);
        }

        public bool IsValid(string id)
        {
            return idList.Contains(id);
        }

        public bool IsAdministrativeAssistance()
        {
            return administrativeAssistanceIDList.Contains(Id);
        }
    }
}