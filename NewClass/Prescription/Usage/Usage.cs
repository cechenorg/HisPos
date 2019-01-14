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

        public Usage(DataRow r)
        {
            Id = r.Field<int>("Usa_ID");
            Name = r.Field<string>("Usa_Name");
            QuickName = r.Field<string>("Usa_QuickName");
            PrintName = r.Field<string>("Usa_PrintName");
            Reg = new Regex(r.Field<string>("Usa_Regex"));
            Days = r.Field<byte?>("Usa_Days")??0;
            Times = r.Field<int?>("Usa_ID")??0;
            PreDefault = r.Field<bool>("Usa_Default");

            PrintIcons[0] = r.Field<bool>("UsaPri_Morning");
            PrintIcons[1] = r.Field<bool>("UsaPri_Noon");
            PrintIcons[2] = r.Field<bool>("UsaPri_Night");
            PrintIcons[3] = r.Field<bool>("UsaPri_Sleep");
            PrintIcons[4] = r.Field<bool>("UsaPri_BeforeMeal");
            PrintIcons[5] = r.Field<bool>("UsaPri_AfterMeal");
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