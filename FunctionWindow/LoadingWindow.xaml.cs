using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using His_Pos.AbstractClass;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.Class.StockTakingOrder;
using His_Pos.Class.StoreOrder;
using His_Pos.Interface;
using His_Pos.NewClass.Person.Employee;
using His_Pos.Struct.Product;
using His_Pos.SYSTEM_TAB.H1_DECLARE.MedBagManage;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingRecord;
using Microsoft.Reporting.WinForms;
using StockTakingView = His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking.StockTakingView;

namespace His_Pos.FunctionWindow
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
            Show();
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
        
        public void GetNecessaryData(Employee userLogin)
        {
            MainWindow mainWindow = new MainWindow(userLogin);
            backgroundWorker.DoWork += (s, o) =>
            {

                ///if (FunctionDb.CheckYearlyHoliday())
                ///{
                ///    ChangeLoadingMessage("更新假日資料...");
                ///    Function function = new Function();
                ///    function.GetLastYearlyHoliday();
                ///}
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

        public void AddNewOrders(ProductPurchaseView productPurchaseView, StoreOrderProductType type,WareHouse wareHouse, Manufactory manufactory = null)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("新增新處理單...");
                ///ManufactoryDb.AddNewOrderBasicSafe(type, wareHouse, manufactory);

                ChangeLoadingMessage("取得進退貨資料...");
                Dispatcher.Invoke((Action)(() =>
                {
                    ObservableCollection<StoreOrder> tempStoreOrderCollection = null;///StoreOrderDb.GetStoreOrderOverview(OrderType.ALL);

                    foreach (StoreOrder stoOrd in tempStoreOrderCollection)
                    {
                        if (stoOrd.Type == OrderType.WAITING)
                        {
                            //Check Order Status
                        }
                    }

                    productPurchaseView.StoreOrderCollection = tempStoreOrderCollection;
                    productPurchaseView.StoOrderOverview.SelectedIndex = 0;
                }));
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

        internal void GetProductPurchaseData(ProductPurchaseView productPurchaseView)
        {
            productPurchaseView.AllGrid.IsEnabled = false;

            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得杏德新訂單...");
                ///StoreOrderDb.GetNewStoreOrderBySingde();

                ChangeLoadingMessage("取得廠商資料...");
                ObservableCollection<Manufactory> tempManufactories = null;///ManufactoryDb.GetManufactoryData();

                ChangeLoadingMessage("取得商品資料...");
                Collection<PurchaseProduct> tempProduct = null;/// ProductDb.GetItemDialogProduct();

                ChangeLoadingMessage("取得進退貨資料...");
                Collection<StoreOrder> tempStoreOrderCollection = null;/// StoreOrderDb.GetStoreOrderOverview(OrderType.ALL);

                foreach (StoreOrder stoOrd in tempStoreOrderCollection)
                {
                    if (stoOrd.Type == OrderType.WAITING)
                    {
                        if (stoOrd.DeclareDataCount > 0)
                        {
                            ///stoOrd.Type = StoreOrderDb.GetDeclareOrderStatusFromSinde(stoOrd.Id);
                        }
                        else
                        {
                            ///stoOrd.Type = StoreOrderDb.GetOrderStatusFromSinde(stoOrd.Id);
                        }

                        if (stoOrd.Type == OrderType.PROCESSING)
                            productPurchaseView.CheckSindeOrderDetail(stoOrd);
                        else if (stoOrd.Type == OrderType.SCRAP) ;
                            ///StoreOrderDb.DeleteOrder(stoOrd.Id);
                    }
                }

                Dispatcher.Invoke((Action)(() =>
                {
                    productPurchaseView.ManufactoryAutoCompleteCollection = tempManufactories;

                    productPurchaseView.SetControlProduct(tempProduct);
                    
                    productPurchaseView.StoreOrderCollection = new ObservableCollection<StoreOrder>(tempStoreOrderCollection.Where(so => so.Type != OrderType.SCRAP).ToList());

                    var cancelOrderList = tempStoreOrderCollection.Where(so => so.Type == OrderType.SCRAP).Select(so => so.Id).ToList();

                    if (cancelOrderList.Count > 0)
                    {
                        string orders = cancelOrderList.Aggregate("", (i, j) => i + " " + j);
                        
                        MessageWindow.ShowMessage($"處理單{orders} 已被杏德取消!\r\n取消處理單可在訂退貨記錄查詢", MessageType.WARNING);
                        
                    }

                }));
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    productPurchaseView.AllGrid.IsEnabled = true;

                    if (productPurchaseView.StoOrderOverview.Items.Count != 0)
                        productPurchaseView.StoOrderOverview.SelectedIndex = 0;

                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        internal void InitProductType(ProductTypeManageView productTypeManageView)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("處理類別資料...");

                ObservableCollection<Product> newProducts = null;///ProductDb.GetProductTypeManageProducts();

                Dispatcher.Invoke((Action)(() =>
                {
                    productTypeManageView.InitTypes();
                    productTypeManageView.Products = newProducts;
                    productTypeManageView.InitPieCharts();
                    productTypeManageView.InitMonthsAndDays();
                }));
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    productTypeManageView.TypeMaster.ItemsSource = productTypeManageView.TypeManageMasters;
                    productTypeManageView.TypeDetail.ItemsSource = productTypeManageView.TypeManageDetails;
                    productTypeManageView.TypeMaster.SelectedIndex = 0;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void MergeProductInventory(InventoryManagementView inventoryManagementView)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("讀取商品資料...");
                string totalWorth = null;/// ProductDb.GetTotalWorth();
                double stockValue = 0;
                inventoryManagementView.InventoryMedicines = null;/// MedicineDb.GetInventoryMedicines();
                inventoryManagementView.InventoryOtcs = null;/// OTCDb.GetInventoryOtcs();
                inventoryManagementView.ProductCollection = null;/// ProductDb.GetItemDialogProduct();
                ChangeLoadingMessage("載入商品資料...");
                Dispatcher.Invoke((Action)(() =>
                {
                    ObservableCollection<Product> products = new ObservableCollection<Product>();

                    foreach (DataRow k in inventoryManagementView.InventoryOtcs.Rows)
                    {
                        InventoryOtc otc = new InventoryOtc(k);

                        products.Add(otc);

                        stockValue += Double.Parse(otc.StockValue);
                    }

                    foreach (DataRow m in inventoryManagementView.InventoryMedicines.Rows)
                    {
                        InventoryMedicine medicine = new InventoryMedicine(m);

                        products.Add(medicine);

                        stockValue += Double.Parse(medicine.StockValue);
                    }

                    inventoryManagementView._DataList = products; 
                    inventoryManagementView.TotalStockValue.Content = totalWorth; 
                    inventoryManagementView.selectStockValue = 0;
                    inventoryManagementView.searchCount = 0;
                    inventoryManagementView.SearchData();
                    inventoryManagementView.SearchCount.Content = inventoryManagementView.searchCount;
                    inventoryManagementView.SelectStockValue.Content = inventoryManagementView.selectStockValue.ToString();
                }));
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    inventoryManagementView.Search.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void GetLocation(LocationManageView locationManageView)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得櫃位資料...");

                var location = new DataTable();/// LocationDb.GetLocationData();

                Dispatcher.Invoke((Action)(() =>
                {
                    locationManageView.LocationCanvus.Children.Clear();
                    foreach (DataRow row in location.Rows)
                    {
                        locationManageView.NewLocation(row["LOC_ID"].ToString(), row["LOC_NAME"].ToString(), Convert.ToDouble(row["LOC_HEIGHT"].ToString()), Convert.ToDouble(row["LOC_WIDTH"].ToString()), Convert.ToDouble(row["LOC_Y"].ToString()), Convert.ToDouble(row["LOC_X"].ToString()));
                    }
                }));
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
        
        

        public void GetStockTakingRecord(StockTakingRecordView stockTakingRecord)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得盤點紀錄資料...");

                var Products = new DataTable();/// StockTakingOrderDb.GetStockTakingRecord();

                Dispatcher.Invoke((Action)(() =>
                {
                    ObservableCollection<StockTakingOrderProduct> products = new ObservableCollection<StockTakingOrderProduct>();
                    var lastProcheId = string.Empty;
                    int count = 1;
                    StockTakingOrder stockTakingOrder;
                    foreach (DataRow p in Products.Rows)
                    {
                        if (p["PROCHE_ID"].ToString() != lastProcheId && count != 1)
                        {
                            stockTakingOrder = new StockTakingOrder(lastProcheId);
                            foreach (var product in products)
                            {
                                if (product.OldValue == product.NewValue)
                                    stockTakingOrder.UnchangedtakingCollection.Add(product);
                                else
                                    stockTakingOrder.ChangedtakingCollection.Add(product);
                            }
                            stockTakingOrder.Amount = products.Count;
                            stockTakingRecord.StocktakingCollection.Add(stockTakingOrder);
                            products.Clear();
                        }
                        else if (count == Products.Rows.Count)
                        {
                            StockTakingOrderProduct laststockTakingOrderProduct = new StockTakingOrderProduct(p);
                            products.Add(laststockTakingOrderProduct);
                            stockTakingOrder = new StockTakingOrder(lastProcheId);
                            foreach (var product in products)
                            {
                                if (product.OldValue == product.NewValue)
                                    stockTakingOrder.UnchangedtakingCollection.Add(product);
                                else
                                    stockTakingOrder.ChangedtakingCollection.Add(product);
                            }
                            stockTakingOrder.Amount = products.Count;
                            stockTakingRecord.StocktakingCollection.Add(stockTakingOrder);
                            products.Clear();
                            break;
                        }
                        StockTakingOrderProduct stockTakingOrderProduct = new StockTakingOrderProduct(p);
                        products.Add(stockTakingOrderProduct);
                        count++;
                        lastProcheId = p["PROCHE_ID"].ToString(); 
                    }
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                { 
                    if (stockTakingRecord.StockTakingRecord.Items.Count > 0) {
                        stockTakingRecord.StockTakingRecord.ScrollIntoView(stockTakingRecord.StockTakingRecord.Items[stockTakingRecord.StockTakingRecord.Items.Count - 1]);
                        stockTakingRecord.StockTakingRecord.SelectedIndex = stockTakingRecord.StockTakingRecord.Items.Count - 1;
                    }
                       
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void GetMedBagData(MedBagManageView medBagManageView)
        {
            medBagManageView.MedBagManageViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得藥袋資料...");
                var tmpMedBags = "";/// MedBagDb.ObservableGetMedBagData();
                Dispatcher.Invoke((Action)(() =>
                {
                    medBagManageView.MedBagCollection = null; /// tmpMedBags;
                    medBagManageView.MedBags.ItemsSource = medBagManageView.MedBagCollection;
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    medBagManageView.MedBagManageViewBox.IsEnabled = true;
                    if (medBagManageView.MedBags.Items.Count > 0)
                        medBagManageView.MedBags.SelectedIndex = 0;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void MergeProductStockTaking(StockTakingView stockTakingView)
        {
            stockTakingView.AddItems.IsEnabled = false;
            stockTakingView.AddStockItems.IsEnabled = false;
            stockTakingView.AddOneItem.IsEnabled = false;
            stockTakingView.ClearProduct.IsEnabled = false;
            stockTakingView.FinishedAddProduct.IsEnabled = false;
            stockTakingView.Print.IsEnabled = false;

            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("處理商品資料...");

                var Products = new DataTable();/// ProductDb.GetStockTakingProduct();

                Dispatcher.Invoke((Action)(() =>
                {
                    ObservableCollection<Product> products = new ObservableCollection<Product>();
                    var lastProid = string.Empty;
                    foreach (DataRow p in Products.Rows)
                    {
                        switch (p["P_TYPE"].ToString())
                        {
                            case "O" when lastProid == p["PRO_ID"].ToString():
                                BatchNumbers obatchnumber = new BatchNumbers(p);
                                ((StockTakingOTC)products[products.Count - 1]).BatchNumbersCollection.Add(obatchnumber);
                                break;

                            case "O":
                                StockTakingOTC otc = new StockTakingOTC(p);
                                products.Add(otc);
                                break;

                            case "M" when lastProid == p["PRO_ID"].ToString():
                                BatchNumbers mbatchnumber = new BatchNumbers(p);
                                ((StockTakingMedicine)products[products.Count - 1]).BatchNumbersCollection.Add(mbatchnumber);
                                break;

                            case "M":
                                StockTakingMedicine medicine = new StockTakingMedicine(p);
                                products.Add(medicine);
                                break;
                        }

                        lastProid = p["PRO_ID"].ToString();
                    }

                    stockTakingView.ProductCollection = products;
                }));
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    stockTakingView.AddItems.IsEnabled = true;
                    stockTakingView.AddStockItems.IsEnabled = true;
                    stockTakingView.AddOneItem.IsEnabled = true;
                    stockTakingView.FinishedAddProduct.IsEnabled = true;
                    stockTakingView.ClearProduct.IsEnabled = true;
                    stockTakingView.Print.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        
        public void ChangeLoadingMessage(string message,int sec = 0)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                LoadingMessage.Content = message;
            }));
        }

        public void SetMedBagData(MedBagManageView medBagManageView)
        {
            medBagManageView.MedBagManageViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("藥袋資料儲存中...");
                medBagManageView.
                    Dispatcher.Invoke((Action)(medBagManageView.SaveMedBagData));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    medBagManageView.MedBagManageViewBox.IsEnabled = true;
                    MessageWindow.ShowMessage("藥袋儲存成功", MessageType.SUCCESS);
                    
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        
        public void PrintInventoryCheckSheet(ReportViewer rptViewer,StockTakingView stockTakingView)
        {
            stockTakingView.StockTakingViewBox.IsEnabled = false;
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += "\\藥健康\\報表\\盤點單\\" + DateTime.Today.ToShortDateString().Replace("/","") +".pdf";
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("產生盤點單...");
                CreatePdf(rptViewer, path,21,29.7);
                Dispatcher.Invoke((Action)(() =>
                {
                    
                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    stockTakingView.StockTakingViewBox.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        private static void CreatePdf(ReportViewer viewer,string fileName,double width,double height)
        {
            var deviceInfo = "<DeviceInfo>" +
                             "  <OutputFormat>PDF</OutputFormat>" +
                             "  <PageWidth>" + width + "cm</PageWidth>" +
                             "  <PageHeight>" + height + "cm</PageHeight>" +
                             "  <MarginTop>0cm</MarginTop>" +
                             "  <MarginLeft>0cm</MarginLeft>" +
                             "  <MarginRight>0cm</MarginRight>" +
                             "  <MarginBottom>0cm</MarginBottom>" +
                             "</DeviceInfo>";
            var bytes = viewer.LocalReport.Render("PDF", deviceInfo);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += "\\藥健康\\報表\\盤點單";
            Directory.CreateDirectory(path);
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding,
            string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        private void Export(LocalReport report, double width, double height)
        {
            try
            {
                string deviceInfo =
                @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>" +
                 "  <PageWidth>" + width + "cm</PageWidth>" +
                 "  <PageHeight>" + height + "cm</PageHeight>" +
                 "  <MarginTop>0cm</MarginTop>" +
                 "  <MarginLeft>0cm</MarginLeft>" +
                 "  <MarginRight>0cm</MarginRight>" +
                 "  <MarginBottom>0cm</MarginBottom>" +
                "</DeviceInfo>";
                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream, out warnings);
                foreach (Stream stream in m_streams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("Export()", MessageType.ERROR);
            }
        }

        private void ReportPrint(string printer)
        {
            MessageWindow m;
            if (m_streams == null || m_streams.Count == 0)
            {
                MessageWindow.ShowMessage("ReportPrint()_1", MessageType.ERROR);
                
                return;
            }
            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = printer;
                printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("ReportPrint()_2", MessageType.ERROR);
            }
        }

        private void printDoc_PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                Metafile pageImage = new
                Metafile(m_streams[m_currentPageIndex]);

                // Adjust rectangular area with printer margins.
                Rectangle adjustedRect = new Rectangle(
                    ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                    ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                    ev.PageBounds.Width,
                    ev.PageBounds.Height);

                // Draw a white background for the report
                ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

                // Draw the report content
                ev.Graphics.DrawImage(pageImage, adjustedRect);

                // Prepare for the next page. Make sure we haven't hit the end.
                m_currentPageIndex++;
                ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("printDoc_PrintPage()", MessageType.ERROR);
            }
        }
    }
}