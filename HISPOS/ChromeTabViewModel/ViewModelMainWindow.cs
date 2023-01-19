using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.HisApi;
using His_Pos.NewClass;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Medicine.Position;
using His_Pos.NewClass.Medicine.Usage;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.CustomerPrescriptions;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.StoreOrder;
using His_Pos.NewClass.WareHouse;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativePrescriptionWindow;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelMainWindow : MainViewModel
    {
        //this property is to show you can lock the tabs with a binding
        private bool canMoveTabs;

        public bool CanMoveTabs
        {
            get => canMoveTabs;
            set
            {
                if (canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref canMoveTabs, value);
                }
            }
        }

        //this property is to show you can bind the visibility of the add button
        private bool showAddButton;

        public bool ShowAddButton
        {
            get => showAddButton;
            set
            {
                if (showAddButton != value)
                {
                    Set(() => ShowAddButton, ref showAddButton, value);
                }
            }
        }

        private string cardReaderStatus;

        public string CardReaderStatus
        {
            get => cardReaderStatus;
            set
            {
                Set(() => CardReaderStatus, ref cardReaderStatus, value);
            }
        }

        private string samDcStatus;

        public string SamDcStatus
        {
            get => samDcStatus;
            set
            {
                Set(() => SamDcStatus, ref samDcStatus, value);
            }
        }

        private string hpcCardStatus;

        public string HpcCardStatus
        {
            get => hpcCardStatus;
            set
            {
                Set(() => HpcCardStatus, ref hpcCardStatus, value);
            }
        }

        public static bool IsConnectionOpened { get; set; }
        public static bool HisApiException { get; set; }
        public static bool IsIcCardValid { get; set; }
        public static bool IsHpcValid { get; set; }
        public static bool IsVerifySamDc { get; set; }

        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }

        private string busyContent;

        public string BusyContent
        {
            get => busyContent;
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }

        public static Institutions Institutions { get; set; }
        public static WareHouses WareHouses { get; set; }
        public static CooperativeClinicSettings CooperativeClinicSettings { get; set; }
        public CusPrePreviewBases cooperativePres { get; set; }
        public static Divisions Divisions { get; set; }
        public static AdjustCases AdjustCases { get; set; }
        public static PaymentCategories PaymentCategories { get; set; }
        public static PrescriptionCases PrescriptionCases { get; set; }
        public static Copayments Copayments { get; set; }
        public static SpecialTreats SpecialTreats { get; set; }
        public static Usages Usages { get; set; }
        public static Positions Positions { get; set; }
        public static int StoreOrderDays { get; set; }
        public static DateTime ClosingDate { get; set; }
        public static Pharmacy CurrentPharmacy { get; set; }
        public static Employee CurrentUser { get; set; }
        public static Employees EmployeeCollection { get; set; }
        public static string CooperativeInstitutionID { get; private set; }
        private int mCurrentPageIndex;
        private IList<Stream> mStreams;
        public string pathFile;

        public static string SingdeWebURI { get; private set; }

        public ViewModelMainWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            MainWindow.ServerConnection.OpenConnection();
            StoreOrderDays = StoreOrderDB.GetStoreOrderDays();
            CurrentPharmacy = Pharmacy.GetCurrentPharmacy();
            ClosingDate = Pharmacy.GetClosingDate();
            CurrentPharmacy.MedicalPersonnels = new Employees();
            CooperativeInstitutionID = WebApi.GetCooperativeClinicId(CurrentPharmacy.ID);
            MainWindow.ServerConnection.CloseConnection();
            CanMoveTabs = true;
            ShowAddButton = false;

            SingdeWebURI = $@"http://kaokaodepon.singde.com.tw:5566/id/{CurrentPharmacy.ID}/pass/{CurrentPharmacy.ID}";

            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "MainWindowClosing")
                    WindowCloseAction();
            });
        }

        private RelayCommand initialData;

        public RelayCommand InitialData
        {
            get =>
                initialData ??
                (initialData = new RelayCommand(ExecuteInitData));
            set => initialData = value;
        }

        private void ExecuteInitData()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "取得庫別名";
                WareHouses = new WareHouses(WareHouseDb.Init());
                BusyContent = StringRes.取得院所;
                Institutions = new Institutions(true);
                BusyContent = StringRes.取得科別;
                Divisions = new Divisions();
                BusyContent = "取得合作院所設定";
                CooperativeClinicSettings = new CooperativeClinicSettings();
                cooperativePres = new CusPrePreviewBases();

                CooperativeClinicSettings.Init();
                var xDocs = new List<XDocument>();
                var cusIdNumbers = new List<string>();
                var paths = new List<string>();
                foreach (var c in CooperativeClinicSettings)
                {
                    if (Properties.Settings.Default.PrePrint == "True")
                    {
                        if (c.AutoPrint == true)
                        {
                            pathFile = c.FilePath;
                            if (c.FilePath == null || Directory.Exists(c.FilePath) == false)
                            {
                                continue;
                            }
                            FileSystemWatcher watch = new FileSystemWatcher(c.FilePath, "*.txt");
                            //開啟監聽
                            watch.EnableRaisingEvents = true;
                            //是否連子資料夾都要偵測
                            watch.IncludeSubdirectories = false;

                            watch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                            //新增時觸發事件
                            watch.Created += new FileSystemEventHandler(watch_Created);
                            //錯誤時觸發事件
                            watch.Error += watch_Error;
                            Thread threadPrint = new Thread(TimePrint);
                            threadPrint.Start();
                        }
                    }

                    if (c.TypeName.Equals("展望") && Directory.Exists(c.FilePath))
                    {
                        FileSystemWatcher watch = new FileSystemWatcher(c.FilePath, "*.xml");
                        //開啟監聽
                        watch.EnableRaisingEvents = true;
                        //是否連子資料夾都要偵測
                        watch.IncludeSubdirectories = false;

                        watch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                        //新增時觸發事件
                        watch.Created += new FileSystemEventHandler(watch_Created);
                    }
                }

                cooperativePres.GetCooperative(DateTime.Today.AddDays(-10), DateTime.Today);//取得10天內的合作處方
                CooperativeClinicSettings.Init();//取得合作診所設定
                CooperativeClinicSettings.FilePurge();//打包檔案
                BusyContent = StringRes.GetAdjustCases;
                AdjustCases = new AdjustCases(true);
                BusyContent = StringRes.取得給付類別;
                PaymentCategories = new PaymentCategories();
                BusyContent = StringRes.取得處方案件;
                PrescriptionCases = new PrescriptionCases();
                BusyContent = StringRes.取得部分負擔;
                Copayments = new Copayments();
                BusyContent = StringRes.取得特定治療;
                SpecialTreats = new SpecialTreats();
                BusyContent = StringRes.取得用法;
                Usages = new Usages();
                BusyContent = StringRes.取得用藥途徑;
                Positions = new Positions();
                BusyContent = "藥袋量計算中";
                ProductDB.UpdateAllInventoryMedBagAmount();
                BusyContent = "同步員工資料";
                EmployeeDb.SyncData();
                BusyContent = "取得員工";
                EmployeeCollection = new Employees();
                EmployeeCollection.Init();
                BusyContent = "準備回傳骨科拋轉資料";
                while (WebApi.SendToCooperClinicLoop100())
                {
                    BusyContent = "回傳合作診所處方中...";
                }

                PrintCooPre();//列印還未列印藥袋
                //骨科上傳
                //OfflineDataSet offlineData = new OfflineDataSet(Institutions, Divisions, CurrentPharmacy.MedicalPersonnels, AdjustCases, PrescriptionCases, Copayments, PaymentCategories, SpecialTreats, Usages, Positions);
                //var bytes = ZeroFormatterSerializer.Serialize(offlineData);
                //File.WriteAllBytes("C:\\Program Files\\HISPOS\\OfflineDataSet.singde", bytes.ToArray());
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                MainWindow.ServerConnection.OpenConnection();
                HisApiFunction.CheckDailyUpload();
                MainWindow.ServerConnection.CloseConnection();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void PrintCooPre()
        {
            CooperativePrescriptionViewModel cooperativePrescriptionViewModel = new CooperativePrescriptionViewModel();
            if (cooperativePrescriptionViewModel.CooPreCollectionView != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (Properties.Settings.Default.PrePrint == "True")
                    {
                        foreach (CusPrePreviewBase prescription in cooperativePres)
                        {
                            foreach (var setting in CooperativeClinicSettings)
                            {
                                if (setting.AutoPrint && prescription.IsPrint == false)
                                {
                                    cooperativePrescriptionViewModel.PrintAction(prescription);
                                }
                            }
                        }
                    }
                }));
            }
        }
        private static void watch_Created(object sender, FileSystemEventArgs e)
        {
            var table = PrescriptionDb.GetXmlOfPrescriptionsByDate(DateTime.Today, DateTime.Today);
           
           

            string DirectPath = e.FullPath;
            //如果偵測到為檔案，則依路徑對此資料夾做檔案處理
            if (DirectPath != null)
            {
                var cooperativeClinicSettings = new CooperativeClinicSettings();
                cooperativeClinicSettings.Init();
               
                #region 先把全部的.txt轉成.xml
                foreach (var setting in cooperativeClinicSettings)
                {
                    if ((string.IsNullOrEmpty(setting.TypeName) ? string.Empty : setting.TypeName) == "杏翔" && setting.FilePath == ((FileSystemWatcher)sender).Path)
                    {
                        if (!string.IsNullOrEmpty(setting.FilePath))
                        {
                            var fileEntries = Directory.GetFiles(setting.FilePath);
                            foreach (var filePath in fileEntries)
                            {
                                if (!string.IsNullOrEmpty(filePath))
                                {
                                    if (Path.GetExtension(filePath).ToLower() == ".txt")
                                    {
                                        string[] file = filePath.Split('_');
                                        if (file != null && file.Length > 2)
                                        {
                                            DataRow[] drs = table.Select(string.Format("Cooli_FilePath= '{0}'", filePath + ".xml"));
                                            if ((drs != null && drs.Length > 0) || Directory.Exists(filePath + ".xml"))
                                            {
                                                continue;
                                            }
                                            if (!NewFunction.IsFileInUse(filePath))
                                            {
                                                try
                                                {
                                                    NewFunction.GetTxtFiles(filePath);
                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                TaiwanCalendar tc = new TaiwanCalendar();
                DateTime now = DateTime.Now;
                string date = string.Format("{0}{1}{2}", tc.GetYear(now), tc.GetMonth(now).ToString().PadLeft(2, '0'), tc.GetDayOfMonth(now).ToString().PadLeft(2, '0'));
                bool isRe = false;
                string isRePost = "0";
                var xDocs = new List<XDocument>();
                var cusIdNumbers = new List<string>();
                var paths = new List<string>();
                foreach (var setting in cooperativeClinicSettings)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(setting.FilePath))
                        {
                            var fileEntries = Directory.GetFiles(setting.FilePath);
                            foreach (var filePath in fileEntries)
                            {
                                string[] file = filePath.Split('_');
                                if (file != null && file.Length > 2 && (string.IsNullOrEmpty(setting.TypeName) ? string.Empty : setting.TypeName) == "杏翔")
                                {
                                    if (date != file[1])//非今日的處方，不新增處方紀錄
                                    {
                                        continue;
                                    }
                                }
                                if (Path.GetExtension(filePath).ToLower() == ".xml")
                                {
                                    var xDocument = XDocument.Load(filePath);
                                    var cusIdNumber = xDocument.Element("case").Element("profile").Element("person").Attribute("id").Value;
                                    if (xDocument.Element("case").Element("continous_prescription").Attribute("other_mo") == null)
                                    {
                                        isRePost = "0";
                                    }
                                    else
                                    {
                                        isRePost = xDocument.Element("case").Element("continous_prescription").Attribute("other_mo").Value.ToString().Trim();
                                        isRePost = string.IsNullOrEmpty(isRePost) ? "0" : isRePost;
                                    }
                                    if (isRePost != "0")
                                    {
                                        isRe = true;
                                    }
                                    else
                                    {
                                        isRe = false;
                                    }
                                    xDocs.Add(xDocument);
                                    cusIdNumbers.Add(cusIdNumber);
                                    paths.Add(filePath);
                                }
                            }
                            if (cusIdNumbers.Count > 0)
                            {
                                XmlOfPrescriptionDb.Insert(cusIdNumbers, paths, xDocs, setting.TypeName, isRe);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            CooperativeClinicSettings.FilePurge();//打包檔案
        }
        private static void watch_Error(object sender, ErrorEventArgs e)
        {
            Exception error = e.GetException();
        }
        private void TimePrint()
        {
            bool isClick = false;
            while (true)
            {
                if (DateTime.Now.Second % 10 == 5 && !isClick)
                {
                    var cooperativeClinicSettings = new CooperativeClinicSettings();
                    cooperativeClinicSettings.Init();
                    var table = PrescriptionDb.GetXmlOfPrescriptionsByDate(DateTime.Today, DateTime.Today);
                    CusPrePreviewBases cusPres = new CusPrePreviewBases();
                    foreach (DataRow r in table.Rows)
                    {
                        var xDocument = new XmlDocument();
                        xDocument.LoadXml(r["CooCli_XML"].ToString());
                        cusPres.Add(new CooperativePreview(XmlService.Deserialize<CooperativePrescription.Prescription>(xDocument.InnerXml),
                            r.Field<DateTime>("CooCli_InsertTime"), r.Field<int>("CooCli_ID").ToString(),
                            r.Field<bool>("CooCli_IsRead"), r.Field<bool>("CooCli_IsPrint")));
                    }

                    if (Properties.Settings.Default.PrePrint == "True")
                    {
                        foreach (CusPrePreviewBase previewBase in cusPres)
                        {
                            foreach (CooperativeClinicSetting setting in cooperativeClinicSettings)
                            {
                                if (setting.AutoPrint && previewBase.IsPrint == false)
                                {
                                    previewBase.PrintDir();
                                }
                            }
                        }
                    }
                    isClick = true;
                }
                else
                {
                    isClick = false;
                }
            }
        }
        public static void GetTxtFiles(string path)
        {
            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            StreamReader file = new StreamReader(path, Encoding.Default);
            line = file.ReadLine();
            string[] ss = new string[] { };

            ss = line.Split(',');
            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            XmlElement orders = doc.CreateElement("orders");
            orders.SetAttribute("days", ss[8]);
            orders.SetAttribute("mill", "");
            orders.SetAttribute("dosage_method", "5");

            if (counter == 0)
            {
                //建立子節點
                XmlElement case1 = doc.CreateElement("case");
                case1.SetAttribute("from", ss[1]);//設定屬性
                case1.SetAttribute("to", "");//設定屬性
                case1.SetAttribute("local_id", ss[13]);//設定屬性
                case1.SetAttribute("date", ss[6]);//設定屬性
                case1.SetAttribute("time", "");//設定屬性
                case1.SetAttribute("lroom", "");//設定屬性
                case1.SetAttribute("app", "VISW");//設定屬性
                case1.SetAttribute("request_method", "UF");//設定屬性
                doc.AppendChild(case1);

                XmlElement profile = doc.CreateElement("profile");//建立節點
                case1.AppendChild(profile);

                XmlElement person = doc.CreateElement("person");
                person.SetAttribute("name", ss[2]);
                person.SetAttribute("type", ss[0]);
                person.SetAttribute("id", ss[34]);
                person.SetAttribute("foreigner", "no");
                person.SetAttribute("sex", "1");
                person.SetAttribute("birth", ss[4]);
                person.SetAttribute("birth_order", "");
                person.SetAttribute("phone", "");
                person.SetAttribute("family", "");
                person.SetAttribute("mobile", "");
                person.SetAttribute("email", "");
                person.SetAttribute("blood", "");
                person.SetAttribute("blood_rh", "");
                profile.AppendChild(person);

                XmlElement addr = doc.CreateElement("addr");
                person.AppendChild(addr);

                XmlElement remark = doc.CreateElement("remark");
                person.AppendChild(remark);

                XmlElement allergy = doc.CreateElement("allergy");
                person.AppendChild(allergy);

                XmlElement weight = doc.CreateElement("weight");
                person.AppendChild(weight);

                XmlElement temperature = doc.CreateElement("temperature");
                person.AppendChild(temperature);

                XmlElement blood_pressure = doc.CreateElement("blood_pressure");
                person.AppendChild(blood_pressure);

                XmlElement insurance = doc.CreateElement("insurance");
                insurance.SetAttribute("insurance_type", "A");
                insurance.SetAttribute("serial_code", ss[7].PadLeft(4, '0'));
                insurance.SetAttribute("except_code", "");
                insurance.SetAttribute("copayment_code", ss[10]);
                insurance.SetAttribute("case_type", ss[25]);
                insurance.SetAttribute("pay_type", "1");
                insurance.SetAttribute("ldistp_type", "4");
                insurance.SetAttribute("release_type", "1");
                insurance.SetAttribute("ldsc", " ");
                insurance.SetAttribute("labeno_type", "");
                case1.AppendChild(insurance);

                XmlElement study = doc.CreateElement("study");
                study.SetAttribute("doctor_id", ss[12]);
                study.SetAttribute("subject", ss[5].Substring(0, 2));
                case1.AppendChild(study);
                XmlElement diseases = doc.CreateElement("diseases");
                study.AppendChild(diseases);

                XmlElement itema = doc.CreateElement("item");
                itema.SetAttribute("code", ss[14]);
                itema.SetAttribute("type", "ICD10");
                itema.SetAttribute("desc", "");
                diseases.AppendChild(itema);
                XmlElement itemb = doc.CreateElement("item");
                itemb.SetAttribute("code", ss[15]);
                itemb.SetAttribute("type", "ICD10");
                itemb.SetAttribute("desc", "");
                diseases.AppendChild(itemb);

                XmlElement treatments = doc.CreateElement("treatments");
                study.AppendChild(treatments);

                XmlElement chief_complain = doc.CreateElement("chief_complain");
                study.AppendChild(chief_complain);

                XmlElement physical_examination = doc.CreateElement("physical_examination");
                study.AppendChild(physical_examination);

                XmlElement continous_prescription = doc.CreateElement("continous_prescription");
                continous_prescription.SetAttribute("start_at", ss[6]);
                if (ss[24] == "")
                {
                    continous_prescription.SetAttribute("count", "");
                    continous_prescription.SetAttribute("total", "");
                }
                else
                {
                    if (Int32.Parse(ss[24]) > Int32.Parse(ss[40]))
                    {
                        if (Int32.Parse(ss[24]) <= 56)
                        {
                            continous_prescription.SetAttribute("count", "1");
                            continous_prescription.SetAttribute("total", "2");
                        }
                        else
                        {
                            continous_prescription.SetAttribute("count", "1");
                            continous_prescription.SetAttribute("total", "3");
                        }

                    }
                    else
                    {
                        continous_prescription.SetAttribute("count", "");
                        continous_prescription.SetAttribute("total", "");
                    }
                }
                continous_prescription.SetAttribute("other_mo", ss[45]);
                continous_prescription.SetAttribute("eat_at", "");
                case1.AppendChild(continous_prescription);

                case1.AppendChild(orders);

                XmlElement item0 = doc.CreateElement("item");
                item0.SetAttribute("remark", "0");
                item0.SetAttribute("local_code", ss[35]);
                item0.SetAttribute("id", ss[35]);
                item0.SetAttribute("nowid", ss[35]);
                item0.SetAttribute("type", ss[43]);
                item0.SetAttribute("divided_dose", ss[37]);
                item0.SetAttribute("daily_dose", ss[37]);
                item0.SetAttribute("total_dose", ss[42]);
                item0.SetAttribute("freq", ss[39]);
                item0.SetAttribute("days", ss[40]);
                item0.SetAttribute("way", ss[38]);
                item0.SetAttribute("price", "1");
                item0.SetAttribute("multiplier", "1.00");
                item0.SetAttribute("memo", "");
                item0.SetAttribute("desc", ss[36]);
                item0.SetAttribute("dum", "");
                item0.SetAttribute("dnop", "");
                item0.SetAttribute("dtp1", "");
                orders.AppendChild(item0);
            }

            while ((line = file.ReadLine()) != null)
            {
                ss = new string[] { };
                ss = line.Split(',');

                XmlElement item = doc.CreateElement("item");
                item.SetAttribute("remark", "0");
                item.SetAttribute("local_code", ss[35]);
                item.SetAttribute("id", ss[35]);
                item.SetAttribute("nowid", ss[35]);
                item.SetAttribute("type", ss[43]);
                item.SetAttribute("divided_dose", ss[37]);
                item.SetAttribute("daily_dose", ss[37]);
                item.SetAttribute("total_dose", ss[42]);
                item.SetAttribute("freq", ss[39]);
                item.SetAttribute("days", ss[40]);
                item.SetAttribute("way", ss[38]);
                item.SetAttribute("price", "1");
                item.SetAttribute("multiplier", "1.00");
                item.SetAttribute("memo", "");
                item.SetAttribute("desc", ss[36]);
                item.SetAttribute("dum", "");
                item.SetAttribute("dnop", "");
                item.SetAttribute("dtp1", "");
                orders.AppendChild(item);

                counter++;
            }

            try
            {
                string path11 = path + ".xml";
                doc.Save(path11);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public static WareHouse GetWareHouse(string id)
        {
            var result = WareHouses.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }

        public static Institution GetInstitution(string id)
        {
            var result = Institutions.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }

        public static AdjustCase GetAdjustCase(string id)
        {
            if (AdjustCases.SingleOrDefault(a => a.ID.Equals(id)) == null)
            {
                MessageBox.Show(id);
            }
            return AdjustCases.SingleOrDefault(a => a.ID.Equals(id));
        }

        public static Division GetDivision(string id)
        {
            var result = Divisions.SingleOrDefault(i => !string.IsNullOrEmpty(i.ID) && i.ID.Equals(id));
            return result;
        }

        public static Division GetDivisionPrint(string id)
        {
            var Div = new Divisions();
            var result = Div.SingleOrDefault(i => !string.IsNullOrEmpty(i.ID) && i.ID.Equals(id));
            return result;
        }

        public static PaymentCategory GetPaymentCategory(string id)
        {
            return PaymentCategories.SingleOrDefault(p => p.ID.Equals(id));
        }

        public static PrescriptionCase GetPrescriptionCases(string id)
        {
            var result = PrescriptionCases.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }

        public static Copayment GetCopayment(string id)
        {
            var result = Copayments.SingleOrDefault(i => i.Id.Equals(id));
            return result;
        }

        public static Usage GetUsage(string name)
        {
            if (string.IsNullOrEmpty(name)) return new Usage();
            var usage = name.Replace("AC", "").Replace("PC", "");
            var result = new Usage();
            if (Usages.Count(u => u.Reg is null && u.Name.Equals(usage)) != 0)
            {
                result = Usages.Where(u => u.Reg is null).SingleOrDefault(u => u.Name.Equals(usage)).DeepCloneViaJson();
                result.Name = name;
                if (name.Contains("AC") || name.Contains("PC"))
                {
                    result.Name = name;
                    if (name.Contains("AC"))
                        result.PrintName += "(飯前)";
                    else if (name.Contains("PC"))
                        result.PrintName += "(飯後)";
                }
                return result;
            }
            if (Usages.Count(u => u.Reg != null && u.Reg.IsMatch(usage)) != 0)
            {
                var resultList = Usages.Where(u => u.Reg != null).Where(u => u.Reg.IsMatch(usage));
                foreach (var r in resultList)
                {
                    var re = r.DeepCloneViaJson();
                    //result.Name += re.Name;
                    result = re;
                    result.Name = name;
                }
                if (name.Contains("AC") || name.Contains("PC"))
                {
                    if (name.Contains("AC"))
                        result.PrintName += "(飯前)";
                    else if (name.Contains("PC"))
                        result.PrintName += "(飯後)";
                }
                return result;
            }
            return new Usage();
        }

        public static Usage FindUsageByQuickName(string quickName)
        {
            return Usages.Where(u => !string.IsNullOrEmpty(u.QuickName)).Count(u => u.QuickName.Equals(quickName)) == 1 ?
                Usages.Where(u => !string.IsNullOrEmpty(u.QuickName)).SingleOrDefault(u => u.QuickName.Equals(quickName)) : null;
        }

        public static Usage FindUsageByName(string name)
        {
            return Usages.Where(u => !string.IsNullOrEmpty(u.Name)).Count(u => u.Name.Equals(name)) == 1 ?
                Usages.Where(u => !string.IsNullOrEmpty(u.Name)).SingleOrDefault(u => u.Name.Equals(name)) : null;
        }

        public static Position GetPosition(string id)
        {
            if (string.IsNullOrEmpty(id)) return new Position();
            if (Positions.Count(p => p.ID.Equals(id.ToUpper())) != 0)
            {
                return Positions.SingleOrDefault(p => p.ID.Equals(id.ToUpper()));
            }
            return new Position { ID = id.ToUpper(), Name = string.Empty };
        }

        public static SpecialTreat GetSpecialTreat(string id)
        {
            var result = SpecialTreats.SingleOrDefault(i => i.ID.Equals(id));
            return result ?? new SpecialTreat();
        }

        public static Employee GetMedicalPersonByID(int id)
        {
            var result = CurrentPharmacy.MedicalPersonnels.SingleOrDefault(i => i.ID.Equals(id));
            return result;
        }

        public static Employee GetEmployeeByID(int ID)
        {
            var result = EmployeeCollection.SingleOrDefault(i => i.ID.Equals(ID));
            return result;
        }

        public void StartPrintMedBag(ReportViewer r)
        {
            IsBusy = true;
            BusyContent = StringRes.MedBagPrinting;
            Export(r.LocalReport, 22, 24);
            ReportPrint(Properties.Settings.Default.MedBagPrinter);
            IsBusy = false;
        }
        public void StartPrintMedBagCE(ReportViewer r)
        {
            IsBusy = false;
            BusyContent = StringRes.MedBagPrinting;
            Export(r.LocalReport, 22, 24);
            ReportPrint(Properties.Settings.Default.MedBagPrinter);
            IsBusy = false;
        }
        public void StartPrintReceipt(ReportViewer r)
        {
            BusyContent = StringRes.收據列印;
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    Export(r.LocalReport, 21, 14.8);
                    break;

                default:
                    Export(r.LocalReport, 25.4, 9.3);
                    break;
            }
            ReportPrint(Properties.Settings.Default.ReceiptPrinter);
        }

        public void StartPrintReserve(ReportViewer r)
        {
            BusyContent = "封包列印...";
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    Export(r.LocalReport, 21, 14.8);
                    break;

                default:
                    Export(r.LocalReport, 25.4, 9.3);
                    break;
            }
            ReportPrint(Properties.Settings.Default.ReceiptPrinter);
        }

        public void StartPrintMedicineTag(ReportViewer r)
        {
            IsBusy = true;
            BusyContent = StringRes.MedBagPrinting;
            Export(r.LocalReport, 21, 29.7);
            ReportPrint(Properties.Settings.Default.ReportPrinter);
            IsBusy = false;
        }

        public void StartPrintDeposit(ReportViewer r)
        {
            BusyContent = StringRes.押金單據列印;
            switch (Properties.Settings.Default.ReceiptForm)
            {
                case "一般":
                    Export(r.LocalReport, 21, 14.8);
                    break;

                default:
                    Export(r.LocalReport, 25.4, 9.3);
                    break;
            }
            ReportPrint(Properties.Settings.Default.ReceiptPrinter);
        }

        private void Export(LocalReport report, double width, double height)
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
            mStreams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in mStreams)
                stream.Position = 0;
        }

        private void ReportPrint(string printer)
        {

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = printer;
            printDoc.PrintController = new System.Drawing.Printing.StandardPrintController();
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            mCurrentPageIndex = 0;
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    {
                        printDoc.Print();
                    }
                }));
            }
            catch (Exception ex)
            {
                FunctionWindow.MessageWindow.ShowMessage(ex.Message, Class.MessageType.ERROR);
            }

        }

        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding,
            string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            mStreams.Add(stream);
            return stream;
        }

        public void WindowCloseAction()
        {
        }

        private void printDoc_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
                Metafile(mStreams[mCurrentPageIndex]);

            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            ev.Graphics.DrawImage(pageImage, adjustedRect);

            mCurrentPageIndex++;
            ev.HasMorePages = (mCurrentPageIndex < mStreams.Count);
        }
    }
}