using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Customer;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription
{
    public class Prescription : INotifyPropertyChanged
    {
        public Prescription() { }

        public Prescription(DataRow r)
        {
            Id = (int)r[""];
        }
        public int Id { get; }
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
        public IcCard Card { get; set; }
        private Treatment.Treatment treatment;//處方資料
        public Treatment.Treatment Treatment
        {
            get => treatment;
            set
            {
                treatment = value;
                OnPropertyChanged(nameof(Treatment));
            }
        }
        public PrescriptionSource Source { get; set; }
        public string SourceId { get; }//合作診所.慢箋Id
        public string OrderNumber { get; set; }//傳送藥健康單號
        public bool IsSendToSingde { get; set; }//是否傳送藥健康
        public bool IsAdjust { get; set; }//是否調劑.扣庫

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
