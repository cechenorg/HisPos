using System;
using System.Data;

namespace His_Pos.Class.Product
{
    public class ProductUnit
    {
        public ProductUnit()
        {
        }

        public ProductUnit(DataRow row)
        {
            Id = row["PROUNI_ID"].ToString();
            Unit = row["PROUNI_TYPE"].ToString();
            Amount = row["PROUNI_QTY"].ToString();
            Price = Double.Parse(row["PRO_SELL_PRICE"].ToString());
            VIPPrice = Double.Parse(row["PRO_VIP_PRICE"].ToString());
            EmpPrice = Double.Parse(row["PRO_EMP_PRICE"].ToString());
            BaseType = Boolean.Parse(row["PRO_BASETYPE_STATUS"].ToString());
        }

        public ProductUnit(string id, string unit, string amount, double price, double vIPPrice, double empPrice)
        {
            Id = id;
            Unit = unit;
            Amount = amount;
            Price = price;
            VIPPrice = vIPPrice;
            EmpPrice = empPrice;
            BaseType = false;
        }

        public string Id { get; set; }
        public string Unit { get; set; }
        public string Amount { get; set; }
        public double Price { get; set; }
        public double VIPPrice { get; set; }
        public double EmpPrice { get; set; }
        public bool BaseType { get; set; }
    }
}
