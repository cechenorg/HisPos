using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    public class SpecialTreat : ObservableObject
    {
        public SpecialTreat() { }
        public SpecialTreat(DataRow r)
        {
            Id = r["SpeTre_ID"].ToString();
            Name = r["SpeTre_Name"].ToString();
            FullName = r["SpeTre_FullName"].ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
