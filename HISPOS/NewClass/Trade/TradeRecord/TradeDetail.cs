using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product.ProductManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Trade.TradeRecord
{
    public class TradeDetail : ObservableObject
    {
        public string TraDet_DetailID
        {
            get { return traDet_DetailID; }
            set { Set(() => TraDet_DetailID, ref traDet_DetailID, value); }
        }
        private string traDet_DetailID;
        public string TraDet_ProductID
        {
            get { return traDet_ProductID; }
            set { Set(() => TraDet_ProductID, ref traDet_ProductID, value); }
        }
        private string traDet_ProductID;
        public string TraDet_ProductName
        {
            get { return traDet_ProductName; }
            set { Set(() => TraDet_ProductName, ref traDet_ProductName, value); }
        }
        private string traDet_ProductName;
        public int TraDet_Amount
        {
            get { return traDet_Amount; }
            set { Set(() => TraDet_Amount, ref traDet_Amount, value); }
        }
        private int traDet_Amount;
        public string TraDet_PriceType
        {
            get { return traDet_PriceType; }
            set { Set(() => TraDet_PriceType, ref traDet_PriceType, value); }
        }
        private string traDet_PriceType;
        public int TraDet_Price
        {
            get { return traDet_Price; }
            set { Set(() => TraDet_Price, ref traDet_Price, value); }
        }
        private int traDet_Price;
        public int TraDet_PriceSum
        {
            get { return traDet_PriceSum; }
            set { Set(() => TraDet_PriceSum, ref traDet_PriceSum, value); }
        }
        private int traDet_PriceSum;
        public bool TraDet_IsGift
        {
            get { return traDet_IsGift; }
            set { Set(() => TraDet_IsGift, ref traDet_IsGift, value); }
        }
        private bool traDet_IsGift;
        public int TraDet_DepositAmount
        {
            get { return traDet_DepositAmount; }
            set { Set(() => TraDet_DepositAmount, ref traDet_DepositAmount, value); }
        }
        private int traDet_DepositAmount;
        public string TraDet_RewardPersonnel
        {
            get { return traDet_RewardPersonnel; }
            set { Set(() => TraDet_RewardPersonnel, ref traDet_RewardPersonnel, value); }
        }
        private string traDet_RewardPersonnel;
        public string TraDet_RewardPersonnelName
        {
            get { return traDet_RewardPersonnelName; }
            set { Set(() => TraDet_RewardPersonnelName, ref traDet_RewardPersonnelName, value); }
        }
        private string traDet_RewardPersonnelName;
        public int TraDet_RewardPercent
        {
            get { return traDet_RewardPercent; }
            set { Set(() => TraDet_RewardPercent, ref traDet_RewardPercent, value); }
        }
        private int traDet_RewardPercent;
        public bool IsReward_Format
        {
            get { return isReward_Format; }
            set { Set(() => IsReward_Format, ref isReward_Format, value); }
        }
        private bool isReward_Format;
        public string Irr
        {
            get { return irr; }
            set { Set(() => Irr, ref irr, value); }
        }
        private string irr;
        public bool Pro_IsReward
        {
            get { return pro_IsReward; }
            set { Set(() => Pro_IsReward, ref pro_IsReward, value); }
        }
        private bool pro_IsReward;
        public EmpList Emp
        {
            get { return emp; }
            set
            {
                Set(() => Emp, ref emp, value);
                ChangePercent();
            }
        }
        private EmpList emp;
        public List<EmpList> Emps
        {
            get { return emps; }
            set { Set(() => Emps, ref emps, value); }
        }
        private List<EmpList> emps;
        public class EmpList : ObservableObject
        {
            public int Emp_ID 
            {
                get { return emp_ID; }
                set { Set(() => Emp_ID, ref emp_ID, value); }
            }
            private int emp_ID;
            public string Emp_Account
            {
                get { return emp_Account; }
                set { Set(() => Emp_Account, ref emp_Account, value); }
            }
            private string emp_Account;
            public string Emp_Name 
            {
                get { return emp_Name; }
                set { Set(() => Emp_Name, ref emp_Name, value); }
            }
            private string emp_Name;
            public string Emp_CashierID 
            {
                get { return emp_CashierID; }
                set { Set(() => Emp_CashierID, ref emp_CashierID, value); }
            }
            private string emp_CashierID;
        }
        public void ChangePercent()
        {
            if (Emp != null)
            {
                DataTable table = ProductDetailDB.GetProductManageMedicineDataByID(TraDet_ProductID);
                if (table != null && table.Rows.Count > 0)
                {
                    int percent = Convert.ToInt32(table.Rows[0]["Pro_RewardPercent"]);
                    if (string.IsNullOrEmpty(Emp.Emp_Name) || percent == 0)
                    {
                        TraDet_RewardPercent = 0;
                    }
                    else
                    {
                        int qty = Convert.ToInt32(TraDet_Amount);
                        TraDet_RewardPercent = qty * percent;
                    }
                }
            }
        }
    }
}
