using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.Product;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using Microsoft.International.Formatters;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace His_Pos.Service
{
    public static class NewFunction
    {
        public static DataTable JoinTables(DataTable dataTable1, DataTable dataTable2, string joinId)
        {
            DataTable resultTable = dataTable1.Copy();

            resultTable.PrimaryKey = new DataColumn[] { resultTable.Columns[joinId] };
            dataTable2.PrimaryKey = new DataColumn[] { dataTable2.Columns[joinId] };

            resultTable.Merge(dataTable2, false, MissingSchemaAction.Add);
            
            return resultTable;
        }

        public static void FindChildGroup<T>(DependencyObject parent, string childName, ref List<T> list) where T : DependencyObject
        {
            // Checks should be made, but preferably one time before calling.
            // And here it is assumed that the programmer has taken into
            // account all of these conditions and checks are not needed.
            //if ((parent == null) || (childName == null) || (<Type T is not inheritable from FrameworkElement>))
            //{
            //    return;
            //}
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                // Get the child
                var child = VisualTreeHelper.GetChild(parent, i);

                // Compare on conformity the type

                // Not compare - go next
                if (!(child is T childTest))
                {
                    // Go the deep
                    FindChildGroup(child, childName, ref list);
                }
                else
                {
                    // If match, then check the name of the item
                    FrameworkElement childElement = childTest as FrameworkElement;

                    Debug.Assert(childElement != null, nameof(childElement) + " != null");
                    if (childElement.Name == childName)
                    {
                        // Found
                        list.Add(childTest);
                    }

                    // We are looking for further, perhaps there are
                    // children with the same name
                    FindChildGroup(child, childName, ref list);
                }
            }
        }
        public static bool DocumentPrinter(FixedDocument document, string documentName)
        {
            PrintDialog pd = new PrintDialog();

            if ((bool) pd.ShowDialog())
            {
                pd.PrintDocument(document.DocumentPaginator, documentName);
                return true;
            }
            return false;
        }
        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        public static int HowManyChinese(string words) {
            string TmmP;
            int count = 0;
            for (int i = 0; i < words.Length; i++)
            {
                TmmP = words.Substring(i, 1);

                byte[] sarr = System.Text.Encoding.GetEncoding("gb2312").GetBytes(TmmP);

                if (sarr.Length == 2)
                {
                    count++;
                }
            }
            return count;
        }

        public static string FindUsageName(string value)
        {
            var reg_QWxyz = new Regex(@"QW\d+");
            var reg_QWxyzAM = new Regex(@"QW\d+AM");
            var reg_yWzD = new Regex(@"\d+W\d+D");
            var reg_MCDxDy = new Regex(@"MCD\d+D\d+");
            var reg_QxD = new Regex(@"Q\d+D");
            var reg_QxW = new Regex(@"Q\d+W[I]*");
            var reg_QxM = new Regex(@"Q\d+M");
            var reg_QxH = new Regex(@"Q\d+[HI]+");
            var reg_QxMN = new Regex(@"Q\d+MN");


            string print = string.Empty;
            string b = string.Empty;

            if (value.Equals("BID1"))
                print = "每週2次(星期二，星期六)";
            if (value.Equals("BID2"))
                print = "每週2次(星期一，星期五)";
            if (value.Equals("TIW1"))
                print = "每星期3次(星期一、星期三、星期五)";
            if (value.Equals("TIW2"))
                print = "每星期3次(星期二、星期四、星期六)";
            if (value.Equals("BIL1"))
                print = "每天2次，每次(  )ml";

            if (reg_QWxyz.IsMatch(value))
            {
                print = "每星期";
                foreach (var s in b)
                {
                    switch (s)
                    {
                        case '1':
                            print += "一,";
                            break;
                        case '2':
                            print += "二,";
                            break;
                        case '3':
                            print += "三,";
                            break;
                        case '4':
                            print += "四,";
                            break;
                        case '5':
                            print += "五,";
                            break;
                        case '6':
                            print += "六,";
                            break;
                        case '7':
                            print += "日,";
                            break;
                    }
                }
                var i = print.Length - 1;
                return print.Substring(0, i) + "使用";
            }

            if (reg_yWzD.IsMatch(value))
            {
                print = "每";
                print += value.Substring(0, value.IndexOf('W')) + "星期使用";
                print += value.Substring(value.IndexOf('W') + 1, value.IndexOf('Z') - value.IndexOf('W') - 1) + "天";
                return print;
            }

            if (reg_QWxyzAM.IsMatch(value))
            {
                print = "每星期";
                var daysOfWeek = string.Empty;
                foreach (var s in b)
                {
                    switch (s)
                    {
                        case '1':
                            daysOfWeek += "一,";
                            break;
                        case '2':
                            daysOfWeek += "二,";
                            break;
                        case '3':
                            daysOfWeek += "三,";
                            break;
                        case '4':
                            daysOfWeek += "四,";
                            break;
                        case '5':
                            daysOfWeek += "五,";
                            break;
                        case '6':
                            daysOfWeek += "六,";
                            break;
                        case '7':
                            daysOfWeek += "日,";
                            break;
                    }
                }
                print += daysOfWeek.Substring(0, daysOfWeek.Length - 1) + "早上使用";
            }

            if (reg_MCDxDy.IsMatch(value))
            {
                print = "月經第";
                int dIndex = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    if (!value[i].Equals('D') || i == 2) continue;
                    dIndex = i;
                    break;
                }
                print += value.Substring(3, dIndex - 3) + "天至第";
                print += value.Substring(dIndex + 1, value.Length - dIndex - 1) + "天使用";
            }

            if (reg_QxD.IsMatch(value))
            {
                print = "每";
                print += value.Substring(1, value.Length - 2) + "日 1 次";
            }

            if (reg_QxW.IsMatch(value))
            {
                print = "每";
                print += value.Substring(1, value.Length - 2) + "星期 1 次";
            }

            if (reg_QxM.IsMatch(value))
            {
                print = "每";
                print += value.Substring(1, value.Length - 2) + "月 1 次";
            }

            if (reg_QxH.IsMatch(value))
            {
                print = "每";
                print += value.Substring(1, value.Length - 2) + "小時使用 1 次";
            }

            if (reg_QxMN.IsMatch(value))
            {
                print = "每";
                print += value.Substring(1, value.Length - 2) + "分鐘使用 1 次";
            }
            return print;
        }

        public static string GetUsagePrintName(string value)
        {
            foreach (var u in MainWindow.Usages)
            {
                if (u.Reg.IsMatch(value))
                    return u.PrintName;
            }

            return value;
        }

        public static T DeepCloneViaJson<T>(this T source)
        {
            if (source != null)
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto
                };

                var serializedObj = JsonConvert.SerializeObject(source, Formatting.Indented, jsonSerializerSettings);
                return JsonConvert.DeserializeObject<T>(serializedObj, jsonSerializerSettings);
            }
            else
            { return default(T); }

        }

        public static string ConvertToAsiaMoneyFormat(int cost)
        {
            return EastAsiaNumericFormatter.FormatWithCulture("L", cost, null, new CultureInfo("zh-TW")) + "元整";
        }

        public static bool CheckCopaymentFreeProject(string copaymentId)
        {
            #region 代碼對照
            /*
             * 007:山地離島地區之就醫（88.7增訂）、山地原住民暨離島地區接受醫療院所戒菸治療服務免除戒菸藥品部分負擔
             * 009:本署其他規定免部分負擔者，如產檢時，同一主治醫師併同開給一般處方，百歲人瑞免部分負擔，921震災，行政協助性病或藥癮病患全面篩檢愛滋計畫、行政協助孕婦全面篩檢愛滋計畫等
             * I21:藥費小於100免收
             * I22:符合本保險藥費免部分負擔範圍規定者，包括慢性病連續處方箋案件、牙醫案件、門診論病例計酬案件
             */
            #endregion 代碼對照

            var freeList = new List<string>() { "007", "009", "I21", "I22" };
            foreach (var id in freeList)
            {
                if (copaymentId.Equals(id))
                    return true;
            }
            return false;
        }

        #region 藥袋.收據列印

        public static void PrintMedBag(Prescription CurrentPrescription, DeclareData CurrentDeclareData, double MedicinePoint, int SelfCost,int Pay, string from, int Charge, PrescriptionDec2View decInstance = null, PrescriptionInquire.PrescriptionInquireOutcome inquireOutcome = null) {
            var medBagResult = new MedBagSelectionWindow();
            var singleMode = (bool)medBagResult.ShowDialog();
            var receiptResult = new YesNoMessageWindow("是否列印收據", "列印收據");
            var receiptPrint = false;
            if (Charge > 0)
            {
                receiptPrint = (bool) receiptResult.ShowDialog();
            }
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new ObservableCollection<MedBagMedicine>();
            foreach (var m in CurrentPrescription.Medicines)
            {
                switch (m)
                {
                    case DeclareMedicine medicine:
                        medBagMedicines.Add(new MedBagMedicine(medicine, singleMode));
                        break;
                    case PrescriptionOTC otc:
                        medBagMedicines.Add(new MedBagMedicine(otc, singleMode));
                        break;
                }
            }

            if (singleMode)
            {
                foreach (var m in medBagMedicines)
                {
                    rptViewer.LocalReport.ReportPath = @"RDLC\MedBagReportSingle.rdlc";
                    rptViewer.ProcessingMode = ProcessingMode.Local;

                    string treatmentDate =
                        DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Treatment.TreatmentDate, true);
                    string treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                              treatmentDate.Split('/')[2] + "日";
                    var parameters = new List<ReportParameter>
                    {
                        new ReportParameter("PharmacyName_Id",
                            MainWindow.CurrentPharmacy.Name + "(" + MainWindow.CurrentPharmacy.Id + ")"),
                        new ReportParameter("PharmacyAddress", MainWindow.CurrentPharmacy.Address),
                        new ReportParameter("PharmacyTel", MainWindow.CurrentPharmacy.Tel),
                        new ReportParameter("MedicalPerson", CurrentPrescription.Pharmacy.MedicalPersonnel.Name),
                        new ReportParameter("PatientName", CurrentPrescription.Customer.Name),
                        new ReportParameter("PatientGender_Birthday",
                            (CurrentPrescription.Customer.Gender ? "男" : "女") + "/" +
                            DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Customer.Birthday, true)),
                        new ReportParameter("TreatmentDate", treatmentDateChi),
                        new ReportParameter("RecId", " "), //病歷號
                        new ReportParameter("Division",
                            CurrentPrescription.Treatment.MedicalInfo.Hospital.Division.Name),
                        new ReportParameter("Hospital", CurrentPrescription.Treatment.MedicalInfo.Hospital.Name),
                        new ReportParameter("PaySelf", SelfCost.ToString()),
                        new ReportParameter("ServicePoint", CurrentDeclareData.D38MedicalServicePoint.ToString()),
                        new ReportParameter("TotalPoint", CurrentDeclareData.D18TotalPoint.ToString()),
                        new ReportParameter("CopaymentPoint", CurrentDeclareData.D17CopaymentPoint.ToString()),
                        new ReportParameter("HcPoint", CurrentDeclareData.D16DeclarePoint.ToString()),
                        new ReportParameter("MedicinePoint", MedicinePoint.ToString(CultureInfo.InvariantCulture)),
                        new ReportParameter("MedicineId", m.Id),
                        new ReportParameter("MedicineName", m.Name),
                        new ReportParameter("MedicineChineseName", m.ChiName),
                        new ReportParameter("Ingredient", m.Ingredient),
                        new ReportParameter("Indication", m.Indication),
                        new ReportParameter("SideEffect", m.SideEffect),
                        new ReportParameter("Note", m.Note),
                        new ReportParameter("Usage", m.Usage),
                        new ReportParameter("MedicineDay", m.MedicineDays),
                        new ReportParameter("Amount", m.Total),
                        new ReportParameter("Form", m.Form)
                    };
                    rptViewer.LocalReport.SetParameters(parameters);
                    rptViewer.LocalReport.DataSources.Clear();
                    rptViewer.LocalReport.Refresh();
                    var loadingWindow = new LoadingWindow();
                    if (from.Equals("登錄"))
                        loadingWindow.PrintMedbag(rptViewer, decInstance, receiptPrint);
                    else
                    {
                        loadingWindow.PrintMedbagFromInquire(rptViewer, inquireOutcome, receiptPrint);
                    }
                    loadingWindow.Show();
                }
            }
            else
            {
                foreach (var m in medBagMedicines.GroupBy(info => info.Usage)
                    .Select(group => new { UsageName = group.Key, count = group.Count() })
                    .OrderBy(x => x.UsageName))
                {
                    var i = 1;
                    foreach (var med in medBagMedicines)
                    {
                        if (!med.Usage.Equals(m.UsageName)) continue;
                        med.MedNo = i.ToString();
                        i++;
                    }
                }

                var json = JsonConvert.SerializeObject(medBagMedicines);
                var dataTable = JsonConvert.DeserializeObject<DataTable>(json);

                rptViewer.LocalReport.ReportPath = @"RDLC\MedBagReport.rdlc";
                rptViewer.ProcessingMode = ProcessingMode.Local;

                string treatmentDate =
                    DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Treatment.TreatmentDate, true);
                string treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                          treatmentDate.Split('/')[2] + "日";
                var parameters = new List<ReportParameter>
                {
                    new ReportParameter("PharmacyName_Id",
                        MainWindow.CurrentPharmacy.Name + "(" + MainWindow.CurrentPharmacy.Id + ")"),
                    new ReportParameter("PharmacyAddress", MainWindow.CurrentPharmacy.Address),
                    new ReportParameter("PharmacyTel", MainWindow.CurrentPharmacy.Tel),
                    new ReportParameter("MedicalPerson", CurrentPrescription.Pharmacy.MedicalPersonnel.Name),
                    new ReportParameter("PatientName", CurrentPrescription.Customer.Name),
                    new ReportParameter("PatientGender_Birthday",
                        (CurrentPrescription.Customer.Gender ? "男" : "女") + "/" +
                        DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Customer.Birthday, true)),
                    new ReportParameter("TreatmentDate", treatmentDateChi),
                    new ReportParameter("Hospital", CurrentPrescription.Treatment.MedicalInfo.Hospital.Name),
                    new ReportParameter("PaySelf", SelfCost.ToString()),
                    new ReportParameter("ServicePoint", CurrentDeclareData.D38MedicalServicePoint.ToString()),
                    new ReportParameter("TotalPoint", CurrentDeclareData.D18TotalPoint.ToString()),
                    new ReportParameter("CopaymentPoint", CurrentDeclareData.D17CopaymentPoint.ToString()),
                    new ReportParameter("HcPoint", CurrentDeclareData.D16DeclarePoint.ToString()),
                    new ReportParameter("MedicinePoint", MedicinePoint.ToString(CultureInfo.InvariantCulture)),
                    new ReportParameter("Division", CurrentPrescription.Treatment.MedicalInfo.Hospital.Division.Name)
                };
                rptViewer.LocalReport.SetParameters(parameters);
                rptViewer.LocalReport.DataSources.Clear();
                var rd = new ReportDataSource("DataSet1", dataTable);
                rptViewer.LocalReport.DataSources.Add(rd);
                rptViewer.LocalReport.Refresh();
                var loadingWindow = new LoadingWindow();
                if (from.Equals("登錄"))
                    loadingWindow.PrintMedbag(rptViewer, decInstance, receiptPrint);
                else
                {
                    loadingWindow.PrintMedbagFromInquire(rptViewer, inquireOutcome, receiptPrint);
                }
                loadingWindow.Show();
            }
            if (receiptPrint)
            {
                if (from.Equals("登錄"))
                    PrintReceipt(CurrentPrescription, CurrentDeclareData, MedicinePoint, SelfCost,Pay, from, decInstance ,null);
                else
                {
                    PrintReceipt(CurrentPrescription, CurrentDeclareData, MedicinePoint, SelfCost, Pay,from, null, inquireOutcome);
                }
            }
            //var defaultMedBag = MedBagDb.GetDefaultMedBagData(messageBoxResult == MessageBoxResult.Yes ? MedBagMode.SINGLE : MedBagMode.MULTI);
            //File.WriteAllText(ReportService.ReportPath, string.Empty);
            //File.AppendAllText(ReportService.ReportPath, ReportService.SerializeObject<Report>(ReportService.CreatReport(defaultMedBag, CurrentPrescription)));
            //for (var i = 0; i < CurrentPrescription.Medicines.Count; i++)
            //{
            //    ReportService.CreatePdf(defaultMedBag,i);
            //}
        }

        public static void PrintReceipt(Prescription CurrentPrescription, DeclareData CurrentDeclareData, double MedicinePoint,int SelfCost ,int Pay,string from, PrescriptionDec2View decInstance = null, PrescriptionInquire.PrescriptionInquireOutcome inquireOutcome = null) {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.ReportPath = @"RDLC\HisReceipt.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var adjustDate =
                DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Treatment.AdjustDate, true);
            var doctor =
                CurrentPrescription.Treatment.MedicalInfo.Hospital.Id.Equals(CurrentPrescription.Treatment.MedicalInfo
                    .Hospital.Doctor.IcNumber)
                    ? CurrentPrescription.Treatment.MedicalInfo.Hospital.Doctor.Name
                    : string.Empty;
            var cusGender = CurrentPrescription.Customer.IcNumber.Substring(1, 1).Equals("2") ? "女" : "男";
            var parameters = new List<ReportParameter>
            {
                new ReportParameter("Pharmacy", MainWindow.CurrentPharmacy.Name),
                new ReportParameter("PatientName", CurrentPrescription.Customer.Name),
                new ReportParameter("Gender", cusGender),
                new ReportParameter("Birthday",
                    DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Customer.Birthday, true)),
                new ReportParameter("AdjustDate", adjustDate),
                new ReportParameter("Hospital", CurrentPrescription.Treatment.MedicalInfo.Hospital.Name),
                new ReportParameter("Doctor", doctor), //病歷號
                new ReportParameter("MedicalNumber", CurrentPrescription.Customer.IcCard.MedicalNumber),
                new ReportParameter("MedicineCost", MedicinePoint.ToString(CultureInfo.InvariantCulture)),
                new ReportParameter("MedicalServiceCost", CurrentDeclareData.D38MedicalServicePoint.ToString()),
                new ReportParameter("TotalMedicalCost",CurrentDeclareData.D16DeclarePoint.ToString()),
                new ReportParameter("CopaymentCost", CurrentDeclareData.D17CopaymentPoint.ToString()),
                new ReportParameter("HcPay", CurrentDeclareData.D16DeclarePoint.ToString()),
                new ReportParameter("SelfCost", SelfCost.ToString()),
                new ReportParameter("ActualReceive", Pay.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(Pay))
            };
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.Refresh();
            var loadingWindow = new LoadingWindow();

            if(from.Equals("登錄"))
                loadingWindow.PrintReceipt(rptViewer, decInstance);
            else
            {
                loadingWindow.PrintReceiptFromInquire(rptViewer, inquireOutcome);
            }
            loadingWindow.Show();
        }
        #endregion
        public static Division CheckHospitalNameContainsDivision(string name)
        {
            var divisionMatch = 0;
            var divisionId = string.Empty;
            foreach (var d in MainWindow.Divisions)
            {
                var r = new Regex(d.Name);
                if (!r.IsMatch(name)) continue;
                divisionId = d.Id;
                divisionMatch++;
            }
            if (divisionMatch == 0 && name.Contains("牙醫"))
            {
                return MainWindow.Divisions.SingleOrDefault(d => d.Id.Equals("40")).DeepCloneViaJson();
            }
            if (divisionMatch != 1 || string.IsNullOrEmpty(divisionId))
                return new Division();
            return MainWindow.Divisions.SingleOrDefault(d => d.Id.Equals(divisionId)).DeepCloneViaJson();
        }
    }
}
