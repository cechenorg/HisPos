using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment.DiseaseCode
{
    public class DiseaseCode : ObservableObject
    {
        public DiseaseCode() { }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
