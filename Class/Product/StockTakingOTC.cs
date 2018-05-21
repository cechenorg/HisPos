using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class StockTakingOTC : AbstractClass.Product, IStockTaking, IDeletable, INotifyPropertyChanged
    {
        public StockTakingOTC(DataRow dataRow) : base(dataRow)
        {
            Location = dataRow["PRO_LOCATION"].ToString();
            Category = dataRow["PROTYP_CHINAME"].ToString();
            LastCheckDate = dataRow["PROCHE_DATE"].ToString();
            Inventory = Double.Parse(dataRow["PRO_INVENTORY"].ToString());
            SafeAmount = Double.Parse(dataRow["PRO_SAFEQTY"].ToString());
            ValidDate = dataRow["STOORDDET_VALIDDATE"].ToString();
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
            TakingResult = "";
            IsChecked = false;
            isEqual = true;
        }
     
        public string Category { get; set; }
        public double Inventory { get; set; }
        public double SafeAmount { get; set; }
        public string ValidDate { get; set; }
        public string LastCheckDate { get; set; }
        public string Location { get; set; }
        public ObservableCollection<BatchNumbers> BatchNumbersCollection { get; set; }
        public bool Status { get; set; }
        private string takingResult;

        public string TakingResult
        {
            get
            {
                return takingResult;
            }
            set
            {
                takingResult = value;
                UserFilledResult();
                CheckIsEqual();
                NotifyPropertyChanged("TakingResult");
            }
        }
        private void CheckIsEqual()
        {
            IsEqual = (TakingResult == Inventory.ToString()) || TakingResult.Equals(String.Empty);
        }
        private void UserFilledResult()
        {
            if (takingResult.Equals(String.Empty))
                IsChecked = false;
            else
            {
                IsChecked = true;
            }
        }
        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }
        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        private bool isEqual;
        public bool IsEqual
        {
            get
            {
                return isEqual;
            }
            set
            {
                isEqual = value;
                NotifyPropertyChanged("IsEqual");
            }
        }
    }
}
