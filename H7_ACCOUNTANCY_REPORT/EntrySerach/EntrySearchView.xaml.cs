using His_Pos.Class.Product;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

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
                FinalStockValue = row["DSV_FINAL_VALUE"].ToString();
            }
            public string Date { get; set; }
            public string InitStockValue { get; set; }
            public string PurchaseValue { get; set; }
            public string ReturnValue { get; set; }
            public string StockCheckValue { get; set; }
            public string MedUseValue { get; set; }
            public string MedIncomeValue { get; set; }
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

        public EntrySearchView()
        {
            InitializeComponent();
            ProductDb.UpdateDailyStockValue();
            DailyStockValueCollection = ProductDb.GetDailyStockValue();
            DataContext = this;

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
                    sw.WriteLine("日期" + "\t" + "期初現值" + "\t" + "進貨" + "\t" + "退貨" + "\t" + "盤點" + "\t" + "調劑耗用" + "\t" + "配藥收入" + "\t" + "期末現值");
                    foreach (DailyStockValue row in DailyStockValueCollection)
                    {
                        sw.WriteLine(row.Date + "\t" + row.InitStockValue + "\t" + row.PurchaseValue + "\t" + row.ReturnValue + "\t" + row.StockCheckValue + "\t" + row.MedUseValue + "\t" + row.MedIncomeValue + "\t" + row.FinalStockValue);
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
