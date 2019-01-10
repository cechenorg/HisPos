using System.ComponentModel;
using System.Runtime.CompilerServices;
using His_Pos.NewClass.Person;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class MedicalPersonnel:INotifyPropertyChanged
    {
        public MedicalPersonnel(){}

        public MedicalPersonnel(Employee e)
        {
            Name = e.Name;
            IdNumber = e.IdNumber;
        }
        public string Name { get; }
        public string IdNumber { get; }
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
