using GalaSoft.MvvmLight;
using His_Pos.Service;
using System;
using System.Data;
using System.Text.RegularExpressions;
using ZeroFormatter;

namespace His_Pos.NewClass.Medicine.Usage
{
    [ZeroFormattable]
    public class Usage : ObservableObject, ICloneable
    {
        public Usage()
        {
        }

        public Usage(DataRow r)
        {
            ID = r.Field<int>("Usa_ID");
            Name = r.Field<string>("Usa_Name");
            QuickName = r.Field<string>("Usa_QuickName");
            PrintName = r.Field<string>("Usa_PrintName");
            if (!string.IsNullOrEmpty(r.Field<string>("Usa_Regex")))
            {
                RegStr = r.Field<string>("Usa_Regex");
                Reg = new Regex(RegStr);
            }
            Days = r.Field<byte?>("Usa_Days") ?? 0;
            Times = r.Field<byte?>("Usa_Times") ?? 0;
            PreDefault = r.Field<bool>("Usa_Default");
            FreqRemark = r.Field<string>("FreqRemark");

        }

        [Index(0)]
        public virtual int ID { get; set; }

        [Index(1)]
        public virtual string Name { get; set; }

        [Index(2)]
        public virtual string PrintName { get; set; }//列印名稱

        [Index(3)]
        public virtual string QuickName { get; set; } //快速輸入用代號

        [Index(4)]
        public virtual string RegStr { get; set; } //快速輸入用代號

        [IgnoreFormat]
        public virtual Regex Reg { get; set; }//Regular Expression規則

        [Index(5)]
        public virtual int Days { get; set; }

        [Index(6)]
        public virtual int Times { get; set; }

        [Index(7)]
        public virtual bool PreDefault { get; set; }

        [Index(8)]
        public virtual string FreqRemark { get; set; } //藥品頻率

        public object Clone()
        {
            return this.DeepCloneViaJson();
        }
    }
}