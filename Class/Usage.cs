using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class
{
    public class Usage : Selection
    {
        public Usage()
        {
        }

        public Usage(DataRow dataRow)
        {
            Id = dataRow["USAGE_ID"].ToString();
            Name = dataRow["USAGE_NAME"].ToString();
            QuickName = dataRow["USAGE_QNAME"].ToString();
            Reg = dataRow["USAGE_REG"].ToString();
            Days = int.Parse(dataRow["USAGE_DAY"].ToString());
            Times = int.Parse(dataRow["USAGE_TIMES"].ToString());
            PreDefault = bool.Parse(dataRow["USAGE_DEFAULT"].ToString());
        }

        public string QuickName { get; set; }//快速輸入用代號

        public string Reg { get; set; }//Regular Expression規則

        //ex : x Days y Times = > x天吃y次
        public int Days { get; set; }

        public int Times { get; set; }

        public bool PreDefault { get; set; }
    }
}