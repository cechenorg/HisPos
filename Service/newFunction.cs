﻿using System;
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
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
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
            foreach (var u in ViewModelMainWindow.Usages)
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
                    var printBySingleMode = new MedBagSelectionWindow();
                    printBySingleMode.ShowDialog();
                    printSingle = printBySingleMode.result;
                }
            }
            result.Add(printMedBag);
            result.Add(printSingle);
            result.Add(receiptPrint);
            return result;
        }
    }
}
