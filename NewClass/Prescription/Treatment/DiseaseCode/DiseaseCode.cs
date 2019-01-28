using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.Prescription.Treatment.DiseaseCode
{
    public class DiseaseCode : ObservableObject
    {
        public DiseaseCode() {
            ID = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }
        public DiseaseCode(DataRow r) {
            ID = r.Field<string>("DisCode_ID");
            Name = r.Field<string>("DisCode_ChiName");
            FullName = r.Field<string>("DisCode_FullName");
        }
        public string ID { get; set; }
        public string Name { get; set; }
        private string fullName;
        public string FullName
        {
            get => fullName;
            set
            {
                Set(() => FullName, ref fullName, value);
            }
        }

        public void GetDataByCodeId(string id) {
            if (!string.IsNullOrEmpty(id))
            {
                DataTable table = DiseaseCodeDb.GetDataByCodeId(id);
                if (table.Rows.Count > 0)
                {
                    var diseaseCode = new DiseaseCode(table.Rows[0]);
                    ID = diseaseCode.ID;
                    Name = diseaseCode.Name;
                    FullName = ID + " " + Name;
                }
            }
        }
        public static DiseaseCode GetDiseaseCodeByID(string id)
        {
            var d = new DiseaseCode();
            MainWindow.ServerConnection.OpenConnection();
            d.GetDataByCodeId(id);
            MainWindow.ServerConnection.CloseConnection();
            if (!string.IsNullOrEmpty(d.ID)) return d;
            MessageWindow.ShowMessage(StringRes.DiseaseCodeNotFound, MessageType.WARNING);
            return null;
        }
    }
}
