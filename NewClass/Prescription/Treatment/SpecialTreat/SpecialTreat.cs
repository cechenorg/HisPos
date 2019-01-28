using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    public class SpecialTreat : ObservableObject
    {
        public SpecialTreat() {
            Id = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }
        public SpecialTreat(DataRow r)
        {
            Id = r.Field<string>("SpeTre_ID");
            Name = r.Field<string>("SpeTre_Name");
            FullName = r.Field<string>("SpeTre_FullName");
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
