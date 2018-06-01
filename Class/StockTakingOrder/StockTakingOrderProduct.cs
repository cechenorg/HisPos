using His_Pos.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.StockTakingOrder
{
   public class StockTakingOrderProduct :AbstractClass.Product,IStockTakingRecord, INotifyPropertyChanged
    {
        public StockTakingOrderProduct(DataRow dataRow) : base(dataRow)
        {
            EmpName = dataRow["EMP_NAME"].ToString();
            OldValue = dataRow["PROCHE_OLDVAL"].ToString();
            NewValue = dataRow["PROCHE_NEWVAL"].ToString();
            PriceDiff = Convert.ToInt32(ValueDiff) > 0 ? "+" + Math.Round(Convert.ToDouble(dataRow["PROCHE_VALUEDIFF"].ToString()), 2): "-" + Math.Round(Convert.ToDouble(dataRow["PROCHE_VALUEDIFF"].ToString()), 2);
        }
        public string empName;
        public string oldValue;
        public string newValue;
        public string priceDiff;
        public string ValueDiff => (Convert.ToInt32(newValue) - Convert.ToInt32(oldValue)) > 0 ? "+" + (Math.Round(Convert.ToDouble(newValue) - Convert.ToDouble(oldValue), 2)).ToString():Math.Round(Convert.ToDouble(newValue) - Convert.ToDouble(oldValue), 2).ToString();
        public string PriceDiff
        {
            get
            {
                return priceDiff;
            }
            set
            {
                priceDiff = value;
                NotifyPropertyChanged("PriceDiff");
            }
        }
        public string EmpName
        {
            get
            {
                return empName;
            }
            set
            {
                empName = value;
                NotifyPropertyChanged("EmpName");
            }
        }
       public string OldValue
        {
            get
            {
                return oldValue;
            }
            set
            {
                oldValue = value;
                NotifyPropertyChanged("OldValue");
            }
        }
       public string NewValue
        {
            get
            {
                return newValue;
            }
            set
            {
                newValue = value;
                NotifyPropertyChanged("NewValue");
            }
        }
       
    }
}
