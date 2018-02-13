using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using His_Pos.Class;
using ImeLib;

namespace His_Pos
{
    
    public class Function
    {

        //用途:將中文字轉成注音符號再轉成英文字母
        //輸入:中文字串
        //輸出:英文字串
        public string ChangeNameToEnglish( string txtInput)
        {
            string[] result;
            string resultOutput = string.Empty;
            using (MsImeFacade ime = new MsImeFacade(ImeClass.Taiwan))
            {

                 result = ime.GetBopomofo(txtInput);
                for (int i = 0; i < result.Length; i++)
                {
                    resultOutput += result[i].Substring(0, 1);
                }
            }
            return resultOutput;
        }//ChangeNameToEnglish

        //用途:取得Enum Description value
        //輸入:Enum值
        //輸出:Description值
        public string GetEnumDescription(string type, string value)
        {
            var reply = string.Empty;
            if (type == "ErrorCode")
            {
                var evalue = (ErrorCode)Enum.Parse(typeof(ErrorCode), value);
                var fi = evalue.GetType().GetField(evalue.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                    reply = attributes[0].Description;
                else
                    reply = value;
            }
            if (type == "Alphabat")
            {
                var evalue = (Alphabat)Enum.Parse(typeof(Alphabat), value);
                var fi = evalue.GetType().GetField(evalue.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                    reply = attributes[0].Description;
                else
                    reply = value;
            }
            return reply;
        } //GetEnumDescription

        public string dateFormatConvert(DateTime selectedDate)
        {
            string month;
            if (selectedDate.Month < 10)
                month = "0" + selectedDate.Month;
            else
            {
                month = selectedDate.Month.ToString();
            }
            string day;
            if (selectedDate.Day < 10)
                day = "0" + selectedDate.Day;
            else
            {
                day = selectedDate.Day.ToString();
            }
            
            string date = (int.Parse(selectedDate.Year.ToString()) - 1911) + month + day;
            return date;
        }

        public void FindChildGroup<T>(DependencyObject parent, string childName, ref List<T> list) where T : DependencyObject
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
        public string GetDateFormat( string date) {
            if (date.Length == 1) date = "0" + date;
            return date;
        }

        public void ExportXml(XmlDocument xml,string FileTypeName) {
            Function function = new Function();
            var twc = new TaiwanCalendar();
            var year = twc.GetYear(DateTime.Now).ToString();
            var month = function.GetDateFormat(twc.GetMonth(DateTime.Now).ToString());
            var day = function.GetDateFormat(twc.GetDayOfMonth(DateTime.Now).ToString());
            var pathsplit = System.Environment.CurrentDirectory.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            var path = pathsplit[0];
            for (var i = 1; i < pathsplit.Length; i++)
            {
                path += "\\" + pathsplit[i];
                if (pathsplit[i] == "System") break;
            }
            
            path += "\\" + FileTypeName; // "匯出健保資料XML檔案"  "匯出申報XML檔案"
            var path_ym = path + "\\" + year + month;
            var path_ymd = path + "\\" + year + month + "\\" + day;
            var path_file = path_ym + "\\" + day + "\\" + year + month + day;
            if (!Directory.Exists(path_ym)) Directory.CreateDirectory(path_ym);
            if (!Directory.Exists(path_ymd)) Directory.CreateDirectory(path_ymd);
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.GetEncoding("big5");
            var writer = XmlWriter.Create(path_file + ".xml", settings);
            xml.Save(writer);
            writer.Close();
            //壓縮XML
            if (File.Exists(path_file + ".zip")) File.Delete(path_file + ".zip");
            var psi = new System.Diagnostics.Process();
            psi.StartInfo.FileName = "makecab.exe";
            psi.StartInfo.Arguments = path_file + ".xml " + path_file + ".zip";
            psi.Start();

            psi.WaitForInputIdle();
            //設定要等待相關的處理序結束的時間 
            psi.WaitForExit();

            //StringBuilder pUploadFileName = new StringBuilder();
            //pUploadFileName.Append(path + "\\" + year + month + "\\" + day + "\\" + year + month + day + ".xml");
            //StringBuilder fFileSize = new StringBuilder();
            //long length = new System.IO.FileInfo(path + "\\" + year + month + "\\" + day + "\\" + year + month + day + ".xml").Length;
            //fFileSize.Append(length);
            //StringBuilder pNumber = new StringBuilder();
            //fFileSize.Append(count);
            //StringBuilder pBuffer = new StringBuilder();
            //int iBufferLen = xml.InnerText.Length;
            //int port = 5; ;
            //HisApi.HisApiBase.csOpenCom(0);
            //HisApi.HisApiBase.csUploadData(pUploadFileName, fFileSize, pNumber, pBuffer, ref iBufferLen);
            //HisApi.HisApiBase.csCloseCom();
        }
        /*
         * 判斷輸入是否為數字
         */
        public static bool IsNumeric(string input)
        {
            try
            {
                var i = Convert.ToInt32(input);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /*
         * 判斷輸入空值
         */
        public static bool CheckEmptyInput(string input,string message)
        {
            if (input != string.Empty) return false;
            MessageBox.Show(message);
            return true;
        }

        public string ToInvCulture(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public string SetStrFormat(double value, string format)
        {
            return string.Format(format, value);
        }

        public string XmlTagCreator(string tagName, string value)
        {
            return "<" + tagName + ">" + value + "</" + tagName + ">";
        }
    }
}
