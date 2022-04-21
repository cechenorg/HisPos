using ClosedXML.Excel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Report;
using His_Pos.NewClass.Report.CashDetailReport;
using His_Pos.NewClass.Report.CashDetailReport.CashDetailRecordReport;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.NewClass.Report.ExtraMoneyDetailReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot;
using His_Pos.NewClass.Report.PrescriptionProfitReport;
using His_Pos.NewClass.Report.RewardDetailReport;
using His_Pos.NewClass.Report.RewardReport;
using His_Pos.NewClass.Report.StockTakingDetailReport;
using His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport;
using His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingOTCDetailRecordReport;
using His_Pos.NewClass.Report.StockTakingOTCReport;
using His_Pos.NewClass.Report.StockTakingReport;
using His_Pos.NewClass.Report.TradeProfitDetailEmpReport;
using His_Pos.NewClass.Report.TradeProfitDetailEmpReport.TradeProfitDetailEmpRecordReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport.ExtraMoneyDetailRecordReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport.RewardDetailRecordReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport;
using His_Pos.NewClass.Report.TradeProfitReport;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionDetail;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using His_Pos.NewClass.Report.DepositReport;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.NewTodayCashStockEntryReport
{
    public class NewTodayCashStockEntryReportViewModel : TabBase
    {
        #region Variables

        public override TabBase getTab()
        {
            return this;
        }

        private int extraMoneyDetailCount;

        public int ExtraMoneyDetailCount
        {
            get => extraMoneyDetailCount;
            set
            {
                Set(() => ExtraMoneyDetailCount, ref extraMoneyDetailCount, value);
            }
        }

        private int empProfit;

        public int EmpProfit
        {
            get => empProfit;
            set
            {
                Set(() => EmpProfit, ref empProfit, value);
            }
        }

        private int rewardDetailCount;

        public int RewardDetailCount
        {
            get => rewardDetailCount;
            set
            {
                Set(() => RewardDetailCount, ref rewardDetailCount, value);
            }
        }

        private int stockDetailCount;

        public int StockDetailCount
        {
            get => stockDetailCount;
            set
            {
                Set(() => StockDetailCount, ref stockDetailCount, value);
            }
        }

        private int stockOTCDetailCount;

        public int StockOTCDetailCount
        {
            get => stockOTCDetailCount;
            set
            {
                Set(() => StockOTCDetailCount, ref stockOTCDetailCount, value);
            }
        }

        private int tradeDetailCount;

        public int TradeDetailCount
        {
            get => tradeDetailCount;
            set
            {
                Set(() => TradeDetailCount, ref tradeDetailCount, value);
            }
        }

        private int tradeEmpDetailCount;

        public int TradeEmpDetailCount
        {
            get => tradeEmpDetailCount;
            set
            {
                Set(() => TradeEmpDetailCount, ref tradeEmpDetailCount, value);
            }
        }

        private Visibility coopVis;

        public Visibility CoopVis
        {
            get => coopVis;
            set
            {
                Set(() => CoopVis, ref coopVis, value);
            }
        }

        private Visibility cashcoopVis;

        public Visibility CashCoopVis
        {
            get => cashcoopVis;
            set
            {
                Set(() => CashCoopVis, ref cashcoopVis, value);
            }
        }

        private Visibility changeVis;

        public Visibility ChangeVis
        {
            get => changeVis;
            set
            {
                Set(() => ChangeVis, ref changeVis, value);
            }
        }

        private Visibility changeOTCVis;

        public Visibility ChangeOTCVis
        {
            get => changeOTCVis;
            set
            {
                Set(() => ChangeOTCVis, ref changeOTCVis, value);
            }
        }

        private Visibility incomeVis;

        public Visibility IncomeVis
        {
            get => incomeVis;
            set
            {
                Set(() => IncomeVis, ref incomeVis, value);
            }
        }

        private Visibility costVis;

        public Visibility CostVis
        {
            get => costVis;
            set
            {
                Set(() => CostVis, ref costVis, value);
            }
        }

        private Visibility profitVis;

        public Visibility ProfitVis
        {
            get => profitVis;
            set
            {
                Set(() => ProfitVis, ref profitVis, value);
            }
        }

        private Visibility ticketVis;

        public Visibility TicketVis
        {
            get => ticketVis;
            set
            {
                Set(() => TicketVis, ref ticketVis, value);
            }
        }

        private List<string> adjustCaseString  = new List<string>() { "全部", "一般箋", "慢箋", "自費調劑" };

        public List<string> AdjustCaseString
        {
            get => adjustCaseString;
            set
            {
                Set(() => AdjustCaseString, ref adjustCaseString, value);
            }
        }

        private List<string> cashcoopString;

        public List<string> CashCoopString
        {
            get => cashcoopString;
            set
            {
                Set(() => CashCoopString, ref cashcoopString, value);
            }
        }

        private List<string> coopString;

        public List<string> CoopString
        {
            get => coopString;
            set
            {
                Set(() => CoopString, ref coopString, value);
            }
        }

        private List<string> changeString;

        public List<string> ChangeString
        {
            get => changeString;
            set
            {
                Set(() => ChangeString, ref changeString, value);
            }
        }

        private List<string> stockTakingOTCString;

        public List<string> StockTakingOTCString
        {
            get => stockTakingOTCString;
            set
            {
                Set(() => StockTakingOTCString, ref stockTakingOTCString, value);
            }
        }

        private List<string> stockTakingString;

        public List<string> StockTakingString
        {
            get => stockTakingString;
            set
            {
                Set(() => StockTakingString, ref stockTakingString, value);
            }
        }

        private string coopSelectItem = "全部";

        public string CoopSelectItem
        {
            get => coopSelectItem;
            set
            {
                Set(() => CoopSelectItem, ref coopSelectItem, value);

                // PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumPrescriptionDetailReport();
            }
        }

        private string cashcoopSelectItem = "全部";

        public string CashCoopSelectItem
        {
            get => cashcoopSelectItem;
            set
            {
                Set(() => CashCoopSelectItem, ref cashcoopSelectItem, value);

                //CashDetailReportViewSource.Filter += CashCoopFilter;
                // SumCashDetailReport();
            }
        }

        private string tradeChangeSelectItem = "全部";

        public string TradeChangeSelectItem
        {
            get => tradeChangeSelectItem;
            set
            {
                Set(() => TradeChangeSelectItem, ref tradeChangeSelectItem, value);
            }
        }

        private string adjustCaseSelectItem = "全部";

        public string AdjustCaseSelectItem
        {
            get => adjustCaseSelectItem;
            set
            {
                Set(() => AdjustCaseSelectItem, ref adjustCaseSelectItem, value);
            }
        }

        private string stockTakingOTCSelectItem = "全部";

        public string StockTakingOTCSelectItem
        {
            get => stockTakingOTCSelectItem;
            set
            {
                Set(() => StockTakingOTCSelectItem, ref stockTakingOTCSelectItem, value);
            }
        }

        private string stockTakingSelectItem = "全部";

        public string StockTakingSelectItem
        {
            get => stockTakingSelectItem;
            set
            {
                Set(() => StockTakingSelectItem, ref stockTakingSelectItem, value);
            }
        }

        private DateTime startDate = DateTime.Today;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime endDate = DateTime.Today;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private CashReport totalCashFlow = new CashReport();

        public CashReport TotalCashFlow
        {
            get => totalCashFlow;
            set
            {
                Set(() => TotalCashFlow, ref totalCashFlow, value);
            }
        }

        private CashReports cashflowCollection;

        public CashReports CashflowCollection
        {
            get => cashflowCollection;
            set
            {
                Set(() => CashflowCollection, ref cashflowCollection, value);
            }
        }
         
        private PrescriptionProfitReport cooperativePrescriptionSelectedItem;

        public PrescriptionProfitReport CooperativePrescriptionSelectedItem
        {
            get => cooperativePrescriptionSelectedItem;
            set
            {
                Set(() => CooperativePrescriptionSelectedItem, ref cooperativePrescriptionSelectedItem, value);
            }
        }

        // 8.12新增
        private TradeProfitReports tradeProfitReportCollection = new TradeProfitReports();

        public TradeProfitReports TradeProfitReportCollection
        {
            get => tradeProfitReportCollection;
            set
            {
                Set(() => TradeProfitReportCollection, ref tradeProfitReportCollection, value);
            }
        }

        private TradeProfitReports tradeDeleteReportCollection = new TradeProfitReports();

        public TradeProfitReports TradeDeleteReportCollection
        {
            get => tradeDeleteReportCollection;
            set
            {
                Set(() => TradeDeleteReportCollection, ref tradeDeleteReportCollection, value);
            }
        }

        private TradeProfitReports tradeChangeReportCollection = new TradeProfitReports();

        public TradeProfitReports TradeChangeReportCollection
        {
            get => tradeChangeReportCollection;
            set
            {
                Set(() => TradeChangeReportCollection, ref tradeChangeReportCollection, value);
            }
        }

        private TradeProfitReports tradeNormalReportCollection = new TradeProfitReports();

        public TradeProfitReports TradeNormalReportCollection
        {
            get => tradeNormalReportCollection;
            set
            {
                Set(() => TradeNormalReportCollection, ref tradeNormalReportCollection, value);
            }
        }

        private TradeProfitReport tradeNormalProfitReport = new TradeProfitReport();

        public TradeProfitReport TradeNormalProfitReport
        {
            get => tradeNormalProfitReport;
            set
            {
                Set(() => TradeNormalProfitReport, ref tradeNormalProfitReport, value);
            }
        }

        private TradeProfitReport tradeProfitReport = new TradeProfitReport();

        public TradeProfitReport TradeProfitReport
        {
            get => tradeProfitReport;
            set
            {
                Set(() => TradeProfitReport, ref tradeProfitReport, value);
            }
        }

        private TradeProfitReport totalTradeProfitReport = new TradeProfitReport();

        public TradeProfitReport TotalTradeProfitReport
        {
            get => totalTradeProfitReport;
            set
            {
                Set(() => TotalTradeProfitReport, ref totalTradeProfitReport, value);
            }
        }

        public TradeProfitReports TotalTradeProfitReportCollection { get; set; } = new TradeProfitReports();
        // 8.12新增^^^

        //9.3新增
        private StockTakingReports stockTakingReportCollection = new StockTakingReports();

        public StockTakingReports StockTakingReportCollection
        {
            get => stockTakingReportCollection;
            set
            {
                Set(() => StockTakingReportCollection, ref stockTakingReportCollection, value);
            }
        }

        private StockTakingReport stockTakingReport = new StockTakingReport();

        public StockTakingReport StockTakingReport
        {
            get => stockTakingReport;
            set
            {
                Set(() => StockTakingReport, ref stockTakingReport, value);
            }
        }

        private StockTakingReport totalStockTakingReport = new StockTakingReport();

        public StockTakingReport TotalStockTakingReport
        {
            get => totalStockTakingReport;
            set
            {
                Set(() => TotalStockTakingReport, ref totalStockTakingReport, value);
            }
        }

         

        //9.4新增
        private StockTakingReport stockTakingSelectedItem;

        public StockTakingReport StockTakingSelectedItem
        {
            get => stockTakingSelectedItem;
            set
            {
                Set(() => StockTakingSelectedItem, ref stockTakingSelectedItem, value);
            }
        }

        private StockTakingDetailReports stockTakingDetailReportCollection;

        public StockTakingDetailReports StockTakingDetailReportCollection
        {
            get => stockTakingDetailReportCollection;
            set
            {
                Set(() => StockTakingDetailReportCollection, ref stockTakingDetailReportCollection, value);
            }
        }

        private CollectionViewSource stockTakingDetailReportViewSource;

        private CollectionViewSource StockTakingDetailReportViewSource
        {
            get => stockTakingDetailReportViewSource;
            set
            {
                Set(() => StockTakingDetailReportViewSource, ref stockTakingDetailReportViewSource, value);
            }
        }

        private ICollectionView stockTakingDetailReportView;

        public ICollectionView StockTakingDetailReportView
        {
            get => stockTakingDetailReportView;
            private set
            {
                Set(() => StockTakingDetailReportView, ref stockTakingDetailReportView, value);
            }
        }

        private StockTakingDetailReport stockTakingDetailReportSelectItem;

        public StockTakingDetailReport StockTakingDetailReportSelectItem
        {
            get => stockTakingDetailReportSelectItem;
            set
            {
                Set(() => StockTakingDetailReportSelectItem, ref stockTakingDetailReportSelectItem, value);
            }
        }

        private StockTakingDetailRecordReports stockTakingDetailRecordReportCollection = new StockTakingDetailRecordReports();

        public StockTakingDetailRecordReports StockTakingDetailRecordReportCollection
        {
            get => stockTakingDetailRecordReportCollection;
            set
            {
                Set(() => StockTakingDetailRecordReportCollection, ref stockTakingDetailRecordReportCollection, value);
            }
        }

        private StockTakingDetailRecordReport stockTakingDetailMedicineReportSelectItem;

        public StockTakingDetailRecordReport StockTakingDetailMedicineReportSelectItem
        {
            get => stockTakingDetailMedicineReportSelectItem;
            set
            {
                Set(() => StockTakingDetailMedicineReportSelectItem, ref stockTakingDetailMedicineReportSelectItem, value);
            }
        }

        //9.4新增^^^^

        //9.7新增
        private TradeProfitReport tradeProfitSelectedItem;

        public TradeProfitReport TradeProfitSelectedItem
        {
            get => tradeProfitSelectedItem;
            set
            {
                Set(() => TradeProfitSelectedItem, ref tradeProfitSelectedItem, value);
            }
        }

        private TradeProfitDetailReports tradeProfitDetailReportCollection;

        public TradeProfitDetailReports TradeProfitDetailReportCollection
        {
            get => tradeProfitDetailReportCollection;
            set
            {
                Set(() => TradeProfitDetailReportCollection, ref tradeProfitDetailReportCollection, value);
            }
        }

        private TradeProfitDetailReports tradeProfitDetailReportCollectionChanged;

        public TradeProfitDetailReports TradeProfitDetailReportCollectionChanged
        {
            get => tradeProfitDetailReportCollectionChanged;
            set
            {
                Set(() => TradeProfitDetailReportCollectionChanged, ref tradeProfitDetailReportCollectionChanged, value);
            }
        }

        private CollectionViewSource tradeProfitDetailReportViewSource;

        private CollectionViewSource TradeProfitDetailReportViewSource
        {
            get => tradeProfitDetailReportViewSource;
            set
            {
                Set(() => TradeProfitDetailReportViewSource, ref tradeProfitDetailReportViewSource, value);
            }
        }

        private ICollectionView tradeProfitDetailReportView;

        public ICollectionView TradeProfitDetailReportView
        {
            get => tradeProfitDetailReportView;
            private set
            {
                Set(() => TradeProfitDetailReportView, ref tradeProfitDetailReportView, value);
            }
        }

        private TradeProfitDetailReport tradeProfitDetailReportSelectItem;

        public TradeProfitDetailReport TradeProfitDetailReportSelectItem
        {
            get => tradeProfitDetailReportSelectItem;
            set
            {
                Set(() => TradeProfitDetailReportSelectItem, ref tradeProfitDetailReportSelectItem, value);
            }
        }

        private TradeProfitDetailRecordReports tradeProfitDetailRecordReportCollection = new TradeProfitDetailRecordReports();

        public TradeProfitDetailRecordReports TradeProfitDetailRecordReportCollection
        {
            get => tradeProfitDetailRecordReportCollection;
            set
            {
                Set(() => TradeProfitDetailRecordReportCollection, ref tradeProfitDetailRecordReportCollection, value);
            }
        }

        private TradeProfitDetailRecordReport tradeProfitDetailMedicineReportSelectItem;

        public TradeProfitDetailRecordReport TradeProfitDetailMedicineReportSelectItem
        {
            get => tradeProfitDetailMedicineReportSelectItem;
            set
            {
                Set(() => TradeProfitDetailMedicineReportSelectItem, ref tradeProfitDetailMedicineReportSelectItem, value);
            }
        }

        //9.7新增^^^^

        //10.5新增

        private ExtraMoneyDetailReports extraMoneyDetailReportCollection;

        public ExtraMoneyDetailReports ExtraMoneyDetailReportCollection
        {
            get => extraMoneyDetailReportCollection;
            set
            {
                Set(() => ExtraMoneyDetailReportCollection, ref extraMoneyDetailReportCollection, value);
            }
        }

        private CollectionViewSource extraMoneyDetailReportViewSource;

        private CollectionViewSource ExtraMoneyDetailReportViewSource
        {
            get => extraMoneyDetailReportViewSource;
            set
            {
                Set(() => ExtraMoneyDetailReportViewSource, ref extraMoneyDetailReportViewSource, value);
            }
        }

        private ICollectionView extraMoneyDetailReportView;

        public ICollectionView ExtraMoneyDetailReportView
        {
            get => extraMoneyDetailReportView;
            private set
            {
                Set(() => ExtraMoneyDetailReportView, ref extraMoneyDetailReportView, value);
            }
        }

        private ExtraMoneyDetailReport extraMoneyDetailReportSelectItem;

        public ExtraMoneyDetailReport ExtraMoneyDetailReportSelectItem
        {
            get => extraMoneyDetailReportSelectItem;
            set
            {
                Set(() => ExtraMoneyDetailReportSelectItem, ref extraMoneyDetailReportSelectItem, value);
            }
        }

        private ExtraMoneyDetailRecordReports extraMoneyDetailRecordReportCollection = new ExtraMoneyDetailRecordReports();

        public ExtraMoneyDetailRecordReports ExtraMoneyDetailRecordReportCollection
        {
            get => extraMoneyDetailRecordReportCollection;
            set
            {
                Set(() => ExtraMoneyDetailRecordReportCollection, ref extraMoneyDetailRecordReportCollection, value);
            }
        }

        private ExtraMoneyDetailRecordReport extraMoneyDetailMedicineReportSelectItem;

        public ExtraMoneyDetailRecordReport ExtraMoneyDetailMedicineReportSelectItem
        {
            get => extraMoneyDetailMedicineReportSelectItem;
            set
            {
                Set(() => ExtraMoneyDetailMedicineReportSelectItem, ref extraMoneyDetailMedicineReportSelectItem, value);
            }
        }

        //10.5新增^^^^

        //10.5OTC新增
        private StockTakingOTCReports stockTakingOTCReportCollection = new StockTakingOTCReports();

        public StockTakingOTCReports StockTakingOTCReportCollection
        {
            get => stockTakingOTCReportCollection;
            set
            {
                Set(() => StockTakingOTCReportCollection, ref stockTakingOTCReportCollection, value);
            }
        }

        private StockTakingOTCReport stockTakingOTCReport = new StockTakingOTCReport();

        public StockTakingOTCReport StockTakingOTCReport
        {
            get => stockTakingOTCReport;
            set
            {
                Set(() => StockTakingOTCReport, ref stockTakingOTCReport, value);
            }
        }

        private StockTakingOTCReport totalStockTakingOTCReport = new StockTakingOTCReport();

        public StockTakingOTCReport TotalStockTakingOTCReport
        {
            get => totalStockTakingOTCReport;
            set
            {
                Set(() => TotalStockTakingOTCReport, ref totalStockTakingOTCReport, value);
            }
        }
         

        private StockTakingOTCReport stockTakingOTCSelectedItem;

        public StockTakingOTCReport StockTakingOTCSelectedItem
        {
            get => stockTakingOTCSelectedItem;
            set
            {
                Set(() => StockTakingOTCSelectedItem, ref stockTakingOTCSelectedItem, value);
            }
        }

        private StockTakingOTCDetailReports stockTakingOTCDetailReportCollection;

        public StockTakingOTCDetailReports StockTakingOTCDetailReportCollection
        {
            get => stockTakingOTCDetailReportCollection;
            set
            {
                Set(() => StockTakingOTCDetailReportCollection, ref stockTakingOTCDetailReportCollection, value);
            }
        }

        private CollectionViewSource stockTakingOTCDetailReportViewSource;

        private CollectionViewSource StockTakingOTCDetailReportViewSource
        {
            get => stockTakingOTCDetailReportViewSource;
            set
            {
                Set(() => StockTakingOTCDetailReportViewSource, ref stockTakingOTCDetailReportViewSource, value);
            }
        }

        private ICollectionView stockTakingOTCDetailReportView;

        public ICollectionView StockTakingOTCDetailReportView
        {
            get => stockTakingOTCDetailReportView;
            private set
            {
                Set(() => StockTakingOTCDetailReportView, ref stockTakingOTCDetailReportView, value);
            }
        }

        private StockTakingOTCDetailReport stockTakingOTCDetailReportSelectItem;

        public StockTakingOTCDetailReport StockTakingOTCDetailReportSelectItem
        {
            get => stockTakingOTCDetailReportSelectItem;
            set
            {
                Set(() => StockTakingOTCDetailReportSelectItem, ref stockTakingOTCDetailReportSelectItem, value);
            }
        }

        private StockTakingOTCDetailRecordReports stockTakingOTCDetailRecordReportCollection = new StockTakingOTCDetailRecordReports();

        public StockTakingOTCDetailRecordReports StockTakingOTCDetailRecordReportCollection
        {
            get => stockTakingOTCDetailRecordReportCollection;
            set
            {
                Set(() => StockTakingOTCDetailRecordReportCollection, ref stockTakingOTCDetailRecordReportCollection, value);
            }
        }

        private StockTakingOTCDetailRecordReport stockTakingOTCDetailMedicineReportSelectItem;

        public StockTakingOTCDetailRecordReport StockTakingOTCDetailMedicineReportSelectItem
        {
            get => stockTakingOTCDetailMedicineReportSelectItem;
            set
            {
                Set(() => StockTakingOTCDetailMedicineReportSelectItem, ref stockTakingOTCDetailMedicineReportSelectItem, value);
            }
        }

        //10.5OTC新增^^^^

        //10.28 新增
        private RewardReports rewardReportCollection = new RewardReports();

        public RewardReports RewardReportCollection
        {
            get => rewardReportCollection;
            set
            {
                Set(() => RewardReportCollection, ref rewardReportCollection, value);
            }
        }

        private RewardReport rewardReport = new RewardReport();

        public RewardReport RewardReport
        {
            get => rewardReport;
            set
            {
                Set(() => RewardReport, ref rewardReport, value);
            }
        }

        private RewardReport totalRewardReport = new RewardReport();

        public RewardReport TotalRewardReport
        {
            get => totalRewardReport;
            set
            {
                Set(() => TotalRewardReport, ref totalRewardReport, value);
            }
        }

        

        private RewardReport rewardSelectedItem;

        public RewardReport RewardSelectedItem
        {
            get => rewardSelectedItem;
            set
            {
                Set(() => RewardSelectedItem, ref rewardSelectedItem, value);
            }
        }

        private RewardDetailReports rewardDetailReportCollection;

        public RewardDetailReports RewardDetailReportCollection
        {
            get => rewardDetailReportCollection;
            set
            {
                Set(() => RewardDetailReportCollection, ref rewardDetailReportCollection, value);
            }
        }

        private CollectionViewSource rewardDetailReportViewSource;

        private CollectionViewSource RewardDetailReportViewSource
        {
            get => rewardDetailReportViewSource;
            set
            {
                Set(() => RewardDetailReportViewSource, ref rewardDetailReportViewSource, value);
            }
        }

        private ICollectionView rewardDetailReportView;

        public ICollectionView RewardDetailReportView
        {
            get => rewardDetailReportView;
            private set
            {
                Set(() => RewardDetailReportView, ref rewardDetailReportView, value);
            }
        }

        private RewardDetailReport rewardDetailReportSelectItem;

        public RewardDetailReport RewardDetailReportSelectItem
        {
            get => rewardDetailReportSelectItem;
            set
            {
                Set(() => RewardDetailReportSelectItem, ref rewardDetailReportSelectItem, value);
            }
        }

        private RewardDetailRecordReports rewardDetailRecordReportCollection = new RewardDetailRecordReports();

        public RewardDetailRecordReports RewardDetailRecordReportCollection
        {
            get => rewardDetailRecordReportCollection;
            set
            {
                Set(() => RewardDetailRecordReportCollection, ref rewardDetailRecordReportCollection, value);
            }
        }

        private RewardDetailRecordReport rewardDetailMedicineReportSelectItem;

        public RewardDetailRecordReport RewardDetailMedicineReportSelectItem
        {
            get => rewardDetailMedicineReportSelectItem;
            set
            {
                Set(() => RewardDetailMedicineReportSelectItem, ref rewardDetailMedicineReportSelectItem, value);
            }
        }

        //10.28新增^^^
        //11.02新增

        private TradeProfitDetailEmpReports tradeProfitDetailEmpReportCollection;

        public TradeProfitDetailEmpReports TradeProfitDetailEmpReportCollection
        {
            get => tradeProfitDetailEmpReportCollection;
            set
            {
                Set(() => TradeProfitDetailEmpReportCollection, ref tradeProfitDetailEmpReportCollection, value);
            }
        }

        private CollectionViewSource tradeProfitDetailEmpReportViewSource;

        private CollectionViewSource TradeProfitDetailEmpReportViewSource
        {
            get => tradeProfitDetailEmpReportViewSource;
            set
            {
                Set(() => TradeProfitDetailEmpReportViewSource, ref tradeProfitDetailEmpReportViewSource, value);
            }
        }

        private ICollectionView tradeProfitDetailEmpReportView;

        public ICollectionView TradeProfitDetailEmpReportView
        {
            get => tradeProfitDetailEmpReportView;
            private set
            {
                Set(() => TradeProfitDetailEmpReportView, ref tradeProfitDetailEmpReportView, value);
            }
        }

        private TradeProfitDetailEmpReport tradeProfitDetailEmpReportSelectItem;

        public TradeProfitDetailEmpReport TradeProfitDetailEmpReportSelectItem
        {
            get => tradeProfitDetailEmpReportSelectItem;
            set
            {
                Set(() => TradeProfitDetailEmpReportSelectItem, ref tradeProfitDetailEmpReportSelectItem, value);
            }
        }

        private TradeProfitDetailEmpRecordReports tradeProfitDetailEmpRecordReportCollection = new TradeProfitDetailEmpRecordReports();

        public TradeProfitDetailEmpRecordReports TradeProfitDetailEmpRecordReportCollection
        {
            get => tradeProfitDetailEmpRecordReportCollection;
            set
            {
                Set(() => TradeProfitDetailEmpRecordReportCollection, ref tradeProfitDetailEmpRecordReportCollection, value);
            }
        }

        private TradeProfitDetailEmpRecordReport tradeProfitDetailEmpMedicineReportSelectItem;

        public TradeProfitDetailEmpRecordReport TradeProfitDetailEmpMedicineReportSelectItem
        {
            get => tradeProfitDetailEmpMedicineReportSelectItem;
            set
            {
                Set(() => TradeProfitDetailEmpMedicineReportSelectItem, ref tradeProfitDetailEmpMedicineReportSelectItem, value);
            }
        }

        //11.02新增^^^
        //11.05新增

        private CollectionViewSource cashDetailReportViewSource;

        private CollectionViewSource CashDetailReportViewSource
        {
            get => cashDetailReportViewSource;
            set
            {
                Set(() => CashDetailReportViewSource, ref cashDetailReportViewSource, value);
            }
        }

        private ICollectionView cashDetailReportView;

        public ICollectionView CashDetailReportView
        {
            get => cashDetailReportView;
            private set
            {
                Set(() => CashDetailReportView, ref cashDetailReportView, value);
            }
        }

        private CashDetailRecordReport cashDetailMedicineReportSelectItem;

        public CashDetailRecordReport CashDetailMedicineReportSelectItem
        {
            get => cashDetailMedicineReportSelectItem;
            set
            {
                Set(() => CashDetailMedicineReportSelectItem, ref cashDetailMedicineReportSelectItem, value);
            }
        }

        //11.05新增^^^
        private PrescriptionProfitReports selfPrescriptionProfitReportCollection = new PrescriptionProfitReports();

        public PrescriptionProfitReports SelfPrescriptionProfitReportCollection
        {
            get => selfPrescriptionProfitReportCollection;
            set
            {
                Set(() => SelfPrescriptionProfitReportCollection, ref selfPrescriptionProfitReportCollection, value);
            }
        }

        private PrescriptionProfitReports selfPrescriptionChangeProfitReportCollection = new PrescriptionProfitReports();

        public PrescriptionProfitReports SelfPrescriptionChangeProfitReportCollection
        {
            get => selfPrescriptionChangeProfitReportCollection;
            set
            {
                Set(() => SelfPrescriptionChangeProfitReportCollection, ref selfPrescriptionChangeProfitReportCollection, value);
            }
        }

        private CashReports cashProfitReportCollection = new CashReports();

        public CashReports CashProfitReportCollection
        {
            get => cashProfitReportCollection;
            set
            {
                Set(() => CashProfitReportCollection, ref cashProfitReportCollection, value);
            }
        }

        private CashReports coopCashReportCollection = new CashReports();

        public CashReports CoopCashProfitReportCollection
        {
            get => coopCashReportCollection;
            set
            {
                Set(() => CoopCashProfitReportCollection, ref coopCashReportCollection, value);
            }
        }

        private PrescriptionProfitReports cooperativePrescriptionProfitReportCollection = new PrescriptionProfitReports();

        public PrescriptionProfitReports CooperativePrescriptionProfitReportCollection
        {
            get => cooperativePrescriptionProfitReportCollection;
            set
            {
                Set(() => CooperativePrescriptionProfitReportCollection, ref cooperativePrescriptionProfitReportCollection, value);
            }
        }

        private PrescriptionProfitReports cooperativePrescriptionChangeProfitReportCollection = new PrescriptionProfitReports();

        public PrescriptionProfitReports CooperativePrescriptionChangeProfitReportCollection
        {
            get => cooperativePrescriptionChangeProfitReportCollection;
            set
            {
                Set(() => CooperativePrescriptionChangeProfitReportCollection, ref cooperativePrescriptionChangeProfitReportCollection, value);
            }
        }

        public PrescriptionProfitReports TotalPrescriptionProfitReportCollection { get; set; } = new PrescriptionProfitReports();
        private PrescriptionPointEditRecords PrescriptionPointEditRecords { get; set; } = new PrescriptionPointEditRecords();
        private PrescriptionProfitReport selfPrescriptionProfitReport = new PrescriptionProfitReport();

        public PrescriptionProfitReport SelfPrescriptionProfitReport
        {
            get => selfPrescriptionProfitReport;
            set
            {
                Set(() => SelfPrescriptionProfitReport, ref selfPrescriptionProfitReport, value);
            }
        }

        private PrescriptionProfitReport selfPrescriptionChangeReport = new PrescriptionProfitReport();

        public PrescriptionProfitReport SelfPrescriptionChangeReport
        {
            get => selfPrescriptionChangeReport;
            set
            {
                Set(() => SelfPrescriptionChangeReport, ref selfPrescriptionChangeReport, value);
            }
        }

        private PrescriptionProfitReport cooperativePrescriptionProfitReport = new PrescriptionProfitReport();

        public PrescriptionProfitReport CooperativePrescriptionProfitReport
        {
            get => cooperativePrescriptionProfitReport;
            set
            {
                Set(() => CooperativePrescriptionProfitReport, ref cooperativePrescriptionProfitReport, value);
            }
        }

        private PrescriptionProfitReport cooperativePrescriptionChangeReport = new PrescriptionProfitReport();

        public PrescriptionProfitReport CooperativePrescriptionChangeReport
        {
            get => cooperativePrescriptionChangeReport;
            set
            {
                Set(() => CooperativePrescriptionChangeReport, ref cooperativePrescriptionChangeReport, value);
            }
        }

        private CashReport coopCashProfitReport = new CashReport();

        public CashReport CoopCashProfitReport
        {
            get => coopCashProfitReport;
            set
            {
                Set(() => CoopCashProfitReport, ref coopCashProfitReport, value);
            }
        }

        private CashReport cashProfitReport = new CashReport();

        public CashReport CashProfitReport
        {
            get => cashProfitReport;
            set
            {
                Set(() => CashProfitReport, ref cashProfitReport, value);
            }
        }

        private PrescriptionProfitReport totalPrescriptionProfitReport = new PrescriptionProfitReport();

        public PrescriptionProfitReport TotalPrescriptionProfitReport
        {
            get => totalPrescriptionProfitReport;
            set
            {
                Set(() => TotalPrescriptionProfitReport, ref totalPrescriptionProfitReport, value);
            }
        }

        private PrescriptionDetailReport prescriptionDetailReportSum;
        private PrescriptionDetailReport prescriptionDetailReportSumMain;
        private PrescriptionDetailReport prescriptionCoopDetailReportSumMain;

        public PrescriptionDetailReport PrescriptionDetailReportSum
        {
            get => prescriptionDetailReportSum;
            set
            {
                Set(() => PrescriptionDetailReportSum, ref prescriptionDetailReportSum, value);
            }
        }

        public PrescriptionDetailReport PrescriptionDetailReportSumMain
        {
            get => prescriptionDetailReportSumMain;
            set
            {
                Set(() => PrescriptionDetailReportSumMain, ref prescriptionDetailReportSumMain, value);
            }
        }  

        public PrescriptionDetailReport PrescriptionCoopDetailReportSumMain
        {
            get => prescriptionCoopDetailReportSumMain;
            set
            {
                Set(() => PrescriptionCoopDetailReportSumMain, ref prescriptionCoopDetailReportSumMain, value);
            }
        }

        private CashDetailReport cashDetailReportSum;

        public CashDetailReport CashDetailReportSum
        {
            get => cashDetailReportSum;
            set
            {
                Set(() => CashDetailReportSum, ref cashDetailReportSum, value);
            }
        }

        private StockTakingDetailReport stockTakingDetailReportSum;

        public StockTakingDetailReport StockTakingDetailReportSum
        {
            get => stockTakingDetailReportSum;
            set
            {
                Set(() => StockTakingDetailReportSum, ref stockTakingDetailReportSum, value);
            }
        }

        private StockTakingOTCDetailReport stockTakingOTCDetailReportSum;

        public StockTakingOTCDetailReport StockTakingOTCDetailReportSum
        {
            get => stockTakingOTCDetailReportSum;
            set
            {
                Set(() => StockTakingOTCDetailReportSum, ref stockTakingOTCDetailReportSum, value);
            }
        }

        private TradeProfitDetailReport tradeDetailReportSum;

        public TradeProfitDetailReport TradeDetailReportSum
        {
            get => tradeDetailReportSum;
            set
            {
                Set(() => TradeDetailReportSum, ref tradeDetailReportSum, value);
            }
        }

        private TradeProfitDetailReport tradeDetailReportSumMain;

        public TradeProfitDetailReport TradeDetailReportSumMain
        {
            get => tradeDetailReportSumMain;
            set
            {
                Set(() => TradeDetailReportSumMain, ref tradeDetailReportSumMain, value);
            }
        }

        private PrescriptionDetailReports prescriptionDetailReportCollection;

        public PrescriptionDetailReports PrescriptionDetailReportCollection
        {
            get => prescriptionDetailReportCollection;
            set
            {
                Set(() => PrescriptionDetailReportCollection, ref prescriptionDetailReportCollection, value);
            }
        }

        private PrescriptionDetailReports prescriptionDetailReportCollectionALL;

        public PrescriptionDetailReports PrescriptionDetailReportCollectionALL
        {
            get => prescriptionDetailReportCollectionALL;
            set
            {
                Set(() => PrescriptionDetailReportCollectionALL, ref prescriptionDetailReportCollectionALL, value);
            }
        }

        private PrescriptionDetailReports prescriptionDetailReportCollectionChanged;

        public PrescriptionDetailReports PrescriptionDetailReportCollectionChanged
        {
            get => prescriptionDetailReportCollectionChanged;
            set
            {
                Set(() => PrescriptionDetailReportCollectionChanged, ref prescriptionDetailReportCollectionChanged, value);
            }
        }

        
        private PrescriptionDetailReports prescriptionCoopDetailReportCollection;

        public PrescriptionDetailReports PrescriptionCoopDetailReportCollection
        {
            get => prescriptionCoopDetailReportCollection;
            set
            {
                Set(() => PrescriptionCoopDetailReportCollection, ref prescriptionCoopDetailReportCollection, value);
            }
        }

        private PrescriptionDetailReports prescriptionCoopChangeDetailReportCollection;

        public PrescriptionDetailReports PrescriptionCoopChangeDetailReportCollection
        {
            get => prescriptionCoopChangeDetailReportCollection;
            set
            {
                Set(() => PrescriptionCoopChangeDetailReportCollection, ref prescriptionCoopChangeDetailReportCollection, value);
            }
        }

        private PrescriptionDetailReports prescriptionCoopChangeDetailReportCollectionChanged;

        public PrescriptionDetailReports PrescriptionCoopChangeDetailReportCollectionChanged
        {
            get => prescriptionCoopChangeDetailReportCollectionChanged;
            set
            {
                Set(() => PrescriptionCoopChangeDetailReportCollectionChanged, ref prescriptionCoopChangeDetailReportCollectionChanged, value);
            }
        }

        private CollectionViewSource prescriptionDetailReportViewSource;

        private CollectionViewSource PrescriptionDetailReportViewSource
        {
            get => prescriptionDetailReportViewSource;
            set
            {
                Set(() => PrescriptionDetailReportViewSource, ref prescriptionDetailReportViewSource, value);
            }
        }

        private ICollectionView prescriptionDetailReportView;

        public ICollectionView PrescriptionDetailReportView
        {
            get => prescriptionDetailReportView;
            private set
            {
                Set(() => PrescriptionDetailReportView, ref prescriptionDetailReportView, value);
            }
        }

        private PrescriptionDetailReport prescriptionDetailReportSelectItem;

        public PrescriptionDetailReport PrescriptionDetailReportSelectItem
        {
            get => prescriptionDetailReportSelectItem;
            set
            {
                Set(() => PrescriptionDetailReportSelectItem, ref prescriptionDetailReportSelectItem, value);
            }
        }

        private CashDetailReports cashDetailReportCollection = new CashDetailReports();

        public CashDetailReports CashDetailReportCollection
        {
            get => cashDetailReportCollection;
            set
            {
                Set(() => CashDetailReportCollection, ref cashDetailReportCollection, value);
            }
        }

        private CashDetailReport cashDetailReportSelectItem;

        public CashDetailReport CashDetailReportSelectItem
        {
            get => cashDetailReportSelectItem;
            set
            {
                Set(() => CashDetailReportSelectItem, ref cashDetailReportSelectItem, value);
            }
        }

        private CashDetailRecordReports cashDetailRecordReportCollection = new CashDetailRecordReports();

        public CashDetailRecordReports CashDetailRecordReportCollection
        {
            get => cashDetailRecordReportCollection;
            set
            {
                Set(() => CashDetailRecordReportCollection, ref cashDetailRecordReportCollection, value);
            }
        }

        private PrescriptionDetailMedicineRepots prescriptionDetailMedicineRepotCollection = new PrescriptionDetailMedicineRepots();

        public PrescriptionDetailMedicineRepots PrescriptionDetailMedicineRepotCollection
        {
            get => prescriptionDetailMedicineRepotCollection;
            set
            {
                Set(() => PrescriptionDetailMedicineRepotCollection, ref prescriptionDetailMedicineRepotCollection, value);
            }
        }

        private PrescriptionDetailMedicineRepot prescriptionDetailMedicineRepotSelectItem;

        public PrescriptionDetailMedicineRepot PrescriptionDetailMedicineRepotSelectItem
        {
            get => prescriptionDetailMedicineRepotSelectItem;
            set
            {
                Set(() => PrescriptionDetailMedicineRepotSelectItem, ref prescriptionDetailMedicineRepotSelectItem, value);
            }
        }

        private CashStockEntryReportEnum cashStockEntryReportEnum = CashStockEntryReportEnum.ExtraMoney;

        public CashStockEntryReportEnum CashStockEntryReportEnum
        {
            get => cashStockEntryReportEnum;
            set
            {
                Set(() => CashStockEntryReportEnum, ref cashStockEntryReportEnum, value);
            }
        }

        private DepositReportDataList _depositReportDataSumMain ;

        public DepositReportDataList DepositReportDataSumMain
        {
            get => _depositReportDataSumMain;
            set
            {
                Set(() => DepositReportDataSumMain, ref _depositReportDataSumMain, value);
            }
        }

        private ObservableCollection<DepositReportData> _depositDetailReportCollection = new ObservableCollection<DepositReportData>();

        public ObservableCollection<DepositReportData> DepositDetailReportCollection
        {
            get => _depositDetailReportCollection;
            set
            {
                Set(() => DepositDetailReportCollection, ref _depositDetailReportCollection, value);
            }
        }

        private DepositReportData _selectedDepositDetailReport;

        public DepositReportData SelectedDepositDetailReport
        {
            get => _selectedDepositDetailReport;
            set
            {
                Set(() => SelectedDepositDetailReport, ref _selectedDepositDetailReport, value);
            }
        }


        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(() => IsBusy, ref _isBusy, value);
            }
        }

        private string _busyContent;

        public string BusyContent
        {
            get => _busyContent;
            set
            {
                Set(() => BusyContent, ref _busyContent, value);
            }
        }

        private decimal? extraMoney;

        public decimal? ExtraMoney
        {
            get => extraMoney;
            set
            {
                Set(() => ExtraMoney, ref extraMoney, value);
            }
        }

        private int discountAmt;

        public int DiscountAmt
        {
            get => discountAmt;
            set
            {
                Set(() => DiscountAmt, ref discountAmt, value);
            }
        }

        private InventoryDifference inventoryDifference;

        public InventoryDifference InventoryDifference
        {
            get => inventoryDifference;
            set
            {
                Set(() => InventoryDifference, ref inventoryDifference, value);
            }
        }

        public DataSet Ds = new DataSet();
        DataTable ddd = new DataTable();
        DataTable sss = new DataTable();

        #endregion Variables

        #region Command

        public RelayCommand SelfPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CooperativePrescriptionSelectionChangedCommand { get; set; } 
        public RelayCommand CooperativePrescriptionChangeSelectionChangedCommand { get; set; }
        public RelayCommand CashDetailMouseDoubleClickCommand { get; set; }
        public RelayCommand CashSelectionChangedCommand { get; set; }
        public RelayCommand CashCoopSelectionChangedCommand { get; set; }
        public RelayCommand CashNotCoopSelectionChangedCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand CashDetailClickCommand { get; set; }
        public RelayCommand PrescriptionDetailClickCommand { get; set; }
        public RelayCommand PrescriptionDetailDoubleClickCommand { get; set; }
        public RelayCommand PrescriptionDetailMedicineDoubleClickCommand { get; set; }
        public RelayCommand PrintCashPerDayCommand { get; set; }
        public RelayCommand PrintPrescriptionProfitDetailCommand { get; set; }
        public RelayCommand StockTakingReportSelectionChangedCommand { get; set; }

        public RelayCommand AllDepositReportSelectionChangedCommand { get; set; }
        public RelayCommand DepositDetailClickCommand { get; set; }
        public RelayCommand DepositDetailDoubleClickCommand { get; set; }
        public RelayCommand StockTakingDetailClickCommand { get; set; }
        public RelayCommand StockTakingOTCReportSelectionChangedCommand { get; set; }
        public RelayCommand StockTakingOTCDetailClickCommand { get; set; } 
        public RelayCommand TradeProfitReportSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitDetailClickCommand { get; set; }
        public RelayCommand TradeProfitDetailDoubleClickCommand { get; set; }
        public RelayCommand TradeProfitDetailEmpClickCommand { get; set; }
        public RelayCommand ExtraMoneyReportSelectionChangedCommand { get; set; }
        public RelayCommand ExtraMoneyDetailClickCommand { get; set; } 

        public RelayCommand RewardReportSelectionChangedCommand { get; set; }
         

        public RelayCommand RewardDetailClickCommand { get; set; }
        public RelayCommand RewardDetailMedicineDoubleClickCommand { get; set; }
        public RelayCommand TradeChangeReportSelectionChangedCommand { get; set; }

        public RelayCommand RewardExcelCommand { get; set; }

        public RelayCommand SelfSelfPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfSlowPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfNormalPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfSelfPrescriptionChangeSelectionChangedCommand { get; set; }
        public RelayCommand SelfSlowPrescriptionChangeSelectionChangedCommand { get; set; }
        public RelayCommand SelfNormalPrescriptionChangeSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitIcomeReportSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitCostCostReportSelectionChangedCommand { get; set; }
        public RelayCommand SelfNormalIncomePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfNormalCostPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfNormalAllPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfSlowIncomePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfSlowCostPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfSlowAllPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfSelfIncomePrescriptionSelectionChangedCommand { get; set; }

        public RelayCommand SelfSelfCostPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfSelfAllPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CooperativeIncomePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CooperativeCostPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CooperativeAllPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitAllReportSelectionChangedCommand { get; set; }
        public RelayCommand AllPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand AllIncomePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand AllCostPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand AllAllPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand AllPrescriptionChangeSelectionChangedCommand { get; set; }

        public RelayCommand TradeProfitTicketReportSelectionChangedCommand { get; set; }
        public RelayCommand PrintTradeProfitDetailCommand{ get; set; }
        #endregion Command

        public NewTodayCashStockEntryReportViewModel()
        {
            SearchCommand = new RelayCommand(GetData);
            SelfPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionSelectionChangedAction);

            AllPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionAction);
            AllIncomePrescriptionSelectionChangedCommand = new RelayCommand(AllIncomePrescriptionSelectionChangedAction);
            AllCostPrescriptionSelectionChangedCommand = new RelayCommand(AllCostPrescriptionSelectionChangedAction);
            AllAllPrescriptionSelectionChangedCommand = new RelayCommand(AllAllPrescriptionSelectionChangedAction);

            SelfNormalPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionAction);
            SelfNormalIncomePrescriptionSelectionChangedCommand = new RelayCommand(SelfNormalIncomePrescriptionSelectionChangedAction);
            SelfNormalCostPrescriptionSelectionChangedCommand = new RelayCommand(SelfNormalCostPrescriptionSelectionChangedAction);
            SelfNormalAllPrescriptionSelectionChangedCommand = new RelayCommand(SelfNormalAllPrescriptionSelectionChangedAction);

            SelfSlowPrescriptionSelectionChangedCommand = new RelayCommand(SelfSlowPrescriptionSelectionChangedAction);
            SelfSlowIncomePrescriptionSelectionChangedCommand = new RelayCommand(SelfSlowIncomePrescriptionSelectionChangedAction);
            SelfSlowCostPrescriptionSelectionChangedCommand = new RelayCommand(SelfSlowCostPrescriptionSelectionChangedAction);
            SelfSlowAllPrescriptionSelectionChangedCommand = new RelayCommand(SelfSlowAllPrescriptionSelectionChangedAction);

            SelfSelfPrescriptionSelectionChangedCommand = new RelayCommand(SelfSelfPrescriptionSelectionChangedAction);
            SelfSelfIncomePrescriptionSelectionChangedCommand = new RelayCommand(SelfSelfIncomePrescriptionSelectionChangedAction);
            SelfSelfCostPrescriptionSelectionChangedCommand = new RelayCommand(SelfSelfCostPrescriptionSelectionChangedAction);
            SelfSelfAllPrescriptionSelectionChangedCommand = new RelayCommand(SelfSelfAllPrescriptionSelectionChangedAction);

            CooperativeIncomePrescriptionSelectionChangedCommand = new RelayCommand(CooperativeIncomePrescriptionSelectionChangedAction);
            CooperativeCostPrescriptionSelectionChangedCommand = new RelayCommand(CooperativeCostPrescriptionSelectionChangedAction);
            CooperativeAllPrescriptionSelectionChangedCommand = new RelayCommand(CooperativeAllPrescriptionSelectionChangedAction);
            CooperativePrescriptionSelectionChangedCommand = new RelayCommand(CooperativePrescriptionSelectionChangedAction);

            SelfNormalPrescriptionChangeSelectionChangedCommand = new RelayCommand(SelfNomalPrescriptionChangeSelectionChangedAction);
            SelfSlowPrescriptionChangeSelectionChangedCommand = new RelayCommand(SelfSlowPrescriptionChangeSelectionChangedAction);
            SelfSelfPrescriptionChangeSelectionChangedCommand = new RelayCommand(SelfSelfPrescriptionChangeSelectionChangedAction);
            AllPrescriptionChangeSelectionChangedCommand = new RelayCommand(AllPrescriptionChangeSelectionChangedAction);

            CooperativePrescriptionChangeSelectionChangedCommand = new RelayCommand(CooperativePrescriptionChangeSelectionChangedAction);
            CashSelectionChangedCommand = new RelayCommand(CashSelectionChangedAction);

            CashCoopSelectionChangedCommand = new RelayCommand(CashCoopSelectionChangedAction);
            CashNotCoopSelectionChangedCommand = new RelayCommand(CashNotCoopSelectionChangedAction);
            CashDetailClickCommand = new RelayCommand(CashDetailClickAction);
            
            PrescriptionDetailClickCommand = new RelayCommand(PrescriptionDetailClickAction);
            PrescriptionDetailDoubleClickCommand = new RelayCommand(PrescriptionDetailDoubleClickAction);
            PrescriptionDetailMedicineDoubleClickCommand = new RelayCommand(PrescriptionDetailMedicineDoubleClickAction);
           
            PrintCashPerDayCommand = new RelayCommand(PrintCashPerDayAction);
            PrintPrescriptionProfitDetailCommand = new RelayCommand(PrintPrescriptionProfitDetailAction);
            StockTakingReportSelectionChangedCommand = new RelayCommand(StockTakingReportSelectionChangedAction);

            AllDepositReportSelectionChangedCommand = new RelayCommand(AllDepositReportSelectionChangedAction);
            DepositDetailClickCommand = new RelayCommand(DepositDetailClickAction);
            DepositDetailDoubleClickCommand = new RelayCommand(DepositDetailDoubleClickAction);

            StockTakingDetailClickCommand = new RelayCommand(StockTakingDetailClickAction);
            CashDetailMouseDoubleClickCommand = new RelayCommand(CashDetailMouseDoubleClickAction);
            StockTakingOTCReportSelectionChangedCommand = new RelayCommand(StockTakingOTCReportSelectionChangedAction);
            StockTakingOTCDetailClickCommand = new RelayCommand(StockTakingOTCDetailClickAction);

            TradeProfitReportSelectionChangedCommand = new RelayCommand(TradeProfitReportSelectionChangedAction);
            TradeProfitIcomeReportSelectionChangedCommand = new RelayCommand(TradeProfitIncomeReportSelectionChangedAction);
            TradeProfitCostCostReportSelectionChangedCommand = new RelayCommand(TradeProfitCostCostReportSelectionChangedAction);
            TradeProfitAllReportSelectionChangedCommand = new RelayCommand(TradeProfitAllReportSelectionChangedAction);
            TradeProfitTicketReportSelectionChangedCommand = new RelayCommand(TradeProfitTicketReportSelectionChangedAction);

            TradeProfitDetailClickCommand = new RelayCommand(TradeProfitDetailClickAction);
            TradeProfitDetailDoubleClickCommand = new RelayCommand(TradeProfitDetailDoubleClickAction);
            TradeProfitDetailEmpClickCommand = new RelayCommand(TradeProfitDetailEmpClickAction);
            ExtraMoneyReportSelectionChangedCommand = new RelayCommand(ExtraMoneyReportSelectionChangedAction);
            ExtraMoneyDetailClickCommand = new RelayCommand(ExtraMoneyDetailClickAction);
            RewardReportSelectionChangedCommand = new RelayCommand(RewardReportSelectionChangedAction);
            RewardDetailClickCommand = new RelayCommand(RewardDetailClickAction);
            RewardDetailMedicineDoubleClickCommand = new RelayCommand(RewardDetailMedicineDoubleClickAction);
            TradeChangeReportSelectionChangedCommand = new RelayCommand(TradeChangeReportSelectionChangedAction);
            RewardExcelCommand = new RelayCommand(RewardExcelAction);
            PrintTradeProfitDetailCommand = new RelayCommand(PrintTradeProfitDetailAction);
            GetData(); 
        }

        private void DepositDetailDoubleClickAction()
        {
            if (SelectedDepositDetailReport is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionDetailMedicineRepotCollection.Clear();
            PrescriptionService.ShowPrescriptionEditWindow(SelectedDepositDetailReport.PremasID);
        }

        private void DepositDetailClickAction()
        {
            if (SelectedDepositDetailReport is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionDetailMedicineRepotCollection.GerDataById(SelectedDepositDetailReport.PremasID);
        }

        private void RewardExcelAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("sDate", StartDate));
            parameters.Add(new SqlParameter("eDate", EndDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[RewardDetailRecordByDateExcel]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "績效明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "-" + EndDate.ToString("yyyyMMdd") + "績效明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("績效明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                var col1 = ws.Column("A");
                col1.Width = 10;
                var col2 = ws.Column("B");
                col2.Width = 15;
                var col3 = ws.Column("C");
                col3.Width = 25;
                var col4 = ws.Column("D");
                col4.Width = 25;
                var col5 = ws.Column("E");
                col5.Width = 10;

                ws.Cell(1, 1).Value = "績效明細";
                ws.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "績效人員";
                ws.Cell("B2").Value = "銷售時間";
                ws.Cell("C2").Value = "商品代碼";
                ws.Cell("D2").Value = "商品名稱";
                ws.Cell("E2").Value = "績效";

                var rangeWithData = ws.Cell(3, 1).InsertData(result.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                wb.SaveAs(fdlg.FileName);
            }
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = (fdlg.FileName);
                myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.StartInfo.Verb = "print";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void PrintTradeProfitDetailAction()
        {
            PrintService.PrintTradeProfitDetail(StartDate,EndDate);
        }
         
        private void CashDetailMouseDoubleClickAction()
        {
            PrescriptionService.ShowPrescriptionEditWindow(CashDetailReportSelectItem.Id);
        }

        private void TradeProfitDetailEmpClickAction()
        {
            if (TradeProfitDetailEmpReportSelectItem is null)
            {
                TradeProfitDetailEmpRecordReportCollection.Clear();
                return;
            }
            TradeProfitDetailEmpRecordReportCollection.GetDateByDate(TradeProfitDetailEmpReportSelectItem.TraMas_Cashier, StartDate, EndDate);
        }

        #region Action

        private void PrintPrescriptionProfitDetailAction()
        {
            PrintService.PrintPrescriptionProfitDetailAction(StartDate,EndDate, PrescriptionDetailReportView); 
        }
         
        private void PrintCashPerDayAction()
        {
            PrintService.PrintCashPerDayAction(StartDate, EndDate, new CashReport()); 
        }

        private void PrescriptionDetailMedicineDoubleClickAction()
        {
            if (PrescriptionDetailMedicineRepotSelectItem is null) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { PrescriptionDetailMedicineRepotSelectItem.Id, "0" }, "ShowProductDetail"));
        }

        private void RewardDetailMedicineDoubleClickAction()
        {
            if (RewardDetailMedicineReportSelectItem is null) return;

            string TradeID = RewardDetailMedicineReportSelectItem.MasterID.ToString();

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", TradeID));
            parameters.Add(new SqlParameter("CustomerID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", ""));
            parameters.Add(new SqlParameter("eDate", ""));
            parameters.Add(new SqlParameter("sInvoice", ""));
            parameters.Add(new SqlParameter("eInvoice", ""));
            parameters.Add(new SqlParameter("flag", "1"));
            parameters.Add(new SqlParameter("ShowIrregular", DBNull.Value));
            parameters.Add(new SqlParameter("ShowReturn", DBNull.Value));
            parameters.Add(new SqlParameter("Cashier", -1));
            parameters.Add(new SqlParameter("ProID", DBNull.Value));
            parameters.Add(new SqlParameter("ProName", DBNull.Value));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            result.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in result.Rows)
            {
                string ogTransTime = dr["TraMas_ChkoutTime"].ToString();
                DateTime dt = DateTime.Parse(ogTransTime);
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                dr["TransTime_Format"] = dt.ToString("yyy/MM/dd", culture);
            }
            DataRow masterRow = result.Rows[0];

            ProductTransactionDetail ptd = new ProductTransactionDetail(masterRow, result);

            ptd.ShowDialog();
            ptd.Activate();
        }

        private void PrescriptionDetailDoubleClickAction()
        {
            if (PrescriptionDetailReportSelectItem is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionDetailMedicineRepotCollection.Clear();
            PrescriptionService.ShowPrescriptionEditWindow(PrescriptionDetailReportSelectItem.Id);
        }

        private void TradeProfitDetailDoubleClickAction()
        {
            if (TradeProfitDetailReportSelectItem is null)
            {
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", TradeProfitDetailReportSelectItem.Id));
            parameters.Add(new SqlParameter("CustomerID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", ""));
            parameters.Add(new SqlParameter("eDate", ""));
            parameters.Add(new SqlParameter("sInvoice", ""));
            parameters.Add(new SqlParameter("eInvoice", ""));
            parameters.Add(new SqlParameter("flag", "1"));
            parameters.Add(new SqlParameter("ShowIrregular", DBNull.Value));
            parameters.Add(new SqlParameter("ShowReturn", DBNull.Value));
            parameters.Add(new SqlParameter("Cashier", -1));
            parameters.Add(new SqlParameter("ProID", DBNull.Value));
            parameters.Add(new SqlParameter("ProName", DBNull.Value));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            DataRow masterRow = result.Rows[0];
            result.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in result.Rows)
            {
                string ogTransTime = dr["TraMas_ChkoutTime"].ToString();
                DateTime dt = DateTime.Parse(ogTransTime);
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                dr["TransTime_Format"] = dt.ToString("yyy/MM/dd", culture);
            }
            ProductTransactionDetail ptd = new ProductTransactionDetail(masterRow, result);

            ptd.ShowDialog();
            ptd.Activate();
        }

        private void PrescriptionDetailClickAction()
        {
            if (PrescriptionDetailReportSelectItem is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionDetailMedicineRepotCollection.GerDataById(PrescriptionDetailReportSelectItem.Id);
        }

        private void CashDetailClickAction()
        {
            if (CashDetailReportSelectItem is null)
            {
                CashDetailRecordReportCollection.Clear();
                return;
            }
            CashDetailRecordReportCollection.GetDateByDate(CashDetailReportSelectItem.Id, StartDate, EndDate);
        }

        private void TradeProfitDetailClickAction()
        {
            if (TradeProfitDetailReportSelectItem is null)
            {
                TradeProfitDetailRecordReportCollection.Clear();
                return;
            }
            TradeProfitDetailRecordReportCollection.GetDateByDate(TradeProfitDetailReportSelectItem.Id, StartDate, EndDate);
        }

        private void ExtraMoneyDetailClickAction()
        {
            if (ExtraMoneyDetailReportSelectItem is null)
            {
                ExtraMoneyDetailRecordReportCollection.Clear();
                return;
            }
            ExtraMoneyDetailRecordReportCollection.GetDateByDate(ExtraMoneyDetailReportSelectItem.Name, StartDate, EndDate);
        }

        private void RewardDetailClickAction()
        {
            if (RewardDetailReportSelectItem is null)
            {
                RewardDetailRecordReportCollection.Clear();
                return;
            }
            RewardDetailRecordReportCollection.GetDateByDate(RewardDetailReportSelectItem.TraDet_RewardPersonnel, StartDate, EndDate);
        }

        private void StockTakingDetailClickAction()
        {
            if (StockTakingDetailReportSelectItem is null)
            {
                StockTakingDetailRecordReportCollection.Clear();
                return;
            }
            StockTakingDetailRecordReportCollection.GetDateByDate(StockTakingDetailReportSelectItem.InvRecSourceID, StartDate, EndDate, StockTakingDetailReportSelectItem.Type, StockTakingDetailReportSelectItem.Time);
        }

        private void StockTakingOTCDetailClickAction()
        {
            if (StockTakingOTCDetailReportSelectItem is null)
            {
                StockTakingOTCDetailRecordReportCollection.Clear();
                return;
            }
            StockTakingOTCDetailRecordReportCollection.GetDateByDate(StockTakingOTCDetailReportSelectItem.InvRecSourceID, StartDate, EndDate, StockTakingOTCDetailReportSelectItem.Type, StockTakingOTCDetailReportSelectItem.Time);
        }

        private void SetVisAllCollapsed()
        {
            CostVis = Visibility.Collapsed;
            IncomeVis = Visibility.Collapsed;
            ProfitVis = Visibility.Collapsed;
            TicketVis = Visibility.Collapsed;
            ChangeOTCVis = Visibility.Collapsed;
        }

        private void TradeProfitCostCostReportSelectionChangedAction()
        {
            SetVisAllCollapsed();
            CostVis = Visibility.Visible; 
            ChangeOTCVis = Visibility.Visible;
            TradeProfitReportSelectionChangedAction();
        }

        private void TradeProfitIncomeReportSelectionChangedAction()
        {
            SetVisAllCollapsed();
            IncomeVis = Visibility.Visible; 
            TicketVis = Visibility.Visible;
            ChangeOTCVis = Visibility.Visible;
            TradeProfitReportSelectionChangedAction();
        }

        private void TradeProfitAllReportSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            TicketVis = Visibility.Visible;
            ChangeOTCVis = Visibility.Visible;
            TradeProfitReportSelectionChangedAction();
        }

        private void TradeProfitTicketReportSelectionChangedAction()
        {
            SetVisAllCollapsed();
            TicketVis = Visibility.Visible;
            ChangeOTCVis = Visibility.Visible;
            TradeProfitReportSelectionChangedAction();
            TradeProfitDetailReportViewSource.Filter += TicketFilter;
        }

        private void TradeProfitReportSelectionChangedAction()
        {
            ResetTradeProfitUI();
            SumOTCReport("0");
        }
         
        private void TradeProfitReportSelectionChangedActionMain()
        {
            ResetTradeProfitUI();
            SumOTCReportMain(); 
        }

        private void ResetTradeProfitUI()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.TradeProfit;
            ChangeVis = Visibility.Collapsed;

            TradeProfitDetailReportViewSource = new CollectionViewSource { Source = TradeProfitDetailReportCollection };
            TradeProfitDetailReportView = TradeProfitDetailReportViewSource.View;
            TradeProfitDetailEmpReportViewSource = new CollectionViewSource { Source = TradeProfitDetailEmpReportCollection };

            TradeProfitDetailEmpReportView = TradeProfitDetailEmpReportViewSource.View;
            //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
            //SumStockTakingDetailReport();
            TradeChangeSelectItem = "全部";
            TradeProfitDetailReportViewSource.Filter += OTCChangeFilter;

            TradeDetailCount = TradeProfitDetailReportCollection.Count();
            TradeEmpDetailCount = TradeProfitDetailEmpReportCollection.Count();
            EmpProfit = TradeProfitDetailEmpReportCollection.Sum(e => e.Profit);
            IsBusy = false;
            
        }

        private void TradeChangeReportSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            ChangeOTCVis = Visibility.Collapsed;

            CashStockEntryReportEnum = CashStockEntryReportEnum.TradeProfit;

            ChangeVis = Visibility.Visible;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            { 
                var StringCopy = new List<string>() { };
                foreach (var r in TradeProfitDetailReportCollectionChanged)
                {
                    StringCopy.Add(r.TypeId);
                }
                var DistinctItems = StringCopy.Select(x => x).Distinct();
                ChangeString = new List<string>() { "全部" };
                foreach (var item in DistinctItems)
                {
                    ChangeString.Add(item);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                TradeProfitDetailReportViewSource = new CollectionViewSource { Source = TradeProfitDetailReportCollectionChanged };
                TradeProfitDetailReportView = TradeProfitDetailReportViewSource.View;
                TradeProfitDetailEmpReportViewSource = new CollectionViewSource { Source = TradeProfitDetailEmpReportCollection };

                TradeProfitDetailEmpReportView = TradeProfitDetailEmpReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumStockTakingDetailReport();
                TradeChangeSelectItem = "全部";
                TradeProfitDetailReportViewSource.Filter += OTCChangeFilter;
                SumOTCReportMainChanged();
                TradeDetailCount = TradeProfitDetailReportCollectionChanged.Count();
                TradeEmpDetailCount = TradeProfitDetailEmpReportCollection.Count();
                EmpProfit = TradeProfitDetailEmpReportCollection.Sum(e => e.Profit);
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void TradeChangeReportSelectionChangedActionMain()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.TradeProfit;

            ChangeVis = Visibility.Visible; 
            var StringCopy = new List<string>() { };
            foreach (var r in TradeProfitDetailReportCollectionChanged)
            {
                StringCopy.Add(r.TypeId);
            }
            var DistinctItems = StringCopy.Select(x => x).Distinct();
            ChangeString = new List<string>() { "全部" };
            foreach (var item in DistinctItems)
            {
                ChangeString.Add(item);
            }

            TradeProfitDetailReportViewSource = new CollectionViewSource { Source = TradeProfitDetailReportCollectionChanged };
            TradeProfitDetailReportView = TradeProfitDetailReportViewSource.View;
            TradeProfitDetailEmpReportViewSource = new CollectionViewSource { Source = TradeProfitDetailEmpReportCollection };

            TradeProfitDetailEmpReportView = TradeProfitDetailEmpReportViewSource.View;
            TradeChangeSelectItem = "全部";
            TradeProfitDetailReportViewSource.Filter += OTCChangeFilter;
            SumOTCReportChangeMain("1");
            TradeDetailCount = TradeProfitDetailReportCollection.Count();
            TradeEmpDetailCount = TradeProfitDetailEmpReportCollection.Count();
            EmpProfit = TradeProfitDetailEmpReportCollection.Sum(e => e.Profit);
            
        }

        private void AllDepositReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.Deposit;
        }

        private void StockTakingReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.StockTaking;
             
            var CashCoopStringCopy = new List<string>() { };
            foreach (var r in StockTakingDetailReportCollection)
            {
                CashCoopStringCopy.Add(r.Type);
            }
            var DistinctItems = CashCoopStringCopy.Select(x => x).Distinct();
            StockTakingString = new List<string>() { "全部" };
            foreach (var item in DistinctItems)
            {
                StockTakingString.Add(item);
            }

            StockTakingDetailReportViewSource = new CollectionViewSource { Source = StockTakingDetailReportCollection };
            StockTakingDetailReportView = StockTakingDetailReportViewSource.View;
            StockTakingSelectItem = "全部";
            StockTakingDetailReportViewSource.Filter += StockTakingDetailFilter;

            SumStockTakingDetailReport();
            StockDetailCount = StockTakingDetailReportCollection.Count();
            
        }

        private void StockTakingOTCReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.OTCStockTaking;

            var CashCoopStringCopy = new List<string>() { };
            foreach (var r in StockTakingOTCDetailReportCollection)
            {
                CashCoopStringCopy.Add(r.Type);
            }
            var DistinctItems = CashCoopStringCopy.Select(x => x).Distinct();
            StockTakingOTCString = new List<string>() { "全部" };
            foreach (var item in DistinctItems)
            {
                StockTakingOTCString.Add(item);
            }

            MainWindow.ServerConnection.CloseConnection();

            StockTakingOTCDetailReportViewSource = new CollectionViewSource { Source = StockTakingOTCDetailReportCollection };
            StockTakingOTCDetailReportView = StockTakingOTCDetailReportViewSource.View;
            StockTakingOTCSelectItem = "全部";
            StockTakingOTCDetailReportViewSource.Filter += StockTakingOTCDetailFilter;
            SumStockTakingOTCDetailReport();
            StockOTCDetailCount = StockTakingOTCDetailReportCollection.Count();
            
        }

        private void CashSelectionChangedAction()
        {
            
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                CashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
                CashDetailReportCollection.GetDataByDate("1", StartDate, EndDate);
                MainWindow.ServerConnection.CloseConnection();
                var CashCoopStringCopy = new List<string>() { };
                foreach (var r in CashDetailReportCollection)
                {
                    CashCoopStringCopy.Add(r.Ins_Name);
                }
                var DistinctItems = CashCoopStringCopy.Select(x => x).Distinct();
                CashCoopString = new List<string>() { "全部" };
                foreach (var item in DistinctItems)
                {
                    CashCoopString.Add(item);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                CashDetailReportViewSource = new CollectionViewSource { Source = CashDetailReportCollection };
                CashDetailReportView = CashDetailReportViewSource.View;
                CashCoopSelectItem = "全部";
                CashDetailReportViewSource.Filter += CashCoopFilter;

                SumCashDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();

            CooperativePrescriptionSelectedItem = null; 
            StockTakingSelectedItem = null;
        }

        private void CashCoopSelectionChangedAction()
        {
           
            CashCoopVis = Visibility.Collapsed;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                CashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
                CashDetailReportCollection = new CashDetailReports("0", StartDate, EndDate);
                MainWindow.ServerConnection.CloseConnection();
                var CashCoopStringCopy = new List<string>() { };
                foreach (var r in CashDetailReportCollection)
                {
                    CashCoopStringCopy.Add(r.Ins_Name);
                }
                var DistinctItems = CashCoopStringCopy.Select(x => x).Distinct();
                CashCoopString = new List<string>() { "全部" };
                foreach (var item in DistinctItems)
                {
                    CashCoopString.Add(item);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                CashDetailReportViewSource = new CollectionViewSource { Source = CashDetailReportCollection };
                CashDetailReportView = CashDetailReportViewSource.View;
                CashCoopSelectItem = "全部";
                CashDetailReportViewSource.Filter += CashCoopFilter;
                SumCashDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();

            CooperativePrescriptionSelectedItem = null; 
            StockTakingSelectedItem = null;
        }

        private void CashNotCoopSelectionChangedAction()
        {
          
            CashCoopVis = Visibility.Visible;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                CashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
                CashDetailReportCollection = new CashDetailReports("1", StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
                var CashCoopStringCopy = new List<string>() { };
                foreach (var r in CashDetailReportCollection)
                {
                    CashCoopStringCopy.Add(r.Ins_Name);
                }
                var DistinctItems = CashCoopStringCopy.Select(x => x).Distinct();
                CashCoopString = new List<string>() { "全部" };
                foreach (var item in DistinctItems)
                {
                    CashCoopString.Add(item);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                CashDetailReportViewSource = new CollectionViewSource { Source = CashDetailReportCollection };
                CashDetailReportView = CashDetailReportViewSource.View;
                CashCoopSelectItem = "全部";
                CashDetailReportViewSource.Filter += CashCoopFilter;
                SumCashDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();

            CooperativePrescriptionSelectedItem = null; 
            StockTakingSelectedItem = null;
        }

        private void CooperativeCostPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Collapsed;
            ProfitVis = Visibility.Collapsed;
            CooperativePrescriptionSelectionChangedAction();
        }

        private void CooperativeAllPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            CooperativePrescriptionSelectionChangedAction();
        }

        private void CooperativeIncomePrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Collapsed;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Collapsed;
            CooperativePrescriptionSelectionChangedAction();
        }

        private void CooperativePrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Visible;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
             
            var CoopStringCopy = new List<string>() { };
            foreach (var r in PrescriptionCoopDetailReportCollection)
            {
                CoopStringCopy.Add(r.InsName);
            }
            var DistinctItems = CoopStringCopy.Select(x => x).Distinct();
            CoopString = new List<string>() { "全部" };
            foreach (var item in DistinctItems)
            {
                CoopString.Add(item);
            }

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionCoopDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部";
            AdjustCaseSelectItem = "全部";

            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumCoopPrescriptionDetailReport();
             
            StockTakingSelectedItem = null;
        }

        private void SelfPrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            MainWindow.ServerConnection.OpenConnection();
            BusyContent = "報表查詢中";
            PrescriptionDetailReportCollection = new PrescriptionDetailReports(sss);
            MainWindow.ServerConnection.CloseConnection();

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部";
            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumPrescriptionDetailReport();

            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void CooperativePrescriptionSelectionChangedActionMain()
        {
            CoopVis = Visibility.Visible;
             
            var CoopStringCopy = new List<string>() { };
            foreach (var r in PrescriptionCoopDetailReportCollection)
            {
                CoopStringCopy.Add(r.InsName);
            }
            var DistinctItems = CoopStringCopy.Select(x => x).Distinct();
            CoopString = new List<string>() { "全部" };
            foreach (var item in DistinctItems)
            {
                CoopString.Add(item);
            }
             

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionCoopDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部";
            
            StockTakingSelectedItem = null;
        }

        private void SelfPrescriptionSelectionChangedActionMain()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            MainWindow.ServerConnection.OpenConnection();

            PrescriptionDetailReportCollection = new PrescriptionDetailReports(sss);
            MainWindow.ServerConnection.CloseConnection();

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "ZZZZZ";
            AdjustCaseSelectItem = "全部";
            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumPrescriptionDetailMain();

            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }
         

        /******* 各種箋明細 *******/

        private void SelfNormalAllPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            AdjustCaseSelectItem = "一般箋";
            SelfPrescriptionAction();
        }

        private void SelfNormalCostPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Collapsed;
            ProfitVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "一般箋";
            SelfPrescriptionAction();
        }

        private void SelfNormalIncomePrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Collapsed;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Collapsed; 
            AdjustCaseSelectItem = "一般箋";
            SelfPrescriptionAction();
        }

        private void SelfSelfAllPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            AdjustCaseSelectItem = "自費調劑";
            SelfPrescriptionAction();
        }

        private void SelfSelfCostPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Collapsed;
            ProfitVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "自費調劑";
            SelfPrescriptionAction();
        }

        private void SelfSelfIncomePrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Collapsed;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "自費調劑";
            SelfPrescriptionAction();
        }

        private void SelfSlowAllPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            AdjustCaseSelectItem = "慢箋";
            SelfSlowPrescriptionSelectionChangedAction();
        }

        private void SelfSlowCostPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Collapsed;
            ProfitVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "慢箋";
            SelfPrescriptionAction();
        }

        private void SelfSlowIncomePrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Collapsed;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "慢箋";
            SelfPrescriptionAction();
        }

        private void AllAllPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            AdjustCaseSelectItem = "全部";
            AllPrescriptionAction();
        }

        private void AllCostPrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Collapsed;
            ProfitVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "慢箋";
            AllPrescriptionAction();
        }

        private void AllIncomePrescriptionSelectionChangedAction()
        {
            CostVis = Visibility.Collapsed;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "慢箋";
            AllPrescriptionAction();
        }
         
        private void AllPrescriptionAction()
        {
            ResetPrescriptionUI(); 
            SumPrescriptionDetailReportALL(); 
        }
         
        private void SelfPrescriptionAction()
        { 
            ResetPrescriptionUI(); 
            SumPrescriptionDetailReport(); 
        }

        private void ResetPrescriptionUI()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部"; 
            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;


            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void SelfSlowPrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                MainWindow.ServerConnection.OpenConnection();
                PrescriptionDetailReportCollection = new PrescriptionDetailReports(sss);
                MainWindow.ServerConnection.CloseConnection();
                //PrescriptionDetailReportCollection = new PrescriptionDetailReports("1", StartDate, EndDate);
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                CoopSelectItem = "全部";
                AdjustCaseSelectItem = "慢箋";
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void SelfSelfPrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                CoopSelectItem = "全部";
                AdjustCaseSelectItem = "自費調劑";
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        /******* 各種箋調整明細 *******/

        private void SelfNomalPrescriptionChangeSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            CoopVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "一般箋";

            RefreshPrescriptionReportView();


        }
         
        private void AllPrescriptionChangeSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            CoopVis = Visibility.Collapsed;

            AdjustCaseSelectItem = "全部";

            RefreshPrescriptionReportView();
        }

        private void SelfSlowPrescriptionChangeSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            CoopVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "慢箋";
            RefreshPrescriptionReportView();
        }
         
        private void SelfSelfPrescriptionChangeSelectionChangedAction()
        {
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            CoopVis = Visibility.Collapsed;
            AdjustCaseSelectItem = "自費調劑";
            RefreshPrescriptionReportView();
        }

        private void RefreshPrescriptionReportView()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollectionChanged };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部";
            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumPrescriptionChangedDetailReport();

            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        /******* 各種箋調整明細 *******/
        /******* 各種箋明細 *******/

        private void SelfPrescriptionChangeSelectionChangedActionMain()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
             
            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollectionChanged };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部";
            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumPrescriptionChangeDetailMain();

            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void CooperativePrescriptionChangeSelectionChangedAction()
        {
            CoopVis = Visibility.Visible;
            CostVis = Visibility.Visible;
            IncomeVis = Visibility.Visible;
            ProfitVis = Visibility.Visible;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            
            var CoopStringCopy = new List<string>() { };
            foreach (var r in PrescriptionCoopChangeDetailReportCollectionChanged)
            {
                CoopStringCopy.Add(r.InsName);
            }
            var DistinctItems = CoopStringCopy.Select(x => x).Distinct();
            CoopString = new List<string>() { "全部" };
            foreach (var item in DistinctItems)
            {
                CoopString.Add(item);
            }
            

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionCoopChangeDetailReportCollectionChanged };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部";
            AdjustCaseSelectItem = "全部";
            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumCoopChangePrescriptionDetailReport();
             
            StockTakingSelectedItem = null;
        }

        private void CooperativePrescriptionChangeSelectionChangedActionMain()
        {
            CoopVis = Visibility.Visible;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
             
            var CoopStringCopy = new List<string>() { };
            foreach (var r in PrescriptionCoopChangeDetailReportCollectionChanged)
            {
                CoopStringCopy.Add(r.InsName);
            }
            var DistinctItems = CoopStringCopy.Select(x => x).Distinct();
            CoopString = new List<string>() { "全部" };
            foreach (var item in DistinctItems)
            {
                CoopString.Add(item);
            } 

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionCoopChangeDetailReportCollectionChanged };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
            CoopSelectItem = "全部";

            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumCoopChangePrescriptionDetailReport();
             
            StockTakingSelectedItem = null;
        }

        private void ExtraMoneyReportSelectionChangedAction()
        {
            ExtraMoneyDetailReportViewSource = new CollectionViewSource { Source = ExtraMoneyDetailReportCollection };
            ExtraMoneyDetailReportView = ExtraMoneyDetailReportViewSource.View;

            CashStockEntryReportEnum = CashStockEntryReportEnum.ExtraMoney;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                ExtraMoneyDetailReportCollection = new ExtraMoneyDetailReports(StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                ExtraMoneyDetailReportViewSource = new CollectionViewSource { Source = ExtraMoneyDetailReportCollection };
                ExtraMoneyDetailReportView = ExtraMoneyDetailReportViewSource.View;
                ExtraMoneyDetailCount = ExtraMoneyDetailReportCollection.Count();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync(); 
        }

        private void RewardReportSelectionChangedAction()
        {
            RewardDetailReportViewSource = new CollectionViewSource { Source = RewardDetailReportCollection };
            RewardDetailReportView = RewardDetailReportViewSource.View;

            CashStockEntryReportEnum = CashStockEntryReportEnum.Reward;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                RewardDetailReportCollection = new RewardDetailReports("0", StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                RewardDetailReportViewSource = new CollectionViewSource { Source = RewardDetailReportCollection };
                RewardDetailReportView = RewardDetailReportViewSource.View; 
                RewardDetailCount = RewardDetailReportCollection.Count();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
         
        private void GetData()
        {
            PrescriptionDetailReportSumMain = new PrescriptionDetailReport();
            PrescriptionCoopDetailReportSumMain = new PrescriptionDetailReport();
            TradeDetailReportSum = new TradeProfitDetailReport();
            StockTakingDetailReportSum = new StockTakingDetailReport();
            StockTakingOTCDetailReportSum = new StockTakingOTCDetailReport();

            BusyContent = "報表查詢中";

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", StartDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", EndDate);
            Ds = MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[TodayCashStockEntryReport]", parameterList);
            MainWindow.ServerConnection.CloseConnection();

            ddd = new DataTable();
            sss = new DataTable();
            ddd.Merge(Ds.Tables[0]);
            ddd.Merge(Ds.Tables[2]);
            ddd.Merge(Ds.Tables[4]);
            ddd.Merge(Ds.Tables[6]);
            sss.Merge(Ds.Tables[2]);
            sss.Merge(Ds.Tables[4]);
            sss.Merge(Ds.Tables[6]);
            PrescriptionDetailReportCollectionALL = new PrescriptionDetailReports(ddd); 

            DataTable ALLCHANGE = new DataTable();
            ALLCHANGE.Merge(Ds.Tables[1]);
            ALLCHANGE.Merge(Ds.Tables[3]);
            ALLCHANGE.Merge(Ds.Tables[5]);
            ALLCHANGE.Merge(Ds.Tables[7]);
            PrescriptionDetailReportCollectionChanged = new PrescriptionDetailReports(ALLCHANGE);
             
            TradeProfitDetailReportCollection = new TradeProfitDetailReports(Ds.Tables[10]);
            TradeProfitDetailEmpReportCollection = new TradeProfitDetailEmpReports(Ds.Tables[13]);
            TradeProfitDetailReportCollectionChanged = new TradeProfitDetailReports(Ds.Tables[11]);
            PrescriptionCoopChangeDetailReportCollectionChanged = new PrescriptionDetailReports(Ds.Tables[1]);
           
           
            PrescriptionDetailReportCollection = new PrescriptionDetailReports(Ds.Tables[4]);
            StockTakingOTCDetailReportCollection = new StockTakingOTCDetailReports(Ds.Tables[9]);
            StockTakingDetailReportCollection = new StockTakingDetailReports(Ds.Tables[8]);

            PrescriptionCoopDetailReportCollection = new PrescriptionDetailReports(Ds.Tables[0]);
            PrescriptionCoopDetailReportSumMain.CoopCount = PrescriptionCoopDetailReportCollection.Count();
            PrescriptionCoopDetailReportSumMain.CoopMeduse = (int)PrescriptionCoopDetailReportCollection.Sum(s => s.Meduse);
            PrescriptionCoopDetailReportSumMain.CoopIncome = (int)PrescriptionCoopDetailReportCollection.Sum(s => s.MedicalPoint) + (int)PrescriptionCoopDetailReportCollection.Sum(s => s.MedicalServicePoint) + (int)PrescriptionCoopDetailReportCollection.Sum(s => s.PaySelfPoint);


            DataTable depositTable = Ds.Tables[14];
            DepositReportDataSumMain = new DepositReportDataList(depositTable);

            foreach (DataRow row in depositTable.Rows)
            {
                DepositDetailReportCollection.Add(new DepositReportData(row));
            }
            
             
            TradeProfitReportSelectionChangedActionMain();
            TradeChangeReportSelectionChangedActionMain();
            
            CooperativePrescriptionSelectionChangedActionMain();
            SelfPrescriptionChangeSelectionChangedActionMain();
            CooperativePrescriptionChangeSelectionChangedActionMain();
            StockTakingOTCReportSelectionChangedAction();
            StockTakingReportSelectionChangedAction();
            RewardReportCollection = new RewardReports(StartDate, EndDate);
            CalculateTotalRewardProfit();

            
            TradeProfitAllReportSelectionChangedAction();
            SelfPrescriptionSelectionChangedActionMain();

            SumOTCProfit();
            SumMedProfit();
            SumAllProfit();
        }

        private void SumCashDetailReport()
        {
            CashDetailReportSum = new CashDetailReport();
            CashDetailReportSum.CusName = "總計";

            var tempCollection = CashDetailReportCollection.Where(p => true);

            if (CashCoopSelectItem != "全部")
            {
                tempCollection = CashDetailReportCollection.Where(p => (p.Ins_Name == CashCoopSelectItem));
            }
            else
            {
                tempCollection = CashDetailReportCollection;
            }

            CashDetailReportSum.SumCashDetail(tempCollection);
           
        }

        private void SumPrescriptionDetailReport()
        {
            var tempCollection = GetPrescriptionDetailReportsByType(PrescriptionDetailReportCollection);
            
            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection);
        }

        private void SumPrescriptionDetailReportALL()
        {
            var tempCollection = GetPrescriptionDetailReportsByType(PrescriptionDetailReportCollectionALL);
            
            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection);
             
        }

        private void SumPrescriptionChangedDetailReport()
        { 
            var tempCollection =  GetPrescriptionDetailReportsByType(PrescriptionDetailReportCollectionChanged);

            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection);
            
        }

        private void SumCoopPrescriptionDetailReport()
        {  
            var tempCollection = GetPrescriptionDetailReportsByType(PrescriptionCoopDetailReportCollection);
             
            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection); 
        }

        private void SumCoopChangePrescriptionDetailReport()
        {
            var tempCollection =
                GetPrescriptionDetailReportsByType(PrescriptionCoopChangeDetailReportCollectionChanged);

            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection);
             
            PrescriptionCoopDetailReportSumMain.CoopChange = (decimal)PrescriptionDetailReportSum.MedicalPoint + (decimal)PrescriptionDetailReportSum.MedicalServicePoint + (decimal)PrescriptionDetailReportSum.PaySelfPoint + PrescriptionDetailReportSum.Meduse;
            PrescriptionCoopDetailReportSumMain.CoopProfit = (int)((decimal)PrescriptionCoopDetailReportSumMain.CoopIncome + 
                                                                   PrescriptionCoopDetailReportSumMain.CoopMeduse + 
                                                                   PrescriptionCoopDetailReportSumMain.CoopChange + 
                                                                   DepositReportDataSumMain.CooperativeDeposit);
        }

        private IEnumerable<PrescriptionDetailReport> GetPrescriptionDetailReportsByType(PrescriptionDetailReports input)
        {
            IEnumerable<PrescriptionDetailReport> result = input.ToList();
            if (CoopSelectItem != "全部")
            {
                switch (AdjustCaseSelectItem)
                {
                    case "一般箋":
                        return input.Where(p => (p.AdjustCaseID == "1" || p.AdjustCaseID == "3") && p.InsName == CoopSelectItem); 
                    case "慢箋":
                        return input.Where(p => p.AdjustCaseID == "2" && p.InsName == CoopSelectItem); 
                    case "自費調劑":
                        return input.Where(p => p.AdjustCaseID == "0" && p.InsName == CoopSelectItem); 
                    case "全部":
                        return input.Where(p => p.InsName == CoopSelectItem); 
                }
            }
            else
            {
                switch (AdjustCaseSelectItem)
                {
                    case "一般箋":
                        return input.Where(p => p.AdjustCaseID == "1" || p.AdjustCaseID == "3"); 
                    case "慢箋":
                        return input.Where(p => p.AdjustCaseID == "2"); 
                    case "自費調劑":
                        return input.Where(p => p.AdjustCaseID == "0"); 
                    case "全部":
                        return input; 
                }
            }

            return result;
        }

        private void SumPrescriptionDetailMain()
        {
            PrescriptionDetailReportSumMain.SumPrescriptionDetail(PrescriptionDetailReportCollection, DepositReportDataSumMain); 
        }

        private void SumPrescriptionChangeDetailMain()
        {
            PrescriptionDetailReportSumMain.SumPrescriptionChangeDetail(PrescriptionDetailReportCollectionChanged);
        }
        
        private void SumOTCReportMainChanged( )
        {
             var tempCollection = TradeProfitDetailReportCollectionChanged.Where(p => (p.TypeId != "1"));

             TradeDetailReportSum.SumOTCReport(tempCollection); 
        }

        private void SumOTCReportMain()
        { 
            var tempCollection = TradeProfitDetailReportCollection.Where(p => (p.TypeId == "1"));
            TradeDetailReportSum.SumOTCReport(tempCollection); 
        }

        private void SumOTCReportChangeMain(string ID)
        {
            var tempCollection = TradeProfitDetailReportCollectionChanged.Where(p => true);
            tempCollection = TradeProfitDetailReportCollectionChanged.Where(p => (p.TypeId != "1"));
            TradeDetailReportSum.TotalChange = tempCollection.Sum(s => s.Profit);
        }

        private void SumOTCReport(string ID)
        {
            //TradeDetailReportSum = new TradeProfitDetailReport();

            var tempCollection = TradeProfitDetailReportCollection.Where(p => true);
            if (ID == "0")
            {
                tempCollection = TradeProfitDetailReportCollection.Where(p => (p.TypeId == "1"));
            }
            else
            {
                if (TradeChangeSelectItem == "全部")
                {
                    tempCollection = TradeProfitDetailReportCollection.Where(p => (p.TypeId != "1"));
                }
                else
                {
                    tempCollection = TradeProfitDetailReportCollection.Where(p => (p.TypeId == TradeChangeSelectItem));
                }
            } 
            TradeDetailReportSum.SumOTCReport(tempCollection);
          
        }

        private void SumOTCProfit()
        {
            //TradeDetailReportSum.TotalProfit = TradeDetailReportSum.Profit + TradeDetailReportSum.TotalChange + (int)TotalRewardReport.RewardAmount + (int)StockTakingOTCDetailReportSum.Price + TradeDetailReportSum.DiscountAmtMinus;
            TradeDetailReportSum.TotalProfit = TradeDetailReportSum.RealTotal + 
                                               (int)TradeDetailReportSum.TotalCost + 
                                               TradeDetailReportSum.TotalChange + 
                                               StockTakingOTCDetailReportSum.Price + 
                                               TradeDetailReportSum.DiscountAmtMinus + 
                                               (int)TotalRewardReport.RewardAmount;
        }

        private void SumMedProfit()
        {
            PrescriptionDetailReportSumMain.SumMedProfit(StockTakingDetailReportSum,DepositReportDataSumMain); 
        }

        private void SumAllProfit()
        {
            TotalCashFlow.AllCount = TradeDetailReportSum.Count + PrescriptionDetailReportSumMain.MedTotalCount;
            TotalCashFlow.AllIncome = (int)(TradeDetailReportSum.RealTotal + PrescriptionDetailReportSumMain.MedTotalIncome);

            TotalCashFlow.AllCost = (int)(TradeDetailReportSum.TotalCost + PrescriptionDetailReportSumMain.MedTotalMeduse);
            TotalCashFlow.AllChange = (int)(TradeDetailReportSum.TotalChange + PrescriptionDetailReportSumMain.MedTotalChange);

            TotalCashFlow.AllStock = StockTakingOTCDetailReportSum.Price + StockTakingDetailReportSum.Price;

           
            TotalCashFlow.AllDeposit = DepositReportDataSumMain.NormalDeposit +
                                       DepositReportDataSumMain.ChronicDeposit +
                                       DepositReportDataSumMain.CooperativeDeposit+ DepositReportDataSumMain.PrescribeDeposit;

            TotalCashFlow.AllProfit = (int)(TradeDetailReportSum.TotalProfit + 
                                            PrescriptionDetailReportSumMain.MedTotalProfit);
        }

        private void SumStockTakingOTCDetailReport()
        {
            var tempCollection = StockTakingOTCDetailReportCollection.Where(p => true);
            if (StockTakingOTCSelectItem == "全部")
            {
                tempCollection = StockTakingOTCDetailReportCollection;
            }
            else
            {
                tempCollection = StockTakingOTCDetailReportCollection.Where(p => (p.Type == StockTakingOTCSelectItem));
            }
            StockTakingOTCDetailReportSum.Price = tempCollection.Sum(s => s.Price);
            StockTakingOTCDetailReportSum.Count = tempCollection.Count();
        }

        private void SumStockTakingDetailReport()
        {
            var tempCollection = StockTakingDetailReportCollection.Where(p => true);

            if (StockTakingSelectItem == "全部")
            {
                tempCollection = StockTakingDetailReportCollection;
            }
            else
            {
                tempCollection = StockTakingDetailReportCollection.Where(p => (p.Type == StockTakingSelectItem));
            }
            StockTakingDetailReportSum.Price = tempCollection.Sum(s => s.Price);
            StockTakingDetailReportSum.Count = tempCollection.Count();
        }


        private void CalculateTotalRewardProfit()
        {
            TotalRewardReport.RewardAmountSum = -RewardReportCollection.Sum(c => c.RewardAmount);
            TotalRewardReport.RewardAmount = -RewardReportCollection.Sum(c => c.RewardAmount);
        }

        private void AdjustCaseFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is PrescriptionDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            PrescriptionDetailReport indexitem = ((PrescriptionDetailReport)e.Item);
            if (CoopSelectItem == indexitem.InsName || CoopSelectItem == "全部")
            {
                if (AdjustCaseSelectItem == "一般箋" && (indexitem.AdjustCaseID == "1" || indexitem.AdjustCaseID == "3"))
                    e.Accepted = true;
                else if (AdjustCaseSelectItem == "慢箋" && indexitem.AdjustCaseID == "2")
                    e.Accepted = true;
                else if (AdjustCaseSelectItem == "自費調劑" && indexitem.AdjustCaseID == "0")
                    e.Accepted = true;
                else if (AdjustCaseSelectItem == "全部")
                    e.Accepted = true;
            } 
        }

      
        private void CashCoopFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            
            CashDetailReport indexitem = ((CashDetailReport)e.Item); 
            e.Accepted = CashCoopSelectItem == indexitem.Ins_Name || CashCoopSelectItem == "全部"; 
        }

        private void OTCChangeFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
             
            TradeProfitDetailReport indexitem = ((TradeProfitDetailReport)e.Item);
            e.Accepted = TradeChangeSelectItem == indexitem.TypeId || TradeChangeSelectItem == "全部"; 
        }

        private void StockTakingDetailFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
             
            StockTakingDetailReport indexitem = ((StockTakingDetailReport)e.Item);
            e.Accepted = StockTakingSelectItem == indexitem.Type || StockTakingSelectItem == "全部";
        }

        private void StockTakingOTCDetailFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
             
            StockTakingOTCDetailReport indexitem = ((StockTakingOTCDetailReport)e.Item);
            e.Accepted = StockTakingOTCSelectItem == indexitem.Type || StockTakingOTCSelectItem == "全部";
        }

        private void TicketFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
             
            TradeProfitDetailReport indexitem = ((TradeProfitDetailReport)e.Item);
            e.Accepted = indexitem.DiscountAmt != 0;
        }

        #endregion Action
    }
}