using His_Pos.Class.Product;
using His_Pos.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace His_Pos.H7_ACCOUNTANCY_REPORT.EntrySerach
{
    /// <summary>
    /// EntrySearchView.xaml 的互動邏輯
    /// </summary>
    public partial class EntrySearchView : System.Windows.Controls.UserControl, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class DailyStockValue {
            public DailyStockValue() {

            }
            public DailyStockValue(DataRow row) {
                Date =  DateTimeExtensions.ToSimpleTaiwanDate(Convert.ToDateTime(row["DSV_DATE"]));
                InitStockValue = row["DSV_INITIAL_VALUE"].ToString();
                PurchaseValue = row["DSV_PURCHASE_VALUE"].ToString();
                ReturnValue = row["DSV_RETURN_VALUE"].ToString();
                StockCheckValue = row["DSV_STOCKCHECK_VALUE"].ToString();
                MedUseValue = row["DSV_MEDUSE_VALUE"].ToString();
                FinalStockValue = row["DSV_FINAL_VALUE"].ToString();
            }
            public string Date { get; set; }
            public string InitStockValue { get; set; }
            public string PurchaseValue { get; set; }
            public string ReturnValue { get; set; }
            public string StockCheckValue { get; set; }
            public string MedUseValue { get; set; }
            public string FinalStockValue { get; set; }

        }
        private ObservableCollection<DailyStockValue> dailyStockValueCollection;
        public ObservableCollection<DailyStockValue> DailyStockValueCollection
        {
            get => dailyStockValueCollection;
            set
            {
                dailyStockValueCollection = value;
                NotifyPropertyChanged("DailyStockValueCollection");
            }
        }
        private DailyStockValue totalDailyStock = new DailyStockValue();
        public DailyStockValue TotalDailyStock
        {
            get => totalDailyStock;
            set
            {
                totalDailyStock = value;
                NotifyPropertyChanged("TotalDailyStock");
            }
        }
        private DateTime startDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }
        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }

        public static EntrySearchView Instance;
        public EntrySearchView()
        {
            InitializeComponent();
            InitData();
            DataContext = this;
            Instance = this;
        }
        public void InitData(string startDate = null,string endDate = null) {
            ProductDb.UpdateDailyStockValue();
            if(startDate == null)
                DailyStockValueCollection = ProductDb.GetDailyStockValue(TotalDailyStock);
            else
                DailyStockValueCollection = ProductDb.GetDailyStockValue(TotalDailyStock, startDate, endDate);
            LabelstartDate.Content = StartDate.AddYears(-1911).ToString("yyy/MM/dd");
            LabelendDate.Content = EndDate.AddYears(-1911).ToString("yyy/MM/dd");
        }
        private void ButtonPrint_Click(object sender, RoutedEventArgs e) {
            
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
                    foreach (DailyStockValue row in DailyStockValueCollection)
                    {
                        sw.WriteLine(row.Date + "\t" + row.InitStockValue + "\t" + row.PurchaseValue + "\t" + row.ReturnValue + "\t" + row.StockCheckValue + "\t" + row.MedUseValue + "\t"  + row.FinalStockValue );
                    }
                    sw.Close();
                } 
            }

        }

        private void showEntryDetail(object sender, MouseButtonEventArgs e) {
            var selectedItem = (sender as DataGridRow).Item;
            if (((DailyStockValue)selectedItem).Date != "總和") {
                EntryDetailWindow entryDetailWindow = new EntryDetailWindow(((DailyStockValue)selectedItem).Date);
                entryDetailWindow.ShowDialog(); 
            }
        
        }

        private void start_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox t)
            {
                t.SelectionStart = 0;
                t.SelectionLength = t.Text.Length;
            }
        }

      
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            InitData(StartDate.ToString("yyyy-MM-dd"),EndDate.ToString("yyyy-MM-dd"));
            NotifyPropertyChanged("TotalDailyStock");
        }
    }
}
