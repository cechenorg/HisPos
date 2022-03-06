using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StockValue;
using His_Pos.NewClass.WareHouse;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ClosedXML.Excel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using His_Pos.FunctionWindow;
using His_Pos.NewClass;



namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach
{
    public class EntrySearchViewModel : TabBase
    {
        #region Var

        public override TabBase getTab()
        {
            return this;
        }

        public StockValue SelectStockValue { get; set; }
        private StockValues dailyStockValueCollection = new StockValues();

        public StockValues DailyStockValueCollection
        {
            get { return dailyStockValueCollection; }
            set { Set(() => DailyStockValueCollection, ref dailyStockValueCollection, value); }
        }

        private StockValue totalDailyStock = new StockValue();

        public StockValue TotalDailyStock
        {
            get { return totalDailyStock; }
            set { Set(() => TotalDailyStock, ref totalDailyStock, value); }
        }

        private DateTime startDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);

        //10.21
        public StockValue SelectOTCStockValue { get; set; }

        private StockValues dailyOTCStockValueCollection = new StockValues();

        public StockValues DailyOTCStockValueCollection
        {
            get { return dailyOTCStockValueCollection; }
            set { Set(() => DailyOTCStockValueCollection, ref dailyOTCStockValueCollection, value); }
        }

        private StockValue totalOTCDailyStock = new StockValue();

        public StockValue TotalOTCDailyStock
        {
            get { return totalOTCDailyStock; }
            set { Set(() => TotalOTCDailyStock, ref totalOTCDailyStock, value); }
        }

        //10.21^^^
        public DateTime StartDate
        {
            get { return startDate; }
            set { Set(() => StartDate, ref startDate, value); }
        }

        private DateTime endDate = DateTime.Now;

        public DateTime EndDate
        {
            get { return endDate; }
            set { Set(() => EndDate, ref endDate, value); }
        }

        private WareHouses wareHouseCollection;

        public WareHouses WareHouseCollection
        {
            get { return wareHouseCollection; }
            set
            {
                Set(() => WareHouseCollection, ref wareHouseCollection, value);
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

        #endregion Var

        #region Command

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ExportCsvCommand { get; set; }
        public RelayCommand ExportOTCCsvCommand { get; set; }
        public RelayCommand ShowEntryDetailCommand { set; get; }
        public RelayCommand DownloadCommand { set; get; }

        #endregion Command

        public EntrySearchViewModel()
        {
            WareHouseCollection = new WareHouses(WareHouseDb.Init());
            SelectedWareHouse = WareHouseCollection[0];
            Search();
            SearchCommand = new RelayCommand(Search);
            ExportCsvCommand = new RelayCommand(ExportCsv);
            ExportOTCCsvCommand = new RelayCommand(ExportOTCCsv);
            ShowEntryDetailCommand = new RelayCommand(ShowEntryDetailAction);
            DownloadCommand = new RelayCommand(DownloadAction);
        }

        #region Function

        private void ShowEntryDetailAction()
        {
            EntryDetailWindow.EntryDetailWindow entryDetailWindow = new EntryDetailWindow.EntryDetailWindow(SelectStockValue.Date);
        }

        private void ExportCsv()
        {
            //DailyOTCStockValueCollection
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csv|*.csv ";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.CreateNew))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                    sw.WriteLine("日期" + "\t" + "期初現值" + "\t" + "進貨" + "\t" + "退貨" + "\t" + "盤點" + "\t" + "調劑耗用" + "\t" + "進貨負庫調整" + "\t" + "期末現值");
                    foreach (var row in DailyStockValueCollection)
                    {
                        sw.WriteLine(row.Date + "\t" + row.InitStockValue + "\t" + row.PurchaseValue + "\t" + row.ReturnValue + "\t" + row.StockCheckValue + "\t" + row.MedUseValue + "\t" + row.MinusStockAdjustValue + "\t" + row.FinalStockValue);
                    }
                    sw.Close();
                }
            }
        }

        private void ExportOTCCsv()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csv|*.csv ";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.CreateNew))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                    sw.WriteLine("日期" + "\t" + "期初現值" + "\t" + "進貨" + "\t" + "退貨" + "\t" + "盤點" + "\t" + "調劑耗用" + "\t" + "進貨負庫調整" + "\t" + "期末現值");
                    foreach (var row in DailyOTCStockValueCollection)
                    {
                        sw.WriteLine(row.Date + "\t" + row.InitStockValue + "\t" + row.PurchaseValue + "\t" + row.ReturnValue + "\t" + row.StockCheckValue + "\t" + row.MedUseValue + "\t" + row.MinusStockAdjustValue + "\t" + row.FinalStockValue);
                    }
                    sw.Close();
                }
            }
        }
        private void Search()
        {
            DailyStockValueCollection.Clear();
            DailyOTCStockValueCollection.Clear();
            DailyStockValueCollection.GetDataByDate(StartDate, EndDate, SelectedWareHouse.ID);
            DailyOTCStockValueCollection.GetOTCDataByDate(StartDate, EndDate, SelectedWareHouse.ID);
            CaculateTotalStock();
            CaculateOTCTotalStock();
        }

        private void CaculateTotalStock()
        {
            if (DailyStockValueCollection.Count > 0)
            {
                TotalDailyStock.InitStockValue = DailyStockValueCollection[0].InitStockValue;
                TotalDailyStock.PurchaseValue = DailyStockValueCollection.Sum(d => d.PurchaseValue);
                TotalDailyStock.ReturnValue = DailyStockValueCollection.Sum(d => d.ReturnValue);
                TotalDailyStock.MedUseValue = DailyStockValueCollection.Sum(d => d.MedUseValue);
                TotalDailyStock.MinusStockAdjustValue = DailyStockValueCollection.Sum(d => d.MinusStockAdjustValue);
                TotalDailyStock.StockCheckValue = DailyStockValueCollection.Sum(d => d.StockCheckValue);
                TotalDailyStock.TrashValue = DailyStockValueCollection.Sum(d => d.TrashValue);
                TotalDailyStock.AdjustValue = DailyStockValueCollection.Sum(d => d.AdjustValue);
                TotalDailyStock.FinalStockValue = DailyStockValueCollection[DailyStockValueCollection.Count - 1].FinalStockValue;
            }
        }

        private void CaculateOTCTotalStock()
        {
            if (DailyOTCStockValueCollection.Count > 0)
            {
                TotalOTCDailyStock.InitStockValue = DailyOTCStockValueCollection[0].InitStockValue;
                TotalOTCDailyStock.PurchaseValue = DailyOTCStockValueCollection.Sum(d => d.PurchaseValue);
                TotalOTCDailyStock.ReturnValue = DailyOTCStockValueCollection.Sum(d => d.ReturnValue);
                TotalOTCDailyStock.MedUseValue = DailyOTCStockValueCollection.Sum(d => d.MedUseValue);
                TotalOTCDailyStock.MinusStockAdjustValue = DailyOTCStockValueCollection.Sum(d => d.MinusStockAdjustValue);
                TotalOTCDailyStock.AdjustValue = DailyOTCStockValueCollection.Sum(d => d.AdjustValue);
                TotalOTCDailyStock.StockCheckValue = DailyOTCStockValueCollection.Sum(d => d.StockCheckValue);
                TotalOTCDailyStock.TrashValue = DailyOTCStockValueCollection.Sum(d => d.TrashValue);
                TotalOTCDailyStock.FinalStockValue = DailyOTCStockValueCollection[DailyOTCStockValueCollection.Count - 1].FinalStockValue;
            }
        }
        private void DownloadAction()
        {

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[GET].[StockNumReport]", parameters);
            MainWindow.ServerConnection.CloseConnection();



            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "庫存盤點";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = StartDate.ToString("yyyyMM") + "-" + "庫存盤點";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("庫存盤點明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                var col1 = ws.Column("A").Width = 8;
                var col2 = ws.Column("B").Width = 25;
                var col3 = ws.Column("C").Width = 55;
                var col4 = ws.Column("D").Width = 17;
                var col5 = ws.Column("E").Width = 10;
                var col6 = ws.Column("F").Width = 10;
                var col7 = ws.Column("G").Width = 10;
                var col8 = ws.Column("H").Width = 15;
                var col9 = ws.Column("I").Width = 10;
                var col10 = ws.Column("J").Width = 10;
                var col11 = ws.Column("K").Width = 10;
                var col12 = ws.Column("L").Width = 15;
                var col13 = ws.Column("M").Width = 10;
                var col14 = ws.Column("N").Width = 17;
                var col15 = ws.Column("O").Width = 10;
                var col16 = ws.Column("P").Width = 20;



                ws.Cell(1, 1).Value = "庫存盤點明細";
                ws.Range(1, 1, 1, 16).Merge().AddToNamed("Titles");
                ws.Range(1, 2, 1, 16).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.Range(1, 2, 1, 16).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("A2").Value = "類型";
                ws.Cell("B2").Value = "庫存碼";
                ws.Cell("C2").Value = "名稱";
                ws.Cell("D2").Value = "期初日期";
                ws.Cell("E2").Value = "期初量";
                ws.Cell("F2").Value = "進貨量";
                ws.Cell("G2").Value = "退貨量";
                ws.Cell("H2").Value = "銷貨量(調劑)";
                ws.Cell("I2").Value = "退回量";
                ws.Cell("J2").Value = "轉讓量";
                ws.Cell("K2").Value = "受讓量";
                ws.Cell("L2").Value = "盤點差異量";
                ws.Cell("M2").Value = "期末量";
                ws.Cell("N2").Value = "期末日期";
                ws.Cell("O2").Value = "期末單價";
                ws.Cell("P2").Value = "期末庫存現值";
                         

                if (result.Rows.Count > 0)
                {
                    var rangeWithData = ws.Cell(3, 1).InsertData(result.AsEnumerable());
                    rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                try
                {
                    ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                    ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                    ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                    wb.SaveAs(fdlg.FileName);

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

        #endregion Function
    }
}