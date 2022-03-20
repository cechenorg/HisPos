using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.AccountReport.ClosingAccountReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    public class ClosingWorkViewModel : TabBase

    {
        private DateTime startDate = DateTime.Today;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private int trade;

        public int Trade
        {
            get => trade;
            set
            {
                Set(() => Trade, ref trade, value);
            }
        }

        private int tradeTodayStock;

        public int TradeTodayStock
        {
            get => tradeTodayStock;
            set
            {
                Set(() => TradeTodayStock, ref tradeTodayStock, value);
            }
        }

        private int tradeTodayProfit;

        public int TradeTodayProfit
        {
            get => tradeTodayProfit;
            set
            {
                Set(() => TradeTodayProfit, ref tradeTodayProfit, value);
            }
        }

        private int coop;

        public int Coop
        {
            get => coop;
            set
            {
                Set(() => Coop, ref coop, value);
            }
        }

        private int other;

        public int Other
        {
            get => other;
            set
            {
                Set(() => Other, ref other, value);
            }
        }

        private int cashTotal;

        public int CashTotal
        {
            get => cashTotal;
            set
            {
                Set(() => CashTotal, ref cashTotal, value);
            }
        }

        private int self;

        public int Self
        {
            get => self;
            set
            {
                Set(() => Self, ref self, value);
            }
        }

        private int count;

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }

        //-------------

        private int tradeCard;

        public int TradeCard
        {
            get => tradeCard;
            set
            {
                Set(() => TradeCard, ref tradeCard, value);
            }
        }

        private int tradeCash;

        public int TradeCash
        {
            get => tradeCash;
            set
            {
                Set(() => TradeCash, ref tradeCash, value);
            }
        }

        private int tradeCashCoupon;

        public int TradeCashCoupon
        {
            get => tradeCashCoupon;
            set
            {
                Set(() => TradeCashCoupon, ref tradeCashCoupon, value);
            }
        }

        private int tradeDiscount;

        public int TradeDiscount
        {
            get => tradeDiscount;
            set
            {
                Set(() => TradeDiscount, ref tradeDiscount, value);
            }
        }

        private int tradeReward;

        public int TradeReward
        {
            get => tradeReward;
            set
            {
                Set(() => TradeReward, ref tradeReward, value);
            }
        }

        private int extra;

        public int Extra
        {
            get => extra;
            set
            {
                Set(() => Extra, ref extra, value);
            }
        }

        private int total;

        public int Total
        {
            get => total;
            set
            {
                Set(() => Total, ref total, value);
            }
        }

        private int checkTotal;

        public int CheckTotal
        {
            get => checkTotal;
            set
            {
                Set(() => CheckTotal, ref checkTotal, value);
            }
        }

        private int closed;

        public int Closed
        {
            get => closed;
            set
            {
                Set(() => Closed, ref closed, value);
            }
        }

        private int closeCash_Total;

        public int CloseCash_Total
        {
            get => closeCash_Total;
            set
            {
                Set(() => CloseCash_Total, ref closeCash_Total, value);
            }
        }

        private string checkClosed;

        public string CheckClosed
        {
            get => checkClosed;
            set
            {
                Set(() => CheckClosed, ref checkClosed, value);
            }
        }

        private int preCash;

        public int PreCash
        {
            get => preCash;
            set
            {
                Set(() => PreCash, ref preCash, value);
            }
        }

        private int preCard;

        public int PreCard
        {
            get => preCard;
            set
            {
                Set(() => PreCard, ref preCard, value);
            }
        }

        private int returnPreCash;

        public int ReturnPreCash
        {
            get => returnPreCash;
            set
            {
                Set(() => ReturnPreCash, ref returnPreCash, value);
            }
        }

        private int returnPreCard;

        public int ReturnPreCard
        {
            get => returnPreCard;
            set
            {
                Set(() => ReturnPreCard, ref returnPreCard, value);
            }
        }

        private int prepayToday;

        public int PrepayToday
        {
            get => prepayToday;
            set
            {
                Set(() => PrepayToday, ref prepayToday, value);
            }
        }

        private Brush checkColor;

        public Brush CheckColor
        {
            get => checkColor;
            set
            {
                Set(() => CheckColor, ref checkColor, value);
            }
        }

        private bool enable;

        public bool Enable
        {
            get => enable;
            set
            {
                Set(() => Enable, ref enable, value);
            }
        }

        private int storeOrderPayCash;

        public int StoreOrderPayCash
        {
            get => storeOrderPayCash;
            set
            {
                Set(() => StoreOrderPayCash, ref storeOrderPayCash, value);
            }
        }

        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----

        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand HistoryCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }

        #endregion ----- Define Commands -----

        public ClosingWorkViewModel()
        {
            ReloadCommand = new RelayCommand(ReloadAction);
            ConfirmCommand = new RelayCommand(ConfirmAction);
            HistoryCommand = new RelayCommand(HistoryAction);
            UpdateCommand = new RelayCommand(UpdateAction);
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

        private void ReloadAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("sDate", StartDate));
            parameters.Add(new SqlParameter("eDate", StartDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[ClosingWorkByDate]", parameters);
            MainWindow.ServerConnection.CloseConnection();

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
        }

        private void ConfirmAction()
        {
            if (CheckTotal == 0)
            {
                MessageWindow.ShowMessage("請輸入點算現金", MessageType.ERROR);
                return;
            }
            if (StartDate != DateTime.Today)
            {
                MessageWindow.ShowMessage("僅能關今天的班", MessageType.ERROR);
                return;
            }

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> EMP = new List<SqlParameter>();
            EMP.Add(new SqlParameter("EmpID", ViewModelMainWindow.CurrentUser.ID));
            DataTable EMPresult = MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeByID]", EMP);
            MainWindow.ServerConnection.CloseConnection();

            ConfirmWindow cw = new ConfirmWindow("關班人員：" + EMPresult.Rows[0]["Person_Name"].ToString()
                + "\r\n" + "關班金額：" + CheckTotal.ToString() + "\r\n\r\n資料送出後無法修改，\r\n是否進行關班作業？", "關班確認");
            if (!(bool)cw.DialogResult) { return; }

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("Value", Total - CheckTotal));
            parameters.Add(new SqlParameter("Total", CheckTotal));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCloseCash]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            if (result.Rows[0]["RESULT"].ToString() == "FAIL")
            {
                MessageWindow.ShowMessage("今日已輸入過", MessageType.ERROR);
            }
            else
            {
                MessageWindow.ShowMessage("成功", MessageType.SUCCESS);
            }
            ReloadAction();
            InsertToClosingAccoutRecord();
        }

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
    }
}