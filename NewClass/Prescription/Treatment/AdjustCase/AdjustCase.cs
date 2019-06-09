﻿using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    [ZeroFormattable]
    public class AdjustCase:ObservableObject
    {
        public AdjustCase()
        {

        }

        public AdjustCase(DataRow r)
        {
            ID = r.Field<string>("Adj_ID");
            Name = r.Field<string>("Adj_Name");
            FullName = r.Field<string>("Adj_FullName");
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

        public bool CheckIsPrescribe()
        {
            return ID.Equals("0");
        }

        public bool CheckIsSimpleForm()
        {
            return ID.Equals("3");
        }
    }
}
