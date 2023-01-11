using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using System;
using System.Data;

namespace His_Pos.NewClass.BalanceSheet
{
    public class ClosingHistory : ObservableObject
    {
        public ClosingHistory()
        {
        }

        public ClosingHistory(DataRow r)
        {
            StrikeTime = r.Field<DateTime>("ClosedDate");
            StrikeDateTime = r.Field<DateTime>("ClosedTime");
            EmpName = r.Field<string>("EmpName");
            OGValue = r.Field<int>("OGValue");
            KeyInValue = r.Field<int>("KeyInValue");
            Value = r.Field<int>("Value");
            DateTime insertDate = new DateTime(StrikeDateTime.Year, StrikeDateTime.Month, StrikeDateTime.Day);
            DateTime date = new DateTime(StrikeTime.Year, StrikeTime.Month, StrikeTime.Day);
            IsMakeUp = insertDate.CompareTo(date) != 0 ? true : false;
        }
        private bool isMakeUp;

        public bool IsMakeUp
        {
            get => isMakeUp;
            set
            {
                Set(() => IsMakeUp, ref isMakeUp, value);
            }
        }

        private int valuee;

        public int Value
        {
            get => valuee;
            set
            {
                Set(() => Value, ref valuee, value);
            }
        }

        private int keyInValue;

        public int KeyInValue
        {
            get => keyInValue;
            set
            {
                Set(() => KeyInValue, ref keyInValue, value);
            }
        }

        private int oGValue;

        public int OGValue
        {
            get => oGValue;
            set
            {
                Set(() => OGValue, ref oGValue, value);
            }
        }

        private string empName;

        public string EmpName
        {
            get => empName;
            set
            {
                Set(() => EmpName, ref empName, value);
            }
        }

        private DateTime strikeDateTime;

        public DateTime StrikeDateTime
        {
            get => strikeDateTime;
            set
            {
                Set(() => StrikeDateTime, ref strikeDateTime, value);
            }
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