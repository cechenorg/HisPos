using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.Manufactory;
using His_Pos.Class.StoreOrder;
using His_Pos.InventoryManagement;
using His_Pos.ProductPurchase;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.AbstractClass;
using System.Linq;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Interface;
using His_Pos.StockTaking;
using His_Pos.StockTakingRecord;
using His_Pos.Class.StockTakingOrder;
using His_Pos.ProductTypeManage;
using His_Pos.H4_BASIC_MANAGE.EmployeeManage;
using His_Pos.Class.Employee;
using His_Pos.Class.MedBag;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.TreatmentCase;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.H1_DECLARE.MedBagManage;
using His_Pos.RDLC;
using His_Pos.Struct.Product;
using His_Pos.PrescriptionInquire;
using System.Xml;
using His_Pos.Class.Declare;
using System.Threading;
using His_Pos.H4_BASIC_MANAGE.CustomerManage;
using His_Pos.H6_DECLAREFILE;
using His_Pos.H6_DECLAREFILE.Export;

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
        public void ImportXmlFile(PrescriptionInquireView prescriptionInquireView,string filename)
        {
            backgroundWorker.WorkerReportsProgress = true;

            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("檢查申報檔是否存在...");
                DeclareDb declareDb = new DeclareDb();
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.Load(filename);
                int maxDecMasId = declareDb.GetMaxDecMasId();
                string decId = declareDb.CheckXmlFileExist(doc);
                if (decId != string.Empty)
                {
                    ChangeLoadingMessage("申報檔匯入...");
                    ObservableCollection<DeclareData> declareDataCollection = new ObservableCollection<DeclareData>();
                    XmlNodeList tweets = doc.GetElementsByTagName("ddata");
                    double totalDecCount = tweets.Count;
                    double currentDecCount = 1;
                    foreach (XmlNode node in tweets)
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml("<ddata>" + node.SelectSingleNode("dhead").InnerXml + node.SelectSingleNode("dbody").InnerXml + "</ddata>");
                        DeclareData declareData = new DeclareData(xDoc.GetElementsByTagName("ddata")[0]);
                        declareData.DecMasId = maxDecMasId.ToString();
                        maxDecMasId++;
                        declareData.Prescription.Pharmacy.Id = doc.SelectSingleNode("pharmacy/tdata/t2").InnerText;
                        declareDataCollection.Add(declareData);
                        backgroundWorker.ReportProgress((int)((currentDecCount / totalDecCount) * 100));
                        currentDecCount++;
                    }
                    declareDb.ImportDeclareData(declareDataCollection, decId);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageWindow mainWindow = new MessageWindow("申報檔匯入成功!", MessageType.SUCCESS);
                        mainWindow.Show();
                    }));
                }
                else {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageWindow mainWindow = new MessageWindow("申報檔已存在!", MessageType.ERROR);
                        mainWindow.Show();
                    }));
                }
               
               
            };
            backgroundWorker.ProgressChanged += (s, e) =>
            {
                ChangeLoadingMessage("申報檔匯入... " + (e.ProgressPercentage.ToString() == "100" ? "99" : e.ProgressPercentage.ToString()) + "%");
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
        public void GetNecessaryData(User userLogin)
        {
            MainWindow mainWindow = new MainWindow(userLogin);
            
            backgroundWorker.DoWork += (s, o) =>
            {
                //ChangeLoadingMessage("取得藥品資料...");
                //MainWindow.MedicineDataTable = MedicineDb.GetMedicineData();
                //MainWindow.View = new DataView(MainWindow.MedicineDataTable) { Sort = "PRO_ID" };

                //ChangeLoadingMessage("取得商品資料...");
                //MainWindow.OtcDataTable = OTCDb.GetOtcData();

                if (FunctionDb.CheckYearlyHoliday())
                {
                    ChangeLoadingMessage("更新假日資料...");
                    Function function = new Function();
                    function.GetLastYearlyHoliday();
                }
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

        public void AddNewOrders(ProductPurchase.ProductPurchaseView productPurchaseView, StoreOrderProductType type,WareHouse wareHouse, Manufactory manufactory = null)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("新增新處理單...");
                ManufactoryDb.AddNewOrderBasicSafe(type, wareHouse, manufactory);

                Dispatcher.Invoke((Action)(() =>
                {
                    //productPurchaseView.UpdateUi();
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
            productPurchaseView.OrderContentControl.IsEnabled = false;

            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得廠商資料...");
                ObservableCollection<Manufactory> tempManufactories = ManufactoryDb.GetManufactoryData();

                ChangeLoadingMessage("取得商品資料...");
                Collection<PurchaseProduct> tempProduct = ProductDb.GetItemDialogProduct();

                ChangeLoadingMessage("取得進退貨資料...");

                Dispatcher.Invoke((Action)(() =>
                {
                    productPurchaseView.ManufactoryAutoCompleteCollection = tempManufactories;

                    productPurchaseView.SetControlProduct(tempProduct);


                    //待修改
                    ObservableCollection<StoreOrder> tempStoreOrderCollection = StoreOrderDb.GetStoreOrderOverview(OrderType.ALL);

                    foreach(StoreOrder stoOrd in tempStoreOrderCollection)
                    {
                        if(stoOrd.Type == OrderType.WAITING)
                        {
                            //Check Order Status
                        }
                    }

                    productPurchaseView.StoreOrderCollection = tempStoreOrderCollection;
                }));
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    productPurchaseView.OrderContentControl.IsEnabled = true;
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

                ObservableCollection<Product> newProducts = ProductDb.GetProductTypeManageProducts();

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
                string totalWorth = ProductDb.GetTotalWorth();
                double stockValue = 0;
                inventoryManagementView.InventoryMedicines = MedicineDb.GetInventoryMedicines();
                inventoryManagementView.InventoryOtcs = OTCDb.GetInventoryOtcs();
                inventoryManagementView.ProductCollection = ProductDb.GetItemDialogProduct();
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
                    inventoryManagementView.selectStockValue = stockValue;
                    inventoryManagementView.TotalStockValue.Content = totalWorth;
                    inventoryManagementView.ProductList.Items.Filter = inventoryManagementView.OrderTypeFilter;
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

        public void GetLocation(LocationManage.LocationManageView locationManageView)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得櫃位資料...");

                var location = LocationDb.GetLocationData();

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
        
        public void GetCustomerData(CustomerManageView customerManage)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得客戶資料...");

                var collection = CustomerDb.GetCustomerData();

                Dispatcher.Invoke((Action)(() =>
                {
                    customerManage.CustomerCollection = collection;
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    customerManage.DataGridCustomer.SelectedIndex = 0;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void GetEmployeeData(EmployeeManageView employeeManage)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得員工資料...");

                var collection = EmployeeDb.GetEmployeeData();

                Dispatcher.Invoke((Action)(() =>
                {
                    employeeManage.EmployeeCollection = collection;
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    employeeManage.DataGridEmployee.SelectedIndex = 0;
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

                var Products = StockTakingOrderDb.GetStockTakingRecord();

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
                    if (stockTakingRecord.StocktakingCollection.Count > 0) stockTakingRecord.StockTakingRecord.SelectedIndex = 0;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void GetMedicinesData(PrescriptionInquireView prescriptionInquireView)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("載入基本資料中...");
                prescriptionInquireView.HospitalCollection = HospitalDb.GetData();
                prescriptionInquireView.DivisionCollection = DivisionDb.GetData();
                prescriptionInquireView.CopaymentCollection = CopaymentDb.GetData();
                prescriptionInquireView.PaymentCategoryCollection = PaymentCategroyDb.GetData();
                prescriptionInquireView.AdjustCaseCollection = AdjustCaseDb.GetData();
                prescriptionInquireView.TreatmentCaseCollection = TreatmentCaseDb.GetData();
                prescriptionInquireView.DeclareMedicinesData = MedicineDb.GetDeclareMedicine();
                Dispatcher.Invoke((Action)(() =>
                {

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
        public void GetMedBagData(MedBagManageView medBagManageView)
        {
            medBagManageView.MedBagManageViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得藥袋資料...");
                medBagManageView.
                    Dispatcher.Invoke((Action)(() =>
                    {
                        medBagManageView.MedBagCollection = MedBagDb.ObservableGetMedBagData();
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
        public void GetDeclareFileData(ExportView exportView)
        {
            exportView.ExportViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("申報資料處理中...");
                exportView.
                    Dispatcher.Invoke((Action)(() =>
                    {
                        exportView.DeclareFiles = DeclareFileDb.GetDeclareFilesData();
                        exportView.DeclareFileList.ItemsSource = exportView.DeclareFiles;
                        exportView.Divisions = DivisionDb.GetData();
                        exportView.Hospitals = HospitalDb.GetData();
                        exportView.AdjustCases = AdjustCaseDb.GetData();
                        exportView.AdjustCaseCombo.ItemsSource = exportView.AdjustCases;
                    }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    exportView.ExportViewBox.IsEnabled = true;
                    if (exportView.DeclareFileList.Items.Count > 0)
                        exportView.DeclareFileList.SelectedIndex = 0;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void GetMedicinesData(PrescriptionDec2View prescriptionDec2View)
        {
            prescriptionDec2View.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得處方資料...");
                prescriptionDec2View.HosiHospitals = HospitalDb.GetData();
                prescriptionDec2View.Divisions = DivisionDb.GetData();
                prescriptionDec2View.DeclareMedicines = MedicineDb.GetDeclareMedicine();
                prescriptionDec2View.TreatmentCases = TreatmentCaseDb.GetData();
                prescriptionDec2View.PaymentCategories = PaymentCategroyDb.GetData();
                prescriptionDec2View.Copayments = CopaymentDb.GetData();
                prescriptionDec2View.AdjustCases = AdjustCaseDb.GetData();
                prescriptionDec2View.Usages = UsageDb.GetUsages();
                Dispatcher.Invoke((Action)(() =>
                {
                    prescriptionDec2View.ReleaseHospital.ItemsSource = prescriptionDec2View.HosiHospitals;
                    prescriptionDec2View.DivisionCombo.ItemsSource = prescriptionDec2View.Divisions;
                    prescriptionDec2View.TreatmentCaseCombo.ItemsSource = prescriptionDec2View.TreatmentCases;
                    prescriptionDec2View.PaymentCategoryCombo.ItemsSource = prescriptionDec2View.PaymentCategories;
                    prescriptionDec2View.CopaymentCombo.ItemsSource = prescriptionDec2View.Copayments;
                    prescriptionDec2View.AdjustCaseCombo.ItemsSource = prescriptionDec2View.AdjustCases;
                    prescriptionDec2View.PrescriptionMedicines.ItemsSource = prescriptionDec2View.CurrentPrescription.Medicines;
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionDec2View.PrescriptionViewBox.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void MergeProductStockTaking(StockTakingView stockTakingView)
        {
            stockTakingView.AddItems.IsEnabled = false;
            stockTakingView.AddOneItem.IsEnabled = false;
            stockTakingView.ClearProduct.IsEnabled = false;
            stockTakingView.FinishedAddProduct.IsEnabled = false;
            stockTakingView.Print.IsEnabled = false;

            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("處理商品資料...");

                var Products = ProductDb.GetStockTakingProduct();

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
                    var m = new MessageWindow("藥袋儲存成功", MessageType.SUCCESS);
                    m.Show();
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
    }
}