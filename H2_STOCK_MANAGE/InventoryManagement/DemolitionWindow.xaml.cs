using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.InventoryManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace His_Pos.H2_STOCK_MANAGE.InventoryManagement
{
    /// <summary>
    /// DemolitionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DemolitionWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
       
        private InventoryOtc InventoryOtc;
        private ObservableCollection<ProductGroup> productGroups;
        public ObservableCollection<ProductGroup> ProductGroups
        {
            get { return productGroups; }
            set
            {
                productGroups = value;
                NotifyPropertyChanged("ProductGroups");
            }
        }
        private ObservableCollection<WareHouseInventory> wareHouseInventoryCollection;
        public ObservableCollection<WareHouseInventory> WareHouseInventoryCollection
        {
            get { return wareHouseInventoryCollection; }
            set
            {
                wareHouseInventoryCollection = value;
                NotifyPropertyChanged("WareHouseInventoryCollection");
            }
        }
        public class WareHouseInventory : INotifyPropertyChanged {
            public WareHouseInventory(DataRow row){
                warId = row["PROWAR_ID"].ToString();
                warName = row["PROWAR_NAME"].ToString();
                Inventory = row["PRO_INVENTORY"].ToString();
                DemolitionAmount = "0";
                AfterDemolitionAmount = (Convert.ToInt32(Inventory) - Convert.ToInt32(demolitionAmount)).ToString();
            }
            public string warId { get; set; }
            public string warName { get; set; }
            public string Inventory { get; set; }
            private string demolitionAmount;
            public string DemolitionAmount
            {
                get { return demolitionAmount; }
                set
                {
                    demolitionAmount = value;
                    AfterDemolitionAmount = String.IsNullOrEmpty(demolitionAmount) ? "0" :(Convert.ToInt32(Inventory) - Convert.ToInt32(demolitionAmount)).ToString();
                }
            }
            private string afterDemolitionAmount;
            public string AfterDemolitionAmount
            {
                get { return afterDemolitionAmount; }
                set
                {
                    afterDemolitionAmount = value;
                    NotifyPropertyChanged("AfterDemolitionAmount");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(string info) {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

        }
        public DemolitionWindow(ObservableCollection<ProductGroup> productGroupCollection, InventoryOtc inventoryOtc)
        {
            InitializeComponent();
            InventoryOtc = inventoryOtc;
            InitData(productGroupCollection);
            DataContext = this;
        }
        private void InitData(ObservableCollection<ProductGroup> productGroupCollection) {
            ProductGroups = productGroupCollection;
            ComboBoxProduct.ItemsSource = ProductGroups;
            ComboBoxProduct.Text = InventoryOtc.Name;
            WareHouseInventoryCollection = WareHouseDb.GetWareHouseInventoryById(InventoryOtc.Id);
        }

        private void TextAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if( String.IsNullOrEmpty((sender as TextBox).Text) ) return;
        }

        private void ButtonSubnmmit_Click(object sender, RoutedEventArgs e)
        {
            if (WareHouseInventoryCollection.Count(war => Convert.ToInt32(war.AfterDemolitionAmount) < 0) > 0) {
                MessageWindow messageWindows = new MessageWindow("拆庫後剩餘量不可為負!", MessageType.ERROR);
                messageWindows.ShowDialog();
                return;
            }
            string newInvId = ProductDb.GetMaxProInvId();
            string proId = ProductGroups.Single(pro => pro.name == ComboBoxProduct.Text).id;
            foreach (WareHouseInventory wareHouseInventory in WareHouseInventoryCollection)
            {
                if (wareHouseInventory.DemolitionAmount == "0") continue;
                ProductDb.DemolitionProduct(newInvId,proId, wareHouseInventory.warId,wareHouseInventory.DemolitionAmount);
            }
            MessageWindow messageWindow = new MessageWindow("拆庫成功",MessageType.SUCCESS);
            messageWindow.ShowDialog();
            Close();
        }
      
      
    }
}
