using His_Pos.NewClass.Cooperative.CooperativeInstitution;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.NewClass.Prescription.CustomerPrescriptions
{
    public class CusPrePreviewBases : ObservableCollection<CusPrePreviewBase>
    {
        public CusPrePreviewBases()
        {
        }

        private void GetOrthopedics(DateTime sDate, DateTime eDate)
        {
            var table = PrescriptionDb.GetOrthopedicsPrescriptions(sDate, eDate);
            foreach (var xmlDocument in table)
            {
                Add(new OrthopedicsPreview(XmlService.Deserialize<OrthopedicsPrescription>(xmlDocument.InnerXml)));
            }
        }

        public void GetCooperative(DateTime sDate, DateTime eDate)
        {
            NewFunction.GetXmlFiles();
            GetOrthopedics(sDate, eDate);
            var table = PrescriptionDb.GetXmlOfPrescriptionsByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                var xDocument = new XmlDocument();
                xDocument.LoadXml(r["CooCli_XML"].ToString());
                Add(new CooperativePreview(XmlService.Deserialize<CooperativePrescription.Prescription>(xDocument.InnerXml),
                    r.Field<DateTime>("CooCli_InsertTime"), r.Field<int>("CooCli_ID").ToString(), 
                    r.Field<bool>("CooCli_IsRead"), r.Field<bool>("CooCli_IsPrint")));
            }
        }
        
        private void GetOrthopedicsByCustomerIDNumber(string idNumber)
        {
            var table = PrescriptionDb.GetOrthopedicsPrescriptionsByCusIdNumber(idNumber);
            foreach (var xmlDocument in table)
            {
                Add(new OrthopedicsPreview(XmlService.Deserialize<OrthopedicsPrescription>(xmlDocument.InnerXml)));
            }
        }

        public void GetCooperativeByCusIDNumber(string idNumber) //取得合作XML格式處方
        {
            if (string.IsNullOrEmpty(idNumber))
            {

                List<string> messagesList = new List<string>() { "顧客資料缺少身分證", "因此無法查詢合作診所處方", "請於下方顧客資料欄位更新顧客身份證" };

                MessageWindow.ShowMessage(messagesList, MessageType.ERROR);
                return;
            }
               
            Clear();
            var table = PrescriptionDb.GetXmlOfPrescriptionsByCusIDNumber(idNumber);
            foreach (DataRow r in table.Rows)
            {
                var xDocument = new XmlDocument();
                xDocument.LoadXml(r["CooCli_XML"].ToString());
                Add(new CooperativePreview(XmlService.Deserialize<CooperativePrescription.Prescription>(xDocument.InnerXml)
                    , r.Field<DateTime>("CooCli_InsertTime"), r.Field<int>("CooCli_ID").ToString(), r.Field<bool>("CooCli_IsRead")));
            }
            GetOrthopedicsByCustomerIDNumber(idNumber);
        }

        public void GetRegisterByCusId(int cusID) //取得登錄慢箋
        {
            Clear();
            var table = PrescriptionDb.GetRegisterPrescriptionByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new ChronicPreview(r, PrescriptionType.ChronicRegister));
            }
        }

        public void GetReserveByCusId(int cusID) //取得預約慢箋
        {
            Clear();
            var table = PrescriptionDb.GetReservePrescriptionByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new ChronicPreview(r, PrescriptionType.ChronicReserve));
            }
        }

        public void GetNoCardByCusId(int cusID) //取得未過卡處方
        {
            Clear();
            var table = PrescriptionDb.GetPrescriptionsNoGetCardByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                if (DateTimeExtensions.CountTimeDifferenceWithoutHoliday(DateTime.Today, r.Field<DateTime>("Adj_Date")) <= 10)
                    Add(new NoCardPreview(r));
            }
        }
    }
}