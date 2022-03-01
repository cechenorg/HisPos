using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using System.Data;
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

        public DiseaseCode(DataRow r)
        {
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

        public bool GetData()
        {
            if (string.IsNullOrEmpty(ID)) return false;
            var table = DiseaseCodeDb.GetDataByCodeId(ID);
            if (table.Rows.Count <= 0)
            {
                MessageWindow.ShowMessage(StringRes.DiseaseCodeNotFound, MessageType.WARNING);
                ID = string.Empty;
                return false;
            }
            if (table.Rows.Count > 1)
            {
                MessageWindow.ShowMessage("疾病代碼" + ID + "不完整", MessageType.WARNING);
                ID = string.Empty;
                return false;
            }
            var diseaseCode = new DiseaseCode(table.Rows[0]);
            ID = diseaseCode.ID;
            Name = diseaseCode.Name;
            FullName = ID + " " + Name;
            return true;
        }

        public static DiseaseCode GetDiseaseCodeByID(string id)
        {
            var d = new DiseaseCode { ID = id };
            MainWindow.ServerConnection.OpenConnection();
            if (!d.GetData()) return null;
            MainWindow.ServerConnection.CloseConnection();
            return !string.IsNullOrEmpty(d.ID) ? d : null;
        }
    }
}