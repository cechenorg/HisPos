﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ChromeTabs;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.FunctionWindow;
using His_Pos.GeneralCustomControl;
using His_Pos.HisApi;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription.Position;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Usage;
using His_Pos.SYSTEM_TAB.SETTINGS;
using Label = System.Windows.Controls.Label;
using MenuItem = System.Windows.Controls.MenuItem;

namespace His_Pos
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public static SQLServerConnection ServerConnection = new SQLServerConnection();
        public static MySQLConnection SingdeConnection = new MySQLConnection();

        public static List<Feature> HisFeatures = new List<Feature>();
        public static Employee CurrentUser;

        public static MainWindow Instance;

        private static int hisApiErrorCode;
        public int HisApiErrorCode
        {
            get => hisApiErrorCode;
            set
            {
                hisApiErrorCode = value;
                SetCardReaderStatus(hisApiErrorCode);
            }
        }

        public static Pharmacy CurrentPharmacy;
        public static bool ItemSourcesSet { get; set; }
        public static Institutions Institutions { get; set; }
        public static Divisions Divisions { get; set; }
        public static AdjustCases AdjustCases { get; set; }
        public static PaymentCategories PaymentCategories { get; set; }
        public static PrescriptionCases PrescriptionCases { get;set; }
        public static Copayments Copayments { get; set; }
        public static SpecialTreats SpecialCode { get; set; }
        public static Usages Usages { get; set; }
        public static Positions Positions { get; set; }
        public MainWindow(Employee user)
        {
            FeatureFactory();
            InitializeComponent();
            WindowState = WindowState.Maximized;
            CurrentUser = user;
            CurrentPharmacy = Pharmacy.GetCurrentPharmacy();
            Instance = this;
            InitializeMenu();
            InitialUserBlock();
            StratClock();
            AddNewTab("每日作業");
            
        }
        
        private void InitialUserBlock()
        {
            UserName.Content = CurrentUser.Name;
        }

        private void FeatureFactory()
        {
            HisFeatures.Add(new Feature( @"..\Images\PrescriptionIcon.png", Properties.Resources.hisPrescription,
                            new string[] { Properties.Resources.hisPrescriptionDeclare, Properties.Resources.hisPrescriptionInquire, Properties.Resources.MedFrequencyManage, Properties.Resources.MedBagManage , Properties.Resources.DeclareFileExport }));

            HisFeatures.Add(new Feature(@"..\Images\Truck_50px.png", Properties.Resources.StockManage,
                            new string[] { Properties.Resources.StockSearch, Properties.Resources.ProductPurchase, Properties.Resources.ProductPurchaseRecord, Properties.Resources.ProductTypeManage, Properties.Resources.LocationManage }));

            HisFeatures.Add(new Feature(@"..\Images\StockTaking.png", Properties.Resources.StockTaking,
                            new string[] { Properties.Resources.NewStockTaking, Properties.Resources.StockTakingRecord }));

            HisFeatures.Add(new Feature(@"..\Images\Management.png", Properties.Resources.DataManagement,
                            new string[] { Properties.Resources.ManufactoryManage, Properties.Resources.PharmacyManage,
                                           Properties.Resources.EmployeeManage, Properties.Resources.AuthenticationManage, Properties.Resources.CustomerManage}));

            HisFeatures.Add(new Feature(@"..\Images\ClockIn.png", Properties.Resources.Attend,
                            new string[] { Properties.Resources.ClockIn, Properties.Resources.WorkScheduleManage }));

            HisFeatures.Add(new Feature(@"..\Images\Report.png", Properties.Resources.ReportSystem,
              new string[] { Properties.Resources.EntrySearch, Properties.Resources.PurchaseReturnReport,Properties.Resources.CooperativeAdjustReport,Properties.Resources.CooperativeEntry }));
            HisFeatures.Add(new Feature(@"..\Images\Report.png", Properties.Resources.AdminManage,
              new string[] { Properties.Resources.AdminFunction }));
        }
        
        private void InitializeMenu()
        {
            
            for (int i = 0; i < HisFeatures.Count; i++)
            {
                    (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelText(HisFeatures[i].Title);
                    (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).SetLabelImage(HisFeatures[i].Icon);
                    SetFeaturesItem((HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem), HisFeatures[i].Functions);
                if ((HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem)._count != 0)
                    (HisMenu.FindName("HisFeature" + (i + 1)) as MenuListItem).Visibility = Visibility.Visible;
                
            }
        }

        private void SetFeaturesItem(MenuListItem features, string [] itemsName)
        {
            if (features == null || itemsName == null)
                throw new ArgumentNullException(nameof(itemsName));

            Collection<string> tabAuth = CurrentUser.GetTabAuth();
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
        private void StratClock()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += TickEvent;
            timer.Start();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Tabs.SelectedItem is null) return;
            //((ViewModelMainWindow)DataContext).AddTabCommandAction(((TabBase)Tabs.SelectedItem).TabName);
        }

        private void Shortcut_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is null) return;

            Label shortcut = sender as Label;

            switch (shortcut.Content.ToString())
            {
                case "調劑":
                    AddNewTab(Properties.Resources.hisPrescriptionDeclare);
                    break;
                case "交易":
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ///var d = new DeclareDb();
            var dailyUploadConfirm = new YesNoMessageWindow("是否執行每日健保上傳","每日上傳確認");
            var upload = (bool) dailyUploadConfirm.ShowDialog();
            if (upload)
               /// d.StartDailyUpload();
            ///ProductDb.UpdateDailyStockValue();
            ///DeclareDb declareDb = new DeclareDb();
           /// declareDb.SendUnSendCooperClinicDeclare();
            if (((ViewModelMainWindow)MainWindow.Instance.DataContext).IsConnectionOpened)
                HisApiBase.CloseCom();
            Environment.Exit(0);
        }

        private void Settings_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var bw = new BackgroundWorker
            {
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            bw.DoWork += VerifySam;
            bw.ProgressChanged += DuringVerify;
            bw.RunWorkerCompleted += AfterVerify;
            bw.RunWorkerAsync();
        }

        private void VerifySam(object sender, DoWorkEventArgs e)
        {
            HisApiBase.CheckCardStatus(1);
            if (!HisApiBase.GetStatus(2))
            {
                HisApiBase.VerifySamDc();
            }
        }

        private void DuringVerify(object sender, ProgressChangedEventArgs e)
        {
            SetSamDcStatus("安全模組認證中...");
        }

        private void AfterVerify(object sender, RunWorkerCompletedEventArgs e)
        {
            SetSamDcStatus(((ViewModelMainWindow) DataContext).IsVerifySamDc ? "認證成功" : "認證失敗");
        }

        private static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return string.Empty;
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute),false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        private void SetCardReaderStatus(int res)
        {
            void MethodDelegate()
            {
                ((ViewModelMainWindow) DataContext).CardReaderStatus = "讀卡機狀態 : " + GetEnumDescription((ErrorCode) res);
            }
            Dispatcher.BeginInvoke((Action) MethodDelegate);
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
