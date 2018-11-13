using System;
using System.Data;

namespace His_Pos.Class
{
    public class Usage : Selection ,ICloneable
    {
        public Usage()
        {
            Id = "";
            Name = "";
            QuickName = "";
            PrintName = "";
            Reg = "";
            Days = 0;
            Times = 0;
            PreDefault = true;
        }
        private Usage(Usage usage)
        {
            Id = usage.Id;
            Name = usage.Name;
            QuickName = usage.QuickName;
            PrintName = usage.PrintName;
            Reg = usage.Reg;
            Days = usage.Days;
            Times = usage.Times;
            PreDefault = usage.PreDefault;
            PrintIcons = (bool[])usage.PrintIcons.Clone();
        }
        public Usage(DataRow dataRow)
        {
            Id = dataRow["USAGE_ID"].ToString();
            Name = dataRow["USAGE_NAME"].ToString();
            QuickName = dataRow["USAGE_QNAME"].ToString();
            PrintName = dataRow["USAGE_PRINTNAME"].ToString();
            Reg = dataRow["USAGE_REG"].ToString();
            Days = short.Parse(dataRow["USAGE_DAY"].ToString());
            Times = short.Parse(dataRow["USAGE_TIMES"].ToString());
            PreDefault = bool.Parse(dataRow["USAGE_DEFAULT"].ToString());
            
            PrintIcons[0] = bool.Parse( dataRow["HISFREPRI_MORNING"].ToString());
            PrintIcons[1] = bool.Parse( dataRow["HISFREPRI_NOON"].ToString());
            PrintIcons[2] = bool.Parse( dataRow["HISFREPRI_NIGHT"].ToString());
            PrintIcons[3] = bool.Parse( dataRow["HISFREPRI_SLEEP"].ToString());
            PrintIcons[4] = bool.Parse( dataRow["HISFREPRI_BEFOREMEAL"].ToString());
            PrintIcons[5] = bool.Parse( dataRow["HISFREPRI_AFTERMEAL"].ToString());
        }

        private string _printName;
        public string PrintName
        {
            get => _printName;
            set
            {
                _printName = value;
                NotifyPropertyChanged(nameof(PrintName));
            }
        }

        private string _quickName;
        public string QuickName
        {
            get => _quickName;
            set
            {
                _quickName = value;
                NotifyPropertyChanged(nameof(QuickName));
            }
        } //快速輸入用代號
        
        public string Reg { get; }//Regular Expression規則

        private int _days;
        public int Days
        {
            get => _days;
            set
            {
                _days = value;
                NotifyPropertyChanged(nameof(Days));
            }
        }

        private int _times;

        public int Times
        {
            get => _times;
            set
            {
                _times = value;
                NotifyPropertyChanged(nameof(Times));
            }
        }

        public bool PreDefault { get; }
        
        public bool[] PrintIcons { get; set; } = new bool[6];

        public object Clone()
        {
            return new Usage(this);
        }
    }
}