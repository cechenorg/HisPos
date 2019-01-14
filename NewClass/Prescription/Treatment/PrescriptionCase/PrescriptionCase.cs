using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    public class PrescriptionCase : ObservableObject
    {
        public PrescriptionCase() { }
        public PrescriptionCase(DataRow r)
        {
            Id = r["PreCase_ID"].ToString();
            Name = r["PreCase_Name"].ToString();
            FullName = r["PreCase_FullName"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
