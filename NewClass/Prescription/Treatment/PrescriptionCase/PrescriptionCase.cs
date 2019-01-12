using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    public class PrescriptionCase : ObservableObject
    {
        public PrescriptionCase() { }
        public PrescriptionCase(DataRow r)
        {
            Id = r[""].ToString();
            Name = r[""].ToString();
            FullName = r[""].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
