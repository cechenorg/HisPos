using System;
using System.Collections.Generic;
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
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.International.Formatters;
using Newtonsoft.Json;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using PrintDialog = System.Windows.Controls.PrintDialog;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.Service
{
    public static class NewFunction
    {
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
        public static void Uncompress(string zipFileName, string targetPath) {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFileName)))
            { 
                DirectoryInfo di = new DirectoryInfo(targetPath); 
                ZipEntry theEntry; 
                // 逐一取出壓縮檔內的檔案(解壓縮)
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    int size = 2048;
                    byte[] data = new byte[2048];
                    using (FileStream fs = new FileStream(di.FullName + "\\" + GetBasename(theEntry.Name), FileMode.Create))
                    {
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);

                            if (size > 0)
                                fs.Write(data, 0, size);
                            else
                                break;
                        } 
                    }
                }
            }
        }

        // 取得檔名(去除路徑)
        public static string GetBasename(string fullName) {
            string result;
            int lastBackSlash = fullName.LastIndexOf("\\");
            result = fullName.Substring(lastBackSlash + 1); 
        return result;
        }

        public static List<bool?> CheckPrint(Prescription p)
        {
            var result = new List<bool?>();
            var medBagPrint = new ConfirmWindow(StringRes.PrintMedBag, StringRes.PrintConfirm, true);
            var printMedBag = medBagPrint.DialogResult;
            bool? printSingle = null;
            bool? receiptPrint = null;
            if (printMedBag != null)
            {
                if (p.PrescriptionPoint.CopaymentPoint + p.PrescriptionPoint.AmountSelfPay > 0)
                {
                    var receiptResult = new ConfirmWindow(StringRes.PrintReceipt, StringRes.PrintConfirm, true);
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
    }
}
