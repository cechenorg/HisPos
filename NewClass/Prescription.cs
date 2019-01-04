using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace His_Pos.NewClass
{
    public class Prescription:INotifyPropertyChanged
    {
        public Prescription() { }

        public Prescription(DataRow r)
        {
            Id = (int)r[""];
        }
        public int Id { get; }
        public string SourceId { get;}//合作診所.慢箋Id
        private Customer patient;//病患
        public Customer Patient
        {
            get => patient;
            set
            {
                patient = value;
                OnPropertyChanged(nameof(Patient));
            }
        }
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
