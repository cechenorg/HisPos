using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public class Division : ObservableObject
    {
        public Division() {}

        public Division(DataRow r)
        {
            Id = r.Field<string>("Div_ID");
            Name = r.Field<string>("Div_Name");
            FullName = r.Field<string>("Div_FullName");
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
