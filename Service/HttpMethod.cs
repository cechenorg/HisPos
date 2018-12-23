using His_Pos.Class;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Service
{
    public class HttpMethod
    {
        public HttpMethod() { }

        public List<XmlDocument> Get(string url,Dictionary<string,string> parameterList) {
            
            string paramString = "?";
            int count = 0;
            foreach (var par in parameterList)
            {
                paramString += par.Key + "=" + par.Value;
                count++;
                if (count < parameterList.Count)
                    paramString += "&";
            } 
            HttpWebRequest request = HttpWebRequest.Create(url + paramString) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded"; 
            List<XmlDocument> xmlDocuments = new List<XmlDocument>();
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string[] split = sr.ReadToEnd().Split(new[] { @"<?xml version=""1.0"" encoding=""utf-16""?>" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string row in split) {
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(row);
                            xmlDocuments.Add(xml);
                        }
                    }
                }
            }
            catch (Exception ex) {
                switch (ex.Message) {
                    case "遠端伺服器傳回一個錯誤: (500) 內部伺服器錯誤。":
                        MessageWindow messageWindow = new MessageWindow("Http Get 讀取\r\n" + url + "異常", MessageType.ERROR);
                        messageWindow.ShowDialog();
                        break;
                    default:
                        MessageWindow errormessageWindow = new MessageWindow(ex.Message,MessageType.ERROR);
                        errormessageWindow.ShowDialog();
                        break; 
                } 
            } 
            return xmlDocuments;
        }
        public List<XmlDocument> Post(string url, Dictionary<string, string> parameterList) {
            ServicePointManager.Expect100Continue = false;
            string paramString = string.Empty;
            int count = 0;
            foreach (var par in parameterList)
            {
                paramString += par.Key + "=" + par.Value;
                count++;
                if (count < parameterList.Count)
                    paramString += "&";
            }
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest; 
            request.Method = "POST";    // 方法
            request.KeepAlive = true; //是否保持連線
            request.ContentType = "application/x-www-form-urlencoded";
             
            byte[] bs = Encoding.ASCII.GetBytes(paramString);

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            string requeststring = string.Empty;
            List<XmlDocument> xmlDocuments = new List<XmlDocument>();
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string[] split = sr.ReadToEnd().Split(new[] { @"<?xml version=""1.0"" encoding=""utf-16""?>" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string row in split)
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(row);
                        xmlDocuments.Add(xml);
                    }
                    sr.Close();
                }
            }
            catch (Exception ex) {
                switch (ex.Message)
                {
                    case "遠端伺服器傳回一個錯誤: (500) 內部伺服器錯誤。":
                        MessageWindow messageWindow = new MessageWindow("Http Post 讀取\r\n" + url + "異常", MessageType.ERROR);
                        messageWindow.ShowDialog();
                        break;
                    default:
                        MessageWindow errormessageWindow = new MessageWindow(ex.Message, MessageType.ERROR);
                        errormessageWindow.ShowDialog();
                        break;
                }
            }
            return xmlDocuments;
        }
        
        
    }
}
