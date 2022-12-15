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
            ViewModelMainWindow.CurrentPharmacy.AllEmployees.Init();
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
            //, StringRes.Activity
            HisFeatures.Add(new Feature(@"..\Images\PrescriptionIcon.png", nameof(FeatureTab.處方作業) ,
                            new[] {
                                nameof(FeatureItem.處方登錄), 
                                nameof(FeatureItem.處方查詢) ,
                                nameof(FeatureItem.匯出申報檔),
                                nameof(FeatureItem.藥袋查詢) }));

            HisFeatures.Add(new Feature(@"..\Images\Transaction.png", nameof(FeatureTab.銷售作業),
                            new[] {
                                nameof(FeatureItem.結帳作業) ,
                                nameof(FeatureItem.銷售紀錄) ,
                                nameof(FeatureItem.商品訂購網站) ,
                            }));
             
            HisFeatures.Add(new Feature(@"..\Images\Transaction.png", nameof(FeatureTab.藥局管理),
                         new[] {  
                             nameof(FeatureItem.顧客管理),
                             nameof(FeatureItem.供應商管理),
                             nameof(FeatureItem.員工管理),
                             nameof(FeatureItem.額外收支),
                             nameof(FeatureItem.關班作業),
                             nameof(FeatureItem.關班帳務查詢) }));

            HisFeatures.Add(new Feature(@"..\Images\Truck_50px.png", nameof(FeatureTab.庫存管理),
                            new[] { nameof(FeatureItem.商品查詢),
                                nameof(FeatureItem.進退貨管理),
                                nameof(FeatureItem.進退貨紀錄),
                                nameof(FeatureItem.櫃位管理),
                                nameof(FeatureItem.新增盤點),
                                nameof(FeatureItem.庫存盤點紀錄),
                                nameof(FeatureItem.盤點計畫) }));
               
            HisFeatures.Add(new Feature(@"..\Images\Report.png", nameof(FeatureTab.庫存報表),
                            new[] { 
                                nameof(FeatureItem.庫存現值報表), 
                                nameof(FeatureItem.進退貨報表), 
                                nameof(FeatureItem.管制藥品簿冊申報) }));

            HisFeatures.Add(new Feature(@"..\Images\SystemManage.png", nameof(FeatureTab.會計作業),
                            new[] { 
                                nameof(FeatureItem.沖帳作業), 
                                nameof(FeatureItem.立帳作業) }));

            HisFeatures.Add(new Feature(@"..\Images\AccountingReport.png", nameof(FeatureTab.會計報表) ,
                            new[] { 
                                nameof(FeatureItem.每日總帳報表), 
                                nameof(FeatureItem.會計總帳報表),
                                nameof(FeatureItem.申報院所點數總表), 
                                nameof(FeatureItem.損益報表),
                                nameof(FeatureItem.新損益報表),
                                nameof(FeatureItem.資產負債表) }));

            HisFeatures.Add(new Feature(@"..\Images\ClockIn.png", nameof(FeatureTab.出勤管理),
                          new[] {
                              nameof(FeatureItem.上下班打卡) , 
                              nameof(FeatureItem.打卡記錄查詢) }));

            HisFeatures.Add(new Feature(@"..\Images\SystemManage.png", StringRes.AdminManage,
                            new[] {StringRes.AdminManage }));
             
           
        }

        private void InitializeMenu()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<string> tabAuth = EmployeeService.GetTabAuth(ViewModelMainWindow.CurrentUser);
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