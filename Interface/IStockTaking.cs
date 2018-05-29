using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.Interface
{
    interface IStockTaking
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
        ObservableCollection<BatchNumbers> BatchNumbersCollection { get; set; }
    }
    public struct BatchNumbers {

        BatchNumbers(string batchnumber,double amount) {
            BatchNumber = batchnumber;
            Amount = amount;
        }
        public BatchNumbers(DataRow dataRow) {
            BatchNumber = dataRow["STOORDDET_BATCHNUMBER"].ToString();
            Amount = Convert.ToDouble(dataRow["STOCK"].ToString());
        }
        public string BatchNumber { get; set; }
        public double Amount { get; set; }
    } 
}
