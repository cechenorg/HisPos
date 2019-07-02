using His_Pos.NewClass.Person.Employee;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockTaking.StockTakingProduct
{
   public class StockTakingProduct : Product.Product
    {
        #region ----- Define Variables -----
        private double inventory;
        public double Inventory {
            get { return inventory; }
            set { Set(() => Inventory, ref inventory, value);
                ValueDiff = NewInventory - Inventory;
            }
        }
        private double newInventory;
        public double NewInventory
        {
            get { return newInventory; }
            set {
                Set(() => NewInventory, ref newInventory, value);
                ValueDiff = NewInventory - Inventory; 
            }
        }
        private double valueDiff;
        public double ValueDiff
        {
            get { return valueDiff; }
            set { Set(() => ValueDiff, ref valueDiff, value); }
        }
        private string note;
        public string Note
        {
            get { return note; }
            set { Set(() => Note, ref note, value); }
        }
        private bool isUpdate;
        public bool IsUpdate
        {
            get { return isUpdate; }
            set { Set(() => IsUpdate, ref isUpdate, value); }
        }
        public Employee Employee { get; set; } 
        #endregion

        public StockTakingProduct(DataRow row) : base(row)
        {
            Inventory = row.Field<double>("StoTakDet_OldValue");
            NewInventory = row.Field<double>("StoTakDet_NewValue");
            Note = row.Field<string>("StoTakDet_Note");
            IsUpdate = false;
        }
        
        #region ----- Define Functions -----
        #endregion
    }
}
