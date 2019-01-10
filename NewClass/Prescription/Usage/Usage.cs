using System;
using System.Data;
using System.Text.RegularExpressions;
using His_Pos.Class;
using His_Pos.Service;

namespace His_Pos.NewClass.Usage
{
    public class Usage : Selection ,ICloneable
    {
        public Usage(){}

        public Usage(DataRow dataRow)
        {
            Id = dataRow[""].ToString();
            Name = dataRow[""].ToString();
            QuickName = dataRow[""].ToString();
            PrintName = dataRow[""].ToString();
            Reg = new Regex(dataRow[""].ToString());
            Days = short.Parse(dataRow[""].ToString());
            Times = short.Parse(dataRow[""].ToString());
            PreDefault = bool.Parse(dataRow[""].ToString());
            
            PrintIcons[0] = bool.Parse( dataRow[""].ToString());
            PrintIcons[1] = bool.Parse( dataRow[""].ToString());
            PrintIcons[2] = bool.Parse( dataRow[""].ToString());
            PrintIcons[3] = bool.Parse( dataRow[""].ToString());
            PrintIcons[4] = bool.Parse( dataRow[""].ToString());
            PrintIcons[5] = bool.Parse( dataRow[""].ToString());
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
        
        public Regex Reg { get; }//Regular Expression規則

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
            return this.DeepCloneViaJson();
        }
    }
}