using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report;
using His_Pos.NewClass.Report.CashDetailReport;
using His_Pos.NewClass.Report.CashDetailReport.CashDetailRecordReport;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot;
using His_Pos.NewClass.Report.PrescriptionProfitReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using His_Pos.NewClass.Prescription.Service;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.NewClass.Report.TradeProfitReport;
using His_Pos.NewClass.Report.StockTakingReport;
using His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport;
using His_Pos.NewClass.Report.StockTakingDetailReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport;
using His_Pos.NewClass.Report.ExtraMoneyDetailReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport.ExtraMoneyDetailRecordReport;
using His_Pos.NewClass.Report.StockTakingOTCReport;
using His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingOTCDetailRecordReport;
using His_Pos.NewClass.Report.RewardReport;
using His_Pos.NewClass.Report.RewardDetailReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport.RewardDetailRecordReport;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionDetail;
using System.Data.SqlClient;
using System.Globalization;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport {
    public class CashStockEntryReportViewModel : TabBase {
        #region Variables
        public override TabBase getTab() {
            return this;
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

        private PrescriptionProfitReports selfPrescriptionProfitReportCollection = new PrescriptionProfitReports();
        public PrescriptionProfitReports SelfPrescriptionProfitReportCollection
        {
            get => selfPrescriptionProfitReportCollection;
            set
            {
                Set(() => SelfPrescriptionProfitReportCollection, ref selfPrescriptionProfitReportCollection, value);
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
        public PrescriptionProfitReports TotalPrescriptionProfitReportCollection { get; set; } = new PrescriptionProfitReports();
        private PrescriptionPointEditRecords PrescriptionPointEditRecords{ get; set; } = new PrescriptionPointEditRecords();
        private PrescriptionProfitReport selfPrescriptionProfitReport = new PrescriptionProfitReport();
        public PrescriptionProfitReport SelfPrescriptionProfitReport
        {
            get => selfPrescriptionProfitReport;
            set
            {
                Set(() => SelfPrescriptionProfitReport, ref selfPrescriptionProfitReport, value);
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
        private PrescriptionDetailMedicineRepots  prescriptionDetailMedicineRepotCollection = new PrescriptionDetailMedicineRepots();
        public PrescriptionDetailMedicineRepots PrescriptionDetailMedicineRepotCollection
        {
            get => prescriptionDetailMedicineRepotCollection;
            set
            {
                Set(() => PrescriptionDetailMedicineRepotCollection, ref prescriptionDetailMedicineRepotCollection, value);
            }
        }
        private PrescriptionDetailMedicineRepot prescriptionDetailMedicineRepotSelectItem ;
        public PrescriptionDetailMedicineRepot PrescriptionDetailMedicineRepotSelectItem
        {
            get => prescriptionDetailMedicineRepotSelectItem;
            set
            {
                Set(() => PrescriptionDetailMedicineRepotSelectItem, ref prescriptionDetailMedicineRepotSelectItem, value);
            }
        }
        private CashStockEntryReportEnum cashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
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
        #endregion
        #region Command
        public RelayCommand SelfPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CooperativePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CashSelectionChangedCommand { get; set; }
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
        public RelayCommand ExtraMoneyReportSelectionChangedCommand { get; set; }
        public RelayCommand ExtraMoneyDetailClickCommand { get; set; }
        public RelayCommand<string> ChangeUiTypeCommand { get; set; }

        public RelayCommand RewardReportSelectionChangedCommand { get; set; }
        public RelayCommand RewardDetailClickCommand { get; set; }
        public RelayCommand RewardDetailMedicineDoubleClickCommand { get; set; }

        #endregion
        public CashStockEntryReportViewModel() {
            SearchCommand = new RelayCommand(SearchAction);
            SelfPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionSelectionChangedAction);
            CooperativePrescriptionSelectionChangedCommand = new RelayCommand(CooperativePrescriptionSelectionChangedAction);
            CashSelectionChangedCommand = new RelayCommand(CashSelectionChangedAction);
            CashDetailClickCommand = new RelayCommand(CashDetailClickAction);
            PrescriptionDetailClickCommand = new RelayCommand(PrescriptionDetailClickAction);
            PrescriptionDetailDoubleClickCommand = new RelayCommand(PrescriptionDetailDoubleClickAction);
            PrescriptionDetailMedicineDoubleClickCommand = new RelayCommand(PrescriptionDetailMedicineDoubleClickAction);
            PrintCashPerDayCommand = new RelayCommand(PrintCashPerDayAction);
            PrintPrescriptionProfitDetailCommand = new RelayCommand(PrintPrescriptionProfitDetailAction);
            StockTakingReportSelectionChangedCommand = new RelayCommand(StockTakingReportSelectionChangedAction);
            StockTakingDetailClickCommand = new RelayCommand(StockTakingDetailClickAction);

            StockTakingOTCReportSelectionChangedCommand = new RelayCommand(StockTakingOTCReportSelectionChangedAction);
            StockTakingOTCDetailClickCommand = new RelayCommand(StockTakingOTCDetailClickAction);
            TradeProfitReportSelectionChangedCommand = new RelayCommand(TradeProfitReportSelectionChangedAction);
            TradeProfitDetailClickCommand = new RelayCommand(TradeProfitDetailClickAction);
            ExtraMoneyReportSelectionChangedCommand = new RelayCommand(ExtraMoneyReportSelectionChangedAction);
            ExtraMoneyDetailClickCommand = new RelayCommand(ExtraMoneyDetailClickAction);
            RewardReportSelectionChangedCommand = new RelayCommand(RewardReportSelectionChangedAction);
            RewardDetailClickCommand = new RelayCommand(RewardDetailClickAction);
            RewardDetailMedicineDoubleClickCommand = new RelayCommand(RewardDetailMedicineDoubleClickAction);
            GetData();
            InitCollection();
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
                        double sumMeduse = 0;
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
        private void InitCollection() {
            AdjustCaseString = new List<string>() { "全部", "一般箋", "慢箋", "自費調劑" }; 
        }
        private void PrintCashPerDayAction() {
            if (CashflowSelectedItem is null) 
                return;  
            DataTable table = CashReportDb.GetPerDayDataByDate(StartDate, EndDate, CashflowSelectedItem.TypeId);
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "金流存檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "_" +  CashflowSelectedItem.TypeName + "金流存檔";
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

        private void PrescriptionDetailDoubleClickAction() {
            if (PrescriptionDetailReportSelectItem is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionService.ShowPrescriptionEditWindow(PrescriptionDetailReportSelectItem.Id); 
        }
        private void PrescriptionDetailClickAction() {
            if (PrescriptionDetailReportSelectItem is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionDetailMedicineRepotCollection.GerDataById(PrescriptionDetailReportSelectItem.Id);
        }
        private void CashDetailClickAction() {
            if (CashDetailReportSelectItem is null) {
                CashDetailRecordReportCollection.Clear();
                return;
            } 
            CashDetailRecordReportCollection.GetDateByDate(CashDetailReportSelectItem.Id,StartDate,EndDate);
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
            StockTakingDetailRecordReportCollection.GetDateByDate(StockTakingDetailReportSelectItem.Id, StartDate, EndDate, StockTakingDetailReportSelectItem.Type);
        }
        private void StockTakingOTCDetailClickAction()
        {
            if (StockTakingOTCDetailReportSelectItem is null)
            {
                StockTakingOTCDetailRecordReportCollection.Clear();
                return;
            }
            StockTakingOTCDetailRecordReportCollection.GetDateByDate(StockTakingOTCDetailReportSelectItem.Id, StartDate, EndDate, StockTakingOTCDetailReportSelectItem.Type);
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

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                TradeProfitDetailReportCollection = new TradeProfitDetailReports("0", StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                TradeProfitDetailReportViewSource = new CollectionViewSource { Source = TradeProfitDetailReportCollection };
                TradeProfitDetailReportView = TradeProfitDetailReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumStockTakingDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            /*SelfPrescriptionSelectedItem = null;
            CooperativePrescriptionSelectedItem = null;*/
            CashflowSelectedItem = null;
        }


        private void StockTakingReportSelectionChangedAction()
        {

            /*if (StockTakingSelectedItem is null ) {
                StockTakingDetailReportCollection.Clear();
                StockTakingDetailReportViewSource = new CollectionViewSource { Source = StockTakingDetailReportCollection };
                StockTakingDetailReportView = StockTakingDetailReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumPrescriptionDetailReport();
            }
            if (StockTakingSelectedItem is null)
                return; */
            CashStockEntryReportEnum = CashStockEntryReportEnum.StockTaking;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                StockTakingDetailReportCollection = new StockTakingDetailReports("0", StartDate, EndDate);
                
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                StockTakingDetailReportViewSource = new CollectionViewSource { Source = StockTakingDetailReportCollection };
                StockTakingDetailReportView = StockTakingDetailReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumStockTakingDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync(); 
            /*SelfPrescriptionSelectedItem = null;
            CooperativePrescriptionSelectedItem = null;*/
            CashflowSelectedItem = null;
        }
        private void StockTakingOTCReportSelectionChangedAction()
        {

            /*if (StockTakingOTCSelectedItem is null)
            {
                StockTakingOTCDetailReportCollection.Clear();
                StockTakingOTCDetailReportViewSource = new CollectionViewSource { Source = StockTakingOTCDetailReportCollection };
                StockTakingOTCDetailReportView = StockTakingOTCDetailReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumPrescriptionDetailReport();
            }
            if (StockTakingOTCSelectedItem is null)
                return;*/
            CashStockEntryReportEnum = CashStockEntryReportEnum.OTCStockTaking;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                StockTakingOTCDetailReportCollection = new StockTakingOTCDetailReports("0", StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                StockTakingOTCDetailReportViewSource = new CollectionViewSource { Source = StockTakingOTCDetailReportCollection };
                StockTakingOTCDetailReportView = StockTakingOTCDetailReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumStockTakingDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            /*SelfPrescriptionSelectedItem = null;
            CooperativePrescriptionSelectedItem = null;*/
            CashflowSelectedItem = null;
        }

        private void CashSelectionChangedAction() {
            /*if (CashflowSelectedItem is null)
            {
                CashDetailReportCollection.Clear();
                return;
            }*/
            CashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
            CashDetailReportCollection.GetDataByDate("1", StartDate, EndDate);
            CooperativePrescriptionSelectedItem = null;
            SelfPrescriptionSelectedItem = null;
            StockTakingSelectedItem = null;
        }
        private void CooperativePrescriptionSelectionChangedAction()
        {

            /*if (SelfPrescriptionSelectedItem is null && CooperativePrescriptionSelectedItem is null) { 
                PrescriptionDetailReportCollection.Clear();
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
            }
            if (CooperativePrescriptionSelectedItem is null)
                return; */
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                PrescriptionDetailReportCollection = new PrescriptionDetailReports("0", StartDate, EndDate);
                
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
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
        private void SelfPrescriptionSelectionChangedAction() {
            /*if (SelfPrescriptionSelectedItem is null && CooperativePrescriptionSelectedItem is null) {
                PrescriptionDetailReportCollection.Clear();
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
            }
               
            if (SelfPrescriptionSelectedItem is null)
                return; */
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
                //if (SelfPrescriptionSelectedItem.TypeId.Equals("5"))
                //{
                //    foreach (var r in PrescriptionDetailReportCollection)
                //    {
                //        var editRecords = PrescriptionPointEditRecords.Where(e => e.ID.Equals(r.Id));
                //        var medicalServicePoint = editRecords.Sum(e => e.MedicalServiceDifference);
                //        var medicinePoint = editRecords.Sum(e => e.MedicineDifference);
                //        var paySelfPoint = editRecords.Sum(e => e.PaySelfDifference);
                //        var profit = editRecords.Sum(e => e.ProfitDifference);
                //        r.MedicalServicePoint += medicalServicePoint;
                //        r.MedicalPoint += medicinePoint;
                //        r.PaySelfPoint += paySelfPoint;
                //        r.Profit += profit;
                //    }
                //}
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
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


        private void ExtraMoneyReportSelectionChangedAction()
        {
            //ExtraMoneyDetailReportCollection.Clear();
            ExtraMoneyDetailReportViewSource = new CollectionViewSource { Source = ExtraMoneyDetailReportCollection };
            ExtraMoneyDetailReportView = ExtraMoneyDetailReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumPrescriptionDetailReport();
          
      
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
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumStockTakingDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            /*SelfPrescriptionSelectedItem = null;
            CooperativePrescriptionSelectedItem = null;*/
            CashflowSelectedItem = null;
        }
        private void RewardReportSelectionChangedAction()
        {
            //ExtraMoneyDetailReportCollection.Clear();
            RewardDetailReportViewSource = new CollectionViewSource { Source = RewardDetailReportCollection };
            RewardDetailReportView = RewardDetailReportViewSource.View;
            //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
            //SumPrescriptionDetailReport();


            CashStockEntryReportEnum = CashStockEntryReportEnum.Reward;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                RewardDetailReportCollection = new RewardDetailReports("0",StartDate, EndDate);

                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                RewardDetailReportViewSource = new CollectionViewSource { Source = RewardDetailReportCollection };
                RewardDetailReportView = RewardDetailReportViewSource.View;
                //StockTakingDetailReportViewSource.Filter += AdjustCaseFilter;
                //SumStockTakingDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            /*SelfPrescriptionSelectedItem = null;
            CooperativePrescriptionSelectedItem = null;*/
            CashflowSelectedItem = null;
        }



        private void SearchAction() { 
                GetData(); 
        }
        private void GetData() {
            TotalPrescriptionProfitReportCollection.Clear();
            SelfPrescriptionProfitReportCollection.Clear();
            CooperativePrescriptionProfitReportCollection.Clear();
            TradeProfitReportCollection.Clear();
            StockTakingReportCollection.Clear();
            StockTakingOTCReportCollection.Clear();
            RewardReportCollection.Clear();
            ExtraMoney = 0;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                CashflowCollection = new CashReports(StartDate,EndDate);
                TotalPrescriptionProfitReportCollection.GetDataByDate(StartDate, EndDate);
                TradeProfitReportCollection=new TradeProfitReports(StartDate, EndDate);
                StockTakingReportCollection = new StockTakingReports(StartDate, EndDate);
                StockTakingOTCReportCollection = new StockTakingOTCReports(StartDate, EndDate);
                RewardReportCollection = new RewardReports(StartDate, EndDate);

                DataTable Extra=PrescriptionProfitReportDb.GetExtraMoneyByDates(StartDate, EndDate);
                ExtraMoney=Extra.Rows[0].Field<decimal?>("CashFlow_Value");

                GetInventoryDifference();
                //PrescriptionPointEditRecords.GetEditRecords(StartDate, EndDate);
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            { 
                foreach (var r in TotalPrescriptionProfitReportCollection)
                {
                    if (r.TypeId.Length == 1)
                        SelfPrescriptionProfitReportCollection.Add(r);
                    else
                        CooperativePrescriptionProfitReportCollection.Add(r);
                }
                //RevertSelfPrescriptionProfitByEditRecords();
                CalculateTotalTradeProfit();
                CalculateTotalStockTaking();
                CalculateTotalStockTakingOTC();
                CalculateTotalCashFlow();
                CalculateTotalPrescriptionProfit();
                CalculateSelfPrescriptionProfit();
                CalculateCooperativePrescriptionProfit();
                CalculateTotalRewardProfit();
                CalculateTotal();

                
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

        private void SumPrescriptionDetailReport() {
            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.InsName = "總計"; 
            var tempCollection = PrescriptionDetailReportCollection.Where(p => true);
            switch (AdjustCaseSelectItem) {
                case "一般箋":
                    tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "1" || p.AdjustCaseID == "3");
                    break;
                case "慢箋":
                    tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "2" );
                    break;                                                                               
                case "自費調劑":                                                                         
                    tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "0" );
                    break;
            }
            PrescriptionDetailReportSum.MedicalPoint = tempCollection.Sum(s => s.MedicalPoint);
            PrescriptionDetailReportSum.MedicalServicePoint = tempCollection.Sum(s => s.MedicalServicePoint);
            PrescriptionDetailReportSum.PaySelfPoint = tempCollection.Sum(s => s.PaySelfPoint);
            PrescriptionDetailReportSum.Meduse = tempCollection.Sum(s => s.Meduse);
            PrescriptionDetailReportSum.Profit = tempCollection.Sum(s => s.Profit);
        }
        private void CalculateTotalCashFlow() {
            
            TotalCashFlow.CopayMentPrice = CashflowCollection.Sum(c => c.CopayMentPrice);
            TotalCashFlow.PaySelfPrice = CashflowCollection.Sum(c => c.PaySelfPrice);
            TotalCashFlow.AllPaySelfPrice = CashflowCollection.Sum(c => c.AllPaySelfPrice);
            TotalCashFlow.DepositPrice = CashflowCollection.Sum(c => c.DepositPrice);
            TotalCashFlow.OtherPrice = CashflowCollection.Sum(c => c.OtherPrice);
            TotalCashFlow.TotalPrice = CashflowCollection.Sum(c => c.TotalPrice); 
        }
        private void CalculateTotalPrescriptionProfit() {
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
            TotalTradeProfitReport.Count = TradeProfitReportCollection.Sum(c => c.Count);
            TotalTradeProfitReport.NetIncome = TradeProfitReportCollection.Sum(c => c.NetIncome);
            TotalTradeProfitReport.Cost = TradeProfitReportCollection.Sum(c => c.Cost);
            TotalTradeProfitReport.Profit = TradeProfitReportCollection.Sum(c => c.Profit);
            TotalTradeProfitReport.CashAmount = TradeProfitReportCollection.Sum(c => c.CashAmount);
            TotalTradeProfitReport.CardAmount = TradeProfitReportCollection.Sum(c => c.CardAmount);
            TotalTradeProfitReport.DiscountAmt = TradeProfitReportCollection.Sum(c => c.DiscountAmt);
            TotalTradeProfitReport.TotalAmt = TotalTradeProfitReport.CashAmount + TotalTradeProfitReport.CardAmount + TotalTradeProfitReport.DiscountAmt;
        }
        private void CalculateTotalRewardProfit()
        {
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
            foreach (var r in SelfPrescriptionProfitReportCollection) {
                SelfPrescriptionProfitReport.Count += r.Count;
                SelfPrescriptionProfitReport.MedicalServicePoint += r.MedicalServicePoint;
                SelfPrescriptionProfitReport.MedicinePoint += r.MedicinePoint;
                SelfPrescriptionProfitReport.PaySelfPoint += r.PaySelfPoint;
                SelfPrescriptionProfitReport.MedUse  += r.MedUse;
                SelfPrescriptionProfitReport.Profit += r.Profit;
            }
            SelfPrescriptionProfitReport.TotalMed = SelfPrescriptionProfitReport.MedicalServicePoint + SelfPrescriptionProfitReport.MedicinePoint;


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
        private void CalculateTotal()
        {
            DiscountAmt = -TotalTradeProfitReport.DiscountAmt;
            InventoryDifference.InventoryTotal = (double)(InventoryDifference.InventoryOverage + InventoryDifference.InventoryShortage + InventoryDifference.InventoryScrap);
            TotalCashFlow.TotalOTC = TotalTradeProfitReport.Profit+ (int)TotalStockTakingOTCReport.Price+ DiscountAmt+ TotalRewardReport.RewardAmount;
            TotalCashFlow.TotalMedProfit = CooperativePrescriptionProfitReport.TotalMed + SelfPrescriptionProfitReport.TotalMed;
            TotalCashFlow.TotalMedUse = CooperativePrescriptionProfitReport.MedUse + SelfPrescriptionProfitReport.MedUse;
            TotalCashFlow.TotalMedCash = TotalCashFlow.CopayMentPrice + TotalCashFlow.PaySelfPrice + TotalCashFlow.AllPaySelfPrice + TotalCashFlow.DepositPrice + TotalCashFlow.OtherPrice;
            TotalCashFlow.TotalMed = TotalCashFlow.TotalMedCash + TotalCashFlow.TotalMedUse + TotalCashFlow.TotalMedProfit+ (double)TotalStockTakingReport.Price;
            TotalCashFlow.Total = (double)(TotalCashFlow.TotalOTC + TotalCashFlow.TotalMed + (double)ExtraMoney);
        }
        private void AdjustCaseFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is PrescriptionDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            PrescriptionDetailReport indexitem = ((PrescriptionDetailReport)e.Item);
            if (AdjustCaseSelectItem == "一般箋" && (indexitem.AdjustCaseID == "1" || indexitem.AdjustCaseID == "3"))
                e.Accepted = true;
            else if (AdjustCaseSelectItem == "慢箋" && indexitem.AdjustCaseID == "2")
                e.Accepted = true;
            else if (AdjustCaseSelectItem == "自費調劑" && indexitem.AdjustCaseID == "0")
                e.Accepted = true;
            else if (AdjustCaseSelectItem == "全部")
                e.Accepted = true;
        }
        #endregion
    }
}
