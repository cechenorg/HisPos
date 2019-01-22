using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment.DiseaseCode
{
    public class DiseaseCode : ObservableObject
    {
        public DiseaseCode() { }
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
    }
}
