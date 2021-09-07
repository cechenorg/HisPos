using ClosedXML.Excel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.ControlMedicineDeclare;
using His_Pos.NewClass.Medicine.ControlMedicineDetail;
using His_Pos.NewClass.WareHouse;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditWindow.WareHouseSelectWindow;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare
{
    public class ControlMedicineDeclareViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region Var

        private DateTime sDateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);

        public DateTime SDateTime
        {
            get { return sDateTime; }
            set
            {
                Set(() => SDateTime, ref sDateTime, value);
            }
        }

        private DateTime eDateTime = DateTime.Now;

        public DateTime EDateTime
        {
            get { return eDateTime; }
            set
            {
                Set(() => EDateTime, ref eDateTime, value);
            }
        }

        private CollectionViewSource controlCollectionViewSource;

        private CollectionViewSource ControlCollectionViewSource
        {
            get => controlCollectionViewSource;
            set
            {
                Set(() => ControlCollectionViewSource, ref controlCollectionViewSource, value);
            }
        }

        private ICollectionView controlCollectionView;

        public ICollectionView ControlCollectionView
        {
            get => controlCollectionView;
            private set
            {
                Set(() => ControlCollectionView, ref controlCollectionView, value);
            }
        }

        private ControlMedicineDeclares controlMedicineDeclares = new ControlMedicineDeclares();

        public ControlMedicineDeclares ControlMedicineDeclares
        {
            get { return controlMedicineDeclares; }
            set
            {
                Set(() => ControlMedicineDeclares, ref controlMedicineDeclares, value);
            }
        }

        private NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare selectItem;

        public NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare SelectItem
        {
            get { return selectItem; }
            set
            {
                Set(() => SelectItem, ref selectItem, value);
            }
        }

        private ControlMedicineDetails controlMedicineDetailsCollection = new ControlMedicineDetails();

        public ControlMedicineDetails ControlMedicineDetailsCollection
        {
            get { return controlMedicineDetailsCollection; }
            set
            {
                Set(() => ControlMedicineDetailsCollection, ref controlMedicineDetailsCollection, value);
            }
        }

        private ControlMedicineDetails controlMedicineBagDetailsCollection = new ControlMedicineDetails();

        public ControlMedicineDetails ControlMedicineBagDetailsCollection
        {
            get { return controlMedicineBagDetailsCollection; }
            set
            {
                Set(() => ControlMedicineBagDetailsCollection, ref controlMedicineBagDetailsCollection, value);
            }
        }

        private WareHouse selectedWareHouse;

        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set
            {
                Set(() => SelectedWareHouse, ref selectedWareHouse, value);
            }
        }

        public WareHouses WareHouseCollection { get; set; } = WareHouses.GetWareHouses();

        #endregion Var

        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand WareHouseSelectionChangedCommand { get; set; }
        public RelayCommand PrintMaserCommand { get; set; }
        public RelayCommand PrintDetailCommand { get; set; }
        public RelayCommand ControlMedEditCommand { get; set; }
        public RelayCommand WareHouseSelectedWindowCommand { get; set; }
        public RelayCommand ShowMedicineDetailCommand { get; set; }
        public RelayCommand ShowControlMedicineEditInputWindowCommand { get; set; }
        public RelayCommand PrintMasterInventoryCommand { get; set; }

        public ControlMedicineDeclareViewModel()
        {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
            SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
            SearchCommand = new RelayCommand(SearchAction);
            WareHouseSelectionChangedCommand = new RelayCommand(WareHouseSelectionChangedAction);
            PrintMaserCommand = new RelayCommand(PrintMaserAction);
            PrintDetailCommand = new RelayCommand(PrintDetailAction);
            ControlMedEditCommand = new RelayCommand(ControlMedEditAction);
            WareHouseSelectedWindowCommand = new RelayCommand(WareHouseSelectedWindowAction);
            ShowMedicineDetailCommand = new RelayCommand(ShowMedicineDetailAction);
            ShowControlMedicineEditInputWindowCommand = new RelayCommand(ShowControlMedicineEditInputWindowAction);
            PrintMasterInventoryCommand = new RelayCommand(PrintMasterInventoryAction);
            SelectedWareHouse = WareHouseCollection[0];
            SearchAction();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "ControlMedicineDeclareSearch")
                    SearchAction();
            });
        }

        private void ShowControlMedicineEditInputWindowAction()
        {
            ControlMedicineEditInputWindow.ControlMedicineEditInputWindow controlMedicineEditInputWindow = new ControlMedicineEditInputWindow.ControlMedicineEditInputWindow();
        }

        private void WareHouseSelectedWindowAction()
        {
            WareHouseSelectWindow wareHouseSelectWindow = new WareHouseSelectWindow(SDateTime, EDateTime);
        }

        private void ControlMedEditAction()
        {
            ControlMedicineEditWindow.ControlMedicineEditWindow controlMedicineEditWindow = new ControlMedicineEditWindow.ControlMedicineEditWindow(SelectItem.ID, SelectedWareHouse.ID);
        }

        private void PrintDetailAction()
        {
            if (ControlMedicineDetailsCollection is null) return;

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "管制藥品收支結存簿冊";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = SDateTime.ToString("yyyyMMdd") + "_" + EDateTime.ToString("yyyyMMdd") + "_" + SelectItem.ID + "管制藥品收支結存簿冊";
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
                        file.WriteLine("管制藥品收支結存簿冊");
                        file.WriteLine($"健保碼,{SelectItem.ID}");
                        file.WriteLine($"藥品名稱,{SelectItem.FullName}");
                        file.WriteLine("日期,收支原因,收入數量,批號,支出數量,結存數量,備註");
                        foreach (var c in ControlMedicineDetailsCollection)
                        {
                            file.WriteLine($"{c.Date},{c.TypeName},{c.InputAmount},{c.BatchNumber},{c.OutputAmount},{c.FinalStock},{c.Description}");
                        }
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

        private void PrintMaserAction()
        {
            if (ControlCollectionView is null) return;

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "管制藥品收支結存簿冊";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = SDateTime.ToString("yyyyMMdd") + "_" + EDateTime.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "管制藥品收支結存簿冊主檔";
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
                        file.WriteLine(SDateTime.ToString("yyyyMMdd") + "_" + EDateTime.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "管制藥品收支結存簿冊");
                        file.WriteLine($"庫別:{SelectedWareHouse.Name}");
                        file.WriteLine("級別,健保碼,名稱,進貨,支出,結存");
                        foreach (var c in ControlCollectionView)
                        {
                            var conMed = ((NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare)c);
                            file.WriteLine($"{conMed.IsControl},{conMed.ID},{conMed.FullName},{conMed.GetValue},{conMed.PayValue},{conMed.FinalValue}");
                        }
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

        private void PrintMasterInventoryAction()
        {
            Process myProcess = new Process();
            DataTable table = ControlMedicineDeclareDb.GetInventoryDataByDate(SDateTime, EDateTime, SelectedWareHouse.ID);
            if (table is null) return;
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "管藥庫存";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = SDateTime.ToString("yyyyMMdd") + "_" + EDateTime.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "管制藥品庫存表";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add(SelectedWareHouse.Name + "管藥管理");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                var col1 = ws.Column("A");
                col1.Width = 18;
                var col2 = ws.Column("B");
                col2.Width = 45;
                var col3 = ws.Column("C");
                col3.Width = 10;
                var col4 = ws.Column("D");
                col4.Width = 10;

                ws.Cell(1, 1).Value = "庫別名稱：" + SelectedWareHouse.Name;
                ws.Range(1, 1, 1, 1).Merge().AddToNamed("Titles");
                ws.Cell(1, 2).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                ws.Range(1, 2, 1, 4).Merge().AddToNamed("Days");
                ws.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell("A2").Value = "管藥代碼";
                ws.Cell("B2").Value = "管藥名稱";
                ws.Cell("C2").Value = "庫存數量";
                ws.Cell("D2").Value = "盤點數量";
                var rangeWithData = ws.Cell(3, 1).InsertData(table.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                try
                {
                    wb.SaveAs(fdlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = (fdlg.FileName);
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.Verb = "print";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void WareHouseSelectionChangedAction()
        {
            ControlCollectionViewSource.Filter += Filter;
        }

        private void SearchAction()
        {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
            SelectItem = ControlMedicineDeclares[0];
            SelectionChangedAction();
            ControlCollectionViewSource = new CollectionViewSource { Source = ControlMedicineDeclares };
            ControlCollectionView = ControlCollectionViewSource.View;
            ControlCollectionViewSource.Filter += Filter;
        }

        private void SelectionChangedAction()
        {
            if (SelectItem == null) return;
            ControlMedicineDetails temp = new ControlMedicineDetails();
            temp.GetDataById(SelectItem.ID, SDateTime, EDateTime, SelectItem.InitStock, SelectedWareHouse.ID);
            ControlMedicineBagDetailsCollection.Clear();
            ControlMedicineDetailsCollection.Clear();
            foreach (ControlMedicineDetail c in temp)
            {
                switch (c.TypeName)
                {
                    case "調劑(未過卡)":
                        ControlMedicineBagDetailsCollection.Add(c);
                        break;

                    default:
                        ControlMedicineDetailsCollection.Add(c);
                        break;
                }
            }
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare src))
                e.Accepted = false;
            NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare controlMedicineDeclare = (NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare)e.Item;
            e.Accepted = controlMedicineDeclare.WareHouse.ID == SelectedWareHouse.ID;
        }

        private void ShowMedicineDetailAction()
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { SelectItem.ID, SelectedWareHouse.ID }, "ShowProductDetail"));
        }
    }
}