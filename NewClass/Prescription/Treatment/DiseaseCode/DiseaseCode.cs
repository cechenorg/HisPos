using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using ZeroFormatter;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.Prescription.Treatment.DiseaseCode
{
    [ZeroFormattable]
    public class DiseaseCode : ObservableObject
    {
        public DiseaseCode()
        {
            ID = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }
        public DiseaseCode(DataRow r) {
            ID = r.Field<string>("DisCode_ID");
            Name = r.Field<string>("DisCode_ChiName");
            FullName = ID + " " + Name;
        }
        [Index(0)]
        public virtual string ID { get; set; }
        [Index(1)]
        public virtual string Name { get; set; }
        private string fullName;
        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => FullName, ref fullName, value);
                if (string.IsNullOrEmpty(fullName))
                {
                    ID = string.Empty;
                    Name = string.Empty;
                }
            }
        }
        [Index(3)]
        public virtual string ICD9_ID { get; set; }

        public void GetData() {
            if (string.IsNullOrEmpty(ID)) return;
            var table = DiseaseCodeDb.GetDataByCodeId(ID);
            if (table.Rows.Count <= 0) return;
            var diseaseCode = new DiseaseCode(table.Rows[0]);
            ID = diseaseCode.ID;
            Name = diseaseCode.Name;
            FullName = ID + " " + Name;
        }
        public static DiseaseCode GetDiseaseCodeByID(string id)
        {
            var d = new DiseaseCode {ID = id};
            MainWindow.ServerConnection.OpenConnection();
            d.GetData();
            if (!d.CheckDiseaseValid())
            {
                MessageWindow.ShowMessage("疾病代碼不完整",MessageType.WARNING);
                return null;
            }
            MainWindow.ServerConnection.CloseConnection();
            if (!string.IsNullOrEmpty(d.ID)) return d;
            MessageWindow.ShowMessage(StringRes.DiseaseCodeNotFound, MessageType.WARNING);
            return null;
        }

        private bool CheckDiseaseValid()
        {
            var table = DiseaseCodeDb.CheckDiseaseValid(ID);
            if (table.Rows.Count > 0)
            {
                var count = table.Rows[0].Field<int>("DiseaseCount");
                return count == 1;
            }
            return true;
        }
    }
}
