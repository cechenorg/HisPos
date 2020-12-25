﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.NewClass.BalanceSheet
{
    public class StrikeHistory : ObservableObject
    {
        public StrikeHistory(){}

        public StrikeHistory(DataRow r)
        {
            StrikeID = r.Field<int>("StrikeID");
            StrikeType = r.Field<string>("StrikeType");
            StrikeName = r.Field<string>("StrikeName");
            StrikeValue = r.Field<double>("StrikeValue") * -1;
            StrikeSource = r.Field<string>("StrikeSource");
            StrikeSourceID = r.Field<string>("StrikeSourceID");
            StrikeTime = r.Field<DateTime>("StrikeTime");
            StrikeNote = r.Field<string>("StrikeNote");
            CanDelete = r.Field<bool>("CAN_DELETE");
            StrikeWay = r.Field<string>("StrikeWay");
            EMP= r.Field<string>("Emp_Name");
        }

        private int strikeID;
        public int StrikeID
        {
            get => strikeID;
            set
            {
                Set(() => StrikeID, ref strikeID, value);
            }
        }

        private string strikeType;
        public string StrikeType
        {
            get => strikeType;
            set
            {
                Set(() => StrikeType, ref strikeType, value);
            }
        }
        private string emp;
        public string EMP
        {
            get => emp;
            set
            {
                Set(() => EMP, ref emp, value);
            }
        }
        private string strikeWay;
        public string StrikeWay
        {
            get => strikeWay;
            set
            {
                Set(() => StrikeWay, ref strikeWay, value);
            }
        }
        public string StrikeTypeName => StrikeType;

        private string strikeName;
        public string StrikeName
        {
            get => strikeName;
            set
            {
                Set(() => StrikeName, ref strikeName, value);
            }
        }

        private double strikeValue;
        public double StrikeValue
        {
            get => strikeValue;
            set
            {
                Set(() => StrikeValue, ref strikeValue, value);
            }
        }

        private string strikeSource;
        public string StrikeSource
        {
            get => strikeSource;
            set
            {
                Set(() => StrikeSource, ref strikeSource, value);
            }
        }

        private string strikeSourceID;
        public string StrikeSourceID
        {
            get => strikeSourceID;
            set
            {
                Set(() => StrikeSourceID, ref strikeSourceID, value);
            }
        }

        private DateTime strikeTime;
        public DateTime StrikeTime
        {
            get => strikeTime;
            set
            {
                Set(() => StrikeTime, ref strikeTime, value);
            }
        }

        private string strikeNote;
        public string StrikeNote
        {
            get => strikeNote;
            set
            {
                Set(() => StrikeNote, ref strikeNote, value);
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                Set(() => IsSelected, ref isSelected, value);
                RaisePropertyChanged(nameof(CanEdit));
            }
        }
        
        public bool CanEdit
        {
            get => (StrikeTime >= DateTime.Today || ViewModelMainWindow.CurrentUser.ID == 1) && CanDelete;
        }

        private bool CanDelete { get; set; }
    }
}
