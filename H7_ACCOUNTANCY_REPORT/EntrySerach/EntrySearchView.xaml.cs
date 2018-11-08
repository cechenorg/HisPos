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
            public DailyStockValue(DataRow row) {
                Date =  DateTimeExtensions.ToSimpleTaiwanDate(Convert.ToDateTime(row["DSV_DATE"]));
                InitStockValue = row["DSV_INITIAL_VALUE"].ToString();
                PurchaseValue = row["DSV_PURCHASE_VALUE"].ToString();
                ReturnValue = row["DSV_RETURN_VALUE"].ToString();
                StockCheckValue = row["DSV_STOCKCHECK_VALUE"].ToString();
                MedUseValue = row["DSV_MEDUSE_VALUE"].ToString();
                MedIncomeValue = row["DSV_MEDINCOME_VALUE"].ToString();
                CopayMentValue = row["DSV_COPAYMENT_VALUE"].ToString();
                PaySelfValue = row["DSV_PAYSELF_VALUE"].ToString();
                DepositValue = row["DSV_DEPOSIT_VALUE"].ToString();
                FinalStockValue = row["DSV_FINAL_VALUE"].ToString();
            }
            public string Date { get; set; }
            public string InitStockValue { get; set; }
            public string PurchaseValue { get; set; }
            public string ReturnValue { get; set; }
            public string StockCheckValue { get; set; }
            public string MedUseValue { get; set; }
            public string MedIncomeValue { get; set; }
            public string CopayMentValue { get; set; }
            public string PaySelfValue { get; set; }
            public string DepositValue { get; set; } //押金
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
        public static EntrySearchView Instance;
        public EntrySearchView()
        {
            InitializeComponent();
            InitData();
            DataContext = this;
            Instance = this;
        }
        public void InitData() {
            ProductDb.UpdateDailyStockValue();
            DailyStockValueCollection = ProductDb.GetDailyStockValue();
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
                    sw.WriteLine("日期" + "\t" + "期初現值" + "\t" + "進貨" + "\t" + "退貨" + "\t" + "盤點" + "\t" + "調劑耗用" + "\t" + "期末現值" + "\t\t\t\t" + "日期" + "\t" + "部分負擔" + "\t" + "自費" + "'\t" + "押金" + "\t" + "配藥收入");
                    foreach (DailyStockValue row in DailyStockValueCollection)
                    {
                        sw.WriteLine(row.Date + "\t" + row.InitStockValue + "\t" + row.PurchaseValue + "\t" + row.ReturnValue + "\t" + row.StockCheckValue + "\t" + row.MedUseValue + "\t" + "\t" + row.FinalStockValue + "\t\t\t\t" 
                            + row.Date + "\t" + row.CopayMentValue + "\t" + row.PaySelfValue + "\t" + row.DepositValue + "\t" + row.MedIncomeValue);
                    }
                    sw.Close();
                } 
            }

        }

        private void showEntryDetail(object sender, MouseButtonEventArgs e) {
            var selectedItem = (sender as DataGridRow).Item;
            EntryDetailWindow entryDetailWindow = new EntryDetailWindow( ((DailyStockValue)selectedItem).Date);
            entryDetailWindow.ShowDialog();
        }
    }
}
