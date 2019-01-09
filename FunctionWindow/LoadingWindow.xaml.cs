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
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.Employee;
using His_Pos.Class.Location;
using His_Pos.Class.Manufactory;
using His_Pos.Class.MedBag;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using His_Pos.Class.Position;
using His_Pos.Class.Product;
using His_Pos.Class.SpecialCode;
using His_Pos.Class.StockTakingOrder;
using His_Pos.Class.StoreOrder;
using His_Pos.Class.TreatmentCase;
using His_Pos.HisApi;
using His_Pos.Interface;
using His_Pos.Service;
using His_Pos.Struct.Product;
using His_Pos.SYSTEM_TAB.H1_DECLARE.Export;
using His_Pos.SYSTEM_TAB.H1_DECLARE.MedBagManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionInquire;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingRecord;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage;
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
                int maxDecMasId = 0;///declareDb.GetMaxDecMasId();
                string decId = "";///declareDb.CheckXmlFileExist(doc);
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
                    /// declareDb.ImportDeclareData(declareDataCollection, decId);
                    ChangeLoadingMessage("預約慢箋中..."); 
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

                ///if (FunctionDb.CheckYearlyHoliday())
                ///{
                ///    ChangeLoadingMessage("更新假日資料...");
                ///    Function function = new Function();
                ///    function.GetLastYearlyHoliday();
                ///}
                ChangeLoadingMessage("取得醫療院所...");
                ///var tmpHospitals = HospitalDb.GetData();
                ChangeLoadingMessage("取得科別資料...");
                ///var tmpDivisions = DivisionDb.GetData();
                ChangeLoadingMessage("取得調劑案件資料...");
                ///var tmpAdjustCases = AdjustCaseDb.GetData();
                ChangeLoadingMessage("取得給付類別資料...");
                ///var tmpPaymentCategroies = PaymentCategroyDb.GetData();
                ChangeLoadingMessage("取得部分負擔資料...");
                ///var tmpCopayments = CopaymentDb.GetData();
                ChangeLoadingMessage("取得處方案件資料...");
                ///var tmpTreatmentCaseDb = TreatmentCaseDb.GetData();
                ChangeLoadingMessage("取得特定治療代碼資料...");
                ///var tmpSpecialCodes = SpecialCodeDb.GetData();
                ChangeLoadingMessage("取得藥品用法資料...");
                ///var tmpUsages = UsageDb.GetData();
                ChangeLoadingMessage("取得途徑/作用部位資料...");
                ///var tmpPositions = PositionDb.GetData();
                Dispatcher.Invoke((Action)(() =>
                {
                    ///MainWindow.Hospitals = tmpHospitals;
                    ///MainWindow.Divisions = tmpDivisions;
                    ///MainWindow.AdjustCases = tmpAdjustCases;
                    ///MainWindow.PaymentCategory = tmpPaymentCategroies;
                    ///MainWindow.Copayments = tmpCopayments;
                    ///MainWindow.TreatmentCase = tmpTreatmentCaseDb;
                    ///MainWindow.SpecialCode = tmpSpecialCodes;
                    ///MainWindow.Usages = tmpUsages;
                    ///MainWindow.Positions = tmpPositions;
                }));
            };
            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    mainWindow.Show();
                    MainWindow.ItemSourcesSet = true;
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
        
        public void GetCustomerData(CustomerManageView customerManage)
        {
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("取得客戶資料...");

                var collection = "";/// CustomerDb.GetCustomerData();

                Dispatcher.Invoke((Action)(() =>
                {
                    customerManage.CustomerCollection = null;///collection;
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
                var employeeCollection = new Collection<Object>();/// EmployeeDb.GetEmployeeData();

                ChangeLoadingMessage("取得權限資料...");
                var positionCollection = new Collection<Object>();/// EmployeeDb.GetPositionData();

                Dispatcher.Invoke((Action)(() =>
                {
                    ///employeeManage.EmployeeCollection = employeeCollection;
                   /// employeeManage.PositionCollection = positionCollection;
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
        public void GetMedicinesData(PrescriptionInquireView prescriptionInquireView)
        {
            prescriptionInquireView.InquireViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("載入基本資料中...");
                prescriptionInquireView.DeclareMedicinesData = new ObservableCollection<Product>();///MedicineDb.GetDeclareMedicine();
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
                prescriptionInquireView.InquireViewBox.IsEnabled = true;
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
        public void GetDeclareFileData(ExportView exportView)
        {
            exportView.ExportViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("申報資料處理中...");
                exportView.DeclareFiles = null;/// DeclareFileDb.GetDeclareFilesData();
                var tmpDeclareMedicine = "";// MedicineDb.GetDeclareFileMedicineData();
                exportView.HospitalCollection = MainWindow.Hospitals;
                exportView.DivisionCollection = MainWindow.Divisions;
                exportView.AdjustCaseCollection = MainWindow.AdjustCases;
                exportView.PaymentCategoryCollection = MainWindow.PaymentCategory;
                exportView.TreatmentCaseCollection = MainWindow.TreatmentCase;
                exportView.CopaymentCollection = MainWindow.Copayments;
                Dispatcher.Invoke((Action)(() =>
                {
                    exportView.DeclareMedicinesData = null;//tmpDeclareMedicine;
                    exportView.HisPerson.ItemsSource = MainWindow.CurrentPharmacy.MedicalPersonnelCollection;
                    exportView.ReleasePalace.ItemsSource = MainWindow.Hospitals;
                    exportView.AdjustCaseCombo.ItemsSource = MainWindow.AdjustCases;
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
                prescriptionDec2View.DeclareMedicines = null;///MedicineDb.GetDeclareMedicine();
                prescriptionDec2View.Hospitals = MainWindow.Hospitals;
                prescriptionDec2View.Divisions = MainWindow.Divisions;
                prescriptionDec2View.TreatmentCases = MainWindow.TreatmentCase;
                prescriptionDec2View.PaymentCategories = MainWindow.PaymentCategory;
                prescriptionDec2View.Copayments = MainWindow.Copayments;
                prescriptionDec2View.AdjustCases = MainWindow.AdjustCases;
                prescriptionDec2View.SpecialCodes = MainWindow.SpecialCode;
                prescriptionDec2View.Usages = MainWindow.Usages;
                prescriptionDec2View.MedicalPersonnels = MainWindow.CurrentPharmacy.MedicalPersonnelCollection;
                prescriptionDec2View.Positions = null;/// PositionDb.GetData();
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
                        prescriptionDec2View.CurrentPrescription.Pharmacy.MedicalPersonnel =
                            prescriptionDec2View.MedicalPersonnels.SingleOrDefault(p =>
                                p.Id.Equals(MainWindow.CurrentUser.Id));
                    }
                    else
                    {
                        prescriptionDec2View.HisPerson.SelectedIndex = 0;
                    }
                    prescriptionDec2View.CurrentPrescription.Treatment.Copayment = prescriptionDec2View.Copayments.SingleOrDefault(c => c.Name.Equals("加收部分負擔"));
                    prescriptionDec2View.CurrentPrescription.Treatment.PaymentCategory = prescriptionDec2View.PaymentCategories.SingleOrDefault(p => p.Name.Equals("普通疾病"));
                    prescriptionDec2View.CurrentPrescription.Treatment.AdjustCase = prescriptionDec2View.AdjustCases.SingleOrDefault(a => a.Name.Equals("一般處方調劑"));
                    prescriptionDec2View.CurrentPrescription.Treatment.MedicalInfo.TreatmentCase = prescriptionDec2View.TreatmentCases.SingleOrDefault(c => c.Name.Equals("一般案件"));
                    
                    if (MainWindow.CurrentUser.Authority.AuthorityValue.Equals("3"))
                    {
                        for (int i = 0; i < prescriptionDec2View.MedicalPersonnels.Count; i++)
                        {
                            if (prescriptionDec2View.MedicalPersonnels[i].Name.Equals(MainWindow.CurrentUser.Name))
                            {
                                prescriptionDec2View.HisPerson.SelectedIndex = i;
                            }
                        }
                    }
                        
                    prescriptionDec2View.DataContext = prescriptionDec2View;
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
                prescriptionDec2View.LoadPatentDataFromIcCard();
                Dispatcher.Invoke((Action)(() =>
                {
                    
                }));
            };
            backgroundWorker.ProgressChanged += (s, o) =>
            {
                ChangeLoadingMessage("卡片資料讀取中...");
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

        public void LoginIcData(PrescriptionDec2View prescriptionDec2View, IcErrorCodeWindow.IcErrorCode errorCode = null)
        {
            prescriptionDec2View.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("卡片資料寫入中...");
                prescriptionDec2View.LogInIcData();
                if (prescriptionDec2View.IsMedicalNumberGet)
                    prescriptionDec2View.CreatIcUploadData();
                else
                {
                    if(!prescriptionDec2View.CurrentPrescription.IsDeposit)
                        prescriptionDec2View.CreatIcErrorUploadData(errorCode);
                }
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

        public void PrintMedbag(ReportViewer rptViewer, PrescriptionDec2View prescriptionDec2View,bool printReceipt)
        {
            prescriptionDec2View.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("藥袋列印中...");
                Export(rptViewer.LocalReport,22,24);
                ReportPrint(Properties.Settings.Default.MedBagPrinter);
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionDec2View.PrescriptionViewBox.IsEnabled = true;
                    if(!printReceipt)
                        prescriptionDec2View.ClearPrescription();
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void PrintReceipt(ReportViewer rptViewer, PrescriptionDec2View prescriptionDec2View)
        {
            prescriptionDec2View.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("收據列印中...");
                Export(rptViewer.LocalReport, 25.4, 9.3);
                ReportPrint(Properties.Settings.Default.ReceiptPrinter);
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionDec2View.PrescriptionViewBox.IsEnabled = true;
                    prescriptionDec2View.ClearPrescription();
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void PrintCooperativeReceipt(ReportViewer rptViewer, CooperativePrescriptSelectWindow cooperativePrescriptSelectWindow) {
            cooperativePrescriptSelectWindow.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("收據列印中...");
                Export(rptViewer.LocalReport, 25.4, 9.3);
                ReportPrint(Properties.Settings.Default.ReceiptPrinter);
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    cooperativePrescriptSelectWindow.IsEnabled = true;
                    
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void PrintMedbagFromInquire(ReportViewer rptViewer, PrescriptionInquireOutcome prescriptionInquire, bool printReceipt) {
            prescriptionInquire.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("藥袋列印中...");
                Export(rptViewer.LocalReport, 22, 24);
                ReportPrint(Properties.Settings.Default.MedBagPrinter);
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionInquire.PrescriptionViewBox.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void PrintMedbagFromCooperativeSelect(ReportViewer rptViewer, CooperativePrescriptSelectWindow cooperativePrescriptSelectWindow, bool printReceipt) {
            cooperativePrescriptSelectWindow.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("藥袋列印中...");
                Export(rptViewer.LocalReport, 22, 24);
                ReportPrint(Properties.Settings.Default.MedBagPrinter);
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    cooperativePrescriptSelectWindow.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        public void PrintReceiptFromInquire(ReportViewer rptViewer, PrescriptionInquireOutcome prescriptionInquire) {
            prescriptionInquire.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("收據列印中...");
                Export(rptViewer.LocalReport, 25.4, 9.3);
                ReportPrint(Properties.Settings.Default.ReceiptPrinter);
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionInquire.PrescriptionViewBox.IsEnabled = true;
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
                var m = new MessageWindow("Export()", MessageType.ERROR, true);
                m.ShowDialog();
                return;
            }
        }

        private void ReportPrint(string printer)
        {
            MessageWindow m;
            if (m_streams == null || m_streams.Count == 0)
            {
                m = new MessageWindow("ReportPrint()_1", MessageType.ERROR,true);
                m.ShowDialog();
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
                m = new MessageWindow("ReportPrint()_2", MessageType.ERROR, true);
                m.ShowDialog();
                return;
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
                var m = new MessageWindow("printDoc_PrintPage()", MessageType.ERROR, true);
                m.ShowDialog();
                return;
            }
        }

        public void LoginIcData(PrescriptionInquireOutcome prescriptionInquireOutcome)
        {
            prescriptionInquireOutcome.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("卡片資料寫入中...");
                prescriptionInquireOutcome.LogInIcData();
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    prescriptionInquireOutcome.PrescriptionViewBox.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void ResetCardReader(PrescriptionDec2View instance)
        {
            instance.PrescriptionViewBox.IsEnabled = false;
            backgroundWorker.DoWork += (s, o) =>
            {
                ChangeLoadingMessage("讀卡機重置中...");
                HisApiBase.ResetCardReader();
                Dispatcher.Invoke((Action)(() =>
                {

                }));
            };
            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    instance.PrescriptionViewBox.IsEnabled = true;
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
    }
}