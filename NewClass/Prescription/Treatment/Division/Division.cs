using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public class Division : ObservableObject
    {
        public Division() {
            Id = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }

        public Division(DataRow r)
        {
            Id = r.Field<string>("Div_ID");
            Name = r.Field<string>("Div_Name");
            FullName = r.Field<string>("Div_FullName");
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
    }
}
