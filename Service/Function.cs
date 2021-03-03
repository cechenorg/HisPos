using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Prescription.ICCard.Upload;
using ICSharpCode.SharpZipLib.Zip;
using NChinese.Phonetic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace His_Pos.Service
{
    public class Function
    {
        //用途:將中文字轉成注音符號再轉成英文字母
        //輸入:中文字串
        //輸出:英文字串
        public static string ChangeNameToEnglish(string txtInput)
        {
            string resultOutput = string.Empty;
            // 取得一串中文字的注音字根
            var zhuyinProvicer = new ZhuyinReverseConversionProvider();
            string[] zhuyinArray = zhuyinProvicer.Convert(txtInput);
            foreach (var s in zhuyinArray)
            {
                if (s == string.Empty) return txtInput;
                resultOutput += GetEnumDescription("Alphabat", s.Substring(0, 1));
            }
            return resultOutput;
        }//ChangeNameToEnglish

        //用途:取得Enum Description value
        //輸入:Enum值
        //輸出:Description值
        public static string GetEnumDescription(string type, string value)
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

        public static string GetDateFormat(string date)
        {
            if (date.Length == 1) date = "0" + date;
            return date;
        }

        public static string ExportXml(XDocument xml, string FileTypeName, DateTime? declareDate, string fileName = null)
        {
            var twc = new TaiwanCalendar();
            var year = twc.GetYear(DateTime.Now).ToString();
            var month = DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day.ToString().PadLeft(2, '0');
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path_ymd;
            string path_file;
            if (FileTypeName.Equals("每月申報檔"))
            {
                path += "\\藥健康系統申報";
                path_ymd = path + "\\" + ((DateTime)declareDate).Month + "月申報檔_" +
                           (DateTime.Now.Year - 1911) + "年" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "月" +
                           DateTime.Now.Day.ToString().PadLeft(2, '0') + "日" +
                           DateTime.Now.Hour.ToString().PadLeft(2, '0') + "點" +
                           DateTime.Now.Minute.ToString().PadLeft(2, '0') + "分轉出";
                path_file = path_ymd + "\\";
                path_file += "DRUGT";
            }
            else
            {
                path = "C:\\Program Files\\HISPOS\\DailyUpload";
                path_ymd = path + "\\" + year + month.PadLeft(2, '0') + day;
                path_file = path_ymd + "\\" + year + month + day + +DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            }
            if (!Directory.Exists(path_ymd)) Directory.CreateDirectory(path_ymd);
            xml.Declaration = new XDeclaration("1.0", "Big5", string.Empty);
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.GetEncoding(950);
            settings.OmitXmlDeclaration = false;
            var writer = XmlWriter.Create(path_file + ".xml", settings);
            xml.Save(writer);
            writer.Close();
            var info = File.ReadAllText(path_file + ".xml", Encoding.GetEncoding(950));
            File.WriteAllText(path_file + ".xml", info.Replace("big5", "Big5"), Encoding.GetEncoding(950));
            var input = path_file + ".xml";
            var output = (string.IsNullOrEmpty(fileName) ? path_file : path_ymd + "\\" + fileName) + ".zip";
            //壓縮XML
            if (File.Exists(output)) File.Delete(output);
            List<string> inputList = new List<string>();
            inputList.Add(input);
            ZipFiles(inputList, output);
            if (FileTypeName.Equals("每月申報檔"))
                File.Delete(input);
            return path_file;
        }

        public string HttpGetJson(string url)
        {
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

        public class Holiday
        {
            public DateTime date { get; set; }
            public string name { get; set; }
            public string isHoliday { get; set; }
            public string holidayCategory { get; set; }
            public string description { get; set; }
        }

        public void GetLastYearlyHoliday()
        { //撈每年國定假日
            var jsondata = HttpGetJson("http://data.ntpc.gov.tw/api/v1/rest/datastore/382000000A-000077-002");
            if (jsondata != string.Empty)
            {
                var year = DateTime.Now.Year;
                string data = JObject.Parse(jsondata)["result"]["records"].ToString();
                Collection<Holiday> tempCollection = JsonConvert.DeserializeObject<Collection<Holiday>>(data);
                Collection<Holiday> holidayCollection = new Collection<Holiday>(tempCollection.Where(x => x.date.Year == year).ToList());
                foreach (Holiday day in holidayCollection)
                {
                    if (day.name == "軍人節") continue;
                    // if (day.isHoliday == "是");
                    // FunctionDb.UpdateLastYearlyHoliday(day);
                }
            }
        }

        public static string ByteArrayToString(int length, byte[] pBuffer, int startIndex)
        {
            var tmpByteArr = new byte[length];
            Array.Copy(pBuffer, startIndex, tmpByteArr, 0, length);
            var result = Encoding.GetEncoding(950).GetString(tmpByteArr);
            return result;
        }

        public void DailyUpload(XDocument dailyUpload, string recCount)
        {
            try
            {
                var filePath = ExportXml(dailyUpload, "DailyUpload", null);
                var fileName = filePath + ".xml";
                var fileNameArr = ConvertData.StringToBytes(fileName, fileName.Length);
                var fileInfo = new FileInfo(fileName);//每日上傳檔案
                var fileSize = ConvertData.StringToBytes(fileInfo.Length.ToString(), fileInfo.Length.ToString().Length);//檔案大小
                var count = ConvertData.StringToBytes(recCount, recCount.Length);
                var pBuffer = new byte[50];
                var iBufferLength = 50;
                if (HisApiFunction.OpenCom() && ViewModelMainWindow.IsVerifySamDc)
                {
                    var res = HisApiBase.csUploadData(fileNameArr, fileSize, count, pBuffer, ref iBufferLength);
                    if (res == 0)
                    {
                        var samCode = ConvertData.ByToString(pBuffer, 0, 12);
                        var insID = ConvertData.ByToString(pBuffer, 12, 10);
                        var uploadDateTime = ConvertData.ByToString(pBuffer, 22, 14);
                        var uploadYear = uploadDateTime.Substring(0, 4);
                        var uploadMonth = uploadDateTime.Substring(4, 2);
                        var uploadDay = uploadDateTime.Substring(6, 2);
                        var uploadHour = uploadDateTime.Substring(8, 2);
                        var uploadMinute = uploadDateTime.Substring(10, 2);
                        var uploadSecond = uploadDateTime.Substring(12, 2);
                        var uploadDateStr = $"{uploadYear}/{uploadMonth}/{uploadDay} {uploadHour}:{uploadMinute}:{uploadSecond}";
                        var receiveDateTime = ConvertData.ByToString(pBuffer, 36, 14);
                        var receiveYear = receiveDateTime.Substring(0, 4);
                        var receiveMonth = receiveDateTime.Substring(4, 2);
                        var receiveDay = receiveDateTime.Substring(6, 2);
                        var receiveHour = receiveDateTime.Substring(8, 2);
                        var receiveMinute = receiveDateTime.Substring(10, 2);
                        var receiveSecond = receiveDateTime.Substring(12, 2);
                        var receiveDateStr = $"{receiveYear}/{receiveMonth}/{receiveDay} {receiveHour}:{receiveMinute}:{receiveSecond}";
                        var uploadTime = new DateTime(int.Parse(uploadYear), int.Parse(uploadMonth), int.Parse(uploadDay), int.Parse(uploadHour), int.Parse(uploadMinute), int.Parse(uploadSecond));
                        var receiveTime = new DateTime(int.Parse(receiveYear), int.Parse(receiveMonth), int.Parse(receiveDay), int.Parse(receiveHour), int.Parse(receiveMinute), int.Parse(receiveSecond));
                        MessageWindow.ShowMessage("上傳成功\n上傳時間:" + uploadDateStr + "\n接收時間:" + receiveDateStr, MessageType.SUCCESS);
                        MainWindow.ServerConnection.OpenConnection();
                        IcDataUploadDb.InsertDailyUploadFile(dailyUpload);
                        IcDataUploadDb.UpdateDailyUploadData(samCode, insID, uploadTime, receiveTime);
                        MainWindow.ServerConnection.CloseConnection();
                    }
                    else
                    {
                        MessageWindow.ShowMessage("上傳異常，請稍後再試。", MessageType.ERROR);
                    }
                }
                HisApiFunction.CloseCom();
            }
            catch (Exception)
            {
                MessageWindow.ShowMessage("上傳異常，請稍後再試。", MessageType.ERROR);
            }
        }

        public void DailyUploadWithoutMessage(XDocument dailyUpload, string recCount)
        {
            try
            {
                var filePath = ExportXml(dailyUpload, "DailyUpload", null);
                var fileName = filePath + ".xml";
                var fileNameArr = ConvertData.StringToBytes(fileName, fileName.Length);
                var fileInfo = new FileInfo(fileName);//每日上傳檔案
                var fileSize = ConvertData.StringToBytes(fileInfo.Length.ToString(), fileInfo.Length.ToString().Length);//檔案大小
                var count = ConvertData.StringToBytes(recCount, recCount.Length);
                var pBuffer = new byte[50];
                var iBufferLength = 50;
                if (HisApiFunction.OpenCom() && ViewModelMainWindow.IsVerifySamDc)
                {
                    var res = HisApiBase.csUploadData(fileNameArr, fileSize, count, pBuffer, ref iBufferLength);
                    if (res == 0)
                    {
                        var samCode = ConvertData.ByToString(pBuffer, 0, 12);
                        var insID = ConvertData.ByToString(pBuffer, 12, 10);
                        var uploadDateTime = ConvertData.ByToString(pBuffer, 22, 14);
                        var uploadYear = uploadDateTime.Substring(0, 4);
                        var uploadMonth = uploadDateTime.Substring(4, 2);
                        var uploadDay = uploadDateTime.Substring(6, 2);
                        var uploadHour = uploadDateTime.Substring(8, 2);
                        var uploadMinute = uploadDateTime.Substring(10, 2);
                        var uploadSecond = uploadDateTime.Substring(12, 2);
                        var receiveDateTime = ConvertData.ByToString(pBuffer, 36, 14);
                        var receiveYear = receiveDateTime.Substring(0, 4);
                        var receiveMonth = receiveDateTime.Substring(4, 2);
                        var receiveDay = receiveDateTime.Substring(6, 2);
                        var receiveHour = receiveDateTime.Substring(8, 2);
                        var receiveMinute = receiveDateTime.Substring(10, 2);
                        var receiveSecond = receiveDateTime.Substring(12, 2);
                        var uploadTime = new DateTime(int.Parse(uploadYear), int.Parse(uploadMonth), int.Parse(uploadDay), int.Parse(uploadHour), int.Parse(uploadMinute), int.Parse(uploadSecond));
                        var receiveTime = new DateTime(int.Parse(receiveYear), int.Parse(receiveMonth), int.Parse(receiveDay), int.Parse(receiveHour), int.Parse(receiveMinute), int.Parse(receiveSecond));
                        MainWindow.ServerConnection.OpenConnection();
                        IcDataUploadDb.InsertDailyUploadFile(dailyUpload);
                        IcDataUploadDb.UpdateDailyUploadData(samCode, insID, uploadTime, receiveTime);
                        MainWindow.ServerConnection.CloseConnection();
                    }
                }
                HisApiFunction.CloseCom();
            }
            catch (Exception)
            {
            }
        }

        public static void ZipFiles(List<string> SourceFiles, string TargetFile)
        {
            var zs = new ZipOutputStream(File.Create(TargetFile));
            zs.SetLevel(9); //壓縮比

            for (int i = 0; i < SourceFiles.Count; i++)
            {
                FileStream s = File.OpenRead(SourceFiles[i]);
                byte[] buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);
                ZipEntry Entry = new ZipEntry(Path.GetFileName(SourceFiles[i]));
                Entry.DateTime = DateTime.Now;
                Entry.Size = s.Length;
                s.Close();
                zs.PutNextEntry(Entry);
                zs.Write(buffer, 0, buffer.Length);
            }
            zs.Finish();
            zs.Close();
        }
    }
}