using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrderDetail
    {
        #region ----- Define Variables -----
        public OrderTypeEnum Type { get; set; }
        public string ID { get; set; }
        public DateTime DoneTime { get; set; }
        private double InitialPrice { get; set; }
        public double UnTaxPrice { get; set; }
        public double Tax { get; set; }
        public double TaxPrice { get; set; }
        #endregion

        public ManufactoryOrderDetail(DataRow dataRow)
        {
            Type = dataRow.Field<string>("").Equals("P")? OrderTypeEnum.PURCHASE : OrderTypeEnum.RETURN;
            ID = dataRow.Field<string>("");
            DoneTime = dataRow.Field<DateTime>("");
            InitialPrice = (double)dataRow.Field<decimal>("");

            CalculateTax(dataRow.Field<bool>(""));
        }

        #region ----- Define Functions -----
        internal void CalculateTax(bool includeTax)
        {
            if (includeTax)
            {
                TaxPrice = InitialPrice;

                UnTaxPrice = TaxPrice * 100 / 105;

                Tax = TaxPrice * 5 / 100;
            }
            else
            {
                UnTaxPrice = InitialPrice;

                Tax = InitialPrice * 5 / 100;

                TaxPrice = UnTaxPrice + Tax;
            }
        }
        #endregion
    }
}
