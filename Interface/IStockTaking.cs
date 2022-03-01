using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.Interface
{
    internal interface IStockTaking
    {
        string Category { get; set; }
        double Inventory { get; set; }
        string EmpId { get; set; }
        double SafeAmount { get; set; }
        string ValidDate { get; set; }
        string LastCheckDate { get; set; }
        string Location { get; set; }
        bool Status { get; set; }
        string TakingResult { get; set; }
        bool IsChecked { get; set; }
        bool IsEqual { get; set; }
        string TakingReason { get; set; }
        double ValueDiff { get; set; }
        ObservableCollection<BatchNumbers> BatchNumbersCollection { get; set; }
    }

    public struct BatchNumbers
    {
        private BatchNumbers(string batchnumber, double amount, double price)
        {
            BatchNumber = batchnumber;
            Amount = amount;
            Price = price;
        }

        public BatchNumbers(DataRow dataRow)
        {
            BatchNumber = dataRow["STOORDDET_BATCHNUMBER"].ToString().Equals("") ? "X" : dataRow["STOORDDET_BATCHNUMBER"].ToString();
            Amount = Convert.ToDouble(dataRow["STOCK"].ToString());
            Price = Convert.ToDouble(dataRow["STOORDDET_PRICE"].ToString());
        }

        public string BatchNumber { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}