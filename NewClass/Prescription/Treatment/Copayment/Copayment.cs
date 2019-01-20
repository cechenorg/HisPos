using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    public class Copayment : ObservableObject
    {
        public Copayment() { }
        public Copayment(DataRow r)
        {
            Id = r.Field<string>("Cop_ID");
            Name = r.Field<string>("Cop_Name");
            FullName = r.Field<string>("Cop_FullName");
        }
        public string Id { get; }
        public string Name { get; }
        private string fullName;
        public string FullName
        {
            get => fullName;
            set
            {
                Set(() => Id, ref fullName, value);
            }
        }
    }
}
