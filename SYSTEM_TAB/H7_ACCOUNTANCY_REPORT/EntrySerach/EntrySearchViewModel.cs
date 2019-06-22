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
        #endregion
        #region Command
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ExportCsvCommand { get; set; }
        public RelayCommand ShowEntryDetailCommand { set; get; }
        #endregion
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
        private void ShowEntryDetailAction() {
           EntryDetailWindow entryDetailWindow = new EntryDetailWindow(SelectStockValue.Date); 
        }
        private void ExportCsv() { 
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
        
        private void Search() { 
            DailyStockValueCollection.Clear();
            DailyStockValueCollection.GetDataByDate(StartDate, EndDate, SelectedWareHouse.ID);
            CaculateTotalStock();
        }
        private void CaculateTotalStock() {
            if (DailyStockValueCollection.Count > 0) {
                TotalDailyStock.InitStockValue = DailyStockValueCollection[0].InitStockValue;
                TotalDailyStock.PurchaseValue = DailyStockValueCollection.Sum(d => d.PurchaseValue);
                TotalDailyStock.ReturnValue = DailyStockValueCollection.Sum(d => d.ReturnValue);
                TotalDailyStock.MedUseValue = DailyStockValueCollection.Sum(d => d.MedUseValue);
                TotalDailyStock.MinusStockAdjustValue = DailyStockValueCollection.Sum(d => d.MinusStockAdjustValue); 
                TotalDailyStock.StockCheckValue = DailyStockValueCollection.Sum(d => d.StockCheckValue); 
                TotalDailyStock.FinalStockValue = DailyStockValueCollection[DailyStockValueCollection.Count - 1].FinalStockValue;
            }
       
        }
        #endregion
    }
} 
 
