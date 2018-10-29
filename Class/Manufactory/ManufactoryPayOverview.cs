﻿using System.Data;

namespace His_Pos.Class.Manufactory
{
    public class ManufactoryPayOverview
    {
        public ManufactoryPayOverview(DataRow row)
        {
            Money = row["MAN_MONEY"].ToString();
            Type = row["MAN_TYPE"].ToString();
            Date = row["MAN_DATE"].ToString();
            ForeignId = row["MAN_FOREIGN"].ToString();
        }

        public string Money { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string ForeignId { get; set; }
    }
}
