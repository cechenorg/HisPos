using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public class Division : ObservableObject
    {
        public Division() {}

        public Division(DataRow r)
        {
            Id = r[""].ToString();
            Name = r[""].ToString();
            FullName = r[""].ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
