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
using His_Pos.AbstractClass;
using System.Linq;
using System.Management.Instrumentation;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Interface;
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
using His_Pos.Struct.Product;
using His_Pos.PrescriptionInquire;
using System.Xml;
using His_Pos.Class.Declare;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.Class.SpecialCode;
using His_Pos.H4_BASIC_MANAGE.CustomerManage;
using His_Pos.H6_DECLAREFILE.Export;
using His_Pos.HisApi;
using His_Pos.Struct.IcData;
using Microsoft.Reporting.WinForms;
using StockTakingView = His_Pos.H3_STOCKTAKING.StockTaking.StockTakingView;

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
                XmlDocument doc = new XmlDocument
                {
                    PreserveWhitespace = true
                };
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
                        DeclareData declareData = new DeclareData(xDoc.GetElementsByTagName("ddata")[0])
                        {
                            DecMasId = maxDecMasId.ToString()
                        };
                        maxDecMasId++;
                        declareData.Prescription.Pharmacy.Id = doc.SelectSingleNode("pharmacy/tdata/t2").InnerText;
                        declareDataCollection.Add(declareData);
                        backgroundWorker.ReportProgress((int)((currentDecCount / totalDecCount) * 100));
                        currentDecCount++;
                    }
                    ChangeLoadingMessage("匯入資料庫...");
                    declareDb.ImportDeclareData(declareDataCollection, decId);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageWindow mainWindow = new MessageWindow("申報檔匯入成功!", MessageType.SUCCESS, true);
                        mainWindow.ShowDialog();
                    }));
                }
                else {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageWindow mainWindow = new MessageWindow("申報檔已存在!", MessageType.ERROR, true);
                        mainWindow.ShowDialog();
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
                ObservableCollection<Hospital> tmpHospitals = HospitalDb.GetData();
                ObservableCollection<Division> tmpDivisions = DivisionDb.GetData();
                ObservableCollection<AdjustCase> tmpAdjustCases = AdjustCaseDb.GetData();
                ObservableCollection<PaymentCategory> tmpPaymentCategroies = PaymentCategroyDb.GetData();
                ObservableCollection<Copayment> tmpCopayments = CopaymentDb.GetData();
                ObservableCollection<TreatmentCase> tmpTreatmentCaseDb = TreatmentCaseDb.GetData();
                ObservableCollection<SpecialCode> tmpSpecialCodes = SpecialCodeDb.GetData();
                ObservableCollection<Usage> tmpUsages = UsageDb.GetData();
                Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.Hospitals = tmpHospitals;
                    MainWindow.Divisions = tmpDivisions;
                    MainWindow.AdjustCases = tmpAdjustCases;
                    MainWindow.PaymentCategory = tmpPaymentCategroies;
                    MainWindow.Copayments = tmpCopayments;
                    MainWindow.TreatmentCase = tmpTreatmentCaseDb;
                    MainWindow.SpecialCode = tmpSpecialCodes;
                    MainWindow.Usages = tmpUsages;
                }));
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
                
                ChangeLoadingMessage("取得進退貨資料...");
                Dispatcher.Invoke((Action)(() =>
                {
                    ObservableCollection<StoreOrder> tempStoreOrderCollection = StoreOrderDb.GetStoreOrderOverview(OrderType.ALL);

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
                StoreOrderDb.GetNewStoreOrderBySingde();

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
                            if (stoOrd.DeclareDataCount > 0)
                            {
                                stoOrd.Type = StoreOrderDb.GetDeclareOrderStatusFromSinde(stoOrd.Id);
                            }
                            else
                            {
                                stoOrd.Type = StoreOrderDb.GetOrderStatusFromSinde(stoOrd.Id);
                            }

                            if (stoOrd.Type == OrderType.PROCESSING)
                                productPurchaseView.CheckSindeOrderDetail(stoOrd);
                        }
                    }

                    productPurchaseView.StoreOrderCollection = tempStoreOrderCollection;
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
                    inventoryManagementView.TotalStockValue.Content = totalWorth; 
                    inventoryManagementView.selectStockValue = 0;
                    inventoryManagementView.searchCount = 0;
                    inventoryManagementView.SearchData();
                    inventoryManagementView.SearchCount.Content = inventoryManagementView.searchCount;
                    inventoryManagementView.SelectStockValue.Content = inventoryManagementView.selectStockValue.ToString("0.#");
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
                prescriptionInquireView.DeclareMedicinesData = MedicineDb.GetDeclareMedicine();
                Dispatcher.Invoke((Action)(() =>
                {
                    prescriptionInquireView.HospitalCollection = MainWindow.Hospitals;
                    prescriptionInquireView.DivisionCollection = MainWindow.Divisions;
                    prescriptionInquireView.CopaymentCollection = MainWindow.Copayments;
                    prescriptionInquireView.PaymentCategoryCollection = MainWindow.PaymentCategory;
                    prescriptionInquireView.AdjustCaseCollection = MainWindow.AdjustCases;
                    prescriptionInquireView.TreatmentCaseCollection = MainWindow.TreatmentCase;
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(Close));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void GetMedBagData(MedBagManageView medBagManageView)
        {
            medBagManageView.MedBagManageViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得藥袋資料...");
                var tmpMedBags = MedBagDb.ObservableGetMedBagData();
                Dispatcher.Invoke((Action)(() =>
                {
                    medBagManageView.MedBagCollection = tmpMedBags;
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
                exportView.DeclareFiles = DeclareFileDb.GetDeclareFilesData();
                var tmpDeclareMedicine = MedicineDb.GetDeclareFileMedicineData();
                Dispatcher.Invoke((Action)(() =>
                {
                    exportView.DeclareMedicinesData = tmpDeclareMedicine;
                    exportView.HospitalCollection = MainWindow.Hospitals;
                    exportView.DivisionCollection = MainWindow.Divisions;
                    exportView.AdjustCaseCollection = MainWindow.AdjustCases;
                    exportView.PaymentCategoryCollection = MainWindow.PaymentCategory;
                    exportView.TreatmentCaseCollection = MainWindow.TreatmentCase;
                    exportView.CopaymentCollection = MainWindow.Copayments;
                    exportView.AdjustCaseCombo.ItemsSource = MainWindow.AdjustCases;
                    exportView.HisPerson.ItemsSource = MainWindow.CurrentPharmacy.MedicalPersonnelCollection;
                    exportView.ReleasePalace.ItemsSource = MainWindow.Hospitals;
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
                prescriptionDec2View.DeclareMedicines = MedicineDb.GetDeclareMedicine();
                prescriptionDec2View.Hospitals = MainWindow.Hospitals;
                prescriptionDec2View.Divisions = MainWindow.Divisions;
                prescriptionDec2View.TreatmentCases = MainWindow.TreatmentCase;
                prescriptionDec2View.PaymentCategories = MainWindow.PaymentCategory;
                prescriptionDec2View.Copayments = MainWindow.Copayments;
                prescriptionDec2View.AdjustCases = MainWindow.AdjustCases;
                prescriptionDec2View.SpecialCodes = MainWindow.SpecialCode;
                prescriptionDec2View.Usages = MainWindow.Usages;
                prescriptionDec2View.MedicalPersonnels = MainWindow.CurrentPharmacy.MedicalPersonnelCollection;
                Dispatcher.Invoke((Action)(() =>
                {
                    prescriptionDec2View.ReleaseHospital.ItemsSource = prescriptionDec2View.Hospitals;
                    prescriptionDec2View.PrescriptionMedicines.ItemsSource = prescriptionDec2View.CurrentPrescription.Medicines;
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionDec2View.PrescriptionViewBox.IsEnabled = true;
                    prescriptionDec2View.DataContext = prescriptionDec2View;
                    var isMedicalPerson = false;
                    foreach (var m in prescriptionDec2View.MedicalPersonnels)
                    {
                        if (!m.Id.Equals(MainWindow.CurrentUser.Id)) continue;
                        isMedicalPerson = true;
                        break;
                    }
                    if (isMedicalPerson)
                    {
                        prescriptionDec2View.HisPerson.SelectedItem =
                            prescriptionDec2View.MedicalPersonnels.SingleOrDefault(p =>
                                p.Id.Equals(MainWindow.CurrentUser.Id));
                    }
                    else
                    {
                        prescriptionDec2View.HisPerson.SelectedIndex = 0;
                    }
                    prescriptionDec2View.CopaymentCombo.SelectedItem =
                        prescriptionDec2View.Copayments.SingleOrDefault(c => c.Id.Equals("I20"));
                    prescriptionDec2View.PaymentCategoryCombo.SelectedItem =
                        prescriptionDec2View.PaymentCategories.SingleOrDefault(p => p.Id.Equals("4"));
                    prescriptionDec2View.AdjustCaseCombo.SelectedItem =
                        prescriptionDec2View.AdjustCases.SingleOrDefault(a => a.Id.Equals("1"));
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
                    var m = new MessageWindow("藥袋儲存成功", MessageType.SUCCESS, true);
                    m.ShowDialog();
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void LoadIcData(PrescriptionDec2View prescriptionDec2View)
        {
            prescriptionDec2View.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("卡片資料讀取中...");
                LoadPatentDataFromIcCard(prescriptionDec2View);
                Dispatcher.Invoke((Action)(() =>
                {
                    if (prescriptionDec2View.CurrentPrescription.IsGetIcCard)
                    {
                        var c = new CustomerSelectWindow(prescriptionDec2View.CurrentPrescription.Customer.IcCard.IcNumber,3);
                        c.ShowDialog();
                        if (string.IsNullOrEmpty(prescriptionDec2View.CurrentPrescription.Customer.IcCard.MedicalNumber) &&
                            !string.IsNullOrEmpty(prescriptionDec2View.MedicalNumber.Text))
                            prescriptionDec2View.CurrentPrescription.Customer.IcCard.MedicalNumber = prescriptionDec2View.MedicalNumber.Text;
                        if (!string.IsNullOrEmpty(prescriptionDec2View.CurrentPrescription.Customer.IcCard.MedicalNumber) &&
                            string.IsNullOrEmpty(prescriptionDec2View.MedicalNumber.Text))
                            prescriptionDec2View.MedicalNumber.Text = prescriptionDec2View.CurrentPrescription.Customer.IcCard.MedicalNumber;
                    }
                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionDec2View.NotifyPropertyChanged(nameof(prescriptionDec2View.CurrentPrescription));
                    prescriptionDec2View.PrescriptionViewBox.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        private void LoadPatentDataFromIcCard(PrescriptionDec2View prescriptionDec2View)
        {
            var strLength = 72;
            var icData = new byte[72];
            var res = HisApiBase.hisGetBasicData(icData, ref strLength);
            icData.CopyTo(prescriptionDec2View.BasicDataArr, 0);
            if (res == 0)
            {
                prescriptionDec2View.SetCardStatusContent("健保卡讀取成功");
                prescriptionDec2View.CurrentPrescription.IsGetIcCard = true;
                prescriptionDec2View.CusBasicData = new BasicData(icData);
                prescriptionDec2View.CurrentPrescription.Customer = new Customer(prescriptionDec2View.CusBasicData);
                strLength = 296;
                icData = new byte[296];
                var cs = new ConvertData();
                var cTreatItem = cs.StringToBytes("AF\0", 3);
                //新生兒就醫註記,長度兩個char
                var cBabyTreat = prescriptionDec2View.TreatRecCollection.Count > 0 ? cs.StringToBytes(prescriptionDec2View.TreatRecCollection[0].NewbornTreatmentMark + "\0", 3) : cs.StringToBytes(" ", 2);
                //補卡註記,長度一個char
                var cTreatAfterCheck = new byte[] { 1 };
                res = HisApiBase.hisGetSeqNumber256(cTreatItem, cBabyTreat, cTreatAfterCheck, icData, ref strLength);
                //取得就醫序號
                if (res == 0)
                {
                    prescriptionDec2View.IsMedicalNumberGet = true;
                    prescriptionDec2View.Seq = new SeqNumber(icData);
                    prescriptionDec2View.CurrentPrescription.Customer.IcCard.MedicalNumber = prescriptionDec2View.Seq.MedicalNumber;
                    prescriptionDec2View.NotifyPropertyChanged(nameof(prescriptionDec2View.CurrentPrescription.Customer.IcCard));
                }
                //未取得就醫序號
                else
                {
                    prescriptionDec2View.GetMedicalNumberErrorCode = res;
                }
                //取得就醫紀錄
                strLength = 498;
                icData = new byte[498];
                res = HisApiBase.hisGetTreatmentNoNeedHPC(icData, ref strLength);
                if (res == 0)
                {
                    var startIndex = 84;
                    for (var i = 0; i < 6; i++)
                    {
                        if (icData[startIndex + 3] == 32)
                            break;
                        prescriptionDec2View.TreatRecCollection.Add(new TreatmentDataNoNeedHpc(icData, startIndex));
                        startIndex += 69;
                    }
                }
            }
            else
            {
                prescriptionDec2View.CurrentPrescription.IsGetIcCard = false;
                switch (res)
                {
                    case 4000:
                        prescriptionDec2View.SetCardStatusContent("讀卡機逾時");
                        break;
                    case 4013:
                        prescriptionDec2View.SetCardStatusContent("未置入健保IC卡");
                        break;
                    case 4029:
                        prescriptionDec2View.SetCardStatusContent("IC卡權限不足");
                        break;
                    case 4033:
                        prescriptionDec2View.SetCardStatusContent("所置入非健保IC卡");
                        break;
                    case 4050:
                        prescriptionDec2View.SetCardStatusContent("安全模組尚未與IDC認證");
                        break;
                }
            }
        }

        public void LoginIcData(PrescriptionDec2View prescriptionDec2View)
        {
            prescriptionDec2View.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("卡片資料寫入中...");
                prescriptionDec2View.LogInIcData();
                Dispatcher.Invoke((Action)(() =>
                {
                    if (prescriptionDec2View.IsMedicalNumberGet)
                        prescriptionDec2View.CreatIcUploadData();
                    else if(!prescriptionDec2View.IsMedicalNumberGet)
                    {
                        prescriptionDec2View.CreatIcErrorUploadData();
                    }
                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionDec2View.PrescriptionViewBox.IsEnabled = true;
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

        public void PrintMedbag(ReportViewer rptViewer, PrescriptionDec2View prescriptionDec2View)
        {
            prescriptionDec2View.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("藥袋列印中...");
                CreatePdf(rptViewer,"Medbag.pdf",22,24);
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionDec2View.PrescriptionViewBox.IsEnabled = true;
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
    }
}