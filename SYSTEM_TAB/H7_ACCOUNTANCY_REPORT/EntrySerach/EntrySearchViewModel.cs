using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StockValue;
using His_Pos.NewClass.WareHouse;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        public RelayCommand ShowEntryDetailCommand { set; get; }

        #endregion Command

        public EntrySearchViewModel()
        {
            WareHouseCollection = WareHouses.GetWareHouses();
            SelectedWareHouse = WareHouseCollection[0];
            Search();
            SearchCommand = new RelayCommand(Search);
            ExportCsvCommand = new RelayCommand(ExportCsv);
            ShowEntryDetailCommand = new RelayCommand(ShowEntryDetailAction);
        }

        #region Function

        private void ShowEntryDetailAction()
        {
            EntryDetailWindow.EntryDetailWindow entryDetailWindow = new EntryDetailWindow.EntryDetailWindow(SelectStockValue.Date);
        }

        private void ExportCsv()
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
                    foreach (var row in DailyStockValueCollection)
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

        #endregion Function
    }
}