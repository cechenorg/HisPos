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
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.OfflineDataSet;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product.Medicine.Position;
using His_Pos.NewClass.Product.Medicine.Usage;
using Microsoft.Reporting.WinForms;
using ZeroFormatter;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelMainWindow : MainViewModel, IViewModelMainWindow, IChromeTabViewModel
    {
        //this property is to show you can lock the tabs with a binding
        private bool _canMoveTabs;
        public bool CanMoveTabs
        {
            get => _canMoveTabs;
            set
            {
                if (_canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref _canMoveTabs, value);
                }
            }
        }
        //this property is to show you can bind the visibility of the add button
        private bool _showAddButton;
        public bool ShowAddButton
        {
            get => _showAddButton;
            set
            {
                if (_showAddButton != value)
                {
                    Set(() => ShowAddButton, ref _showAddButton, value);
                }
            }
        }

        private string _cardReaderStatus;
        public string CardReaderStatus
        {
            get => _cardReaderStatus;
            set
            {
                Set(() => CardReaderStatus, ref _cardReaderStatus, value);
            }
        }
        private string _samDcStatus;
        public string SamDcStatus
        {
            get => _samDcStatus;
            set
            {
                Set(() => SamDcStatus, ref _samDcStatus, value);
            }
        }

        private string _hpcCardStatus;
        public string HpcCardStatus
        {
            get => _hpcCardStatus;
            set
            {
                Set(() => HpcCardStatus, ref _hpcCardStatus, value);
            }
        }
        public static bool IsConnectionOpened { get; set; }
        public static bool HisApiException { get; set; }
        public static bool IsIcCardValid { get; set; }
        public static bool IsHpcValid { get; set; }
        public static bool IsVerifySamDc { get; set; }

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

        public static Institutions Institutions { get; set; }
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
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        public ViewModelMainWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPharmacy = Pharmacy.GetCurrentPharmacy();
            CurrentPharmacy.MedicalPersonnels = new MedicalPersonnels();
            MainWindow.ServerConnection.CloseConnection();
            CanMoveTabs = true;
            ShowAddButton = false;
            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias.

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
                BusyContent = StringRes.GetInstitutions;
                Institutions = new Institutions(true);
                BusyContent = StringRes.GetDivisions;
                Divisions = new Divisions();
                BusyContent = StringRes.GetAdjustCases;
                AdjustCases = new AdjustCases();
                BusyContent = StringRes.GetPaymentCategories;
                PaymentCategories = new PaymentCategories();
                BusyContent = StringRes.GetPrescriptionCases;
                PrescriptionCases = new PrescriptionCases();
                BusyContent = StringRes.GetCopayments;
                Copayments = new Copayments();
                BusyContent = StringRes.GetSpecialTreats;
                SpecialTreats = new SpecialTreats();
                BusyContent = StringRes.GetUsages;
                Usages = new Usages();
                BusyContent = StringRes.GetPositions;
                Positions = new Positions();
                //OfflineDataSet offlineData = new OfflineDataSet(Institutions, Divisions, CurrentPharmacy.MedicalPersonnels, AdjustCases, PrescriptionCases, Copayments, PaymentCategories, SpecialTreats, Usages, Positions);
                //var bytes = ZeroFormatterSerializer.Serialize(offlineData);
                //File.WriteAllBytes("C:\\Program Files\\HISPOS\\OfflineDataSet.singde", bytes.ToArray());
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
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
            var result = Divisions.SingleOrDefault(i => i.ID.Equals(id));
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
            if (Usages.Count(u => u.Reg is null && u.Name.Equals(name)) != 0)
            {
                return Usages.Where(u => u.Reg is null).SingleOrDefault(u => u.Name.Equals(name));
            }
            if (Usages.Count(u => u.Reg != null && u.Reg.IsMatch(name)) != 0)
            {
                return Usages.Where(u => u.Reg != null).SingleOrDefault(u => u.Reg.IsMatch(name));
            }
            return new Usage();
        }
        public static Usage FindUsageByQuickName(string quickName)
        {
            if (Usages.Count(u => u.QuickName.Equals(quickName)) == 1)
            {
                return Usages.SingleOrDefault(u => u.QuickName.Equals(quickName));
            }
            return null;
        }
        public static Position GetPosition(string name)
        {
            if (string.IsNullOrEmpty(name)) return new Position();
            if (Positions.Count(p => p.Name.Equals(name)) != 0)
            {
                return Positions.SingleOrDefault(p => p.Name.Equals(name));
            }
            return new Position { Name = name };
        }
        public static SpecialTreat GetSpecialTreat(string id)
        {
            var result = SpecialTreats.SingleOrDefault(i => i.ID.Equals(id));
            return result ?? new SpecialTreat();
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
            Export(r.LocalReport, 25.4, 9.3);
            ReportPrint(Properties.Settings.Default.ReceiptPrinter);
        }
        public void StartPrintDeposit(ReportViewer r)
        {
            BusyContent = StringRes.押金單據列印;
            Export(r.LocalReport, 25.4, 9.3);
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
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
            //try
            //{
            //    string deviceInfo =
            //        @"<DeviceInfo>
            //    <OutputFormat>EMF</OutputFormat>" +
            //        "  <PageWidth>" + width + "cm</PageWidth>" +
            //        "  <PageHeight>" + height + "cm</PageHeight>" +
            //        "  <MarginTop>0cm</MarginTop>" +
            //        "  <MarginLeft>0cm</MarginLeft>" +
            //        "  <MarginRight>0cm</MarginRight>" +
            //        "  <MarginBottom>0cm</MarginBottom>" +
            //        "</DeviceInfo>";
            //    Warning[] warnings;
            //    m_streams = new List<Stream>();
            //    report.Render("Image", deviceInfo, CreateStream, out warnings);
            //    foreach (Stream stream in m_streams)
            //        stream.Position = 0;
            //}
            //catch (Exception ex)
            //{
            //    Application.Current.Dispatcher.Invoke((Action)delegate
            //    {
            //        MessageWindow.ShowMessage("Export:" + ex.Message, MessageType.ERROR);
            //    });
            //}
        }

        private void ReportPrint(string printer)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = printer;
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            m_currentPageIndex = 0;
            printDoc.Print();
            //if (m_streams == null || m_streams.Count == 0)
            //{
            //    return;
            //}
            //try
            //{
            //    PrintDocument printDoc = new PrintDocument();
            //    printDoc.PrinterSettings.PrinterName = printer;
            //    printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            //    m_currentPageIndex = 0;
            //    printDoc.Print();
            //}
            //catch (Exception ex)
            //{
            //    Application.Current.Dispatcher.Invoke((Action)delegate {
            //        MessageWindow.ShowMessage("ReportPrint:" + ex.Message, MessageType.ERROR);
            //    });
            //}
        }
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding,
            string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        public void WindowCloseAction() {
           
        }
        private void printDoc_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
                Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            //try
            //{
            //    Metafile pageImage = new
            //        Metafile(m_streams[m_currentPageIndex]);

            //    // Adjust rectangular area with printer margins.
            //    Rectangle adjustedRect = new Rectangle(
            //        ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            //        ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            //        ev.PageBounds.Width,
            //        ev.PageBounds.Height);

            //    // Draw a white background for the report
            //    ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            //    // Draw the report content
            //    ev.Graphics.DrawImage(pageImage, adjustedRect);

            //    // Prepare for the next page. Make sure we haven't hit the end.
            //    m_currentPageIndex++;
            //    ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            //}
            //catch (Exception ex)
            //{
            //    Application.Current.Dispatcher.Invoke((Action)delegate {
            //        MessageWindow.ShowMessage("printDoc_PrintPage" + ex.Message, MessageType.ERROR);
            //    });
            //}
        }
    }
}
