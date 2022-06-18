using ChromeTabs;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.FunctionWindow;
using His_Pos.GeneralCustomControl;
using His_Pos.HisApi;
using His_Pos.NewClass;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Person.Employee;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.SETTINGS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Label = System.Windows.Controls.Label;
using MenuItem = System.Windows.Controls.MenuItem;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    /// 
    
    public partial class MainWindow
    {
        public static SQLServerConnection ServerConnection = new SQLServerConnection();
        public static MySQLConnection SingdeConnection = new MySQLConnection();

        public static List<Feature> HisFeatures = new List<Feature>();
        public static MainWindow Instance;


        public MainWindow(Employee user)
        {
            InitializeComponent();

            Instance = this;
            FeatureFactory();
            WindowState = WindowState.Maximized;
            ViewModelMainWindow.CurrentUser = user;
            ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels.InitPharmacists();
            if (ViewModelMainWindow.CurrentUser.IsLocalPharmist() &&
                ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels.Count(e => e.IDNumber.Equals(ViewModelMainWindow.CurrentUser.IDNumber)) == 0)
            {
                ViewModelMainWindow.CurrentPharmacy.MedicalPersonnel = ViewModelMainWindow.CurrentUser;
                ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels.Add(ViewModelMainWindow.CurrentUser);
            }
            InitializeMenu();
            InitialUserBlock();
            StartClock();
     

            AddNewTab("每日作業");
        }

        private void InitialUserBlock()
        {
            UserName.Content = ViewModelMainWindow.CurrentUser.Name;
            PharmacyName.Content = ViewModelMainWindow.CurrentPharmacy.Name;
        }

        private void FeatureFactory()
        {
            HisFeatures.Add(new Feature(@"..\Images\PrescriptionIcon.png", StringRes.hisPrescription,
                            new[] { StringRes.hisPrescriptionDeclare, StringRes.hisPrescriptionInquire, StringRes.DeclareFileExport }));

            HisFeatures.Add(new Feature(@"..\Images\Transaction.png", StringRes.Transaction,
                            new[] { StringRes.ProductTransaction, StringRes.ProductTransactionRecord, StringRes.Activity }));

            HisFeatures.Add(new Feature(@"..\Images\Transaction.png", StringRes.AdditionalCashFlowManage,
                         new[] { StringRes.AdditionalCashFlowManage }));

            HisFeatures.Add(new Feature(@"..\Images\Truck_50px.png", StringRes.StockManage,
                            new[] { StringRes.StockSearch, StringRes.MedBagManage, StringRes.ProductPurchase, StringRes.ProductPurchaseRecord, StringRes.ProductTypeManage, StringRes.LocationManage }));

            HisFeatures.Add(new Feature(@"..\Images\StockTaking.png", StringRes.StockTaking,
                            new[] { StringRes.NewStockTaking, StringRes.StockTakingRecord, StringRes.StockTakingPlan }));

            HisFeatures.Add(new Feature(@"..\Images\Management.png", StringRes.DataManagement,
                            new[] { StringRes.ManufactoryManage, StringRes.PharmacyManage, StringRes.EmployeeManage, StringRes.AuthenticationManage, StringRes.CustomerManage }));

            HisFeatures.Add(new Feature(@"..\Images\ClockIn.png", StringRes.Attend,
                            new[] { StringRes.ClockIn, StringRes.WorkScheduleManage, StringRes.ClockInSearch })); // add by SHANI

            HisFeatures.Add(new Feature(@"..\Images\Report.png", StringRes.ReportSystem,
                            new[] { StringRes.EntrySearch, StringRes.PurchaseReturnReport, StringRes.ControlMedicineDeclare, StringRes.CashStockEntryReport , StringRes.TodayCashStockEntryReport, StringRes.NewTodayCashStockEntryReport }));

            HisFeatures.Add(new Feature(@"..\Images\AccountingReport.png", StringRes.AccountReportSystem,
                            new[] { StringRes.InstitutionDeclarePointReport, StringRes.IncomeStatement, StringRes.BalanceSheet, StringRes.StrikeManage, StringRes.AccountsManage }));
            HisFeatures.Add(new Feature(@"..\Images\SystemManage.png", StringRes.AdminManage,
                            new[] { StringRes.AdminManage }));
            HisFeatures.Add(new Feature(@"..\Images\AccountingReport.png", StringRes.SystemTutorial,
                            new[] { StringRes.SystemTutorial }));
            /*HisFeatures.Add(new Feature(@"..\Images\AccountingReport.png", StringRes.Web,
                            new[] { StringRes.CompanyWeb }));*/
            HisFeatures.Add(new Feature(@"..\Images\AccountingReport.png", StringRes.ClosingWork,
                          new[] { StringRes.ClosingWork,StringRes.ClosingCashSelect }));
        }

        private void InitializeMenu()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<string> tabAuth = ViewModelMainWindow.CurrentUser.GetTabAuth();
            MainWindow.ServerConnection.CloseConnection();
            for (int i = 0; i < HisFeatures.Count; i++)
            {
                (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelText(HisFeatures[i].Title);
                (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelImage(HisFeatures[i].Icon);
                SetFeaturesItem((HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem), HisFeatures[i].Functions, tabAuth);
                if ((HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem)._count != 0)
                    (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).Visibility = Visibility.Visible;
            }
        }

        private void SetFeaturesItem(MenuListItem features, string[] itemsName, List<string> tabAuth)
        {
            if (features == null || itemsName == null)
                throw new ArgumentNullException(nameof(itemsName));
          
            foreach (var t in itemsName)
            {
                if (tabAuth.Count(tab => tab == t) != 0)
                {
                    var newItem = new MenuItem();
                    features.NewMenuItem(t, newItem, FeaturesItemMouseDown);
                }
            }
        }

        private void FeatureMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is MenuListItem feature)) throw new ArgumentNullException(nameof(feature));
            feature.ListMenuTimer?.Start();
        }

        private void FeaturesItemMouseDown(object sender, RoutedEventArgs e)
        {
            var m = sender as MenuItem;
            Debug.Assert(m != null, nameof(m) + " != null");
            AddNewTab(m.Name);
        }

        public void AddNewTab(string tabName)
        {
            ((ViewModelMainWindow)DataContext).AddTabCommandAction(tabName);
            this.Focus();
        }

        private void TickEvent(Object sender, EventArgs e)
        {
            SystemTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        }

        /*
         *啟動處方登錄時間Timer
         */

        private void StartClock()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += TickEvent;
            timer.Start();
        }

        private void Shortcut_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is null) return;

            Label shortcut = sender as Label;

            switch (shortcut.Content.ToString())
            {
                case "調劑":
                    AddNewTab(StringRes.hisPrescriptionDeclare);
                    break;

                case "交易":
                    AddNewTab(StringRes.ProductTransaction);
                    break;
            }
        }

        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            if (e.TabItem != null && e.Model is TabBase viewModel)
            {
                e.TabItem.IsPinned = viewModel.IsPinned;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                ServerConnection.OpenConnection();
                while (WebApi.SendToCooperClinicLoop100())
                {
                } //骨科上傳
                ServerConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("合作診所扣庫資料回傳失敗 請聯絡工程師", MessageType.ERROR);
                NewFunction.ExceptionLog(ex.Message);
            }
            HisApiFunction.CheckDailyUpload();
            Environment.Exit(0);
        }

        private void Settings_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingWindowView settingWindow = new SettingWindowView();
            settingWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var bw = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            bw.DoWork += (o, ea) => { HisApiFunction.VerifySamDc(); };
            bw.RunWorkerAsync();
        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return string.Empty;
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public void SetCardReaderStatus(string status)
        {
            void MethodDelegate()
            {
                ((ViewModelMainWindow)DataContext).CardReaderStatus = "讀卡機狀態 : " + status;
            }
            Dispatcher.BeginInvoke((Action)MethodDelegate);
        }

        public void SetHpcCardStatus(string status)
        {
            void MethodDelegate()
            {
                ((ViewModelMainWindow)DataContext).HpcCardStatus = "醫事卡認證狀態 : " + status;
            }
            Dispatcher.BeginInvoke((Action)MethodDelegate);
        }

        public void SetSamDcStatus(string status)
        {
            void MethodDelegate()
            {
                ((ViewModelMainWindow)DataContext).SamDcStatus = "安全模組狀態 : " + status;
            }
            Dispatcher.BeginInvoke((Action)MethodDelegate);
        }
    }
}