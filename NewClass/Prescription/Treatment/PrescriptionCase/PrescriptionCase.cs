using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    public class PrescriptionCase : ObservableObject
    {
        public PrescriptionCase() { }
        public PrescriptionCase(DataRow r)
        {
            Id = r.Field<string>("PreCase_ID");
            Name = r.Field<string>("PreCase_Name");
            FullName = r.Field<string>("PreCase_FullName");
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
