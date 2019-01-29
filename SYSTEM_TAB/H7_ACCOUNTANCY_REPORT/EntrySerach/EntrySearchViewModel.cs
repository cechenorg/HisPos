﻿using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StockValue;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach
{
    public   class EntrySearchViewModel : TabBase
    {
        #region Var
        public override TabBase getTab()
        {
            return this;
        }
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
        #endregion
        #region Command
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ExportCsvCommand { get; set; }
        #endregion
        public EntrySearchViewModel() {
            Search();
            SearchCommand = new RelayCommand(Search);
            ExportCsvCommand = new RelayCommand(ExportCsv);
        }
        #region Function 
         
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
                    sw.WriteLine("日期" + "\t" + "期初現值" + "\t" + "進貨" + "\t" + "退貨" + "\t" + "盤點" + "\t" + "調劑耗用" + "\t" + "期末現值");
                    foreach (var row in DailyStockValueCollection)
                    {
                        sw.WriteLine(row.Date + "\t" + row.InitStockValue + "\t" + row.PurchaseValue + "\t" + row.ReturnValue + "\t" + row.StockCheckValue + "\t" + row.MedUseValue + "\t" + row.FinalStockValue);
                    }
                    sw.Close();
                }
            }

        } 
        
        private void Search() {
            StockValue.UpdateDailyStockValue();
            DailyStockValueCollection.GetDataByDate(StartDate, EndDate);
            CaculateTotalStock();
        }
        private void CaculateTotalStock() {
            TotalDailyStock.InitStockValue = DailyStockValueCollection[0].InitStockValue;
            TotalDailyStock.PurchaseValue = DailyStockValueCollection.Sum(d => d.PurchaseValue);
            TotalDailyStock.ReturnValue = DailyStockValueCollection.Sum(d => d.ReturnValue);
            TotalDailyStock.MedUseValue = DailyStockValueCollection.Sum(d => d.MedUseValue);
            TotalDailyStock.StockCheckValue = DailyStockValueCollection.Sum(d => d.StockCheckValue);
        }
        #endregion
    }
} 
 