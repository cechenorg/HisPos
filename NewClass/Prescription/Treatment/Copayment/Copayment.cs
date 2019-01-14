using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    public class Copayment : ObservableObject
    {
        public Copayment() { }
        public Copayment(DataRow r)
        {
            Id = r["Cop_ID"].ToString();
            Name = r["Cop_Name"].ToString(); 
            FullName = r["Cop_FullName"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
