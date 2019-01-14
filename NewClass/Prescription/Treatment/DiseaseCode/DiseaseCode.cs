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
            Id = r["DisCode_ID"].ToString();
            Name = r["DisCode_ChiName"].ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }

        public DiseaseCode GetDataByCodeId(string code) {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = DiseaseCodeDb.GetDataByCodeId(code);
            MainWindow.ServerConnection.CloseConnection();
            return table.Rows.Count == 0 ? null : new DiseaseCode(table.Rows[0]);
        }
    }
}
