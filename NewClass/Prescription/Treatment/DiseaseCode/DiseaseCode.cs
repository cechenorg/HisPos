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
            Id = r.Field<string>("DisCode_ID");
            Name = r.Field<string>("DisCode_ChiName");
            FullName = r.Field<string>("DisCode_FullName");
        }
        public string Id { get; set; }
        public string Name { get; set; }
        private string fullName;
        public string FullName
        {
            get => fullName;
            set
            {
                Set(() => Id, ref fullName, value);
            }
        }

        public DiseaseCode GetDataByCodeId(string id) {
            if (string.IsNullOrEmpty(id))
                return new DiseaseCode();
            DataTable table = DiseaseCodeDb.GetDataByCodeId(id);
            return table.Rows.Count == 0 ? null : new DiseaseCode(table.Rows[0]);
        }
    }
}
