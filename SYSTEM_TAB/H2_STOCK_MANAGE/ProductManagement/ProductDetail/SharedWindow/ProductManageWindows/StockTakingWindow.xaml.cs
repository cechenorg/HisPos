﻿using System;
using System.Collections.Generic;
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
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using His_Pos.NewClass.StockTaking.StockTaking;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using His_Pos.NewClass.WareHouse;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductManageWindows
{
    /// <summary>
    /// StockTakingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingWindow : Window, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        private string productID;
        private WareHouse wareHouse;
        private MedicineStockDetail stockDetail;
        private string newInventory;

        public string NewPrice { get; set; }
        public string NewInventory
        {
            get { return newInventory; }
            set
            {
                newInventory = value;
                OnPropertyChanged(nameof(IsOverage));
            }
        }
        public bool IsOverage
        {
            get
            {
                double newCheckedInventory = 0;
                bool isDouble = double.TryParse(NewInventory, out newCheckedInventory);

                if (!isDouble) return false;

                if (newCheckedInventory > stockDetail.ShelfInventory) return true;
                else return false;
            }
        }
        #endregion

        public StockTakingWindow(string proID, WareHouse ware, MedicineStockDetail stock)
        {
            InitializeComponent();

            DataContext = this;

            productID = proID;
            wareHouse = ware;
            stockDetail = stock;
            NewPrice = stock.LastPrice.ToString("0.##");
        }

        #region ----- Define Functions -----
        private bool IsNewInventoryValid()
        {
            if (NewInventory.Equals(""))
            {
                MessageWindow.ShowMessage("盤點架上量不得為空!", MessageType.ERROR);
                return false;
            }
            else
            {
                double newCheckedInventory = 0;
                bool isDouble = double.TryParse(NewInventory, out newCheckedInventory);

                if (!isDouble)
                {
                    MessageWindow.ShowMessage("輸入數值錯誤!", MessageType.ERROR);
                    return false;
                }

                if (newCheckedInventory < 0)
                {
                    MessageWindow.ShowMessage("輸入數值不可小於0!", MessageType.ERROR);
                    return false;
                }
            }

            if (NewPrice.Equals("") && IsOverage)
            {
                MessageWindow.ShowMessage("盤點單價不得為空!", MessageType.ERROR);
                return false;
            }
            else if(IsOverage)
            {
                double newCheckedPrice = 0;
                bool isDouble = double.TryParse(NewPrice, out newCheckedPrice);

                if (!isDouble)
                {
                    MessageWindow.ShowMessage("輸入數值錯誤!", MessageType.ERROR);
                    return false;
                }
            }

            return true;
        }
        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsNewInventoryValid()) return;

            double finalInv = (stockDetail.MedBagInventory > stockDetail.TotalInventory) ? stockDetail.TotalInventory + double.Parse(NewInventory) : stockDetail.MedBagInventory + double.Parse(NewInventory);

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將總庫存調整為 {finalInv} ?", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            StockTaking stockTaking = new StockTaking();
            stockTaking.SingleStockTaking(productID, stockDetail.TotalInventory, finalInv, double.Parse(NewPrice), wareHouse);
            MainWindow.ServerConnection.CloseConnection();

            DialogResult = true;
            Close();
        }
        #endregion

        #region ----- Define PropertyChanged -----
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
