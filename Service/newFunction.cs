using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using Microsoft.International.Formatters;
using Newtonsoft.Json;
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
        public static void ExceptionLog(string log) {
            string logpath = @"C:\Program Files\HISPOS\ExceptionLog.txt";
            StreamWriter str = new StreamWriter(logpath, true);
            str.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + "  Event:" + log);
            str.Close();
        }

        public static List<bool?> CheckPrint(Prescription p,bool? focus = null)
        {
            var print = focus ?? true;
            var result = new List<bool?>();
            var medBagPrint = new ConfirmWindow(StringRes.藥袋列印確認, StringRes.列印確認, print);
            var printMedBag = medBagPrint.DialogResult;
            bool? printSingle = null;
            bool? receiptPrint = null;
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
            return result;
        }

        public static void GetXmlFiles()
        {
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
                            var xDocument = XDocument.Load(s);
                            var cusIdNumber = xDocument.Element("case").Element("profile").Element("person").Attribute("id").Value;
                            xDocs.Add(xDocument);
                            cusIdNumbers.Add(cusIdNumber);
                            paths.Add(s);
                        }
                        catch (Exception ex)
                        {
                            ExceptionLog(ex.Message);
                        }
                    }
                    XmlOfPrescriptionDb.Insert(cusIdNumbers, paths, xDocs, c.TypeName);
                }
                catch (Exception ex)
                {
                    ExceptionLog(ex.Message);
                }
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

        public static bool CheckNotIntMedicalNumber(string medicalNumber,string adjustCaseID,int? chronicSeq)
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
                    MessageWindow.ShowMessage("卡序:J000 僅可使用於慢箋第二次以後調劑處方無填載就醫序號",MessageType.ERROR);
                    return false;
            }
            int number;
            var conversionSuccessful = int.TryParse(medicalNumber, out number);
            if (conversionSuccessful)
                return true;
            MessageWindow.ShowMessage("就醫序號格式錯誤",MessageType.ERROR);
            return false;
        }

        public static bool CheckHomeCareMedicalNumber(string medicalNumber,AdjustCase adjust)
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

        public static void ShowMessageFromDispatcher(string message,MessageType type)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWindow.ShowMessage(message, type);
            });
        }
    }
}
