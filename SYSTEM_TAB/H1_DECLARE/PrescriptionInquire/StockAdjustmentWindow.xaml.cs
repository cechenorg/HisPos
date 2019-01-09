using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionInquire
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
            public string OriginAmount { get; set; } = "0";
            public string AdjustAmount { get; set; } = "0";
            public string AdjustReason { get; set; } = string.Empty;
            public string Adjustmethod { get; set; } = string.Empty;
            public Visibility IsBuckle { get; set; } = Visibility.Collapsed;
            public Visibility IsAddStock { get; set; } = Visibility.Collapsed;
        }
        public string DecMasId;
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
        public StockAdjustmentWindow(string decMasId,ObservableCollection<Product> originMedCollection, ObservableCollection<Product> adjustMedCollection )
        {
            InitializeComponent();
            DecMasId = decMasId;
            DataContext = this;
            foreach (Product pro in originMedCollection) {
                if (pro is DeclareMedicine) {
                   
                    StockAdjust temp = new StockAdjust(pro);
                    temp.OriginAmount = ((DeclareMedicine)pro).Amount.ToString();
                    var tempadjust = adjustMedCollection.SingleOrDefault(x => x.Id == temp.ProId);
                    if (tempadjust != null)
                        temp.AdjustAmount = ((DeclareMedicine)tempadjust).Amount.ToString();
                    if (((DeclareMedicine)pro).IsBuckle == false) {
                        temp.OriginAmount = "0";
                    }
                       
                    temp.Adjustmethod = Convert.ToInt32(temp.AdjustAmount) >= Convert.ToInt32(temp.OriginAmount) ? "扣庫" : "補回庫存";
                    if (Convert.ToInt32(temp.AdjustAmount) == Convert.ToInt32(temp.OriginAmount)) {
                        temp.Adjustmethod = "不調整";
                        continue;
                    }
                     

                    switch (temp.Adjustmethod) {
                        case "扣庫":
                            temp.IsBuckle = Visibility.Visible;
                            break;
                        case "補回庫存":
                            temp.IsAddStock = Visibility.Visible;
                            break;  
                    }
                    StockAdjustCollection.Add(temp);
                } 
            }
            foreach (Product pro in adjustMedCollection) {
                if (pro is DeclareMedicine && originMedCollection.SingleOrDefault(x => x.Id == pro.Id) == null)
                {
                    StockAdjust temp = new StockAdjust(pro);
                    temp.OriginAmount = "0";
                    temp.AdjustAmount = ((DeclareMedicine)pro).Amount.ToString();
                    temp.Adjustmethod = Convert.ToInt32(temp.AdjustAmount) >= Convert.ToInt32(temp.OriginAmount) ? "扣庫" : "補回庫存";
                    if (Convert.ToInt32(temp.AdjustAmount) == Convert.ToInt32(temp.OriginAmount))
                        temp.Adjustmethod = "不調整";

                    switch (temp.Adjustmethod)
                    {
                        case "扣庫":
                            temp.IsBuckle = Visibility.Visible;
                            break;
                        case "補回庫存":
                            temp.IsAddStock = Visibility.Visible;
                            break;
                    }
                    StockAdjustCollection.Add(temp);
                }
            }
        }

        private void ButtonSubmmit_Click(object sender, RoutedEventArgs e)
        {
            foreach (StockAdjust stockAdjust in StockAdjustCollection) {
                if (stockAdjust.AdjustReason == string.Empty || stockAdjust.Adjustmethod == string.Empty) {
                    MessageWindow.ShowMessage("調整原因與調整方法皆不可為空!",MessageType.ERROR);
                    messageWindow.ShowDialog();
                    return;
                }
            }
            double totalPrice = 0;
            foreach (StockAdjust stockAdjust in StockAdjustCollection)
            {
                if (stockAdjust.Adjustmethod == "扣庫") {

                    totalPrice -= 0;/// double.Parse(ProductDb.GetBucklePrice(stockAdjust.ProId, (Convert.ToInt32(stockAdjust.AdjustAmount) - Convert.ToInt32(stockAdjust.OriginAmount)).ToString()));
                    ///ProductDb.BuckleInventory(stockAdjust.ProId, (Convert.ToInt32(stockAdjust.AdjustAmount) - Convert.ToInt32(stockAdjust.OriginAmount)).ToString(),"處方修改調整",DecMasId);
                }
                if (stockAdjust.Adjustmethod == "補回庫存") {
                    totalPrice += 0;/// double.Parse(ProductDb.GetAddStockValue(DecMasId, stockAdjust.ProId, (Convert.ToInt32(stockAdjust.OriginAmount) - Convert.ToInt32(stockAdjust.AdjustAmount)).ToString()));
                    ///ProductDb.RecoveryInventory( DecMasId, stockAdjust.ProId, (Convert.ToInt32(stockAdjust.OriginAmount) - Convert.ToInt32(stockAdjust.AdjustAmount)).ToString());
                }
            }
            ///ProductDb.InsertEntry("處方修改調整", totalPrice.ToString(), "DecMasId", DecMasId);

            PrescriptionInquireOutcome.IsAdjust = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
