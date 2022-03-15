﻿using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.HisApi;
using His_Pos.NewClass;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Medicine.Position;
using His_Pos.NewClass.Medicine.Usage;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription.CustomerPrescriptions;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.WareHouse;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativePrescriptionWindow;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelMainWindow : MainViewModel, IViewModelMainWindow, IChromeTabViewModel
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
        public static Pharmacy CurrentPharmacy { get; set; }
        public static Employee CurrentUser { get; set; }
        public static Employees EmployeeCollection { get; set; }
        public static string CooperativeInstitutionID { get; private set; }
        private int mCurrentPageIndex;
        private IList<Stream> mStreams;
        public string pathFile;

        public ViewModelMainWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            MainWindow.ServerConnection.OpenConnection();
            CurrentPharmacy = Pharmacy.GetCurrentPharmacy();
            CurrentPharmacy.MedicalPersonnels = new Employees();
            CooperativeInstitutionID = WebApi.GetCooperativeClinicId(CurrentPharmacy.ID);
            MainWindow.ServerConnection.CloseConnection();
            CanMoveTabs = true;
            ShowAddButton = false;

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
                            if (c.FilePath == null)
                            {
                                continue;
                            }
                            FileSystemWatcher watch = new FileSystemWatcher(c.FilePath, "*.txt");
                            //開啟監聽
                            watch.EnableRaisingEvents = true;
                            //是否連子資料夾都要偵測
                            watch.IncludeSubdirectories = true;
                            //新增時觸發事件



                            watch.Created += watch_Created;
                        }
                    }

                }

                cooperativePres.GetCooperative(DateTime.Today.AddDays(-10), DateTime.Today);
                CooperativeClinicSettings.Init();
                CooperativeClinicSettings.FilePurge();
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

        void watch_Created(object sender, FileSystemEventArgs e)
        {
            bool isRe=false;
            string isRePost="0";
            
            string DirectPath = e.FullPath;
            //如果偵測到為檔案，則依路徑對此資料夾做檔案處理
            if (DirectPath != null)
            {
                var cooperativeClinicSettings = new CooperativeClinicSettings();
                cooperativeClinicSettings.Init();
                var xDocs = new List<XDocument>();
                var cusIdNumbers = new List<string>();
                var paths = new List<string>();
                foreach (var c in cooperativeClinicSettings)
                {
                    try
                    {
                        
                        var fileEntries = Directory.GetFiles(pathFile);
                        foreach (var s in fileEntries)
                        {

                            try
                            {
                                if (c.TypeName == "杏翔" && Path.GetExtension(s) == ".txt")
                                {
                                    GetTxtFiles(s, pathFile, Path.GetFileName(s));
                                }
                
                                var xDocument = XDocument.Load(s);
                               
                                var cusIdNumber = xDocument.Element("case").Element("profile").Element("person").Attribute("id").Value;
                                if (xDocument.Element("case").Element("continous_prescription").Attribute("other_mo") == null ) { isRePost = "2"; }
                                else
                                {
                                    isRePost = xDocument.Element("case").Element("continous_prescription").Attribute("other_mo").Value.ToString();
                                }
                                if (isRePost != "0") {
                                    isRe = true;
                                 
                                }
                                else
                                { isRe = false;}
                                xDocs.Add(xDocument);
                                cusIdNumbers.Add(cusIdNumber);
                                paths.Add(s);
                            }
                            catch (Exception ex)
                            {
                                
                            }
                        }
                        if (ViewModelMainWindow.CurrentPharmacy.ID == "5931017216"|| ViewModelMainWindow.CurrentPharmacy.ID == "7777777777")
                        {
                            XmlOfPrescriptionDb.Insert(cusIdNumbers, paths, xDocs, c.TypeName, isRe);
                        }
                        else
                        {
                            XmlOfPrescriptionDb.Insert(cusIdNumbers, paths, xDocs, c.TypeName, false);
                        }
                    }
                    catch (Exception ex)
                    {
                       
                    }
                }

                CooperativePrescriptionViewModel gg = new CooperativePrescriptionViewModel(11);
                if (gg.CooPreCollectionView == null)
                {

                }
                else
                {
                    foreach (CusPrePreviewBase ff in gg.CooPreCollectionView)
                    {
                  
                        MainWindow.Instance.Dispatcher.Invoke(() =>
                        {
                            if (Properties.Settings.Default.PrePrint == "True")
                            {
                                foreach (var c in cooperativeClinicSettings)
                                {
                                    if (c.AutoPrint == true)
                                    {
                                        if (ff.IsPrint == false)
                                        {

                                            gg.PrintAction(ff);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
                
               
            }

        }
        public static void GetTxtFiles(string path, string path1, string v)
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
                insurance.SetAttribute("serial_code", ss[7].PadLeft(4,'0'));
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

        public static Employee GetMedicalPersonByIDNumber(string idNum)
        {
            var result = CurrentPharmacy.MedicalPersonnels.SingleOrDefault(i => i.IDNumber.Equals(idNum));
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
                FunctionWindow.MessageWindow.ShowMessage(ex.Message, NewClass.MessageType.ERROR);
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