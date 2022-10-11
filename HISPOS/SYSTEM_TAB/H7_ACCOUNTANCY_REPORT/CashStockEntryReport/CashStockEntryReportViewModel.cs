using ClosedXML.Excel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
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

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport
{
    public class CashStockEntryReportViewModel : TabBase
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

        private List<string> adjustCaseString;

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

                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
            }
        }

        private string cashcoopSelectItem = "全部";

        public string CashCoopSelectItem
        {
            get => cashcoopSelectItem;
            set
            {
                Set(() => CashCoopSelectItem, ref cashcoopSelectItem, value);

                CashDetailReportViewSource.Filter += CashCoopFilter;
                SumCashDetailReport();
            }
        }

        private string tradeChangeSelectItem = "全部";

        public string TradeChangeSelectItem
        {
            get => tradeChangeSelectItem;
            set
            {
                Set(() => TradeChangeSelectItem, ref tradeChangeSelectItem, value);

                TradeProfitDetailReportViewSource.Filter += OTCChangeFilter;
                SumOTCReport("1");
            }
        }

        private string adjustCaseSelectItem = "全部";

        public string AdjustCaseSelectItem
        {
            get => adjustCaseSelectItem;
            set
            {
                Set(() => AdjustCaseSelectItem, ref adjustCaseSelectItem, value);
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
            }
        }

        private string stockTakingOTCSelectItem = "全部";

        public string StockTakingOTCSelectItem
        {
            get => stockTakingOTCSelectItem;
            set
            {
                Set(() => StockTakingOTCSelectItem, ref stockTakingOTCSelectItem, value);
                StockTakingOTCDetailReportViewSource.Filter += StockTakingOTCDetailFilter;
                SumStockTakingOTCDetailReport();
            }
        }

        private string stockTakingSelectItem = "全部";

        public string StockTakingSelectItem
        {
            get => stockTakingSelectItem;
            set
            {
                Set(() => StockTakingSelectItem, ref stockTakingSelectItem, value);
                StockTakingDetailReportViewSource.Filter += StockTakingDetailFilter;
                SumStockTakingDetailReport();
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

        private CashReport cashflowSelectedItem = new CashReport();

        public CashReport CashflowSelectedItem
        {
            get => cashflowSelectedItem;
            set
            {
                Set(() => CashflowSelectedItem, ref cashflowSelectedItem, value);
            }
        }

        private PrescriptionProfitReport selfPrescriptionSelectedItem;

        public PrescriptionProfitReport SelfPrescriptionSelectedItem
        {
            get => selfPrescriptionSelectedItem;
            set
            {
                Set(() => SelfPrescriptionSelectedItem, ref selfPrescriptionSelectedItem, value);
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

        public StockTakingReports TotalStockTakingReportCollection { get; set; } = new StockTakingReports();

        //9.3新增^^^^

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

        private ObservableCollection<StockTakingDetailRecordReport> stockTakingDetailRecordReportCollection = new ObservableCollection<StockTakingDetailRecordReport>();

        public ObservableCollection<StockTakingDetailRecordReport> StockTakingDetailRecordReportCollection
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

        public StockTakingOTCReports TotalStockTakingOTCReportCollection { get; set; } = new StockTakingOTCReports();

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

        private ObservableCollection<StockTakingDetailRecordReport> stockTakingOTCDetailRecordReportCollection = new ObservableCollection<StockTakingDetailRecordReport>();

        public ObservableCollection<StockTakingDetailRecordReport> StockTakingOTCDetailRecordReportCollection
        {
            get => stockTakingOTCDetailRecordReportCollection;
            set
            {
                Set(() => StockTakingOTCDetailRecordReportCollection, ref stockTakingOTCDetailRecordReportCollection, value);
            }
        }

        private StockTakingDetailRecordReport stockTakingOTCDetailMedicineReportSelectItem;

        public StockTakingDetailRecordReport StockTakingOTCDetailMedicineReportSelectItem
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

        public RewardReports TotalRewardReportCollection { get; set; } = new RewardReports();

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

        public PrescriptionDetailReport PrescriptionDetailReportSum
        {
            get => prescriptionDetailReportSum;
            set
            {
                Set(() => PrescriptionDetailReportSum, ref prescriptionDetailReportSum, value);
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

        private PrescriptionDetailReports prescriptionDetailReportCollection;

        public PrescriptionDetailReports PrescriptionDetailReportCollection
        {
            get => prescriptionDetailReportCollection;
            set
            {
                Set(() => PrescriptionDetailReportCollection, ref prescriptionDetailReportCollection, value);
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

        private CashDetailReports cashDetailReportCollection =new CashDetailReports();

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

        private ObservableCollection<PrescriptionDetailMedicineRepot> prescriptionDetailMedicineRepotCollection = new ObservableCollection<PrescriptionDetailMedicineRepot>();

        public ObservableCollection<PrescriptionDetailMedicineRepot> PrescriptionDetailMedicineRepotCollection
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

        #endregion Variables

        #region Command

        public RelayCommand SelfPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CooperativePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand SelfPrescriptionChangeSelectionChangedCommand { get; set; }
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
        public RelayCommand StockTakingDetailClickCommand { get; set; }
        public RelayCommand StockTakingOTCReportSelectionChangedCommand { get; set; }
        public RelayCommand StockTakingOTCDetailClickCommand { get; set; }
        public RelayCommand StockTakingDetailMedicineDoubleClickCommand { get; set; }
        public RelayCommand TradeProfitReportSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitDetailClickCommand { get; set; }
        public RelayCommand TradeProfitDetailDoubleClickCommand { get; set; }
        public RelayCommand TradeProfitDetailEmpClickCommand { get; set; }
        public RelayCommand ExtraMoneyReportSelectionChangedCommand { get; set; }
        public RelayCommand ExtraMoneyDetailClickCommand { get; set; }
        public RelayCommand<string> ChangeUiTypeCommand { get; set; }

        public RelayCommand RewardReportSelectionChangedCommand { get; set; }
        public RelayCommand RewardDetailClickCommand { get; set; }
        public RelayCommand RewardDetailMedicineDoubleClickCommand { get; set; }
        public RelayCommand TradeChangeReportSelectionChangedCommand { get; set; }

        public RelayCommand RewardExcelCommand { get; set; }
        public RelayCommand DownloadMonTradeReportCommand { get; set; }

        #endregion Command

        public CashStockEntryReportViewModel()
        {
            SearchCommand = new RelayCommand(SearchAction);
            SelfPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionSelectionChangedAction);
            CooperativePrescriptionSelectionChangedCommand = new RelayCommand(CooperativePrescriptionSelectionChangedAction);
            SelfPrescriptionChangeSelectionChangedCommand = new RelayCommand(SelfPrescriptionChangeSelectionChangedAction);
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
            StockTakingDetailClickCommand = new RelayCommand(StockTakingDetailClickAction);
            CashDetailMouseDoubleClickCommand = new RelayCommand(CashDetailMouseDoubleClickAction);
            StockTakingOTCReportSelectionChangedCommand = new RelayCommand(StockTakingOTCReportSelectionChangedAction);
            StockTakingOTCDetailClickCommand = new RelayCommand(StockTakingOTCDetailClickAction);
            TradeProfitReportSelectionChangedCommand = new RelayCommand(TradeProfitReportSelectionChangedAction);
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
            DownloadMonTradeReportCommand = new RelayCommand(DownloadMonTradeReportAction);
            GetData();
            InitCollection();
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
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "-"+ EndDate.ToString("yyyyMMdd") + "績效明細";
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

        //add 20210819 shani 下載當月發票DownloadMonTradeReportAction
        private void DownloadMonTradeReportAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Id", null));
            parameters.Add(new SqlParameter("sDate", StartDate));
            parameters.Add(new SqlParameter("eDate", EndDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeProfitDetailEmpRecordByDate]", parameters);
            MainWindow.ServerConnection.CloseConnection();



            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "當月發票";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = StartDate.ToString("yyyyMM") + "-"+ "當月發票";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("發票明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                var col1 = ws.Column("A");
                col1.Width = 25;
                var col2 = ws.Column("B");
                col2.Width = 55;
                var col3 = ws.Column("C");
                col3.Width = 15;
                var col4 = ws.Column("D");
                col4.Width = 55;
                var col5 = ws.Column("E");
                col4.Width = 15;


                ws.Cell(1, 1).Value = "發票明細";
                ws.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "時間";
                ws.Cell("B2").Value = "發票號碼";
                ws.Cell("C2").Value = "發票金額";
                ws.Cell("D2").Value = "作廢發票號碼";
                ws.Cell("E2").Value = "作廢發票號碼";

                if (result.Rows.Count > 0)
                {
                    var rangeWithData = ws.Cell(3, 1).InsertData(result.AsEnumerable());
                    rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }


                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters2 = new List<SqlParameter>();
                parameters2.Add(new SqlParameter("Id", "1"));
                parameters2.Add(new SqlParameter("sDate", StartDate));
                DataTable result2 = MainWindow.ServerConnection.ExecuteProc("[GET].[InvoiceRecordByDate]", parameters2);
                MainWindow.ServerConnection.CloseConnection();

                ws = wb.Worksheets.Add("發票明細2");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                col1 = ws.Column("A");
                col1.Width = 15;
                col2 = ws.Column("B");
                col2.Width = 15;
                col3 = ws.Column("C");
                col3.Width = 15;
                col4 = ws.Column("D");
                col3.Width = 15;



                ws.Cell(1, 1).Value = "發票明細";
                ws.Range(1, 1, 1, 4).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "日期";
                ws.Cell("B2").Value = "發票號碼";
                ws.Cell("C2").Value = "發票金額";
                ws.Cell("D2").Value = "作廢";//result2

                if (result2.Rows.Count > 0)
                {
                    var rangeWithData = ws.Cell(3, 1).InsertData(result2.AsEnumerable());
                    rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }






                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                wb.SaveAs(fdlg.FileName);

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
            if (PrescriptionDetailReportView is null) return;

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "處方毛利明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "_" + EndDate.ToString("yyyyMMdd") + "處方毛利明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("姓名,醫療院所,藥服(估),藥品,自費,耗用,毛利");
                        double sumMedicalServicePoint = 0;
                        double sumMedicalPoint = 0;
                        double sumPaySelfPoint = 0;
                        decimal sumMeduse = 0;
                        double sumProfit = 0;
                        foreach (var c in PrescriptionDetailReportView)
                        {
                            PrescriptionDetailReport presc = (PrescriptionDetailReport)c;
                            file.WriteLine($"{presc.CusName},{presc.InsName},{presc.MedicalServicePoint},{presc.MedicalPoint},{presc.PaySelfPoint},{presc.Meduse},{presc.Profit}");
                            sumMedicalServicePoint += presc.MedicalServicePoint;
                            sumMedicalPoint += presc.MedicalPoint;
                            sumPaySelfPoint += presc.PaySelfPoint;
                            sumMeduse += presc.Meduse;
                            sumProfit += presc.Profit;
                        }
                        file.WriteLine($"合計,,{sumMedicalServicePoint},{sumMedicalPoint},{sumPaySelfPoint},{sumMeduse},{sumProfit}");
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }

        private void InitCollection()
        {
            AdjustCaseString = new List<string>() { "全部", "一般箋", "慢箋", "自費調劑" };
        }

        private void PrintCashPerDayAction()
        {
            if (CashflowSelectedItem is null)
                return;
            DataTable table = CashReportDb.GetPerDayDataByDate(StartDate, EndDate, CashflowSelectedItem.TypeId);
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "金流存檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "_" + CashflowSelectedItem.TypeName + "金流存檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("日期,部分負擔,自費,自費調劑,押金,其他,總計");
                        int cCopayMentPrice = 0;
                        int cPaySelfPrice = 0;
                        int cAllPaySelfPrice = 0;
                        int cDepositPrice = 0;
                        int cOtherPrice = 0;
                        int cTotalPrice = 0;
                        foreach (DataRow s in table.Rows)
                        {
                            file.WriteLine($"{s.Field<DateTime>("date").AddYears(-1911).ToString("yyy/MM/dd")}," +
                                $"{s.Field<int>("CopayMentPrice")},{s.Field<int>("PaySelfPrice")},{s.Field<int>("AllPaySelfPrice")}" +
                                $",{s.Field<int>("DepositPrice")},{s.Field<int>("OtherPrice")},{s.Field<int>("TotalPrice")}");

                            cCopayMentPrice += s.Field<int>("CopayMentPrice");
                            cPaySelfPrice += s.Field<int>("PaySelfPrice");
                            cAllPaySelfPrice += s.Field<int>("AllPaySelfPrice");
                            cDepositPrice += s.Field<int>("DepositPrice");
                            cOtherPrice += s.Field<int>("OtherPrice");
                            cTotalPrice += s.Field<int>("TotalPrice");
                        }
                        file.WriteLine($"總計,{cCopayMentPrice},{cPaySelfPrice},{cAllPaySelfPrice},{cDepositPrice},{cOtherPrice},{cTotalPrice}");
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
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

            var data = ReportService.GetPrescriptionDetailMedicineReportById(PrescriptionDetailReportSelectItem.Id,StartDate,EndDate);
            PrescriptionDetailMedicineRepotCollection = new ObservableCollection<PrescriptionDetailMedicineRepot>(data);
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
            StockTakingDetailRecordReportCollection
                = new ObservableCollection<StockTakingDetailRecordReport>(
                    ReportService.GetStockTakingDetailRecordByDate(StockTakingDetailReportSelectItem.InvRecSourceID, StartDate, EndDate));
        }

        private void StockTakingOTCDetailClickAction()
        {
            if (StockTakingOTCDetailReportSelectItem is null)
            {
                StockTakingOTCDetailRecordReportCollection.Clear();
                return;
            }

            var data
                = ReportService.GetStockTakingDetailRecordByDate(StockTakingOTCDetailReportSelectItem.InvRecSourceID,
                    StartDate, EndDate);

            StockTakingOTCDetailRecordReportCollection = new ObservableCollection<StockTakingDetailRecordReport>(data);
        }

        private void TradeProfitReportSelectionChangedAction()
        {
            /* if (TradeProfitSelectedItem is null)
             {
                 TradeProfitDetailReportCollection.Clear();
                 TradeProfitDetailReportViewSource = new CollectionViewSource { Source = TradeProfitDetailReportCollection };
                 TradeProfitDetailReportView = TradeProfitDetailReportViewSource.View;
                 //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                 //SumPrescriptionDetailReport();
             }
             if (TradeProfitSelectedItem is null)
                 return;*/
            CashStockEntryReportEnum = CashStockEntryReportEnum.TradeProfit;
            ChangeVis = Visibility.Collapsed;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                TradeProfitDetailReportCollection = new TradeProfitDetailReports("000", StartDate, EndDate);
                TradeProfitDetailEmpReportCollection = new TradeProfitDetailEmpReports("0", StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                TradeProfitDetailReportViewSource = new CollectionViewSource { Source = TradeProfitDetailReportCollection };
                TradeProfitDetailReportView = TradeProfitDetailReportViewSource.View;
                TradeProfitDetailEmpReportViewSource = new CollectionViewSource { Source = TradeProfitDetailEmpReportCollection };

                TradeProfitDetailEmpReportView = TradeProfitDetailEmpReportViewSource.View;
                TradeChangeSelectItem = "全部";
                TradeProfitDetailReportViewSource.Filter += OTCChangeFilter;
                SumOTCReport("0");
                TradeDetailCount = TradeProfitDetailReportCollection.Count();
                TradeEmpDetailCount = TradeProfitDetailEmpReportCollection.Count();
                EmpProfit = TradeProfitDetailEmpReportCollection.Sum(e => e.Profit);
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CashflowSelectedItem = null;
        }

        private void TradeChangeReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.TradeProfit;

            ChangeVis = Visibility.Visible;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                TradeProfitDetailReportCollection = new TradeProfitDetailReports("1", StartDate, EndDate);
                TradeProfitDetailEmpReportCollection = new TradeProfitDetailEmpReports("0", StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
                var StringCopy = new List<string>() { };
                foreach (var r in TradeProfitDetailReportCollection)
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
                TradeProfitDetailReportViewSource = new CollectionViewSource { Source = TradeProfitDetailReportCollection };
                TradeProfitDetailReportView = TradeProfitDetailReportViewSource.View;
                TradeProfitDetailEmpReportViewSource = new CollectionViewSource { Source = TradeProfitDetailEmpReportCollection };

                TradeProfitDetailEmpReportView = TradeProfitDetailEmpReportViewSource.View;
              
                TradeChangeSelectItem = "全部";
                TradeProfitDetailReportViewSource.Filter += OTCChangeFilter;
                SumOTCReport("1");
                TradeDetailCount = TradeProfitDetailReportCollection.Count();
                TradeEmpDetailCount = TradeProfitDetailEmpReportCollection.Count();
                EmpProfit = TradeProfitDetailEmpReportCollection.Sum(e => e.Profit);
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CashflowSelectedItem = null;
        }

        private void StockTakingReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.StockTaking;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                StockTakingDetailReportCollection = new StockTakingDetailReports("0", StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
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
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                StockTakingDetailReportViewSource = new CollectionViewSource { Source = StockTakingDetailReportCollection };
                StockTakingDetailReportView = StockTakingDetailReportViewSource.View;
                StockTakingSelectItem = "全部";
                StockTakingDetailReportViewSource.Filter += StockTakingDetailFilter;

                SumStockTakingDetailReport();
                StockDetailCount = StockTakingDetailReportCollection.Count();
                IsBusy = false;
            };
            IsBusy = true;
            CashflowSelectedItem = null;
        }

        private void StockTakingOTCReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.OTCStockTaking;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                StockTakingOTCDetailReportCollection = new StockTakingOTCDetailReports("0", StartDate, EndDate);
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
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                StockTakingOTCDetailReportViewSource = new CollectionViewSource { Source = StockTakingOTCDetailReportCollection };
                StockTakingOTCDetailReportView = StockTakingOTCDetailReportViewSource.View;
                StockTakingOTCSelectItem = "全部";
                StockTakingOTCDetailReportViewSource.Filter += StockTakingOTCDetailFilter;
                SumStockTakingOTCDetailReport();
                StockOTCDetailCount = StockTakingOTCDetailReportCollection.Count();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CashflowSelectedItem = null;
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
            SelfPrescriptionSelectedItem = null;
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
            SelfPrescriptionSelectedItem = null;
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
            SelfPrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void CooperativePrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Visible;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                PrescriptionDetailReportCollection = new PrescriptionDetailReports("000", StartDate, EndDate);

                var CoopStringCopy = new List<string>() { };
                foreach (var r in PrescriptionDetailReportCollection)
                {
                    CoopStringCopy.Add(r.InsName);
                }
                var DistinctItems = CoopStringCopy.Select(x => x).Distinct();
                CoopString = new List<string>() { "全部" };
                foreach (var item in DistinctItems)
                {
                    CoopString.Add(item);
                }

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                CoopSelectItem = "全部";

                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            SelfPrescriptionSelectedItem = null;
            CashflowSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void SelfPrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                PrescriptionDetailReportCollection = new PrescriptionDetailReports("1", StartDate, EndDate);
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                CoopSelectItem = "全部";
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CooperativePrescriptionSelectedItem = null;
            CashflowSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void SelfPrescriptionChangeSelectionChangedAction()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                PrescriptionDetailReportCollection = new PrescriptionDetailReports("11", StartDate, EndDate);
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                CoopSelectItem = "全部";
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CooperativePrescriptionSelectedItem = null;
            CashflowSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void CooperativePrescriptionChangeSelectionChangedAction()
        {
            CoopVis = Visibility.Visible;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                PrescriptionDetailReportCollection = new PrescriptionDetailReports("22", StartDate, EndDate);

                var CoopStringCopy = new List<string>() { };
                foreach (var r in PrescriptionDetailReportCollection)
                {
                    CoopStringCopy.Add(r.InsName);
                }
                var DistinctItems = CoopStringCopy.Select(x => x).Distinct();
                CoopString = new List<string>() { "全部" };
                foreach (var item in DistinctItems)
                {
                    CoopString.Add(item);
                }

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                CoopSelectItem = "全部";

                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            SelfPrescriptionSelectedItem = null;
            CashflowSelectedItem = null;
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
            CashflowSelectedItem = null;
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
            CashflowSelectedItem = null;
        }

        private void SearchAction()
        {
            GetData();
        }

        private void GetData()
        {
            TotalPrescriptionProfitReportCollection.Clear();
            SelfPrescriptionProfitReportCollection.Clear();
            CooperativePrescriptionProfitReportCollection.Clear();
            TradeProfitReportCollection.Clear();
            StockTakingReportCollection.Clear();
            StockTakingOTCReportCollection.Clear();
            RewardReportCollection.Clear();
            CashProfitReportCollection.Clear();
            CoopCashProfitReportCollection.Clear();
            SelfPrescriptionChangeProfitReportCollection.Clear();
            CooperativePrescriptionChangeProfitReportCollection.Clear();
            TradeNormalReportCollection.Clear();
            TradeChangeReportCollection.Clear();
            TradeDeleteReportCollection.Clear();

            ExtraMoney = 0;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                CashflowCollection = new CashReports(StartDate, EndDate);
                TotalPrescriptionProfitReportCollection.GetDataByDate(StartDate, EndDate);
                TradeProfitReportCollection = new TradeProfitReports(StartDate, EndDate);
                StockTakingReportCollection = new StockTakingReports(StartDate, EndDate);
                StockTakingOTCReportCollection = new StockTakingOTCReports(StartDate, EndDate);
                RewardReportCollection = new RewardReports(StartDate, EndDate);

                DataTable Extra = PrescriptionProfitReportDb.GetExtraMoneyByDates(StartDate, EndDate);
                ExtraMoney = Extra.Rows[0].Field<decimal?>("CashFlow_Value");

                GetInventoryDifference();
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                foreach (var r in TotalPrescriptionProfitReportCollection)
                {
                    if (r.TypeId.Length == 1)
                    {
                        if (r.TypeName == "調整")
                        {
                            SelfPrescriptionChangeProfitReportCollection.Add(r);
                        }
                        else if (r.TypeName == "合作調整")
                        {
                            CooperativePrescriptionChangeProfitReportCollection.Add(r);
                        }
                        else
                        {
                            SelfPrescriptionProfitReportCollection.Add(r);
                        }
                    }
                    else
                    {
                        CooperativePrescriptionProfitReportCollection.Add(r);
                    }
                }

                foreach (var r in CashflowCollection)
                {
                    if (r.TypeId == "0")
                        CashProfitReportCollection.Add(r);
                    else
                        CoopCashProfitReportCollection.Add(r);
                }
                foreach (var r in TradeProfitReportCollection)
                {
                    if (r.TypeId == "1")
                        TradeNormalReportCollection.Add(r);
                    else if (r.TypeId == "5")
                        TradeChangeReportCollection.Add(r);
                    else if (r.TypeId == "6")
                        TradeDeleteReportCollection.Add(r);
                }
                

                CalculateTotalTradeProfit();
                CalculateTotalTradeNormal();
                CalculateTotalTradeDelete();
                CalculateTotalTradeChange();
                CalculateTotalStockTaking();
                CalculateTotalStockTakingOTC();
                CalculateTotalCashFlow();
                CalculateTotalPrescriptionProfit();
                CalculateSelfPrescriptionProfit();
                CalculateCooperativePrescriptionProfit();
                CalculateTotalRewardProfit();
                CalculateCoopCashProfit();
                CalculateCashProfit();
                CalculateSelfPrescriptionChange();
                CalculateCooperativeChangePrescriptionProfit();
                CalculateTotal();


                if (CashStockEntryReportEnum == CashStockEntryReportEnum.Cash && CashCoopVis == Visibility.Collapsed)
                {
                    CashCoopSelectionChangedAction();

                }
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.Cash && CashCoopVis != Visibility.Collapsed)
                {
               
                    CashNotCoopSelectionChangedAction();
                    CashSelectionChangedAction();
                }

                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.Prescription && CoopVis == Visibility.Visible)
                {
                    CooperativePrescriptionChangeSelectionChangedAction();
  
                }   
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.Prescription && CoopVis != Visibility.Visible)
                {
                   
                    CooperativePrescriptionSelectionChangedAction();
                    SelfPrescriptionSelectionChangedAction();
                }
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.StockTaking)
                {
                    StockTakingReportSelectionChangedAction();
                }
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.TradeProfit && ChangeVis != Visibility.Visible)
                {
                    TradeProfitReportSelectionChangedAction();
                }
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.TradeProfit && ChangeVis == Visibility.Visible)
                {
                    TradeChangeReportSelectionChangedAction();
                }
                
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.ExtraMoney)
                {
                    ExtraMoneyReportSelectionChangedAction();
                }
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.OTCStockTaking)
                {
                    StockTakingOTCReportSelectionChangedAction();
                }
                else if (CashStockEntryReportEnum == CashStockEntryReportEnum.Reward)
                {
                    RewardReportSelectionChangedAction();
                }
                
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void GetInventoryDifference()
        {
            var table = CashReportDb.GetInventoryDifferenceByDate(StartDate, EndDate);
            if (table.Rows.Count > 0)
            {
                InventoryDifference = new InventoryDifference(table.Rows[0]);
            }
        }

        private void RevertSelfPrescriptionProfitByEditRecords()
        {
            foreach (var s in SelfPrescriptionProfitReportCollection)
            {
                var editRecords = PrescriptionPointEditRecords.Where(r => r.TypeID.EndsWith(s.TypeId));
                if (editRecords.Any())
                {
                    var medicalServicePoint = editRecords.Sum(e => e.MedicalServiceDifference) * -1;
                    var medicinePoint = editRecords.Sum(e => e.MedicineDifference) * -1;
                    var paySelfPoint = editRecords.Sum(e => e.PaySelfDifference) * -1;
                    var profit = editRecords.Sum(e => e.ProfitDifference) * -1;
                    s.MedicalServicePoint += medicalServicePoint;
                    s.MedicinePoint += medicinePoint;
                    s.PaySelfPoint += paySelfPoint;
                    s.Profit += profit;
                }
            }
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
            CashDetailReportSum.CusName = "總計";
            CashDetailReportSum.CopayMentPrice = tempCollection.Sum(s => s.CopayMentPrice);
            CashDetailReportSum.PaySelfPrice = tempCollection.Sum(s => s.PaySelfPrice);
            CashDetailReportSum.PaySelfPrescritionPrice = tempCollection.Sum(s => s.PaySelfPrescritionPrice);
            CashDetailReportSum.Deposit = tempCollection.Sum(s => s.Deposit);
            CashDetailReportSum.Other = tempCollection.Sum(s => s.Other);
            CashDetailReportSum.Count = tempCollection.Count();
        }

        private void SumPrescriptionDetailReport()
        {
            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.InsName = "總計";

            var tempCollection = PrescriptionDetailReportCollection.Where(p => true);

            if (CoopSelectItem != "全部")
            {
                switch (AdjustCaseSelectItem)
                {
                    case "一般箋":
                        tempCollection = PrescriptionDetailReportCollection.Where(p => (p.AdjustCaseID == "1" || p.AdjustCaseID == "3") && p.InsName == CoopSelectItem);
                        break;

                    case "慢箋":
                        tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "2" && p.InsName == CoopSelectItem);
                        break;

                    case "自費調劑":
                        tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "0" && p.InsName == CoopSelectItem);
                        break;

                    case "全部":
                        tempCollection = PrescriptionDetailReportCollection.Where(p => p.InsName == CoopSelectItem);
                        break;
                }
            }
            else
            {
                switch (AdjustCaseSelectItem)
                {
                    case "一般箋":
                        tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "1" || p.AdjustCaseID == "3");
                        break;

                    case "慢箋":
                        tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "2");
                        break;

                    case "自費調劑":
                        tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "0");
                        break;

                    case "全部":
                        tempCollection = PrescriptionDetailReportCollection;
                        break;
                }
            }

            PrescriptionDetailReportSum.MedicalPoint = tempCollection.Sum(s => s.MedicalPoint);
            PrescriptionDetailReportSum.MedicalServicePoint = tempCollection.Sum(s => s.MedicalServicePoint);
            PrescriptionDetailReportSum.PaySelfPoint = tempCollection.Sum(s => s.PaySelfPoint);
            PrescriptionDetailReportSum.Meduse = tempCollection.Sum(s => s.Meduse);
            PrescriptionDetailReportSum.Profit = tempCollection.Sum(s => s.Profit);
            PrescriptionDetailReportSum.Count = tempCollection.Count();
        }

        private void SumOTCReport(string ID)
        {
            TradeDetailReportSum = new TradeProfitDetailReport();

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
            TradeDetailReportSum.CardAmount = tempCollection.Sum(s => s.CardAmount);
            TradeDetailReportSum.CashAmount = tempCollection.Sum(s => s.CashAmount);
            TradeDetailReportSum.DiscountAmt = tempCollection.Sum(s => s.DiscountAmt);
            TradeDetailReportSum.CashCoupon = tempCollection.Sum(s => s.CashCoupon);
            TradeDetailReportSum.Profit = tempCollection.Sum(s => s.Profit);
            TradeDetailReportSum.RealTotal = tempCollection.Sum(s => s.RealTotal);
            TradeDetailReportSum.ValueDifference = tempCollection.Sum(s => s.ValueDifference);
            TradeDetailReportSum.CardFee = tempCollection.Sum(s => s.CardFee);
            TradeDetailReportSum.Count = tempCollection.Count();
        }

        private void SumStockTakingOTCDetailReport()
        {
            StockTakingOTCDetailReportSum = new StockTakingOTCDetailReport();

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
            StockTakingDetailReportSum = new StockTakingDetailReport();

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

        private void CalculateTotalCashFlow()
        {
            TotalCashFlow.CopayMentPrice = CashflowCollection.Sum(c => c.CopayMentPrice);
            TotalCashFlow.PaySelfPrice = CashflowCollection.Sum(c => c.PaySelfPrice);
            TotalCashFlow.AllPaySelfPrice = CashflowCollection.Sum(c => c.AllPaySelfPrice);
            TotalCashFlow.DepositPrice = CashflowCollection.Sum(c => c.DepositPrice);
            TotalCashFlow.OtherPrice = CashflowCollection.Sum(c => c.OtherPrice);
            TotalCashFlow.TotalPrice = CashflowCollection.Sum(c => c.TotalPrice);
        }

        private void CalculateTotalPrescriptionProfit()
        {
            TotalPrescriptionProfitReport = new PrescriptionProfitReport();
            foreach (var r in TotalPrescriptionProfitReportCollection)
            {
                TotalPrescriptionProfitReport.Count += r.Count;
                TotalPrescriptionProfitReport.MedicalServicePoint += r.MedicalServicePoint;
                TotalPrescriptionProfitReport.MedicinePoint += r.MedicinePoint;
                TotalPrescriptionProfitReport.PaySelfPoint += r.PaySelfPoint;
                TotalPrescriptionProfitReport.MedUse += r.MedUse;
                TotalPrescriptionProfitReport.Profit += r.Profit;
            }
        }

        private void CalculateTotalTradeProfit()
        {
            TotalTradeProfitReport = new TradeProfitReport();
            foreach (var r in TradeNormalReportCollection)
            {
                TotalTradeProfitReport.Count += r.Count;
                TotalTradeProfitReport.NetIncome += r.NetIncome;
                TotalTradeProfitReport.Cost += r.Cost;
                TotalTradeProfitReport.Profit += r.Profit;
                TotalTradeProfitReport.CashAmount += r.CashAmount;
                TotalTradeProfitReport.CardAmount += r.CardAmount;
                TotalTradeProfitReport.DiscountAmt += r.DiscountAmt;
                TotalTradeProfitReport.CardFee += r.CardFee;
                TotalTradeProfitReport.CashCoupon += r.CashCoupon;
            }
            TotalTradeProfitReport.TotalAmt = TotalTradeProfitReport.CashAmount + TotalTradeProfitReport.CardAmount + TotalTradeProfitReport.DiscountAmt + TotalTradeProfitReport.CashCoupon;

            TotalTradeProfitReport.TotalCostTotal = (int)(TotalTradeProfitReport.Cost /*+ TotalTradeProfitReport.CardFee*/);
        }

        private void CalculateTotalTradeNormal()
        {
            foreach (var r in TradeNormalReportCollection)
            {
                TradeNormalProfitReport.TotalNormalAmt += r.DiscountAmt + r.CardAmount + r.CashAmount;
            }
        }

        private void CalculateTotalTradeDelete()
        {
            foreach (var r in TradeDeleteReportCollection)
            {
                TotalTradeProfitReport.TotalDeleteAmt += r.DiscountAmt + r.CardAmount + r.CashAmount + r.Cost;
                TotalTradeProfitReport.TotalDeleteCardAmt += r.CardAmount;
                TotalTradeProfitReport.TotalDeleteCashAmt += r.CashAmount;
                TotalTradeProfitReport.TotalDeleteDiscountAmt += r.DiscountAmt;
                TotalTradeProfitReport.TotalDeleteCostAmt += r.Cost/*+(int)(r.CardFee)*/;
                TotalTradeProfitReport.TotalDeleteCashCouponAmt += r.CashCoupon;
            }
        }

        private void CalculateTotalTradeChange()
        {
            foreach (var r in TradeChangeReportCollection)
            {
                TotalTradeProfitReport.TotalChangeAmt += r.DiscountAmt + r.CardAmount + r.CashAmount + r.Cost /*+ (int)(r.CardFee)*/;
                TotalTradeProfitReport.TotalChangeCardAmt += r.CardAmount;
                TotalTradeProfitReport.TotalChangeCashAmt += r.CashAmount;
                TotalTradeProfitReport.TotalChangeDiscountAmt += r.DiscountAmt;
                TotalTradeProfitReport.TotalChangeCostAmt += r.Cost /*+ (int)(r.CardFee)*/;
                TotalTradeProfitReport.TotalChangeCashCouponAmt += -r.CashCoupon;
            }
        }

        private void CalculateTotalRewardProfit()
        {
            TotalRewardReport.RewardAmountSum = RewardReportCollection.Sum(c => c.RewardAmount);
            TotalRewardReport.RewardAmount = -RewardReportCollection.Sum(c => c.RewardAmount);
        }

        private void CalculateTotalStockTaking()
        {
            TotalStockTakingReport.Count = StockTakingReportCollection.Sum(c => c.Count);
            TotalStockTakingReport.Price = StockTakingReportCollection.Sum(c => c.Price);
        }

        private void CalculateTotalStockTakingOTC()
        {
            TotalStockTakingOTCReport.Count = StockTakingOTCReportCollection.Sum(c => c.Count);
            TotalStockTakingOTCReport.Price = StockTakingOTCReportCollection.Sum(c => c.Price);
        }

        private void CalculateSelfPrescriptionProfit()
        {
            SelfPrescriptionProfitReport = new PrescriptionProfitReport();
            foreach (var r in SelfPrescriptionProfitReportCollection)
            {
                SelfPrescriptionProfitReport.Count += r.Count;
                SelfPrescriptionProfitReport.MedicalServicePoint += r.MedicalServicePoint;
                SelfPrescriptionProfitReport.MedicinePoint += r.MedicinePoint;
                SelfPrescriptionProfitReport.PaySelfPoint += r.PaySelfPoint;
                SelfPrescriptionProfitReport.MedUse += r.MedUse;
                SelfPrescriptionProfitReport.Profit += r.Profit;
            }
            SelfPrescriptionProfitReport.TotalMed = SelfPrescriptionProfitReport.MedicalServicePoint + SelfPrescriptionProfitReport.MedicinePoint;
        }

        private void CalculateSelfPrescriptionChange()
        {
            SelfPrescriptionChangeReport = new PrescriptionProfitReport();
            foreach (var r in SelfPrescriptionChangeProfitReportCollection)
            {
                SelfPrescriptionChangeReport.Count += r.Count;
                SelfPrescriptionChangeReport.MedicalServicePoint += r.MedicalServicePoint;
                SelfPrescriptionChangeReport.MedicinePoint += r.MedicinePoint;
                SelfPrescriptionChangeReport.PaySelfPoint += r.PaySelfPoint;
                SelfPrescriptionChangeReport.MedUse += r.MedUse;
                SelfPrescriptionChangeReport.Profit += r.Profit;
            }
            SelfPrescriptionChangeReport.TotalMed = SelfPrescriptionChangeReport.MedUse;
        }

        private void CalculateCooperativePrescriptionProfit()
        {
            CooperativePrescriptionProfitReport = new PrescriptionProfitReport();
            foreach (var r in CooperativePrescriptionProfitReportCollection)
            {
                CooperativePrescriptionProfitReport.Count += r.Count;
                CooperativePrescriptionProfitReport.MedicalServicePoint += r.MedicalServicePoint;
                CooperativePrescriptionProfitReport.MedicinePoint += r.MedicinePoint;
                CooperativePrescriptionProfitReport.PaySelfPoint += r.PaySelfPoint;
                CooperativePrescriptionProfitReport.MedUse += r.MedUse;
                CooperativePrescriptionProfitReport.Profit += r.Profit;
            }
            CooperativePrescriptionProfitReport.TotalMed = CooperativePrescriptionProfitReport.MedicinePoint + CooperativePrescriptionProfitReport.MedicalServicePoint;
        }

        private void CalculateCooperativeChangePrescriptionProfit()
        {
            CooperativePrescriptionChangeReport = new PrescriptionProfitReport();
            foreach (var r in CooperativePrescriptionChangeProfitReportCollection)
            {
                CooperativePrescriptionChangeReport.Count += r.Count;
                CooperativePrescriptionChangeReport.MedicalServicePoint += r.MedicalServicePoint;
                CooperativePrescriptionChangeReport.MedicinePoint += r.MedicinePoint;
                CooperativePrescriptionChangeReport.PaySelfPoint += r.PaySelfPoint;
                CooperativePrescriptionChangeReport.MedUse += r.MedUse;
                CooperativePrescriptionChangeReport.Profit += r.Profit;
            }
            CooperativePrescriptionChangeReport.TotalMed = CooperativePrescriptionChangeReport.MedUse;
        }

        private void CalculateCoopCashProfit()
        {
            CoopCashProfitReport = new CashReport();
            foreach (var r in CoopCashProfitReportCollection)
            {
                CoopCashProfitReport.AllPaySelfPrice += r.AllPaySelfPrice;
                CoopCashProfitReport.PaySelfPrice += r.PaySelfPrice;
            }
            CoopCashProfitReport.TotalPrice = CoopCashProfitReport.AllPaySelfPrice + CoopCashProfitReport.PaySelfPrice;
        }

        private void CalculateCashProfit()
        {
            CashProfitReport = new CashReport();
            foreach (var r in CashProfitReportCollection)
            {
                CashProfitReport.AllPaySelfPrice += r.AllPaySelfPrice;
                CashProfitReport.PaySelfPrice += r.PaySelfPrice;
            }
            CashProfitReport.TotalPrice = CashProfitReport.AllPaySelfPrice + CashProfitReport.PaySelfPrice;
        }

        private void CalculateTotal()
        {
            TotalTradeProfitReport.TotalCostTotalAmt = TotalTradeProfitReport.TotalDeleteCostAmt + TotalTradeProfitReport.TotalChangeCostAmt;
            TotalTradeProfitReport.TotalCashTotalAmt = TotalTradeProfitReport.TotalDeleteCashAmt + TotalTradeProfitReport.TotalChangeCashAmt;
            TotalTradeProfitReport.TotalCardTotalAmt = TotalTradeProfitReport.TotalDeleteCardAmt + TotalTradeProfitReport.TotalChangeCardAmt;
            TotalTradeProfitReport.TotalDiscountTotalAmt = TotalTradeProfitReport.TotalDeleteDiscountAmt + TotalTradeProfitReport.TotalChangeDiscountAmt;
            TotalTradeProfitReport.TotalCashCouponTotalAmt = TotalTradeProfitReport.TotalDeleteCashCouponAmt + TotalTradeProfitReport.TotalChangeCashCouponAmt;

            TotalCashFlow.TotalOTCChange = TotalTradeProfitReport.TotalDeleteAmt + TotalTradeProfitReport.TotalChangeAmt;


            TotalCashFlow.TotalOTCCash = TotalTradeProfitReport.CashAmount + TotalTradeProfitReport.TotalCashTotalAmt;
            TotalCashFlow.TotalOTCCard = TotalTradeProfitReport.CardAmount + TotalTradeProfitReport.TotalCardTotalAmt;
            TotalCashFlow.TotalOTCTicket = TotalTradeProfitReport.DiscountAmt + TotalTradeProfitReport.TotalDiscountTotalAmt;
            TotalCashFlow.TotalOTCCashTicket = TotalTradeProfitReport.CashCoupon + TotalTradeProfitReport.TotalCashCouponTotalAmt;
            

            TotalCashFlow.TotalMedCoop = CooperativePrescriptionProfitReport.TotalMed + CooperativePrescriptionProfitReport.MedUse + CoopCashProfitReport.TotalPrice + CooperativePrescriptionChangeReport.Profit;
            TotalCashFlow.TotalMedNotCoop = SelfPrescriptionChangeReport.Profit + SelfPrescriptionProfitReport.TotalMed + SelfPrescriptionProfitReport.MedUse + CashProfitReport.TotalPrice;





            DiscountAmt = -TotalTradeProfitReport.DiscountAmt;
            TotalCashFlow.TotalOTCAmount = TotalTradeProfitReport.Cost + TotalTradeProfitReport.TotalCostTotalAmt + (double)TotalStockTakingOTCReport.Price + (double)TotalRewardReport.RewardAmount + (double)DiscountAmt;

            InventoryDifference.InventoryTotal = (double)(InventoryDifference.InventoryOverage + InventoryDifference.InventoryShortage + InventoryDifference.InventoryScrap);
            TotalCashFlow.TotalOTC = TotalTradeProfitReport.Profit + (int)TotalStockTakingOTCReport.Price + DiscountAmt + TotalRewardReport.RewardAmount + TotalCashFlow.TotalOTCChange;
            TotalCashFlow.TotalMedProfit = CooperativePrescriptionProfitReport.TotalMed + SelfPrescriptionProfitReport.TotalMed;
            TotalCashFlow.TotalMedUse = CooperativePrescriptionProfitReport.MedUse + SelfPrescriptionProfitReport.MedUse;
            TotalCashFlow.TotalMedCash = /*TotalCashFlow.CopayMentPrice +*/ TotalCashFlow.PaySelfPrice + TotalCashFlow.AllPaySelfPrice /*+ TotalCashFlow.DepositPrice*/ + TotalCashFlow.OtherPrice;
            TotalCashFlow.TotalMedChange = SelfPrescriptionChangeReport.TotalMed + CooperativePrescriptionChangeReport.TotalMed;
            TotalCashFlow.TotalMed = TotalCashFlow.TotalMedCash + TotalCashFlow.TotalMedUse + TotalCashFlow.TotalMedProfit + (double)TotalStockTakingReport.Price + TotalCashFlow.TotalMedChange;
            TotalCashFlow.Total = (double)(TotalCashFlow.TotalOTC + TotalCashFlow.TotalMed + (double)ExtraMoney);
        }

        private void AdjustCaseFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is PrescriptionDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            PrescriptionDetailReport indexitem = ((PrescriptionDetailReport)e.Item);
            if (CoopSelectItem == indexitem.InsName)
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
            else if (CoopSelectItem == "全部")
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

        private void CoopFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is PrescriptionDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            PrescriptionDetailReport indexitem = ((PrescriptionDetailReport)e.Item);
            if (CoopSelectItem == indexitem.InsName)
                e.Accepted = true;
            else if (CoopSelectItem == "全部")
                e.Accepted = true;
        }

        private void CashCoopFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is CashDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            CashDetailReport indexitem = ((CashDetailReport)e.Item);
            if (CashCoopSelectItem == indexitem.Ins_Name)
                e.Accepted = true;
            else if (CashCoopSelectItem == "全部")
                e.Accepted = true;
        }

        private void OTCChangeFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is TradeProfitDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            TradeProfitDetailReport indexitem = ((TradeProfitDetailReport)e.Item);
            if (TradeChangeSelectItem == indexitem.TypeId)
            {
                e.Accepted = true;
            }
            else if (TradeChangeSelectItem == "全部")
            {
                e.Accepted = true;
            }
        }

        private void StockTakingDetailFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is StockTakingDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            StockTakingDetailReport indexitem = ((StockTakingDetailReport)e.Item);
            if (StockTakingSelectItem == indexitem.Type)
            {
                e.Accepted = true;
            }
            else if (StockTakingSelectItem == "全部")
            {
                e.Accepted = true;
            }
        }

        private void StockTakingOTCDetailFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is StockTakingOTCDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            StockTakingOTCDetailReport indexitem = ((StockTakingOTCDetailReport)e.Item);
            if (StockTakingOTCSelectItem == indexitem.Type)
            {
                e.Accepted = true;
            }
            else if (StockTakingOTCSelectItem == "全部")
            {
                e.Accepted = true;
            }
        }

        #endregion Action
    }
}