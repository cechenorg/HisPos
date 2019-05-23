﻿using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    [ZeroFormattable]
    public class Institution : ObservableObject
    {
        public Institution(){
            
        }
         
        public Institution(DataRow r)
        {
            ID = r.Field<string>("Ins_ID");
            Name = r.Field<string>("Ins_Name");
            FullName = r.Field<string>("Ins_FullName");
        }
        [Index(0)]
        public virtual string ID { get; set; } = string.Empty;
        [Index(1)]
        public virtual string Name { get; set; } = string.Empty;
        private string fullName = string.Empty;
        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => FullName, ref fullName, value);
            }
        }
        [IgnoreFormat]
        public bool Common { get; set; }
        [IgnoreFormat]
        public bool IsCooperative { get; set; }
        #region Function
        public void UpdateUsedTime() {
            InstitutionDb.UpdateUsedTime(ID);
        }
        #endregion
    }
}
