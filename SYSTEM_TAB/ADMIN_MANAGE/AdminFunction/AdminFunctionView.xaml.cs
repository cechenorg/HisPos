using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.IcData.Upload;
using His_Pos.NewClass.Prescription.Search;
using His_Pos.Service;
using Customer = His_Pos.Class.Person.Customer;
using Prescription = His_Pos.NewClass.CooperativeInstitution.Prescription;

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
            //    foreach (var m in r.MainMessage.MedicalMessageList)
            //    {
            //        if (!m.MedicalOrderTreatDateTime.Equals(r.MainMessage.IcMessage.TreatmentDateTime))
            //            m.MedicalOrderTreatDateTime = r.MainMessage.IcMessage.TreatmentDateTime;
            //    }
            //    if(string.IsNullOrEmpty(r.MainMessage.IcMessage.MedicalNumber))
            //        r.MainMessage.IcMessage.MedicalNumber = "1";
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
                {
                    MessageWindow.ShowMessage("上傳成功", MessageType.SUCCESS);
                    MainWindow.ServerConnection.OpenConnection();
                    MainWindow.ServerConnection.CloseConnection();
                    IcDataUploadDb.UpdateDailyUploadData();
                }
                else
                {
                    MessageWindow.ShowMessage("上傳異常，請稍後再試，", MessageType.ERROR);
                }
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

        private void ChangeCus_Click(object sender, RoutedEventArgs e)
        {
            //var prescriptionsPreviews = new PrescriptionSearchPreviews();
            //var prescriptions = new Prescriptions();
            //DataTable table = PrescriptionDb.GetSearchPrescriptionsData(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            //    DateTime.Today, null, null, null, null, null, null, null);
            //foreach (DataRow r in table.Rows)
            //{
            //    prescriptionsPreviews.Add(new PrescriptionSearchPreview(r,PrescriptionSource.Normal));
            //}

            //foreach (var p in prescriptionsPreviews)
            //{
            //    var pre = new NewClass.Prescription.Prescription(PrescriptionDb.GetPrescriptionByID(p.ID).Rows[0],
            //        PrescriptionSource.Normal);
            //    MainWindow.ServerConnection.OpenConnection();
            //    pre.Patient = pre.Patient.GetCustomerByCusId(pre.Patient.ID);
            //    pre.AdjustMedicinesType();
            //    MainWindow.ServerConnection.CloseConnection();
            //    if (pre.Treatment.Division != null)
            //        pre.Treatment.Division = ViewModelMainWindow.GetDivision(pre.Treatment.Division?.ID);
            //    pre.Treatment.Pharmacist =
            //        ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(pr => pr.IdNumber.Equals(pre.Treatment.Pharmacist.IdNumber));
            //    pre.Treatment.AdjustCase = ViewModelMainWindow.GetAdjustCase(pre.Treatment.AdjustCase.ID);
            //    pre.Treatment.Copayment = ViewModelMainWindow.GetCopayment(pre.Treatment.Copayment?.Id);
            //    if (pre.Treatment.PrescriptionCase != null)
            //        pre.Treatment.PrescriptionCase = ViewModelMainWindow.GetPrescriptionCases(pre.Treatment.PrescriptionCase?.ID);
            //    if (pre.Treatment.SpecialTreat != null)
            //        pre.Treatment.SpecialTreat = ViewModelMainWindow.GetSpecialTreat(pre.Treatment.SpecialTreat?.ID);
            //    pre.PrescriptionPoint.GetDeposit(pre.Id);
            //    pre.CheckIsCooperative();
            //    if(pre.PrescriptionStatus.IsDeclare)
            //        prescriptions.Add(pre);
            //}
        }
    }
}
