using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Trade.TradeRecord
{
    public class TradeRecordDetail : ObservableObject
    {
        #region ----- Define Variables -----
        public int RowNo
        {
            get { return rowNo; }
            set { Set(() => RowNo, ref rowNo, value); }
        }
        private int rowNo;
        public int TraMas_ID
        {
            get { return traMas_ID; }
            set { Set(() => TraMas_ID, ref traMas_ID, value); }
        }
        private int traMas_ID;
        public int TraDet_Price
        {
            get { return traDet_Price; }
            set { Set(() => TraDet_Price, ref traDet_Price, value); }
        }
        private int traDet_Price;
        public int TraMas_RealTotal
        {
            get { return traMas_RealTotal; }
            set { Set(() => TraMas_RealTotal, ref traMas_RealTotal, value); }
        }
        private int traMas_RealTotal;
        public int TraMas_CustomerID
        {
            get { return traMas_CustomerID; }
            set { Set(() => TraMas_CustomerID, ref traMas_CustomerID, value); }
        }
        private int traMas_CustomerID;
        public string Cus_Name
        {
            get { return cus_Name; }
            set { Set(() => Cus_Name, ref cus_Name, value); }
        }
        private string cus_Name;
        public string Cus_Phone
        {
            get { return cus_Phone; }
            set { Set(() => Cus_Phone, ref cus_Phone, value); }
        }
        private string cus_Phone;
        public DateTime TraMas_ChkoutTime
        {
            get { return traMas_ChkoutTime; }
            set { Set(() => TraMas_ChkoutTime, ref traMas_ChkoutTime, value); }
        }
        private DateTime traMas_ChkoutTime;
        public string TraMas_PayMethod
        {
            get { return traMas_PayMethod; }
            set { Set(() => TraMas_PayMethod, ref traMas_PayMethod, value); }
        }
        private string traMas_PayMethod;
        public int TraMas_PreTotal
        {
            get { return traMas_PreTotal; }
            set { Set(() => TraMas_PreTotal, ref traMas_PreTotal, value); }
        }
        private int traMas_PreTotal;
        public int TraMas_DiscountAmt
        {
            get { return traMas_DiscountAmt; }
            set { Set(() => TraMas_DiscountAmt, ref traMas_DiscountAmt, value); }
        }
        private int traMas_DiscountAmt;
        public string TraMas_CardNumber
        {
            get { return traMas_CardNumber; }
            set { Set(() => TraMas_CardNumber, ref traMas_CardNumber, value); }
        }
        private string traMas_CardNumber;
        public string TraMas_InvoiceNumber
        {
            get { return traMas_InvoiceNumber; }
            set { Set(() => TraMas_InvoiceNumber, ref traMas_InvoiceNumber, value); }
        }
        private string traMas_InvoiceNumber;
        public string TraMas_TaxNumber
        {
            get { return traMas_TaxNumber; }
            set { Set(() => TraMas_TaxNumber, ref traMas_TaxNumber, value); }
        }
        private string traMas_TaxNumber;
        public string TraMas_Cashier
        {
            get { return traMas_Cashier; }
            set { Set(() => TraMas_Cashier, ref traMas_Cashier, value); }
        }
        private string traMas_Cashier;
        public string Emp_Name
        {
            get { return emp_Name; }
            set { Set(() => Emp_Name, ref emp_Name, value); }
        }
        private string emp_Name;
        public string TraMas_Note
        {
            get { return traMas_Note; }
            set { Set(() => TraMas_Note, ref traMas_Note, value); }
        }
        private string traMas_Note;
        public bool TraMas_IsEnable
        {
            get { return traMas_IsEnable; }
            set { Set(() => TraMas_IsEnable, ref traMas_IsEnable, value); }
        }
        private bool traMas_IsEnable;
        public DateTime? TraMas_UpdateTime
        {
            get { return traMas_UpdateTime; }
            set { Set(() => TraMas_UpdateTime, ref traMas_UpdateTime, value); }
        }
        private DateTime? traMas_UpdateTime;
        public int TraMas_CashAmount
        {
            get { return traMas_CashAmount; }
            set { Set(() => TraMas_CashAmount, ref traMas_CashAmount, value); }
        }
        private int traMas_CashAmount;
        public int TraMas_CardAmount
        {
            get { return traMas_CardAmount; }
            set { Set(() => TraMas_CardAmount, ref traMas_CardAmount, value); }
        }
        private int traMas_CardAmount;
        public int TraMas_VoucherAmount
        {
            get { return traMas_VoucherAmount; }
            set { Set(() => TraMas_VoucherAmount, ref traMas_VoucherAmount, value); }
        }
        private int traMas_VoucherAmount;
        public int TraMas_CashCoupon
        {
            get { return traMas_CashCoupon; }
            set { Set(() => TraMas_CashCoupon, ref traMas_CashCoupon, value); }
        }
        private int traMas_CashCoupon;
        public int TraMas_Prepay
        {
            get { return traMas_Prepay; }
            set { Set(() => TraMas_Prepay, ref traMas_Prepay, value); }
        }
        private int traMas_Prepay;
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
        public bool Pro_IsReward
        {
            get { return pro_IsReward; }
            set { Set(() => Pro_IsReward, ref pro_IsReward, value); }
        }
        private bool pro_IsReward;
        public string Irr
        {
            get { return irr; }
            set { Set(() => Irr, ref irr, value); }
        }
        private string irr;
        public int TraDet_RewardPercent
        {
            get { return traDet_RewardPercent; }
            set { Set(() => TraDet_RewardPercent, ref traDet_RewardPercent, value); }
        }
        private int traDet_RewardPercent;
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

        /// <summary>
        /// 客戶銷費筆數
        /// </summary>
        public int TraCount
        {
            get { return traCount; }
            set { Set(() => TraCount, ref traCount, value); }
        }
        private int traCount;
        /// <summary>
        /// 客戶彙總毛利
        /// </summary>
        public int Profit
        {
            get { return profit; }
            set { Set(() => Profit, ref profit, value); }
        }
        private int profit;
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal ProfitPercent
        {
            get { return profitPercent; }
            set { Set(() => ProfitPercent, ref profitPercent, value); }
        }
        private decimal profitPercent;
        /// <summary>
        /// 總成本
        /// </summary>
        public int TotalCost
        {
            get { return totalCost; }
            set { Set(() => TotalCost, ref totalCost, value); }
        }
        private int totalCost;

        public TradeRecordDetail(DataRow row)
        {
            TraMas_ID = row.Table.Columns.Contains("TraMas_ID") && row["TraMas_ID"] != DBNull.Value ? Convert.ToInt32(row["TraMas_ID"]) : 0;
            TraMas_RealTotal = row.Table.Columns.Contains("TraMas_RealTotal") && row["TraMas_RealTotal"] != DBNull.Value ? Convert.ToInt32(row["TraMas_RealTotal"]) : 0;
            TraMas_CustomerID = row.Table.Columns.Contains("TraMas_CustomerID") && row["TraMas_CustomerID"] != DBNull.Value ? Convert.ToInt32(row["TraMas_CustomerID"]) : 0;
            Cus_Name = row.Table.Columns.Contains("Cus_Name") && row["Cus_Name"] != DBNull.Value ? Convert.ToString(row["Cus_Name"]) : string.Empty;
            Cus_Phone = row.Table.Columns.Contains("Cus_Phone") && row["Cus_Phone"] != DBNull.Value ? Convert.ToString(row["Cus_Phone"]) : string.Empty;
            TraMas_ChkoutTime = row.Table.Columns.Contains("TraMas_ChkoutTime") && row["TraMas_ChkoutTime"] != DBNull.Value ? Convert.ToDateTime(row["TraMas_ChkoutTime"]) : new DateTime();
            TraMas_PayMethod = row.Table.Columns.Contains("TraMas_PayMethod") && row["TraMas_PayMethod"] != DBNull.Value ? Convert.ToString(row["TraMas_PayMethod"]) : string.Empty;
            TraMas_PreTotal = row.Table.Columns.Contains("TraMas_PreTotal") && row["TraMas_PreTotal"] != DBNull.Value ? Convert.ToInt32(row["TraMas_PreTotal"]) : 0;
            TraMas_DiscountAmt = row.Table.Columns.Contains("TraMas_DiscountAmt") && row["TraMas_DiscountAmt"] != DBNull.Value ? Convert.ToInt32(row["TraMas_DiscountAmt"]) : 0;
            TraMas_CardNumber = row.Table.Columns.Contains("TraMas_CardNumber") && row["TraMas_CardNumber"] != DBNull.Value ? Convert.ToString(row["TraMas_CardNumber"]) : string.Empty;
            TraMas_InvoiceNumber = row.Table.Columns.Contains("TraMas_InvoiceNumber") && row["TraMas_InvoiceNumber"] != DBNull.Value ? Convert.ToString(row["TraMas_InvoiceNumber"]) : string.Empty;
            TraMas_TaxNumber = row.Table.Columns.Contains("TraMas_TaxNumber") && row["TraMas_TaxNumber"] != DBNull.Value ? Convert.ToString(row["TraMas_TaxNumber"]) : string.Empty;
            TraMas_Cashier = row.Table.Columns.Contains("TraMas_Cashier") && row["TraMas_Cashier"] != DBNull.Value ? Convert.ToString(row["TraMas_Cashier"]) : string.Empty;
            Emp_Name = row.Table.Columns.Contains("Emp_Name") && row["Emp_Name"] != DBNull.Value ? Convert.ToString(row["Emp_Name"]) : string.Empty;
            TraMas_Note = row.Table.Columns.Contains("TraMas_Note") && row["TraMas_Note"] != DBNull.Value ? Convert.ToString(row["TraMas_Note"]) : string.Empty;
            TraMas_IsEnable = row.Table.Columns.Contains("TraMas_IsEnable") && row["TraMas_IsEnable"] != DBNull.Value ? Convert.ToBoolean(row["TraMas_IsEnable"]) : false;
            if (row.Table.Columns.Contains("TraMas_UpdateTime") && row["TraMas_UpdateTime"] != DBNull.Value)
            {
                TraMas_UpdateTime = Convert.ToDateTime(row["TraMas_UpdateTime"]);
            }
            TraMas_CashAmount = row.Table.Columns.Contains("TraMas_CashAmount") && row["TraMas_CashAmount"] != DBNull.Value ? Convert.ToInt32(row["TraMas_CashAmount"]) : 0;
            TraMas_CardAmount = row.Table.Columns.Contains("TraMas_CardAmount") && row["TraMas_CardAmount"] != DBNull.Value ? Convert.ToInt32(row["TraMas_CardAmount"]) : 0;
            TraMas_VoucherAmount = row.Table.Columns.Contains("TraMas_VoucherAmount") && row["TraMas_VoucherAmount"] != DBNull.Value ? Convert.ToInt32(row["TraMas_VoucherAmount"]) : 0;
            TraMas_CashCoupon = row.Table.Columns.Contains("TraMas_CashCoupon") && row["TraMas_CashCoupon"] != DBNull.Value ? Convert.ToInt32(row["TraMas_CashCoupon"]) : 0;
            TraMas_Prepay = row.Table.Columns.Contains("TraMas_Prepay") && row["TraMas_Prepay"] != DBNull.Value ? Convert.ToInt32(row["TraMas_Prepay"]) : 0;
            TraDet_ProductName = row.Table.Columns.Contains("TraDet_ProductName") && row["TraDet_ProductName"] != DBNull.Value ? Convert.ToString(row["TraDet_ProductName"]) : string.Empty;
            TraDet_ProductID = row.Table.Columns.Contains("TraDet_ProductID") && row["TraDet_ProductID"] != DBNull.Value ? Convert.ToString(row["TraDet_ProductID"]) : string.Empty;
            TraDet_Amount = row.Table.Columns.Contains("TraDet_Amount") && row["TraDet_Amount"] != DBNull.Value ? Convert.ToInt32(row["TraDet_Amount"]) : 0;
            Pro_IsReward = row.Table.Columns.Contains("Pro_IsReward") && row["Pro_IsReward"] != DBNull.Value ? Convert.ToBoolean(row["Pro_IsReward"]) : false;
            Irr = row.Table.Columns.Contains("Irr") && row["Irr"] != DBNull.Value ? Convert.ToString(row["Irr"]) : string.Empty;
            TraDet_RewardPercent = row.Table.Columns.Contains("TraDet_RewardPercent") && row["TraDet_RewardPercent"] != DBNull.Value ? Convert.ToInt32(row["TraDet_RewardPercent"]) : 0;
            TraDet_PriceSum = row.Table.Columns.Contains("TraDet_PriceSum") && row["TraDet_PriceSum"] != DBNull.Value ? Convert.ToInt32(row["TraDet_PriceSum"]) : 0;
            TraDet_Price = row.Table.Columns.Contains("TraDet_Price") && row["TraDet_Price"] != DBNull.Value ? Convert.ToInt32(row["TraDet_Price"]) : 0;
            TraDet_IsGift = row.Table.Columns.Contains("TraDet_IsGift") && row["TraDet_IsGift"] != DBNull.Value ? Convert.ToBoolean(row["TraDet_IsGift"]) : false;
            TraDet_RewardPersonnel = row.Table.Columns.Contains("TraDet_RewardPersonnel") && row["TraDet_RewardPersonnel"] != DBNull.Value ? Convert.ToString(row["TraDet_RewardPersonnel"]) : string.Empty;
            TraDet_RewardPersonnelName = row.Table.Columns.Contains("TraDet_RewardPersonnelName") && row["TraDet_RewardPersonnelName"] != DBNull.Value ? Convert.ToString(row["TraDet_RewardPersonnelName"]) : string.Empty;
            TraCount = row.Table.Columns.Contains("TraCount") && row["TraCount"] != DBNull.Value ? Convert.ToInt32(row["TraCount"]) : 0;
            TotalCost = row.Table.Columns.Contains("TotalCost") && row["TotalCost"] != DBNull.Value ? Convert.ToInt32(row["TotalCost"]) : 0;
            Profit = row.Table.Columns.Contains("Profit") && row["Profit"] != DBNull.Value ? Convert.ToInt32(row["Profit"]) : 0;
            ProfitPercent = row.Table.Columns.Contains("ProfitPercent") && row["ProfitPercent"] != DBNull.Value ? Convert.ToDecimal(row["ProfitPercent"]) : 0;
        }
        #endregion ----- Define Variables -----
    }
}