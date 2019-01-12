using System;
using System.Data;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.Service;

namespace His_Pos.NewClass.Usage
{
    public class Usage:ObservableObject,ICloneable
    {
        public Usage(){}

        public Usage(DataRow dataRow)
        {
            Id = (int)dataRow[""];
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrintName { get; set; }//列印名稱
        public string QuickName { get; set; } //快速輸入用代號
        public Regex Reg { get; }//Regular Expression規則
        public int Days { get; set; }
        public int Times { get; set; }
        public bool PreDefault { get; }
        
        public bool[] PrintIcons { get; set; } = new bool[6];

        public object Clone()
        {
            return this.DeepCloneViaJson();
        }
    }
}