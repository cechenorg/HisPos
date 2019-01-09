using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.FunctionWindow;
using His_Pos.Interface;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement
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
       
        private IInventory Inventory;
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
        public DemolitionWindow(ObservableCollection<ProductGroup> productGroupCollection, IInventory inventory)
        {
            InitializeComponent();
            Inventory = inventory;
            InitData(productGroupCollection);
            DataContext = this;
        }
        private void InitData(ObservableCollection<ProductGroup> productGroupCollection) {
            ProductGroups = productGroupCollection;
            ComboBoxProduct.ItemsSource = ProductGroups;
            ComboBoxProduct.Text = Inventory.Name;
            if(productGroupCollection.Count(pro => pro.name == Inventory.Name) == 0)
            {
                ComboBoxProduct.Text = ((InventoryMedicine)Inventory).ChiName;
            }
               
           ///WareHouseInventoryCollection = WareHouseDb.GetWareHouseInventoryById(Inventory.Id);
        }

        private void TextAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if( String.IsNullOrEmpty((sender as TextBox).Text) ) return;
        }

        private void ButtonSubnmmit_Click(object sender, RoutedEventArgs e)
        {
            if (WareHouseInventoryCollection.Count(war => Convert.ToInt32(war.AfterDemolitionAmount) < 0) > 0) {
                MessageWindow messageWindows = new MessageWindow("拆庫後剩餘量不可為負!", MessageType.ERROR,true);
                messageWindows.ShowDialog();
                return;
            }
            string newInvId = "";/// ProductDb.GetMaxProInvId();
            string proId =  ProductGroups.Single(pro => pro.id == ((ProductGroup)ComboBoxProduct.SelectedItem).id).id;
            foreach (WareHouseInventory wareHouseInventory in WareHouseInventoryCollection)
            {
                if (wareHouseInventory.DemolitionAmount == "0") continue;
                ///ProductDb.DemolitionProduct(newInvId,proId, wareHouseInventory.warId,wareHouseInventory.DemolitionAmount);
            }
            MessageWindow.ShowMessage("拆庫成功",MessageType.SUCCESS, true);
            messageWindow.ShowDialog();
            Close();
        }
      
      
    }
}
