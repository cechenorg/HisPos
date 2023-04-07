using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.AccountReport.ClosingAccountReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media;
using His_Pos.NewClass.Person.Employee;
using System.Windows;
using DomainModel.Enum;
using His_Pos.NewClass.ClosingWork;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    public class ClosingWorkViewModel : TabBase
    {
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime startDate = DateTime.Today;

        public int Trade
        {
            get => trade;
            set
            {
                Set(() => Trade, ref trade, value);
            }
        }
        private int trade;

        public int TradeTodayStock
        {
            get => tradeTodayStock;
            set
            {
                Set(() => TradeTodayStock, ref tradeTodayStock, value);
            }
        }
        private int tradeTodayStock;

        public int TradeTodayProfit
        {
            get => tradeTodayProfit;
            set
            {
                Set(() => TradeTodayProfit, ref tradeTodayProfit, value);
            }
        }
        private int tradeTodayProfit;

        public int Coop
        {
            get => coop;
            set
            {
                Set(() => Coop, ref coop, value);
            }
        }
        private int coop;

        public int Other
        {
            get => other;
            set
            {
                Set(() => Other, ref other, value);
            }
        }
        private int other;

        public int CashTotal
        {
            get => cashTotal;
            set
            {
                Set(() => CashTotal, ref cashTotal, value);
            }
        }
        private int cashTotal;

        public int Self
        {
            get => self;
            set
            {
                Set(() => Self, ref self, value);
            }
        }
        private int self;

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }
        private int count;

        public int TradeCard
        {
            get => tradeCard;
            set
            {
                Set(() => TradeCard, ref tradeCard, value);
            }
        }
        private int tradeCard;

        public int TradeCash
        {
            get => tradeCash;
            set
            {
                Set(() => TradeCash, ref tradeCash, value);
            }
        }
        private int tradeCash;

        public int TradeCashCoupon
        {
            get => tradeCashCoupon;
            set
            {
                Set(() => TradeCashCoupon, ref tradeCashCoupon, value);
            }
        }
        private int tradeCashCoupon;

        public int TradeDiscount
        {
            get => tradeDiscount;
            set
            {
                Set(() => TradeDiscount, ref tradeDiscount, value);
            }
        }
        private int tradeDiscount;

        public int TradeReward
        {
            get => tradeReward;
            set
            {
                Set(() => TradeReward, ref tradeReward, value);
            }
        }
        private int tradeReward;

        public int Extra
        {
            get => extra;
            set
            {
                Set(() => Extra, ref extra, value);
            }
        }
        private int extra;

        public int Total
        {
            get => total;
            set
            {
                Set(() => Total, ref total, value);
            }
        }
        private int total;

        public int CheckTotal
        {
            get => checkTotal;
            set
            {
                Set(() => CheckTotal, ref checkTotal, value);
            }
        }
        private int checkTotal;

        public int Closed
        {
            get => closed;
            set
            {
                Set(() => Closed, ref closed, value);
            }
        }
        private int closed;

        public int CloseCash_Total
        {
            get => closeCash_Total;
            set
            {
                Set(() => CloseCash_Total, ref closeCash_Total, value);
            }
        }
        private int closeCash_Total;

        public string CheckClosed
        {
            get => checkClosed;
            set
            {
                Set(() => CheckClosed, ref checkClosed, value);
            }
        }
        private string checkClosed;

        public int PreCash
        {
            get => preCash;
            set
            {
                Set(() => PreCash, ref preCash, value);
            }
        }
        private int preCash;

        public int PreCard
        {
            get => preCard;
            set
            {
                Set(() => PreCard, ref preCard, value);
            }
        }
        private int preCard;

        public int ReturnPreCash
        {
            get => returnPreCash;
            set
            {
                Set(() => ReturnPreCash, ref returnPreCash, value);
            }
        }
        private int returnPreCash;

        public int ReturnPreCard
        {
            get => returnPreCard;
            set
            {
                Set(() => ReturnPreCard, ref returnPreCard, value);
            }
        }
        private int returnPreCard;

        public int PrepayToday
        {
            get => prepayToday;
            set
            {
                Set(() => PrepayToday, ref prepayToday, value);
            }
        }
        private int prepayToday;

        public Brush CheckColor
        {
            get => checkColor;
            set
            {
                Set(() => CheckColor, ref checkColor, value);
            }
        }
        private Brush checkColor;

        public bool Enable
        {
            get => enable;
            set
            {
                Set(() => Enable, ref enable, value);
            }
        }
        private bool enable;

        public int StoreOrderPayCash
        {
            get => storeOrderPayCash;
            set
            {
                Set(() => StoreOrderPayCash, ref storeOrderPayCash, value);
            }
        }
        private int storeOrderPayCash;

        public Visibility BtnVisibility
        {
            get => btnVisibility;
            set
            {
                Set(() => BtnVisibility, ref btnVisibility, value);
            }
        }
        private Visibility btnVisibility = VM.CurrentUser.Authority == Authority.Admin || VM.CurrentUser.Authority == Authority.AccountingStaff ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// OTC現金確認
        /// </summary>
        public bool OtcCash
        {
            get => otcCash;
            set
            {
                Set(() => OtcCash, ref otcCash, value);
            }
        }
        private bool otcCash;

        /// <summary>
        /// OTC信用卡確認
        /// </summary>
        public bool OtcCard
        {
            get => otcCard;
            set
            {
                Set(() => OtcCard, ref otcCard, value);
            }
        }
        private bool otcCard;

        /// <summary>
        /// OTC禮券確認
        /// </summary>
        public bool OtcTicket
        {
            get => otcTicket;
            set
            {
                Set(() => OtcTicket, ref otcTicket, value);
            }
        }
        private bool otcTicket;

        /// <summary>
        /// OTC現金券確認
        /// </summary>
        public bool OtcCashTicket
        {
            get => otcCashTicket;
            set
            {
                Set(() => OtcCashTicket, ref otcCashTicket, value);
            }
        }
        private bool otcCashTicket;

        /// <summary>
        /// 額外收支確認
        /// </summary>
        public bool OtherCash
        {
            get => otherCash;
            set
            {
                Set(() => OtherCash, ref otherCash, value);
            }
        }
        private bool otherCash;

        /// <summary>
        /// 下貨付現確認
        /// </summary>
        public bool PayCash
        {
            get => payCash;
            set
            {
                Set(() => PayCash, ref payCash, value);
            }
        }
        private bool payCash;

        /// <summary>
        /// 自費現金確認
        /// </summary>
        public bool MedCash
        {
            get => medCash;
            set
            {
                Set(() => MedCash, ref medCash, value);
            }
        }
        private bool medCash;

        /// <summary>
        /// 當日收訂金(現金)確認
        /// </summary>
        public bool PrepayCash
        {
            get => prepayCash;
            set
            {
                Set(() => PrepayCash, ref prepayCash, value);
            }
        }
        private bool prepayCash;

        /// <summary>
        /// 當日收訂金(信用卡)確認
        /// </summary>
        public bool PrepayCard
        {
            get => prepayCard;
            set
            {
                Set(() => PrepayCard, ref prepayCard, value);
            }
        }
        private bool prepayCard;

        /// <summary>
        /// 當日退訂金(現金)確認
        /// </summary>
        public bool RePreCash
        {
            get => rePreCash;
            set
            {
                Set(() => RePreCash, ref rePreCash, value);
            }
        }
        private bool rePreCash;

        /// <summary>
        /// 當日退訂金(信用卡)確認
        /// </summary>
        public bool RePreCard
        {
            get => rePreCard;
            set
            {
                Set(() => RePreCard, ref rePreCard, value);
            }
        }
        private bool rePreCard;

        /// <summary>
        /// 是否可轉傳票
        /// </summary>
        public bool IsCanToJonual
        {
            get => isCanToJonual;
            set
            {
                Set(() => IsCanToJonual, ref isCanToJonual, value);
            }
        }
        private bool isCanToJonual = true;
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----

        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand HistoryCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand ToJournalCommand { get; set; }

        #endregion ----- Define Commands -----

        public ClosingWorkViewModel()
        {
            ReloadCommand = new RelayCommand(ReloadAction);
            ConfirmCommand = new RelayCommand(ConfirmAction);
            HistoryCommand = new RelayCommand(HistoryAction);
            UpdateCommand = new RelayCommand(UpdateAction);
            ToJournalCommand = new RelayCommand(ToJournalAction);
            ReloadAction();
        }

        private void UpdateAction()
        {
            ReloadAction();
            InsertToClosingAccoutRecord();
        }

        private void HistoryAction()
        {
            var historyWindow = new ClosingHistoryWindow();
            historyWindow.ShowDialog();
        }

        /// <summary>
        /// 重新整理畫面，讀取關班紀錄
        /// </summary>
        private void ReloadAction()
        {
            DataTable result = ClosingWorkDB.ClosingWorkByDate(StartDate);

            Trade = (int)result.Rows[0]["trade"];
            Coop = (int)result.Rows[0]["coop"];
            Self = (int)result.Rows[0]["selff"];
            Other = (int)result.Rows[0]["other"];
            Count = (int)result.Rows[0]["count"];
            CashTotal = (int)result.Rows[0]["CashTotal"];
            TradeCard = (int)result.Rows[0]["tradeCard"];
            TradeCash = (int)result.Rows[0]["tradeCash"];
            TradeCashCoupon = (int)result.Rows[0]["tradeCashCoupon"];
            TradeDiscount = (int)result.Rows[0]["tradeDiscount"];
            TradeReward = (int)result.Rows[0]["tradeReward"];
            Extra = (int)result.Rows[0]["Extra"];
            TradeTodayProfit = (int)result.Rows[0]["tradeTodayProfit"];
            TradeTodayStock = (int)result.Rows[0]["tradeTodayStock"];
            Closed = (int)result.Rows[0]["Closed"];
            PreCash = (int)result.Rows[0]["PreCash"];
            PreCard = (int)result.Rows[0]["PreCard"];
            ReturnPreCash = (int)result.Rows[0]["ReturnPreCash"];
            ReturnPreCard = (int)result.Rows[0]["ReturnPreCard"];
            PrepayToday = (int)result.Rows[0]["PrepayToday"];
            StoreOrderPayCash = (int)result.Rows[0]["StoreOrderPayCash"];

            CheckClosed = result.Rows[0]["CheckClosed"].ToString();
            CloseCash_Total = (int)result.Rows[0]["CloseCash_Total"];
            CheckColor = Brushes.Green;
            Enable = false;
            if (CheckClosed == null || CheckClosed == "")
            {
                CheckClosed = "未關班";
                CheckColor = Brushes.Red;
                Enable = true;
            }
            Total = TradeCash + CashTotal + TradeReward + Extra + PreCash - ReturnPreCash - StoreOrderPayCash;
            if (CheckClosed == "未關班")
            {
                CheckTotal = 0;
            }
            else
            {
                CheckTotal = CloseCash_Total;
            }

            IsCanToJonual = CheckClosed.Equals("未關班") ? false : true;
            ClearCheckBox();
        }

        private void ConfirmAction()
        {
            if (!IsChkAllOn())
            {
                MessageWindow.ShowMessage("請確認所有項目！", MessageType.ERROR);
                return;
            }

            if (CheckTotal == 0)
            {
                MessageWindow.ShowMessage("請輸入點算現金", MessageType.ERROR);
                return;
            }
            if (StartDate != DateTime.Today && ViewModelMainWindow.CurrentUser.Authority != Authority.Admin && ViewModelMainWindow.CurrentUser.Authority != Authority.AccountingStaff)
            {
                MessageWindow.ShowMessage("僅能關今天的班", MessageType.ERROR);
                return;
            }

            EmployeeService employeeService = new EmployeeService(new EmployeeDb());
            var emp = employeeService.GetDataByID(ViewModelMainWindow.CurrentUser.ID);
              
            ConfirmWindow cw = new ConfirmWindow("關班人員：" + emp.Name
                + "\r\n" + "關班金額：" + CheckTotal + "\r\n\r\n資料送出後無法修改，\r\n是否進行關班作業？", "關班確認");
            if (!(bool)cw.DialogResult)
                return;

            DataTable result = ClosingWorkDB.InsertCloseCash(Total, CheckTotal, StartDate);
            if (result.Rows[0]["RESULT"].ToString() == "FAIL")
            {
                MessageWindow.ShowMessage("今日已輸入過", MessageType.ERROR);
            }
            else
            {
                MessageWindow.ShowMessage("成功", MessageType.SUCCESS);

                ReloadAction();
                InsertToClosingAccoutRecord();
                if (VM.AutoAddJournal)
                {
                    InsertJournal();
                }
            }
        }

        /// <summary>
        /// 新增關班紀錄，SD_MainServer
        /// </summary>
        private void InsertToClosingAccoutRecord()
        {
            DailyClosingAccount data = new DailyClosingAccount()
            {
                ClosingDate = StartDate,

                PharmacyName = ViewModelMainWindow.CurrentPharmacy.Name,
                OTCSaleProfit = Trade - TradeTodayStock,
                DailyAdjustAmount = Count,
                CooperativeClinicProfit = Coop,
                PrescribeProfit = Self,
                ChronicAndOtherProfit = Other
            };
            data.SelfProfit = data.OTCSaleProfit + data.ChronicAndOtherProfit + data.PrescribeProfit;
            data.TotalProfit = data.SelfProfit + data.CooperativeClinicProfit; ;

            if (data.OTCSaleProfit == 0 && data.ChronicAndOtherProfit == 0 && data.PrescribeProfit == 0 && data.CooperativeClinicProfit == 0)
                return;

            ClosingAccountReportRepository repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();
            repo.InsertDailyClosingAccountRecord(data);
            MainWindow.ServerConnection.CloseConnection();
        }
        private void ToJournalAction()
        {
            if (!IsChkAllOn())
            {
                MessageWindow.ShowMessage("請確認所有項目！", MessageType.ERROR);
                return;
            }
            if (CheckClosed.Equals("已關班"))
            {
                DataTable table = ClosingWorkDB.GetClosingWorkToJournal(StartDate);
                if (table != null && table.Rows.Count > 0)
                {
                    string orderID = string.Empty;
                    foreach (DataRow dr in table.Rows)
                    {
                        orderID += Convert.ToString(dr["JouMas_ID"]) + "\r\n";
                    }
                    MessageWindow.ShowMessage(string.Format("已有關班傳票\r\n{0}", orderID), MessageType.ERROR);
                    return;
                }
                else
                {
                    InsertJournal();
                    table = ClosingWorkDB.GetClosingWorkToJournal(StartDate);
                    if (table != null && table.Rows.Count > 0)
                    {
                        string orderID = string.Empty;
                        foreach (DataRow dr in table.Rows)
                        {
                            orderID += Convert.ToString(dr["JouMas_ID"]) + "\r\n";
                        }
                        MessageWindow.ShowMessage(string.Format("關班傳票新增成功\r\n{0}", orderID), MessageType.SUCCESS);
                        return;
                    }
                }
            }
            else
            {
                MessageWindow.ShowMessage("尚未關班", MessageType.ERROR);
                return;
            }
        }
        /// <summary>
        /// 新增關班傳票
        /// </summary>
        private void InsertJournal()
        {
            ClosingWorkDB.SetClosingWorkToJournal(StartDate);
        }
        private void ClearCheckBox()
        {
            OtcCash = false;
            OtcCard = false;
            OtcTicket = false;
            OtcCashTicket = false;
            OtherCash  = false;
            PayCash = false;
            MedCash = false;
            PrepayCash = false;
            PrepayCard = false;
            RePreCash = false;
            RePreCard = false;
        }
        private bool IsChkAllOn()
        {
            if (OtcCash && OtcCard && OtcTicket && OtcCashTicket && OtherCash && PayCash && MedCash && PrepayCash && PrepayCard && RePreCash && RePreCard)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}