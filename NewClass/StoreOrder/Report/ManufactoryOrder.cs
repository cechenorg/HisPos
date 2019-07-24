﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrder : ObservableObject
    {
        #region ----- Define Variables -----
        private bool includeTax;
        private ManufactoryOrderDetails orderDetails;
        private ICollectionView purchaseOrderCollectionView;
        private ICollectionView returnOrderCollectionView;

        public int ManufactoryID { get; set; }
        public string ManufactoryName { get; set; }
        public int PurchaseCount { get; set; }
        public double PurchasePrice { get; set; }
        public int ReturnCount { get; set; }
        public double ReturnPrice { get; set; }
        public ICollectionView PurchaseOrderCollectionView
        {
            get => purchaseOrderCollectionView;
            set
            {
                Set(() => PurchaseOrderCollectionView, ref purchaseOrderCollectionView, value);
            }
        }
        public ICollectionView ReturnOrderCollectionView
        {
            get => returnOrderCollectionView;
            set
            {
                Set(() => ReturnOrderCollectionView, ref returnOrderCollectionView, value);
            }
        }
        #endregion

        public ManufactoryOrder(DataRow dataRow)
        {
            ManufactoryID = dataRow.Field<int>("Man_ID");
            ManufactoryName = dataRow.Field<string>("Man_Name");
            PurchaseCount = dataRow.Field<int>("PURCHASE_COUNT");
            PurchasePrice = (double)dataRow.Field<decimal>("PURCHASE_PRICE");
            ReturnCount = dataRow.Field<int>("RETURN_COUNT");
            ReturnPrice = (double)dataRow.Field<decimal>("RETURN_PRICE");
        }

        #region ----- Define Functions -----
        public void GetOrderDetails(DateTime searchStartDate, DateTime searchEndDate, string wareID)
        {
            orderDetails = ManufactoryOrderDetails.GetOrderDetails(ManufactoryID, searchStartDate, searchEndDate, wareID);

            PurchaseOrderCollectionView = CollectionViewSource.GetDefaultView(orderDetails.Where(o => o.Type == OrderTypeEnum.PURCHASE));
            ReturnOrderCollectionView = CollectionViewSource.GetDefaultView(orderDetails.Where(o => o.Type == OrderTypeEnum.RETURN));
        }
        public void ExportToCSV(DateTime searchStartDate, DateTime searchEndDate)
        {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "進退貨報表存檔";
            fdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            fdlg.Filter = "報表格式|*.csv";
            fdlg.FileName = $"{ManufactoryName}進退貨報表_{searchStartDate.ToString("yyyyMMdd")}-{searchEndDate.ToString("yyyyMMdd")}";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("訂單類型,訂單編號,結案時間,未稅金額,稅額(5%),含稅金額");
                        foreach (var order in orderDetails)
                        {
                            string typeName = order.Type == OrderTypeEnum.PURCHASE ? "進貨" : "退貨";
                            file.WriteLine($"{typeName},{order.ID},{order.DoneTime.ToString("yyyyMMdd HH:mm:ss")},{order.UnTaxPrice},{order.Tax},{order.TaxPrice}");
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        #endregion
    }
}
