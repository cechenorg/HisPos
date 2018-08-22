﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

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
            Days = "";
            Times = "";
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
            Days = (dataRow["USAGE_DAY"].ToString());
            Times = (dataRow["USAGE_TIMES"].ToString());
            PreDefault = bool.Parse(dataRow["USAGE_DEFAULT"].ToString());
            
            PrintIcons[0] = Boolean.Parse( dataRow["HISFREPRI_MORNING"].ToString());
            PrintIcons[1] = Boolean.Parse( dataRow["HISFREPRI_NOON"].ToString());
            PrintIcons[2] = Boolean.Parse( dataRow["HISFREPRI_NIGHT"].ToString());
            PrintIcons[3] = Boolean.Parse( dataRow["HISFREPRI_SLEEP"].ToString());
            PrintIcons[4] = Boolean.Parse( dataRow["HISFREPRI_BEFOREMEAL"].ToString());
            PrintIcons[5] = Boolean.Parse( dataRow["HISFREPRI_AFTERMEAL"].ToString());
        }
        public string PrintName { get; set; }
        public string QuickName { get; set; }//快速輸入用代號

        public string Reg { get; set; }//Regular Expression規則

        //ex : x Days y Times = > x天吃y次
        public string Days { get; set; }

        public string Times { get; set; }

        public bool PreDefault { get; set; }
        
        public bool[] PrintIcons { get; set; } = new bool[6];

        public object Clone()
        {
            return new Usage(this);
        }
    }
}