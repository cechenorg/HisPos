using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class MedicalPersonnel:INotifyPropertyChanged
    {
        public MedicalPersonnel(){}

        public MedicalPersonnel(Employee e)
        {
            Name = e.Name;
            IcNumber = e.IcNumber;
        }
        public string Name { get; }
        public string IcNumber { get; }
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
