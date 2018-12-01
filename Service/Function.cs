using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using His_Pos.Class;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.HisApi;
using His_Pos.Properties;
using His_Pos.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NChinese.Phonetic;
using His_Pos.Class.Person;

namespace His_Pos
{

    public class Function 
    {

        //用途:將中文字轉成注音符號再轉成英文字母
        //輸入:中文字串
        //輸出:英文字串
        public string ChangeNameToEnglish(string txtInput)
        {
            string resultOutput = string.Empty;
            // 取得一串中文字的注音字根
            var zhuyinProvicer = new ZhuyinReverseConversionProvider();
            string[] zhuyinArray = zhuyinProvicer.Convert(txtInput);
            foreach (var s in zhuyinArray) {
                if (s == string.Empty) return txtInput;
                resultOutput += GetEnumDescription("Alphabat", s.Substring(0, 1));
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
        
        public string GetDateFormat(string date) {
            if (date.Length == 1) date = "0" + date;
            return date;
        }

        public string ExportXml(XDocument xml, string FileTypeName) {
            var twc = new TaiwanCalendar();
            var year = twc.GetYear(DateTime.Now).ToString();
            var month = GetDateFormat(twc.GetMonth(DateTime.Now).ToString());
            var day = GetDateFormat(twc.GetDayOfMonth(DateTime.Now).ToString());
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += "\\藥健康\\"+FileTypeName;
            var path_ym = path + "\\" + year + month;
            var path_ymd = path + "\\" + year + month + "\\" + day;
            var path_file = path_ym + "\\" + day + "\\";
            if (FileTypeName.Equals("匯出申報XML檔案"))
                path_file += "DRUGT";
            else
                path_file +=  year + month + day;
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
            var psi = new Process();
            psi.StartInfo.FileName = "makecab.exe";
            psi.StartInfo.Arguments = path_file + ".xml " + path_file + ".zip";
            psi.Start();
            //psi.WaitForInputIdle();
            //設定要等待相關的處理序結束的時間 
            psi.WaitForExit();
            return path_file;
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
        public static string CheckEmptyInput(string input, string message)
        {
            return input == string.Empty ? message : string.Empty;
        }

        public string ToInvCulture(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public string SetStrFormat(double value, string format)
        {
            return string.Format(format, value);
        }

        public string SetStrFormatInt(int value, string format)
        {
            return string.Format(format, value);
        }

        public string XmlTagCreator(string tagName, string value)
        {
            return "<" + tagName + ">" + value + "</" + tagName + ">";
        }
        /*
         * 取得xml node資料
         */
        public string GetXmlNodeData(XmlDocument xml, string node)
        {
            return xml.SelectSingleNode(node)?.InnerText;
        }
        /*
         * 取得Procedure資料並以DataTable回傳
         */
        public DataTable GetDataFromProc(string procName, List<SqlParameter> param = null)
        {
            var conn = new DbConnection(Settings.Default.SQL_local);
            return conn.ExecuteProc(procName, param);
        }
        public string HttpGetJson(string url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            return responseReader.ReadToEnd();
                        }
                    }
                }
                return string.Empty;
            }
       
        }
        public class Holiday{
          public  DateTime date { get; set; }
          public  string name { get; set; }
          public string isHoliday { get; set; }
          public  string holidayCategory { get; set; }
          public string description { get; set; }
        }
        public void GetLastYearlyHoliday() { //撈每年國定假日
           var jsondata = HttpGetJson("http://data.ntpc.gov.tw/api/v1/rest/datastore/382000000A-000077-002");
            if (jsondata != string.Empty) {
                var year = DateTime.Now.Year;
                string data = JObject.Parse(jsondata)["result"]["records"].ToString();
                Collection<Holiday> tempCollection = JsonConvert.DeserializeObject<Collection<Holiday>>(data);
                Collection<Holiday> holidayCollection = new Collection<Holiday>(tempCollection.Where(x => x.date.Year == year).ToList());
                foreach (Holiday day in holidayCollection) {
                    if (day.name == "軍人節") continue;
                    if (day.isHoliday == "是")
                        FunctionDb.UpdateLastYearlyHoliday(day);
                }
            }
        }

        public string ByteArrayToString(int length,byte[] pBuffer,int startIndex)
        {
            var tmpByteArr = new byte[length];
            Array.Copy(pBuffer, startIndex, tmpByteArr, 0, length);
            var result = Encoding.GetEncoding(950).GetString(tmpByteArr);
            return result;
        }

        public void DailyUpload(XDocument dailyUpload)
        {
            var filePath = ExportXml(dailyUpload, "每日上傳");
            var fileName = filePath + ".xml";
            //var cs = new ConvertData();
            //var fileNameArr = cs.StringToBytes(fileName, fileName.Length);
            //var currentFile = Directory.GetFiles(filePath)[0];//每日上傳檔案
            //var fileInfo = new FileInfo(currentFile);
            //var fileSize = cs.StringToBytes(fileInfo.Length.ToString(), fileInfo.Length.ToString().Length);//檔案大小
            //var element = dailyUpload.Root.Element("REC");
            //var count = cs.StringToBytes(element.Elements().Count().ToString(), element.Elements().Count().ToString().Length);
            //var pBuffer = new byte[50];
            //var iBufferLength = 50;
            //HisApiBase.csUploadData(fileNameArr, fileSize, count, pBuffer, ref iBufferLength);
        }
      
    }
}
