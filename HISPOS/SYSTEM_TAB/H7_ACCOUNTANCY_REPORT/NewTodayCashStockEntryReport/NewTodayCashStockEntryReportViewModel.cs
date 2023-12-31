﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using His_Pos.NewClass.Report.DepositReport;
using System.Diagnostics;
using System.Windows.Forms;
using ClosedXML.Excel;
using His_Pos.FunctionWindow;
using His_Pos.Class;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.NewTodayCashStockEntryReport
{
    public class NewTodayCashStockEntryReportViewModel : TabBase
    {
        #region Variables
        public readonly string schema;

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

        private List<string> adjustCaseString = new List<string>() { "全部", "一般箋", "慢箋", "自費調劑" };

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

        private string cashcoopSelectItem = "全部";

        public string CashCoopSelectItem
        {
            get => cashcoopSelectItem;
            set
            {
                Set(() => CashCoopSelectItem, ref cashcoopSelectItem, value);

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

        public SelectAdjustCaseType AdjustCaseSelectItem { get; set; }


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

        private PrescriptionDetailReport prescriptionDetailReportSumMain;

        public PrescriptionDetailReport PrescriptionDetailReportSumMain
        {
            get => prescriptionDetailReportSumMain;
            set
            {
                Set(() => PrescriptionDetailReportSumMain, ref prescriptionDetailReportSumMain, value);
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

        private DepositReportDataList _depositReportDataSumMain;

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
        DataTable PrescriptionAllDataTable = new DataTable();

        private ReportDetailType _currentDetailType;

        #endregion Variables

        #region Command


        public RelayCommand CooperativePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CashDetailMouseDoubleClickCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand CashDetailClickCommand { get; set; }
        public RelayCommand PrescriptionDetailClickCommand { get; set; }
        public RelayCommand PrescriptionDetailDoubleClickCommand { get; set; }
        public RelayCommand PrescriptionDetailMedicineDoubleClickCommand { get; set; }
        public RelayCommand TradeProfitDetailMedicineDoubleClickCommand { get; set; }
        public RelayCommand StockTakingOTCDetailMedicineDoubleClickCommand { get; set; }

        public RelayCommand PrintPrescriptionProfitDetailCommand { get; set; }
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
        public RelayCommand TradeProfitIcomeReportSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitCostCostReportSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitAllReportSelectionChangedCommand { get; set; }
        public RelayCommand AllPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand TradeProfitTicketReportSelectionChangedCommand { get; set; }
        public RelayCommand PrintTradeProfitDetailCommand { get; set; }
        public RelayCommand PrintTradeCrashProfitCommand { get; set; }

        public RelayCommand<ReportDetailType> ViewReportDetailCommand { get; set; }
        #endregion Command

        public NewTodayCashStockEntryReportViewModel(DateTime startDate, DateTime endDate, string schema)
        {
            InitCommands();

            StartDate = startDate;
            EndDate = endDate;

            if (schema == string.Empty)
                this.schema = Properties.Settings.Default.SystemSerialNumber;
            else
                this.schema = schema;

            GetData();

            PrescriptionDetailReportCollection = new PrescriptionDetailReports(PrescriptionAllDataTable);
            AdjustCaseSelectItem = SelectAdjustCaseType.Chronic;
            SumPrescriptionDetailReport(PrescriptionDetailReportCollectionALL);
        }

        public NewTodayCashStockEntryReportViewModel()
        {
            schema = Properties.Settings.Default.SystemSerialNumber;

            InitCommands();

            GetData();
        }

        private void InitCommands()
        {
            SearchCommand = new RelayCommand(GetData);

            AllPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionAction);
            SelfNormalPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionAction);

            SelfSlowPrescriptionSelectionChangedCommand = new RelayCommand(SelfSlowPrescriptionSelectionChangedAction);
            SelfSelfPrescriptionSelectionChangedCommand = new RelayCommand(SelfSelfPrescriptionSelectionChangedAction);
            CooperativePrescriptionSelectionChangedCommand = new RelayCommand(CooperativePrescriptionSelectionChangedAction);

            CashDetailClickCommand = new RelayCommand(CashDetailClickAction);

            PrescriptionDetailClickCommand = new RelayCommand(PrescriptionDetailClickAction);
            PrescriptionDetailDoubleClickCommand = new RelayCommand(PrescriptionDetailDoubleClickAction);
            PrescriptionDetailMedicineDoubleClickCommand = new RelayCommand(PrescriptionDetailMedicineDoubleClickAction);
            TradeProfitDetailMedicineDoubleClickCommand = new RelayCommand(TradeProfitDetailMedicineDoubleClickAction);
            StockTakingOTCDetailMedicineDoubleClickCommand = new RelayCommand(StockTakingDetailDoubleClickAction);

            PrintPrescriptionProfitDetailCommand = new RelayCommand(PrintPrescriptionProfitDetailAction);

            AllDepositReportSelectionChangedCommand = new RelayCommand(AllDepositReportSelectionChangedAction);
            DepositDetailClickCommand = new RelayCommand(DepositDetailClickAction);
            DepositDetailDoubleClickCommand = new RelayCommand(DepositDetailDoubleClickAction);

            StockTakingDetailClickCommand = new RelayCommand(StockTakingDetailClickAction);
            CashDetailMouseDoubleClickCommand = new RelayCommand(CashDetailMouseDoubleClickAction);
            StockTakingOTCReportSelectionChangedCommand = new RelayCommand(StockTakingOTCReportSelectionChangedAction);
            StockTakingOTCDetailClickCommand = new RelayCommand(StockTakingOTCDetailClickAction);

            TradeProfitReportSelectionChangedCommand = new RelayCommand(TradeProfitReportSelectionChangedAction);
            TradeProfitIcomeReportSelectionChangedCommand = new RelayCommand(TradeProfitIncomeReportSelectionChangedAction);
            TradeProfitCostCostReportSelectionChangedCommand =
                new RelayCommand(TradeProfitCostCostReportSelectionChangedAction);
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
            PrintTradeCrashProfitCommand = new RelayCommand(PrintTradeCrashProfitAction);
            ViewReportDetailCommand = new RelayCommand<ReportDetailType>(ViewReportDetailAction);
        }

        private void ViewReportDetailAction(ReportDetailType type)
        {
            _currentDetailType = type;
            switch (type)
            {

                //全部處方
                case ReportDetailType.AllPrescription_Count:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    AdjustCaseSelectItem = SelectAdjustCaseType.ALL;
                    SelfPrescriptionAction();
                    break;
                case ReportDetailType.AllPrescription_Income:
                    CostVis = Visibility.Collapsed;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.ALL;
                    SelfPrescriptionAction();
                    break;
                case ReportDetailType.AllPrescription_Cost:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Collapsed;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.ALL;
                    SelfPrescriptionAction();
                    break;
                case ReportDetailType.AllPrescription_Change:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    CoopVis = Visibility.Collapsed;

                    AdjustCaseSelectItem = SelectAdjustCaseType.ALL;
                    RefreshPrescriptionReportView();
                    break;
                case ReportDetailType.AllPrescription_StockTaking:
                    GetAllPrescriptionStockTaking();
                    break;


                //合作
                case ReportDetailType.CoopPrescription_Count:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    CooperativePrescriptionSelectionChangedAction();
                    break;

                case ReportDetailType.CoopPrescription_Income:
                    CostVis = Visibility.Collapsed;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Collapsed;
                    CooperativePrescriptionSelectionChangedAction();
                    break;

                case ReportDetailType.CoopPrescription_Cost:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Collapsed;
                    ProfitVis = Visibility.Collapsed;
                    CooperativePrescriptionSelectionChangedAction();
                    break;

                case ReportDetailType.CoopPrescription_Change:
                    CoopVis = Visibility.Visible;
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

                    PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionCoopChangeDetailReportCollectionChanged };
                    PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                    AdjustCaseSelectItem = SelectAdjustCaseType.ALL;
                    PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                    SumCoopChangePrescriptionDetailReport();

                    StockTakingSelectedItem = null;
                    break;

                //慢箋
                case ReportDetailType.Chironic_Count:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Chronic;
                    SelfSlowPrescriptionSelectionChangedAction();
                    break;

                case ReportDetailType.Chironic_Income:
                    CostVis = Visibility.Collapsed;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Chronic;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.Chironic_Cost:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Collapsed;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Chronic;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.Chironic_Change:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    CoopVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Chronic;
                    RefreshPrescriptionReportView();
                    break;

                //一般處方
                case ReportDetailType.NormalPrescription_Count:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Normal;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.NormalPrescription_Income:
                    CostVis = Visibility.Collapsed;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Normal;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.NormalPrescription_Cost:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Collapsed;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Normal;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.NormalPrescription_Change:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    CoopVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Normal;
                    RefreshPrescriptionReportView();
                    break;

                //配藥
                case ReportDetailType.Prescribtion_Count:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Presribtion;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.Prescribtion_Income:
                    CostVis = Visibility.Collapsed;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Presribtion;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.Prescribtion_Cost:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Collapsed;
                    ProfitVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Presribtion;
                    SelfPrescriptionAction();
                    break;

                case ReportDetailType.Prescribtion_Change:
                    CostVis = Visibility.Visible;
                    IncomeVis = Visibility.Visible;
                    ProfitVis = Visibility.Visible;
                    CoopVis = Visibility.Collapsed;
                    AdjustCaseSelectItem = SelectAdjustCaseType.Presribtion;
                    RefreshPrescriptionReportView();
                    break;
            }
        }

        private void GetAllPrescriptionStockTaking()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.StockTaking;

            StockTakingString = new List<string>() { "全部" };
            StockTakingString.AddRange(StockTakingDetailReportCollection.Select(x => x.Type).Distinct());

            StockTakingDetailReportViewSource = new CollectionViewSource { Source = StockTakingDetailReportCollection };
            StockTakingDetailReportView = StockTakingDetailReportViewSource.View;
            StockTakingSelectItem = "全部";
            StockTakingDetailReportViewSource.Filter += StockTakingDetailFilter;

            SumStockTakingDetailReport();
            StockDetailCount = StockTakingDetailReportCollection.Count();
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

            var data = ReportService.GetPrescriptionDetailMedicineReportById(SelectedDepositDetailReport.PremasID,null,null);
            PrescriptionDetailMedicineRepotCollection = new ObservableCollection<PrescriptionDetailMedicineRepot>(data);
        }

        private void RewardExcelAction()
        {
            PrintService.PrintRewardDetail(StartDate, EndDate);
        }

        private void PrintTradeProfitDetailAction()
        {
            //PrintService.PrintTradeProfitDetail(StartDate, EndDate);
            PrintService.PrintTradeProfitDetail(TradeProfitDetailReportCollection, StartDate, EndDate);
        }

        private void PrintTradeCrashProfitAction()
        {
            DataTable table = CashReportDb.GetExportCashData(StartDate, EndDate);
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "調劑現金帳";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "-" + EndDate.ToString("yyyyMMdd") + "調劑現金帳";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {

                AdjustCases cases = ViewModelMainWindow.AdjustCases;
                foreach (DataRow dr in table.Rows)
                {
                    string adjustCase = Convert.ToString(dr["PreMas_AdjustCaseID"]);
                    if (!string.IsNullOrEmpty(adjustCase))
                    {
                        dr["PreMas_AdjustCaseID"] = cases.Where(w => w.ID.Equals(adjustCase)).FirstOrDefault().Name;
                    }
                }
                
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;
                var ws = wb.Worksheets.Add("調劑現金帳");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                ws.Cell("A1").Value = "處方單號";
                ws.Cell("B1").Value = "調劑案件";
                ws.Cell("C1").Value = "醫療院所";
                ws.Cell("D1").Value = "姓名";
                ws.Cell("E1").Value = "部分負擔";
                ws.Cell("F1").Value = "自費";
                ws.Cell("G1").Value = "配藥";
                ws.Cell("H1").Value = "押金";
                ws.Cell("I1").Value = "其他";

                var rangeWithData = ws.Cell(2, 1).InsertData(table.AsEnumerable());
                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Columns().AdjustToContents();//欄位寬度根據資料調整
                wb.SaveAs(fdlg.FileName);
                MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
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
            PrintService.PrintPrescriptionProfitDetailAction(StartDate, EndDate, PrescriptionDetailReportView);
        }

        private void StockTakingDetailDoubleClickAction()
        {
            if (StockTakingOTCDetailMedicineReportSelectItem is null) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { StockTakingOTCDetailMedicineReportSelectItem.ID, "0" }, "ShowProductDetail"));
        }

        private void TradeProfitDetailMedicineDoubleClickAction()
        {
            if (TradeProfitDetailMedicineReportSelectItem is null) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { TradeProfitDetailMedicineReportSelectItem.ProductID, "0" }, "ShowProductDetail"));
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
            DataTable result = ReportService.TradeRecordQuery(TradeID);

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

            DataTable result = ReportService.TradeRecordQuery(TradeProfitDetailReportSelectItem.Id.ToString());
            DataRow masterRow = result.Rows[0];
            result.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in result.Rows)
            {
                string ogTransTime = dr["TraMas_ChkoutTime"].ToString();
                DateTime dt = DateTime.Parse(ogTransTime);
                //CultureInfo culture = new CultureInfo("zh-TW");
                //culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                //dr["TransTime_Format"] = dt.ToString("yyy/MM/dd", culture);
                dr["TransTime_Format"] = dt.ToString("yyyy/MM/dd HH:mm");
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


            bool isChangedTab = _currentDetailType == ReportDetailType.AllPrescription_Change ||
                                _currentDetailType == ReportDetailType.Chironic_Change ||
                                _currentDetailType == ReportDetailType.CoopPrescription_Change ||
                                _currentDetailType == ReportDetailType.Prescribtion_Change ||
                                _currentDetailType == ReportDetailType.NormalPrescription_Change;

            var data = isChangedTab
                ? ReportService.GetPrescriptionDetailMedicineReportById(PrescriptionDetailReportSelectItem.Id, StartDate, EndDate)
                : ReportService.GetPrescriptionDetailMedicineReportById(PrescriptionDetailReportSelectItem.Id, null, null);
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

            var data
                = ReportService.GetStockTakingDetailRecordByDate(StockTakingDetailReportSelectItem.InvRecSourceID, StartDate, EndDate);
            StockTakingDetailRecordReportCollection = new ObservableCollection<StockTakingDetailRecordReport>(data);
        }

        private void StockTakingOTCDetailClickAction()
        {
            if (StockTakingOTCDetailReportSelectItem is null)
            {
                StockTakingOTCDetailRecordReportCollection.Clear();
                return;
            }

            var data
                = ReportService.GetStockTakingDetailRecordByDate(StockTakingOTCDetailReportSelectItem.InvRecSourceID, StartDate, EndDate);
            StockTakingOTCDetailRecordReportCollection = new ObservableCollection<StockTakingDetailRecordReport>(data);
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

            var tempCollection = TradeProfitDetailReportCollection.Where(p => (p.TypeId == "1"));
            SumOTCReportMainChanged(tempCollection);
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
                TradeChangeSelectItem = "全部";
                TradeProfitDetailReportViewSource.Filter += OTCChangeFilter;

                var tempCollection = TradeProfitDetailReportCollectionChanged.Where(p => (p.TypeId != "1"));
                SumOTCReportMainChanged(tempCollection);
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

            TradeDetailReportSum.TotalChange = TradeProfitDetailReportCollectionChanged.Where(p => (p.TypeId != "1")).Sum(s => s.Profit);

            TradeDetailCount = TradeProfitDetailReportCollection.Count();
            TradeEmpDetailCount = TradeProfitDetailEmpReportCollection.Count();
            EmpProfit = TradeProfitDetailEmpReportCollection.Sum(e => e.Profit);

        }

        private void AllDepositReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.Deposit;
        }



        private void StockTakingOTCReportSelectionChangedAction()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.OTCStockTaking;

            var DistinctItems = StockTakingOTCDetailReportCollection.Select(_ => _.Type).Distinct();
            StockTakingOTCString = new List<string>() { "全部" };
            StockTakingOTCString.AddRange(DistinctItems);


            StockTakingOTCDetailReportViewSource = new CollectionViewSource { Source = StockTakingOTCDetailReportCollection };
            StockTakingOTCDetailReportView = StockTakingOTCDetailReportViewSource.View;
            StockTakingOTCSelectItem = "全部";
            StockTakingOTCDetailReportViewSource.Filter += StockTakingOTCDetailFilter;
            SumStockTakingOTCDetailReport();
            StockOTCDetailCount = StockTakingOTCDetailReportCollection.Count();

        }


        private void CooperativePrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Visible;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;

            AdjustCaseSelectItem = SelectAdjustCaseType.Cooperative;
            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;

            var tempCollection = GetPrescriptionDetailReportsByType(PrescriptionDetailReportCollection);

            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection);

            StockTakingSelectedItem = null;
        }

        /******* 各種箋明細 *******/
      
        private void SelfPrescriptionAction()
        {
            ResetPrescriptionUI();
            SumPrescriptionDetailReport(PrescriptionDetailReportCollectionALL);
        }

        private void ResetPrescriptionUI()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;

            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;


            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        private void SelfSlowPrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            
            IsBusy = true;
            BusyContent = "報表查詢中";
            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
            Task.Run(() =>
            {
                PrescriptionDetailReportCollection = new PrescriptionDetailReports(PrescriptionAllDataTable);

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    PrescriptionDetailReportViewSource = new CollectionViewSource
                        { Source = PrescriptionDetailReportCollection };
                    PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;

                    AdjustCaseSelectItem = SelectAdjustCaseType.Chronic;
                    PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                    SumPrescriptionDetailReport(PrescriptionDetailReportCollectionALL);
                    IsBusy = false;
                });
            });
        }

        private void SelfSelfPrescriptionSelectionChangedAction()
        {
            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "報表查詢中";
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                AdjustCaseSelectItem = SelectAdjustCaseType.Presribtion;
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport(PrescriptionDetailReportCollectionALL);
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        /******* 各種箋調整明細 *******/

        private void RefreshPrescriptionReportView()
        {
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollectionChanged };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;

            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            SumPrescriptionDetailReport(PrescriptionDetailReportCollectionChanged);

            CooperativePrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }

        /******* 各種箋調整明細 *******/
        /******* 各種箋明細 *******/


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
                //MainWindow.ServerConnection.OpenConnection();
                //BusyContent = "報表查詢中";
                //RewardDetailReportCollection = new RewardDetailReports("0", StartDate, EndDate);

                //MainWindow.ServerConnection.CloseConnection();

                RewardDetailReportCollection = new RewardDetailReports(Ds.Tables[12]);


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
            TradeDetailReportSum = new TradeProfitDetailReport();
            StockTakingDetailReportSum = new StockTakingDetailReport();
            StockTakingOTCDetailReportSum = new StockTakingOTCDetailReport();

            BusyContent = "報表查詢中";

            Ds = ReportService.TodayCashStockEntryReport(schema, StartDate, EndDate);

            PrescriptionAllDataTable = new DataTable();
            PrescriptionAllDataTable.Merge(Ds.Tables[0]);
            PrescriptionAllDataTable.Merge(Ds.Tables[2]);
            PrescriptionAllDataTable.Merge(Ds.Tables[4]);
            PrescriptionAllDataTable.Merge(Ds.Tables[6]);
            PrescriptionDetailReportCollectionALL = new PrescriptionDetailReports(PrescriptionAllDataTable);

            DataTable ALLCHANGE = new DataTable();
            ALLCHANGE.Merge(Ds.Tables[1]);
            ALLCHANGE.Merge(Ds.Tables[3]);
            ALLCHANGE.Merge(Ds.Tables[5]);
            ALLCHANGE.Merge(Ds.Tables[7]);
            PrescriptionDetailReportCollectionChanged = new PrescriptionDetailReports(ALLCHANGE);
            PrescriptionDetailReportSumMain.SumPrescriptionChangeDetail(PrescriptionDetailReportCollectionChanged);

            TradeProfitDetailReportCollection = new TradeProfitDetailReports(Ds.Tables[10]);
            TradeProfitDetailEmpReportCollection = new TradeProfitDetailEmpReports(Ds.Tables[13]);
            TradeProfitDetailReportCollectionChanged = new TradeProfitDetailReports(Ds.Tables[11]);
            PrescriptionCoopChangeDetailReportCollectionChanged = new PrescriptionDetailReports(Ds.Tables[1]);


            PrescriptionDetailReportCollection = new PrescriptionDetailReports(Ds.Tables[4]);
            StockTakingOTCDetailReportCollection = new StockTakingOTCDetailReports(Ds.Tables[9]);
            StockTakingDetailReportCollection = new StockTakingDetailReports(Ds.Tables[8]);

            PrescriptionDetailReports tempCooperativePres = new PrescriptionDetailReports(Ds.Tables[0]);
            PrescriptionDetailReportSumMain.CoopCount = tempCooperativePres.Count();
            PrescriptionDetailReportSumMain.CoopMeduse = (int)tempCooperativePres.Sum(s => s.Meduse);
            PrescriptionDetailReportSumMain.CoopVirtualMeduse = tempCooperativePres.Sum(s => s.VirtualMeduse);
            PrescriptionDetailReportSumMain.CoopIncome = (int)tempCooperativePres.Sum(s => s.MedicalPoint) +
                                                         (int)tempCooperativePres.Sum(s => s.MedicalServicePoint) +
                                                         (int)tempCooperativePres.Sum(s => s.PaySelfPoint);

            DataTable depositTable = Ds.Tables[14];
            DepositReportDataSumMain = new DepositReportDataList(depositTable);
            DepositDetailReportCollection.Clear();
            foreach (DataRow row in depositTable.Rows)
            {
                DepositDetailReportCollection.Add(new DepositReportData(row));
            }


            TradeProfitReportSelectionChangedActionMain();
            TradeChangeReportSelectionChangedActionMain();
            SumCoopChangePrescriptionDetailReport();
            StockTakingOTCReportSelectionChangedAction();
            CalculateTotalRewardProfit();
            TradeProfitAllReportSelectionChangedAction();
            GetAllPrescriptionStockTaking();

            CoopVis = Visibility.Collapsed;
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            AdjustCaseSelectItem = SelectAdjustCaseType.ALL;

            PrescriptionDetailReportCollection = new PrescriptionDetailReports(PrescriptionAllDataTable);

            PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
            PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;


            PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
            PrescriptionDetailReportSumMain.SumPrescriptionDetail(PrescriptionDetailReportCollection);

            TradeDetailReportSum.TotalProfit = TradeDetailReportSum.RealTotal + TradeDetailReportSum.TotalCost +
                                               TradeDetailReportSum.TotalChange + StockTakingOTCDetailReportSum.Price +
                                               TradeDetailReportSum.DiscountAmtMinus + (int)TotalRewardReport.RewardAmount;
            PrescriptionDetailReportSumMain.SumMedProfit(StockTakingDetailReportSum);

            SumAllProfit();

            //清空下方處方詳細
            PrescriptionDetailReportView = null;
            PrescriptionDetailMedicineRepotCollection.Clear();
        }


        private void SumPrescriptionDetailReport(PrescriptionDetailReports reports)
        {
            var tempCollection = GetPrescriptionDetailReportsByType(reports);

            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection);
        }


        private void SumCoopChangePrescriptionDetailReport()
        {
            var tempCollection = GetPrescriptionDetailReportsByType(PrescriptionCoopChangeDetailReportCollectionChanged);

            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.SumCoopChangePrescriptionDetail(tempCollection);

            PrescriptionDetailReportSumMain.CoopChange = (decimal)PrescriptionDetailReportSum.MedicalPoint + (decimal)PrescriptionDetailReportSum.MedicalServicePoint + (decimal)PrescriptionDetailReportSum.PaySelfPoint + PrescriptionDetailReportSum.Meduse;
            PrescriptionDetailReportSumMain.CoopProfit = (int)((decimal)PrescriptionDetailReportSumMain.CoopIncome +
                                                               PrescriptionDetailReportSumMain.CoopMeduse +
                                                               PrescriptionDetailReportSumMain.CoopChange);
        }

        private IEnumerable<PrescriptionDetailReport> GetPrescriptionDetailReportsByType(PrescriptionDetailReports input)
        {
            IEnumerable<PrescriptionDetailReport> result = input.ToList();
            switch (AdjustCaseSelectItem)
            {
                case SelectAdjustCaseType.Normal:
                    return input.Where(p => p.IsCooperative == false && (p.AdjustCaseID == "1" || p.AdjustCaseID == "3"));
                case SelectAdjustCaseType.Chronic:
                    return input.Where(p => p.IsCooperative == false && p.AdjustCaseID == "2");
                case SelectAdjustCaseType.Presribtion:
                    return input.Where(p => p.IsCooperative == false && p.AdjustCaseID == "0");
                case SelectAdjustCaseType.ALL:
                    return input;
                case SelectAdjustCaseType.Cooperative:
                    return input.Where(p => p.IsCooperative == true);
            }

            return result;
        }

        private void SumOTCReportMainChanged(IEnumerable<TradeProfitDetailReport> reports)
        {
            TradeDetailReportSum.SumOTCReport(reports);
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


        private void SumAllProfit()
        {
            TotalCashFlow.AllCount = TradeDetailReportSum.TotalCount + PrescriptionDetailReportSumMain.MedTotalCount;
            TotalCashFlow.AllIncome = (int)(TradeDetailReportSum.RealTotal + PrescriptionDetailReportSumMain.MedTotalIncome);

            TotalCashFlow.AllCost = (int)(TradeDetailReportSum.TotalCost + PrescriptionDetailReportSumMain.MedTotalMeduse);
            TotalCashFlow.AllChange = (int)(TradeDetailReportSum.TotalChange + PrescriptionDetailReportSumMain.MedTotalChange);

            TotalCashFlow.AllStock = StockTakingOTCDetailReportSum.Price + StockTakingDetailReportSum.Price;


            TotalCashFlow.AllDeposit = DepositReportDataSumMain.NormalDeposit +
                                       DepositReportDataSumMain.ChronicDeposit +
                                       DepositReportDataSumMain.CooperativeDeposit + DepositReportDataSumMain.PrescribeDeposit;

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
            //RewardReportCollection = new RewardReports(schema, StartDate, EndDate);
            //TotalRewardReport.RewardAmountSum = -RewardReportCollection.Sum(c => c.RewardAmount);
            //TotalRewardReport.RewardAmount = -RewardReportCollection.Sum(c => c.RewardAmount);
            RewardDetailReportCollection = new RewardDetailReports(Ds.Tables[12]);
            TotalRewardReport.RewardAmountSum = RewardDetailReportCollection.Sum(c => c.RewardAmount);
            TotalRewardReport.RewardAmount = RewardDetailReportCollection.Sum(c => c.RewardAmount);
        }

        private void AdjustCaseFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is PrescriptionDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            PrescriptionDetailReport indexitem = ((PrescriptionDetailReport)e.Item);

            if (AdjustCaseSelectItem == SelectAdjustCaseType.Normal && indexitem.IsCooperative == false && (indexitem.AdjustCaseID == "1" || indexitem.AdjustCaseID == "3"))
                e.Accepted = true;
            else if (AdjustCaseSelectItem == SelectAdjustCaseType.Chronic && indexitem.IsCooperative == false && indexitem.AdjustCaseID == "2")
                e.Accepted = true;
            else if (AdjustCaseSelectItem == SelectAdjustCaseType.Presribtion && indexitem.IsCooperative == false && indexitem.AdjustCaseID == "0")
                e.Accepted = true;
            else if (AdjustCaseSelectItem == SelectAdjustCaseType.Cooperative && indexitem.IsCooperative == true)
                e.Accepted = true;
            else if (AdjustCaseSelectItem == SelectAdjustCaseType.ALL)
                e.Accepted = true;
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

        public enum SelectAdjustCaseType
        {
            ALL,
            Normal, //一般箋
            Chronic, //慢箋
            Presribtion, //自費調劑
            Cooperative //合作
        }

        public enum ReportDetailType
        {

            AllPrescription_Count,  //全部處方-張數
            AllPrescription_Income, //全部處方-收入
            AllPrescription_Cost, //全部處方-耗用
            AllPrescription_Change, //全部處方-調整
            AllPrescription_StockTaking, //全部處方-盤差

            CoopPrescription_Count,  //合作處方-張數
            CoopPrescription_Income, //合作處方-收入
            CoopPrescription_Cost, //合作處方-耗用
            CoopPrescription_Change, //合作處方-調整

            Chironic_Count,  //慢箋處方-張數
            Chironic_Income, //慢箋處方-收入
            Chironic_Cost, //慢箋處方-耗用
            Chironic_Change, //慢箋處方-調整

            NormalPrescription_Count,  //一般箋處方-張數
            NormalPrescription_Income, //一般箋處方-收入
            NormalPrescription_Cost, //一般箋處方-耗用
            NormalPrescription_Change, //一般箋處方-調整

            Prescribtion_Count,  //配藥-張數
            Prescribtion_Income, //配藥-收入
            Prescribtion_Cost, //配藥-耗用
            Prescribtion_Change, //配藥-調整

        }
    }
}