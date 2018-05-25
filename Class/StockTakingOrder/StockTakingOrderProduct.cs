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
            EmpId = dataRow["EMP_ID"].ToString();
            OldValue = dataRow["PROCHE_OLDVAL"].ToString();
            NewValue = dataRow["PROCHE_NEWVAL"].ToString();
           
        }
        public string empId;
        public string oldValue;
        public string newValue;
        public string ValueDiff => (Convert.ToInt32(oldValue) - Convert.ToInt32(newValue)).ToString();

        public string EmpId
        {
            get
            {
                return empId;
            }
            set
            {
                empId = value;
                NotifyPropertyChanged("EmpId");
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
