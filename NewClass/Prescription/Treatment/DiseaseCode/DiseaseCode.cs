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
        public DiseaseCode() {}
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
            protected set
            {
                Set(() => FullName, ref fullName, value);
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
            MainWindow.ServerConnection.CloseConnection();
            if (!string.IsNullOrEmpty(d.ID)) return d;
            MessageWindow.ShowMessage(StringRes.DiseaseCodeNotFound, MessageType.WARNING);
            return null;
        }
    }
}
