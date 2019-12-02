using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.HisApi;
using His_Pos.NewClass;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Medicine.Position;
using His_Pos.NewClass.Medicine.Usage;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.WareHouse;
using His_Pos.Service;
using Microsoft.Reporting.WinForms;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelMainWindow : MainViewModel, IViewModelMainWindow, IChromeTabViewModel
    {
        //this property is to show you can lock the tabs with a binding
        private bool canMoveTabs;
        public bool CanMoveTabs
        {
            get => canMoveTabs;
            set
            {
                if (canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref canMoveTabs, value);
                }
            }
        }
        //this property is to show you can bind the visibility of the add button
        private bool showAddButton;
        public bool ShowAddButton
        {
            get => showAddButton;
            set
            {
                if (showAddButton != value)
                {
                    Set(() => ShowAddButton, ref showAddButton, value);
                }
            }
        }

        private string cardReaderStatus;
        public string CardReaderStatus
        {
            get => cardReaderStatus;
            set
            {
                Set(() => CardReaderStatus, ref cardReaderStatus, value);
            }
        }
        private string samDcStatus;
        public string SamDcStatus
        {
            get => samDcStatus;
            set
            {
                Set(() => SamDcStatus, ref samDcStatus, value);
            }
        }

        private string hpcCardStatus;
        public string HpcCardStatus
        {
            get => hpcCardStatus;
            set
            {
                Set(() => HpcCardStatus, ref hpcCardStatus, value);
            }
        }
        public static bool IsConnectionOpened { get; set; }
        public static bool HisApiException { get; set; }
        public static bool IsIcCardValid { get; set; }
        public static bool IsHpcValid { get; set; }
        public static bool IsVerifySamDc { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }
        private string busyContent;

        public string BusyContent
        {
            get => busyContent;
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }

        public static Institutions Institutions { get; set; }
        public static WareHouses WareHouses { get; set; }
        public static CooperativeClinicSettings CooperativeClinicSettings { get; set; }
        public static Divisions Divisions { get; set; }
        public static AdjustCases AdjustCases { get; set; }
        public static PaymentCategories PaymentCategories { get; set; }
        public static PrescriptionCases PrescriptionCases { get; set; }
        public static Copayments Copayments { get; set; }
        public static SpecialTreats SpecialTreats { get; set; }
        public static Usages Usages { get; set; }
        public static Positions Positions { get; set; }
        public static Pharmacy CurrentPharmacy { get; set; }
        public static Employee CurrentUser { get; set; }
        public static Employees EmployeeCollection { get; set; }
        public static string CooperativeInstitutionID { get; private set; }
        private int mCurrentPageIndex;
        private IList<Stream> mStreams;
        public ViewModelMainWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPharmacy = Pharmacy.GetCurrentPharmacy();
            CurrentPharmacy.MedicalPersonnels = new Employees();
            CooperativeInstitutionID = WebApi.GetCooperativeClinicId(CurrentPharmacy.ID);
            MainWindow.ServerConnection.CloseConnection();
            CanMoveTabs = true;
            ShowAddButton = false;
        
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "MainWindowClosing")
                    WindowCloseAction();
            });
            
        }
        private RelayCommand initialData;
        public RelayCommand InitialData
        {
            get =>
                initialData ??
                (initialData = new RelayCommand(ExecuteInitData));
            set => initialData = value;
        }

        private void ExecuteInitData()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection(); 
                BusyContent = "取得庫別名";
                WareHouses = WareHouses.GetWareHouses();
                BusyContent = StringRes.取得院所;
                Institutions = new Institutions(true);
                BusyContent = "取得合作院所設定";
                CooperativeClinicSettings = new CooperativeClinicSettings();
                CooperativeClinicSettings.Init();
                CooperativeClinicSettings.FilePurge();
                BusyContent = StringRes.取得科別;
                Divisions = new Divisions();
                BusyContent = StringRes.GetAdjustCases;
                AdjustCases = new AdjustCases(true);
                BusyContent = StringRes.取得給付類別;
                PaymentCategories = new PaymentCategories();
                BusyContent = StringRes.取得處方案件;
                PrescriptionCases = new PrescriptionCases();
                BusyContent = StringRes.取得部分負擔;
                Copayments = new Copayments();
                BusyContent = StringRes.取得特定治療;
                SpecialTreats = new SpecialTreats();
                BusyContent = StringRes.取得用法;
                Usages = new Usages();
                BusyContent = StringRes.取得用藥途徑;
                Positions = new Positions();
                BusyContent = "藥袋量計算中";
                ProductDB.UpdateAllInventoryMedBagAmount();
                BusyContent = "同步員工資料";
                EmployeeDb.SyncData();
                BusyContent = "取得員工";
                EmployeeCollection = new Employees();
                EmployeeCollection.Init();
                BusyContent = "準備回傳骨科拋轉資料";
                while (WebApi.SendToCooperClinicLoop100())
                { 
                    BusyContent = "回傳合作診所處方中...";
                } //骨科上傳
                //OfflineDataSet offlineData = new OfflineDataSet(Institutions, Divisions, CurrentPharmacy.MedicalPersonnels, AdjustCases, PrescriptionCases, Copayments, PaymentCategories, SpecialTreats, Usages, Positions);
                //var bytes = ZeroFormatterSerializer.Serialize(offlineData);
                //File.WriteAllBytes("C:\\Program Files\\HISPOS\\OfflineDataSet.singde", bytes.ToArray());
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                MainWindow.ServerConnection.OpenConnection();
                HisApiFunction.CheckDailyUpload();
                MainWindow.ServerConnection.CloseConnection();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        public static WareHouse GetWareHouse(string id) {
            var result = WareHouses.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }
        public static Institution GetInstitution(string id)
        {
            var result = Institutions.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }
        public static AdjustCase GetAdjustCase(string id)
        {
            return AdjustCases.SingleOrDefault(a => a.ID.Equals(id));
        }
        public static Division GetDivision(string id)
        {
            var result = Divisions.SingleOrDefault(i => !string.IsNullOrEmpty(i.ID) && i.ID.Equals(id));
            return result;
        }
        public static PaymentCategory GetPaymentCategory(string id)
        {
            return PaymentCategories.SingleOrDefault(p => p.ID.Equals(id));
        }
        public static PrescriptionCase GetPrescriptionCases(string id)
        {
            var result = PrescriptionCases.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }
        public static Copayment GetCopayment(string id)
        {
            var result = Copayments.SingleOrDefault(i => i.Id.Equals(id));
            return result;
        }
        public static Usage GetUsage(string name)
        {
            if (string.IsNullOrEmpty(name)) return new Usage();
            var usage = name.Replace("AC", "").Replace("PC", "");
            var result = new Usage();
            if (Usages.Count(u => u.Reg is null && u.Name.Equals(usage)) != 0)
            {
                result = Usages.Where(u => u.Reg is null).SingleOrDefault(u => u.Name.Equals(usage)).DeepCloneViaJson();
                result.Name = name;
                if (name.Contains("AC") || name.Contains("PC"))
                {
                    result.Name = name;
                    if (name.Contains("AC"))
                        result.PrintName += "(飯前)";
                    else if (name.Contains("PC"))
                        result.PrintName += "(飯後)";
                }
                return result;
            }
            if (Usages.Count(u => u.Reg != null && u.Reg.IsMatch(usage)) != 0)
            {
                var resultList = Usages.Where(u => u.Reg != null).Where(u => u.Reg.IsMatch(usage));
                foreach (var r in resultList)
                {
                    var re = r.DeepCloneViaJson();
                    //result.Name += re.Name; 
                    result = re;
                    result.Name = name;
                }
                if (name.Contains("AC") || name.Contains("PC"))
                {
                    if (name.Contains("AC"))
                        result.PrintName += "(飯前)";
                    else if (name.Contains("PC"))
                        result.PrintName += "(飯後)";
                }
                return result;
            }
            return new Usage();
        }
        public static Usage FindUsageByQuickName(string quickName)
        {
            return Usages.Where(u => !string.IsNullOrEmpty(u.QuickName)).Count(u => u.QuickName.Equals(quickName)) == 1 ? 
                Usages.Where(u => !string.IsNullOrEmpty(u.QuickName)).SingleOrDefault(u => u.QuickName.Equals(quickName)) : null;
        }
        public static Position GetPosition(string id)
        {
            if (string.IsNullOrEmpty(id)) return new Position();
            if (Positions.Count(p => p.ID.Equals(id.ToUpper())) != 0)
            {
                return Positions.SingleOrDefault(p => p.ID.Equals(id.ToUpper()));
            }
            return new Position { ID = id.ToUpper(), Name = string.Empty};
        }
        public static SpecialTreat GetSpecialTreat(string id)
        {
            var result = SpecialTreats.SingleOrDefault(i => i.ID.Equals(id));
            return result ?? new SpecialTreat();
        }

        public static Employee GetMedicalPersonByID(int id)
        {
            var result = CurrentPharmacy.MedicalPersonnels.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }
        public static Employee GetMedicalPersonByIDNumber(string idNum)
        {
            var result = CurrentPharmacy.MedicalPersonnels.SingleOrDefault(i => i.IDNumber.Equals(idNum));
            return result;
        }
        public static Employee GetEmployeeByID(int ID)
        {
            var result = EmployeeCollection.SingleOrDefault(i => i.ID.Equals(ID));
            return result;
        }
        
        public void StartPrintMedBag(ReportViewer r)
        {
            IsBusy = true;
            BusyContent = StringRes.MedBagPrinting;
            Export(r.LocalReport, 22, 24);
            ReportPrint(Properties.Settings.Default.MedBagPrinter);
            IsBusy = false;
        }

        public void StartPrintReceipt(ReportViewer r)
        {
            BusyContent = StringRes.收據列印;
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    Export(r.LocalReport, 21, 14.8);
                    break;
                default:
                    Export(r.LocalReport, 25.4, 9.3);
                    break;
            }
            ReportPrint(Properties.Settings.Default.ReceiptPrinter);
        }

        public void StartPrintReserve(ReportViewer r) {
            BusyContent = "封包列印...";
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    Export(r.LocalReport, 21, 14.8);
                    break;
                default:
                    Export(r.LocalReport, 25.4, 9.3);
                    break;
            }
            ReportPrint(Properties.Settings.Default.ReceiptPrinter);
        }
        public void StartPrintMedicineTag(ReportViewer r)
        {
            IsBusy = true;
            BusyContent = StringRes.MedBagPrinting;
            Export(r.LocalReport, 21, 29.7);
            ReportPrint(Properties.Settings.Default.ReportPrinter);
            IsBusy = false;
        }
        public void StartPrintDeposit(ReportViewer r)
        {
            BusyContent = StringRes.押金單據列印;
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    Export(r.LocalReport, 21, 14.8);
                    break;
                default:
                    Export(r.LocalReport, 25.4, 9.3);
                    break;
            }
            ReportPrint(Properties.Settings.Default.ReceiptPrinter);
        }
        private void Export(LocalReport report, double width, double height)
        {
            string deviceInfo =
                @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>" +
                "  <PageWidth>" + width + "cm</PageWidth>" +
                "  <PageHeight>" + height + "cm</PageHeight>" +
                "  <MarginTop>0cm</MarginTop>" +
                "  <MarginLeft>0cm</MarginLeft>" +
                "  <MarginRight>0cm</MarginRight>" +
                "  <MarginBottom>0cm</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            mStreams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in mStreams)
                stream.Position = 0;
        }

        private void ReportPrint(string printer)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = printer;
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            mCurrentPageIndex = 0;
            try
            {
                printDoc.Print();
            }
            catch (Exception ex) {
                FunctionWindow.MessageWindow.ShowMessage(ex.Message,Class.MessageType.ERROR);
            }
        }
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding,
            string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            mStreams.Add(stream);
            return stream;
        }
        public void WindowCloseAction() {
           
        }
        private void printDoc_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
                Metafile(mStreams[mCurrentPageIndex]);

            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            ev.Graphics.DrawImage(pageImage, adjustedRect);

            mCurrentPageIndex++;
            ev.HasMorePages = (mCurrentPageIndex < mStreams.Count);
        }
    }
}
