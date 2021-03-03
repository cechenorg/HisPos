using His_Pos.Interface;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;

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
            ValueDiff = 0;
            TakingResult = "";
            EmpId = "";
            IsChecked = false;
            isEqual = true;
            BatchNumbers batchNumber = new BatchNumbers(dataRow);
            BatchNumbersCollection.Add(batchNumber);
        }

        public string Category { get; set; }
        public double Inventory { get; set; }
        public string EmpId { get; set; }
        public double SafeAmount { get; set; }
        public string ValidDate { get; set; }
        public string LastCheckDate { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        private string takingReason;

        public string TakingReason
        {
            get
            {
                return takingReason;
            }
            set
            {
                takingReason = value;
                NotifyPropertyChanged("TakingReason");
            }
        }

        private ObservableCollection<BatchNumbers> batchNumbersCollection = new ObservableCollection<BatchNumbers>();

        public ObservableCollection<BatchNumbers> BatchNumbersCollection
        {
            get
            {
                return batchNumbersCollection;
            }
            set
            {
                batchNumbersCollection = value;
                NotifyPropertyChanged("BatchNumbersCollection");
            }
        }

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
                CountValueDiff();
                NotifyPropertyChanged("TakingResult");
            }
        }

        private void CountValueDiff()
        {
            if (takingResult.Equals(String.Empty)) return;

            double diff = Double.Parse(takingResult) - Inventory;

            if (diff > 0)
                ValueDiff = 0;
            else
            {
                double valueDiff = 0;

                for (int x = batchNumbersCollection.Count - 1; x >= 0; x--)
                {
                    if (diff + batchNumbersCollection[x].Amount >= 0)
                    {
                        valueDiff += (-diff) * batchNumbersCollection[x].Price;
                        break;
                    }
                    else
                    {
                        valueDiff += batchNumbersCollection[x].Amount * batchNumbersCollection[x].Price;
                    }

                    diff += batchNumbersCollection[x].Amount;
                }

                ValueDiff = valueDiff;
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

        private double valueDiff;

        public double ValueDiff
        {
            get { return valueDiff; }
            set
            {
                valueDiff = value;
                NotifyPropertyChanged("ValueDiff");
            }
        }
    }
}