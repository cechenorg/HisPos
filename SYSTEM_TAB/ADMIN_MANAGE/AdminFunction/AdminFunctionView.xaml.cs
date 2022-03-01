using His_Pos.FunctionWindow;
using His_Pos.NewClass;
using His_Pos.NewClass.Prescription;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace His_Pos.SYSTEM_TAB.ADMIN_MANAGE.AdminFunction
{
    /// <summary>
    /// AdminFunctionView.xaml 的互動邏輯
    /// </summary>
    public partial class AdminFunctionView : UserControl
    {
        public AdminFunctionView()
        {
            InitializeComponent();
        }

        private void ButtonPredictChronic_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionDb.PredictThreeMonthPrescription();
            MessageWindow.ShowMessage("預約慢箋完成!", MessageType.SUCCESS);
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

        private void CooperApi_Click(object sender, RoutedEventArgs e)
        {
            while (WebApi.SendToCooperClinicLoop100())
            {
            }
            MessageWindow.ShowMessage("骨科重拋成功", MessageType.SUCCESS);
        }

        private void MergeProduct_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}