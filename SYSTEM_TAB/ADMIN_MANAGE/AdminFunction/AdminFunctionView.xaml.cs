using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.IcData.Upload;

namespace His_Pos.SYSTEM_TAB.ADMIN_MANAGE.AdminFunction {
    /// <summary>
    /// AdminFunctionView.xaml 的互動邏輯
    /// </summary>
    public partial class AdminFunctionView : UserControl {
        public AdminFunctionView() {
            InitializeComponent();
        }

        private void ButtonPredictChronic_Click(object sender, RoutedEventArgs e) {
            Prescriptions.PredictThreeMonthPrescription();
            MessageWindow.ShowMessage("預約慢箋完成!",MessageType.SUCCESS);
            
        }

        private void DayliUpload_Click(object sender, RoutedEventArgs e)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += "\\Declare\\dailyUpload";
            var path_ym = path + "\\" + (DateTime.Today.Year - 1911) + DateTime.Today.Month.ToString().PadLeft(2, '0');
            var path_file = path_ym + "\\" + DateTime.Today.Date.Day + "\\" + (DateTime.Today.Year - 1911) + DateTime.Today.Month.ToString().PadLeft(2, '0') + DateTime.Today.Date.Day;
            var filePath = path_file;
            var fileName = filePath + ".xml";
            var fileNameArr = ConvertData.StringToBytes(fileName, fileName.Length);
            var fileInfo = new FileInfo(fileName);//每日上傳檔案
            var fileSize = ConvertData.StringToBytes(fileInfo.Length.ToString(), fileInfo.Length.ToString().Length);//檔案大小
            //XDocument xdoc = XDocument.Load(fileName);
            //Recs recs = Deserialize<Recs>(xdoc);
            //foreach (var r in recs.Rec)
            //{
            //    r.MainMessage.IcMessage.TreatmentDateTime =
            //        r.MainMessage.MedicalMessageList[0].MedicalOrderTreatDateTime;
            //    if(string.IsNullOrEmpty(r.MainMessage.IcMessage.MedicalNumber))
            //        r.MainMessage.IcMessage.MedicalNumber = "1";
            //    if (string.IsNullOrEmpty(r.MainMessage.IcMessage.TreatmentDateTime))
            //        Console.WriteLine(r.MainMessage.IcMessage.CardNo);
            //}
            //xdoc = recs.SerializeObjectToXDocument();
            //filePath = Function.ExportXml(xdoc, "dailyUpload");
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNodeList xnList = doc.SelectNodes("/RECS/REC");
            var count = ConvertData.StringToBytes(xnList.Count.ToString(), xnList.Count.ToString().Length);
            var pBuffer = new byte[50];
            var iBufferLength = 50;
            if (HisApiFunction.OpenCom())
            {
                var res = HisApiBase.csUploadData(fileNameArr, fileSize, count, pBuffer, ref iBufferLength);
                if (res == 0)
                    MessageWindow.ShowMessage("上傳成功", MessageType.SUCCESS);
            }
            HisApiFunction.CloseCom();
        }
        public static T Deserialize<T>(XDocument doc)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (var reader = doc.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }
    }
}
