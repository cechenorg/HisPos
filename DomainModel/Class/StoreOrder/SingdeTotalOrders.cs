using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Enum;

namespace DomainModel.Class.StoreOrder
{
    public class dSingdeTotalOrder
    { 
        public string Date { get; set; }
        public int PurchaseCount { get; set; }
        public int ReturnCount { get; set; }
        public double PurchasePrice { get; set; }
        public double ReturnPrice { get; set; }
         
        public dSingdeTotalOrder() // for Dapper
        {

        }

        public dSingdeTotalOrder(DataRow dataRow)
        {
            Date = dataRow.Field<string>("DATE");
            PurchaseCount = dataRow.Field<int>("P_COUNT");
            ReturnCount = dataRow.Field<int>("R_COUNT");
            PurchasePrice = (double)dataRow.Field<decimal>("P_TOTAL");
            ReturnPrice = (double)dataRow.Field<decimal>("R_TOTAL");
        }
          
    }
}
