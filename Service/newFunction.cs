using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using Microsoft.International.Formatters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using PrintDialog = System.Windows.Controls.PrintDialog;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.Service
{
    public static class NewFunction
    {
        public static List<T> GetLogicalChildCollection<T>(this DependencyObject parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        public static void FindChildGroup<T>(DependencyObject parent, string childName, ref List<T> list) where T : DependencyObject
        {
            // Checks should be made, but preferably one time before calling.
            // And here it is assumed that the programmer has taken into
            // account all of these conditions and checks are not needed.
            //if ((parent == null) || (childName == null) || (<Type T is not inheritable from FrameworkElement>))
            //{
            ///    return;
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

            if ((bool)pd.ShowDialog())
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

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        public static int HowManyChinese(string words)
        {
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

        public static T DeepCloneViaJson<T>(this T source)
        {
            if (source != null)
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto
                };

                var serializedObj = JsonConvert.SerializeObject(source, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);
                return JsonConvert.DeserializeObject<T>(serializedObj, jsonSerializerSettings);
            }
            else
            { return default(T); }
        }

        public static string ConvertToAsiaMoneyFormat(int cost)
        {
            return EastAsiaNumericFormatter.FormatWithCulture("L", cost, null, new CultureInfo("zh-TW")) + "元整";
        }

        public static NewClass.Prescription.Treatment.Division.Division CheckHospitalNameContainsDivision(string name)
        {
            var divisionMatch = 0;
            var divisionId = string.Empty;
            foreach (var d in ViewModelMainWindow.Divisions)
            {
                var r = new Regex(d.Name);
                if (!r.IsMatch(name)) continue;
                divisionId = d.ID;
                divisionMatch++;
            }
            if (divisionMatch == 0 && name.Contains("牙醫"))
            {
                return ViewModelMainWindow.Divisions.SingleOrDefault(d => d.ID.Equals("40"));
            }
            if (divisionMatch != 1 || string.IsNullOrEmpty(divisionId))
                return new NewClass.Prescription.Treatment.Division.Division();
            return ViewModelMainWindow.Divisions.SingleOrDefault(d => d.ID.Equals(divisionId));
        }

        public static void ExceptionLog(string log)
        {
            string logpath = @"C:\Program Files\HISPOS\ExceptionLog.txt";
            StreamWriter str = new StreamWriter(logpath, true);
            str.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + "  Event:" + log);
            str.Close();
        }

        public static List<bool?> CheckPrint(Prescription p, bool? focus = null)
        {
            bool? receiptPrint = null;
            var result = new List<bool?>();

            if (p.PrescriptionStatus.IsPrint == true)
            {
                if (p.PrescriptionPoint.AmountsPay > 0)
                {
                    var receiptResult = new ConfirmWindow(StringRes.收據列印確認, StringRes.列印確認, true);
                    receiptPrint = receiptResult.DialogResult;
                }
                else
                    receiptPrint = false;
                result.Add(false);
                result.Add(false);
                result.Add(receiptPrint);
            }
            else
            {
                var print = focus ?? true;
                var medBagPrint = new ConfirmWindow(StringRes.藥袋列印確認, StringRes.列印確認, print);
                var printMedBag = medBagPrint.DialogResult;
                bool? printSingle = null;
                if (printMedBag != null)
                {
                    if (p.PrescriptionPoint.AmountsPay > 0)
                    {
                        var receiptResult = new ConfirmWindow(StringRes.收據列印確認, StringRes.列印確認, true);
                        receiptPrint = receiptResult.DialogResult;
                    }
                    else
                        receiptPrint = false;
                    if ((bool)printMedBag)
                    {
                        var printBySingleMode = new SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedBagSelectionWindow();
                        printBySingleMode.ShowDialog();
                        printSingle = printBySingleMode.result;
                    }
                }
                result.Add(printMedBag);
                result.Add(printSingle);
                result.Add(receiptPrint);
            }
            return result;
        }
        public static List<bool?> CheckPrintDir(Prescription p, bool? focus = null)
        {
            var print = focus ?? true;
            var result = new List<bool?>();

            bool? printSingle = null;
            bool? receiptPrint = null;

            
            result.Add(true);
            result.Add(true);
            result.Add(false);
            return result;
        }

        public static void GetXmlFiles()
        {
            bool isRe = false;
            string isRePost = "";
            var cooperativeClinicSettings = new CooperativeClinicSettings();
            cooperativeClinicSettings.Init();
            var xDocs = new List<XDocument>();
            var cusIdNumbers = new List<string>();
            var paths = new List<string>();
            foreach (var c in cooperativeClinicSettings)
            {
                var path = c.FilePath;
                if (string.IsNullOrEmpty(path)) continue;
                try
                {
                    var fileEntries = Directory.GetFiles(path);
                    foreach (var s in fileEntries)
                    {
                        
                        try
                        {
                            if (c.TypeName == "杏翔"&& Path.GetExtension(s)==".txt")
                            {
                                GetTxtFiles(s,path,Path.GetFileName(s));
                            }
                       
                            var xDocument = XDocument.Load(s);
                            var cusIdNumber = xDocument.Element("case").Element("profile").Element("person").Attribute("id").Value;
                            if (String.IsNullOrEmpty(xDocument.Element("case").Element("continous_prescription").Attribute("other_mo").Value.ToString())) { isRePost = "2"; }
                            else
                            {
                                isRePost = xDocument.Element("case").Element("continous_prescription").Attribute("other_mo").Value.ToString();
                            }
                            if (isRePost != "1")
                            {
                                isRe = true;
                            }
                            else
                            { isRe = false; }
                            xDocs.Add(xDocument);
                            cusIdNumbers.Add(cusIdNumber);
                            paths.Add(s);
                        }
                        catch (Exception ex)
                        {
                            ExceptionLog(ex.Message);

                           
                        }
                    }
                    XmlOfPrescriptionDb.Insert(cusIdNumbers, paths, xDocs, c.TypeName, isRe);
                }
                catch (Exception ex)
                {
                    ExceptionLog(ex.Message);
                }
            }
        }
        public static void GetTxtFiles(string path, string path1, string v) {
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
                MessageBox.Show(e.ToString()) ;
            }

        }
        public static bool CheckDataRowContainsColumn(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column);
        }

        public static bool CheckTransaction(DataTable table)
        {
            return table.Rows.Count == 0 || !table.Rows[0].Field<bool>("Result");
        }

        public static bool CheckNotIntMedicalNumber(string medicalNumber, string adjustCaseID, int? chronicSeq)
        {
            if (medicalNumber.StartsWith("IC")) return true;
            switch (medicalNumber)
            {
                case "A000":
                case "A010":
                case "A011":
                case "A020":
                case "A021":
                case "A030":
                case "A031":
                case "B000":
                case "B001":
                case "C000":
                case "C001":
                case "D000":
                case "F000":
                case "D001":
                case "G000":
                case "D010":
                case "H000":
                case "D011":
                case "E000":
                case "Z000":
                case "E001":
                case "Z001":
                case "J000" when adjustCaseID.Equals("2") && chronicSeq != null && chronicSeq >= 2:
                    return true;

                case "J000" when !adjustCaseID.Equals("2") || chronicSeq is null || chronicSeq < 2:
                    MessageWindow.ShowMessage("卡序:J000 僅可使用於慢箋第二次以後調劑處方無填載就醫序號", MessageType.ERROR);
                    return false;
            }
            int number;
            var conversionSuccessful = int.TryParse(medicalNumber, out number);
            if (conversionSuccessful)
                return true;
            MessageWindow.ShowMessage("就醫序號格式錯誤", MessageType.ERROR);
            return false;
        }

        public static bool CheckHomeCareMedicalNumber(string medicalNumber, AdjustCase adjust)
        {
            switch (medicalNumber)
            {
                case "N":
                    if (adjust.CheckIsHomeCare())
                        return true;
                    else
                    {
                        MessageWindow.ShowMessage("非藥事居家照護就醫序號不可為\"N\"", MessageType.ERROR);
                        return false;
                    }
                default:
                    MessageWindow.ShowMessage("就醫序號長度錯誤，應為4碼", MessageType.ERROR);
                    return false;
            }
        }

        public static void ShowMessageFromDispatcher(string message, MessageType type)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWindow.ShowMessage(message, type);
            });
        }
    }
}