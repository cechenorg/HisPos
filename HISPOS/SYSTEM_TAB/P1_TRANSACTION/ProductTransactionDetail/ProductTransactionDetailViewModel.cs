using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Trade;
using His_Pos.NewClass.Trade.TradeRecord;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionDetail
{
    public class ProductTransactionDetailViewModel : ViewModelBase
    {
        public ProductTransactionDetailViewModel()
        {
            DeleteCommand = new RelayCommand(DeleteAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            ReturnCommand = new RelayCommand(ReturnAction);
        }
        public string MasterID//銷售代碼
        {
            get { return masterID; }
            set { Set(() => MasterID, ref masterID, value); }
        }
        private string masterID;
        public string PayMethod
        {
            get { return payMethod; }
            set { Set(() => PayMethod, ref payMethod, value); }
        }
        private string payMethod;
        public int RealTotal//結帳金額
        {
            get { return realTotal; }
            set { Set(() => RealTotal, ref realTotal, value); }
        }
        private int realTotal;
        public string Cashier//結帳人員
        {
            get { return cashier; }
            set { Set(() => Cashier, ref cashier, value); }
        }
        private string cashier;
        public int Cash//現金
        {
            get { return cash; }
            set { Set(() => Cash, ref cash, value); }
        }
        private int cash;
        public int Card//信用卡
        {
            get { return card; }
            set { Set(() => Card, ref card, value); }
        }
        private int card;
        public int Voucher//禮券
        {
            get { return voucher; }
            set { Set(() => Voucher, ref voucher, value); }
        }
        private int voucher;
        public int CashCoupon//現金券
        {
            get { return cashCoupon; }
            set { Set(() => CashCoupon, ref cashCoupon, value); }
        }
        private int cashCoupon;
        public int Prepay//訂金沖銷
        {
            get { return prepay; }
            set { Set(() => Prepay, ref prepay, value); }
        }
        private int prepay;
        public string CardNum//卡號
        {
            get { return cardNum; }
            set { Set(() => CardNum, ref cardNum, value); }
        }
        private string cardNum;
        public string TaxNum//統編
        {
            get { return taxNum; }
            set { Set(() => TaxNum, ref taxNum, value); }
        }
        private string taxNum;
        public string InvoiceNum//發票號
        {
            get { return invoiceNum; }
            set { Set(() => InvoiceNum, ref invoiceNum, value); }
        }
        private string invoiceNum;
        public string CusID//顧客代碼
        {
            get { return cusID; }
            set { Set(() => CusID, ref cusID, value); }
        }
        private string cusID;
        public string CusName//顧客名稱
        {
            get { return cusName; }
            set { Set(() => CusName, ref cusName, value); }
        }
        private string cusName;
        public string CusPhone//顧客電話
        {
            get { return cusPhone; }
            set { Set(() => CusPhone, ref cusPhone, value); }
        }
        private string cusPhone;
        public int PreTotal//折扣前總計
        {
            get { return preTotal; }
            set { Set(() => PreTotal, ref preTotal, value); }
        }
        private int preTotal;
        public int DiscountAmt//折價
        {
            get { return discountAmt; }
            set { Set(() => DiscountAmt, ref discountAmt, value); }
        }
        private int discountAmt;
        public string PriceType//套用價格
        {
            get { return priceType; }
            set { Set(() => PriceType, ref priceType, value); }
        }
        private string priceType;
        public string TradeTime//結帳時間
        {
            get { return tradeTime; }
            set { Set(() => TradeTime, ref tradeTime, value); }
        }
        private string tradeTime;
        public string UpdateTime//修改時間
        {
            get { return updateTime; }
            set { Set(() => UpdateTime, ref updateTime, value); }
        }
        private string updateTime;
        public bool IsEnable
        {
            get { return isEnable; }
            set { Set(() => IsEnable, ref isEnable, value); }
        }
        private bool isEnable;
        public string Note//備註
        {
            get { return note; }
            set { Set(() => Note, ref note, value); }
        }
        private string note;
        public DataTable MasterTable//明細
        {
            get { return masterTable; }
            set { Set(() => MasterTable, ref masterTable, value); }
        }
        private DataTable masterTable;

        public List<TradeDetail> Details
        {
            get { return details; }
            set 
            { 
                Set(() => Details, ref details, value); 
            }
        }
        private List<TradeDetail> details;

        public TradeDetail Detail
        {
            get { return detail; }
            set 
            { 
                Set(() => Detail, ref detail, value); 
            }
        }
        private TradeDetail detail;

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand ReturnCommand { get; set; }
        private void DeleteAction()
        {
            ConfirmWindow cw = new ConfirmWindow("是否刪除交易紀錄?", "刪除紀錄確認");
            if (!(bool)cw.DialogResult) { return; }
            else
            {
                DataTable result = TradeService.TradeRecordDelete(masterID);

                if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                {
                    MessageWindow.ShowMessage("刪除成功！", MessageType.SUCCESS);
                }
                else if (result.Rows[0].Field<string>("RESULT").Equals("NORETURN"))
                {
                    MessageWindow.ShowMessage("訂金餘額不足！", MessageType.ERROR);
                }
                else if (result.Rows[0].Field<string>("RESULT").Equals("Strike"))
                {
                    MessageWindow.ShowMessage("寄售品已沖帳", MessageType.ERROR);
                }
                else { MessageWindow.ShowMessage("刪除失敗！", MessageType.ERROR); }
            }
        }
        private void SubmitAction()
        {
            DataTable result = TradeService.TradeRecordInsert(masterID, cusID, payMethod, preTotal, realTotal, discountAmt, cardNum, invoiceNum, taxNum, cashier, note, cash, card, voucher, cashCoupon, Details);

            if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("修改成功！", MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("YesAction"));
            }
            else { MessageWindow.ShowMessage("修改失敗！", MessageType.ERROR); }
        }
        private void ReturnAction()
        {
            ConfirmWindow cw = new ConfirmWindow("是否進行退貨?", "退貨確認");
            if (!(bool)cw.DialogResult) { return; }
            else
            {
                DataTable result = TradeService.TradeRecordReturn(masterID);

                if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                {
                    MessageWindow.ShowMessage("退貨成功！", MessageType.SUCCESS);
                    Messenger.Default.Send(new NotificationMessage("YesAction"));
                }
                else if (result.Rows[0].Field<string>("RESULT").Equals("Strike"))
                {
                    MessageWindow.ShowMessage("寄售品已沖帳", MessageType.ERROR);
                }
                else { MessageWindow.ShowMessage("退貨失敗！", MessageType.ERROR); }
            }
        }
        public void GetData()
        {
            if (MasterTable != null && MasterTable.Rows.Count > 0)
            {
                Dispatcher.CurrentDispatcher.Invoke(delegate ()
                {
                    CusName = MasterTable.Rows[0]["Cus_Name"].ToString();
                    RealTotal = (int)MasterTable.Rows[0]["TraMas_RealTotal"];
                    Cashier = (string)MasterTable.Rows[0]["Emp_Name"];
                    CardNum = MasterTable.Rows[0]["TraMas_CardNumber"].ToString();
                    TaxNum = MasterTable.Rows[0]["TraMas_TaxNumber"].ToString();
                    InvoiceNum = MasterTable.Rows[0]["TraMas_InvoiceNumber"].ToString();
                    PreTotal = (int)MasterTable.Rows[0]["TraMas_PreTotal"];
                    DiscountAmt = (int)MasterTable.Rows[0]["TraMas_DiscountAmt"];
                    cusID = MasterTable.Rows[0]["TraMas_CustomerID"].ToString();
                    payMethod = MasterTable.Rows[0]["TraMas_PayMethod"].ToString();
                    Cash = (int)MasterTable.Rows[0]["TraMas_CashAmount"];
                    Card = (int)MasterTable.Rows[0]["TraMas_CardAmount"];
                    Voucher = (int)MasterTable.Rows[0]["TraMas_VoucherAmount"];
                    CashCoupon = (int)MasterTable.Rows[0]["TraMas_CashCoupon"];
                    Prepay = (int)MasterTable.Rows[0]["TraMas_Prepay"];
                    TradeTime = Convert.ToString(MasterTable.Rows[0]["TraMas_ChkoutTime"]);
                    if (!string.IsNullOrEmpty(TradeTime))
                    {
                        TradeTime = Convert.ToDateTime(TradeTime).ToString("yyyy/MM/dd hh:mm");
                    }
                    Note = MasterTable.Rows[0]["TraMas_Note"].ToString();
                    CusPhone = MasterTable.Rows[0]["Cus_Phone"].ToString();
                    UpdateTime = Convert.ToString(MasterTable.Rows[0]["TraMas_UpdateTime"]);
                    if (!string.IsNullOrEmpty(UpdateTime))
                    {
                        UpdateTime = Convert.ToDateTime(UpdateTime).ToString("yyyy/MM/dd hh:mm");
                    }
                    
                    IsEnable = Convert.ToBoolean(MasterTable.Rows[0]["TraMas_IsEnable"]);
                    if (Details != null && Details.Count > 0)
                    {
                        string type = Details[0].TraDet_PriceType.ToString();
                        switch (type)
                        {
                            case "Pro_RetailPrice":
                                PriceType = "零售價";
                                break;

                            case "Pro_MemberPrice":
                                PriceType = "會員價";
                                break;

                            case "Pro_EmployeePrice":
                                PriceType = "員工價";
                                break;

                            case "Pro_SpecialPrice":
                                PriceType = "特殊價";
                                break;

                            default:
                                PriceType = "零售價";
                                break;
                        }

                        foreach (var item in Details)
                        {
                            int PerPrice = Math.Abs((int)item.TraDet_Price);

                            if (TradeService.GetPriceList(item.TraDet_ProductID.ToString()).Rows[0]["Pro_MemberPrice"].ToString() == PerPrice.ToString() || TradeService.GetPriceList(item.TraDet_ProductID.ToString()).Rows[0]["Pro_RetailPrice"].ToString() == PerPrice.ToString() || item.TraDet_IsGift.ToString() == "1")
                            {
                                item.Irr = "";
                            }
                            else
                            {
                                item.Irr = "Yes";
                            }
                        }
                    }
                });
            }
        }
    }
}