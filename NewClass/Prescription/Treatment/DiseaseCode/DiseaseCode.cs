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
        public string FullName { get; set; }

        public DiseaseCode GetDataByCodeId() {
            if (string.IsNullOrEmpty(Id))
                return new DiseaseCode();
            DataTable table = DiseaseCodeDb.GetDataByCodeId(Id);
            return table.Rows.Count == 0 ? null : new DiseaseCode(table.Rows[0]);
        }
    }
}
