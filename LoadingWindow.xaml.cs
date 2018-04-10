﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.Manufactory;
using His_Pos.Class.StoreOrder;
using His_Pos.ProductPurchase;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos
{
    /// <summary>
    /// LoadingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LoadingWindow
    {
        public BackgroundWorker backgroundWorker = new BackgroundWorker();

        public LoadingWindow()
        {
            InitializeComponent();
        }

        //public LoadingWindow(string message)
        //{
        //    InitializeComponent();

        //    LoadingMessage.Content = message;

        //    backgroundWorker.WorkerSupportsCancellation = true;

        //    backgroundWorker.RunWorkerCompleted += (s, args) =>
        //    {
        //        Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            Close();
        //        }));
        //    };

        //    Show();
        //    backgroundWorker.RunWorkerAsync();
        //}

        public void GetNecessaryData(User userLogin)
        {
            MainWindow mainWindow = new MainWindow(userLogin);

            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("Loading Medicine Data...");
                MainWindow.MedicineDataTable = MedicineDb.GetMedicineData();
                MainWindow.View = new DataView(MainWindow.MedicineDataTable) { Sort = "PRO_ID" };
                
                ChangeLoadingMessage("Loading Product Data...");
                MainWindow.OtcDataTable = OTCDb.GetOtcData();

                ChangeLoadingMessage("Loading Manufactory Data...");
                MainWindow.ManufactoryTable = ManufactoryDb.GetManufactoryData();


            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    mainWindow.Show();
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void AddNewOrders(ProductPurchaseView productPurchaseView,StoreOrderProductType type, Manufactory manufactory = null)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("Adding New Orders...");

                ObservableCollection<Manufactory> manufactories = (manufactory is null)
                    ? ManufactoryDb.GetManufactoriesBasicSafe(type)
                    : new ObservableCollection<Manufactory>() { manufactory };

                foreach (Manufactory man in manufactories)
                {
                    StoreOrder newStoreOrder = new StoreOrder(MainWindow.CurrentUser, man, ProductDb.GetBasicOrSafe(man, type));
                    StoreOrderDb.SaveOrderDetail(newStoreOrder);

                    Dispatcher.Invoke((Action)(() =>
                    {
                        productPurchaseView.UpdateUi();
                        productPurchaseView.StoOrderOverview.SelectedIndex = 0;
                    }));
                }
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        private void ChangeLoadingMessage(string message)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                LoadingMessage.Content = message;
            }));
        }
    }
}
