using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.PrescriptionInquire;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace His_Pos.H1_DECLARE.PrescriptionInquire
{
    /// <summary>
    /// StockAdjustmentWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StockAdjustmentWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public class StockAdjust {
            public StockAdjust(Product product) {
                ProId = product.Id;
                ProName = product.Name;
            }
            public string ProId { get; set; }
            public string ProName { get; set; }
            public string OriginAmount { get; set; }
            public string AdjustAmount { get; set; }
            public string AdjustReason { get; set; } = string.Empty;
            public string Adjustmethod { get; set; } = string.Empty;
            public string BuckleStatus { get; set; }
        }

        private ObservableCollection<StockAdjust> stockAdjustCollection = new ObservableCollection<StockAdjust>();
        public ObservableCollection<StockAdjust> StockAdjustCollection
        {
            get => stockAdjustCollection;
            set
            {
                stockAdjustCollection = value;
                NotifyPropertyChanged("StockAdjustCollection");
            }
        }
        public StockAdjustmentWindow(ObservableCollection<Product> originMedCollection, ObservableCollection<Product> adjustMedCollection )
        {
            InitializeComponent();
            DataContext = this;
            foreach (Product pro in originMedCollection) {
                if (pro is DeclareMedicine) {
                    StockAdjust temp = new StockAdjust(pro);
                    temp.OriginAmount = ((DeclareMedicine)pro).Amount.ToString();
                    var tempadjust = adjustMedCollection.SingleOrDefault(x => x.Id == temp.ProId);
                    if (tempadjust != null)
                        temp.AdjustAmount = ((DeclareMedicine)tempadjust).Amount.ToString();

                    temp.BuckleStatus = Convert.ToInt32(temp.AdjustAmount) >= Convert.ToInt32(temp.OriginAmount) ? "Visible" : "Hidden";
                    if (Convert.ToInt32(temp.AdjustAmount) == Convert.ToInt32(temp.OriginAmount))
                        temp.Adjustmethod = "不調整";
                    StockAdjustCollection.Add(temp);
                } 
            }
            foreach (Product pro in adjustMedCollection) {
                if (pro is DeclareMedicine && originMedCollection.SingleOrDefault(x => x.Id == pro.Id) == null)
                {
                    StockAdjust temp = new StockAdjust(pro);
                    temp.OriginAmount = "0";
                    temp.AdjustAmount = ((DeclareMedicine)pro).Amount.ToString();
                    temp.BuckleStatus = Convert.ToInt32(temp.AdjustAmount) >= Convert.ToInt32(temp.OriginAmount) ? "Visible" : "Hidden";
                    if (Convert.ToInt32(temp.AdjustAmount) == Convert.ToInt32(temp.OriginAmount))
                        temp.Adjustmethod = "不調整";
                    StockAdjustCollection.Add(temp);
                }
            }
        }

        private void ButtonSubmmit_Click(object sender, RoutedEventArgs e)
        {
            foreach (StockAdjust stockAdjust in StockAdjustCollection) {
                if (stockAdjust.AdjustReason == string.Empty || stockAdjust.Adjustmethod == string.Empty) {
                    MessageWindow messageWindow = new MessageWindow("調整原因與調整方法皆不可為空!",MessageType.ERROR);
                    messageWindow.ShowDialog();
                    return;
                }
            } 
            PrescriptionInquireOutcome.IsAdjust = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
