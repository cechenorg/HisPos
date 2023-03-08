using System;
using System.Data;

namespace His_Pos.NewClass.BalanceSheet
{
    public class BalanceSheetData
    {
        #region ----- Define Variables -----

        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }

        #endregion ----- Define Variables -----

        public BalanceSheetData(DataRow row)
        {
            //ID = row.Field<string>("ID");
            //Name = row.Field<string>("HEADER");
            //Type = row.Field<string>("TYPE");
            //Value = (double)row.Field<decimal>("VALUE");

            ID = row.Field<string>("acctLevel2");
            Name = row.Field<string>("acctName2");
            //Type = row.Field<string>("acctLevel1") == "1" ? "借" : "貸";
            Value = Convert.ToInt32(row["acctValue"]);//row.Field<int>("acctValue");
        }
    }
}