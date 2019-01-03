using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace His_Pos.NewClass
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
