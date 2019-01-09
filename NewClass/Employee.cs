﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace His_Pos.NewClass
{
    public class Employee:INotifyPropertyChanged
    {
        public Employee(){}
        public Employee(DataRow r)
        {
            Id = (int) r["EMP_ID"];
            Name = r["EMP_NAME"].ToString();
            IcNumber = r["EMP_ICNUMBER"]?.ToString();
            Birthday = (DateTime?)r["EMP_BIRTHDAY"];
        }
        public int Id { get; }
        private string name;//姓名
        public string Name
        {
            get =>name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string icNumber;//姓名
        public string IcNumber
        {
            get => icNumber;
            set
            {
                icNumber = value;
                OnPropertyChanged(nameof(IcNumber));
            }
        }
        private DateTime? birthday;//姓名
        public DateTime? Birthday
        {
            get => birthday;
            set
            {
                birthday = value;
                OnPropertyChanged(nameof(Birthday));
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