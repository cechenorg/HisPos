using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.MedicineRefactoring;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.StoreOrder;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace His_Pos.NewClass.Prescription.IndexReserve
{
    public class IndexReserve : ObservableObject
    {
        public IndexReserve(DataRow r) {
            IndexReserveDetailCollection = new IndexReserveDetails();
            Id = r.Field<int>("Id");
            CusId = r.Field<int>("Cus_ID");
            CusName = r.Field<string>("Cus_Name");
            InsName = r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            TreatDate = r.Field<DateTime>("TreatmentDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            PhoneNote = r.Field<string>("Cus_UrgentNote");
            Profit = Convert.ToInt32(r.Field<double>("Profit"));
            IsExpensive = r.Field<bool>("IsExpensive"); 
            switch (r.Field<string>("MedPrepareStatus")) {
                case "N":
                    PrepareMedStatus = "未處理";
                    break;
                case "D":
                    PrepareMedStatus = "備藥";
                    IsSend = true;
                    break;
                case "F":
                    PrepareMedStatus = "不備藥";
                    break; 
            }
            switch (r.Field<string>("CallStatus"))
            {
                case "N":
                    PhoneCallStatusName = "未處理";
                    break;
                case "D":
                    PhoneCallStatusName = "已聯絡";
                    break;
                case "F":
                    PhoneCallStatusName = "電話未接";
                    break;
            } 
        }
        public int CusId { get; set; }
        public int Id { get; set; }
        public string StoOrdID { get; set; }
        public string CusName { get; set; }
        public string InsName { get; set; }
        public string DivName { get; set; }
        public int Profit { get; set; }
        public DateTime TreatDate { get; set; }
        public DateTime AdjustDate { get; set; }
        public string PhoneNote { get; set; }
        private string prepareMedType;
        public string PrepareMedType
        {
            get => prepareMedType;
            set
            {
                Set(() => PrepareMedType, ref prepareMedType, value);
            }
        }
        private bool isSend;
        public bool IsSend
        {
            get => isSend;
            set
            {
                Set(() => IsSend, ref isSend, value);
            }
        }

        private string phoneCallStatus;
        public string PhoneCallStatus {
            get => phoneCallStatus;
            set
            {
                Set(() => PhoneCallStatus, ref phoneCallStatus, value);
            }
        }
        private string phoneCallStatusName;
        public string PhoneCallStatusName { 
            get => phoneCallStatusName;
            set
            {
                Set(() => PhoneCallStatusName, ref phoneCallStatusName, value);
                switch (PhoneCallStatusName)
                {
                    case "未處理":
                        PhoneCallStatus = "N";
                        break;
                    case "已聯絡":
                        PhoneCallStatus = "D";
                        break;
                    case "電話未接":
                        PhoneCallStatus  = "F";
                        break;
                }
                
            }
        }
        private IndexReserveDetails indexReserveDetailCollection;
        public IndexReserveDetails IndexReserveDetailCollection
        {
            get => indexReserveDetailCollection;
            set
            {
                Set(() => IndexReserveDetailCollection, ref indexReserveDetailCollection, value);
            }
        }
         
        private string prepareMedStatus;
        public string PrepareMedStatus
        {
            get => prepareMedStatus;
            set
            {
                Set(() => PrepareMedStatus, ref prepareMedStatus, value);
            }
        }
        public bool IsExpensive { get; set; }
        public void SaveStatus() {
            IndexReserveDb.Save(Id, PhoneCallStatus, PrepareMedStatus,StoOrdID);
        }
        public void GetIndexDetail() {
            IndexReserveDetailCollection.GetDataById(Id);
        }
        public bool StoreOrderToSingde() {
            int count = StoreOrderDB.GetStoOrdMasterCountByDate().Rows[0].Field<int>("Count");
            bool result = false;
            string newStoOrdID = "P" + DateTime.Today.ToString("yyyyMMdd") + "-" + count.ToString().PadLeft(2, '0');
            this.StoOrdID = newStoOrdID;
            for (int j = 0; j < this.IndexReserveDetailCollection.Count; j++)
            {
                IndexReserveDetailCollection[j].StoOrdID = newStoOrdID;
            } 
            MainWindow.ServerConnection.OpenConnection();
            MainWindow.SingdeConnection.OpenConnection();
            StoreOrderDB.InsertIndexReserveOrder(this);

            if (StoreOrderDB.SendStoreOrderToSingde(this).Rows[0][0].ToString() == "SUCCESS")
            {
                StoreOrderDB.StoreOrderToWaiting(StoOrdID);
                IsSend = true;
                SaveStatus();
                result = true;
            }
            else  
                MessageWindow.ShowMessage(StoOrdID + "傳送失敗", Class.MessageType.ERROR); 

            MainWindow.ServerConnection.CloseConnection();
            MainWindow.SingdeConnection.CloseConnection();
            return result;
        }
        public void SetReserveMedicinesSheetReportViewer(ReportViewer rptViewer) { 
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new ReserveMedicines(IndexReserveDetailCollection);
            var json = JsonConvert.SerializeObject(medBagMedicines);
            var dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            rptViewer.LocalReport.ReportPath = @"RDLC\ReserveSheet.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var parameters = CreateReserveMedicinesSheetParameters();
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.DataSources.Clear();
            var rd = new ReportDataSource("ReserveMedicinesDataSet", dataTable);
            rptViewer.LocalReport.DataSources.Add(rd);
            rptViewer.LocalReport.Refresh();
        }

        private IEnumerable<ReportParameter> CreateReserveMedicinesSheetParameters() {
            return new List<ReportParameter>
            {
                new ReportParameter("PatientName",CusName),
                new ReportParameter("PatientBirthday","105/02/28"),
                new ReportParameter("PatientTel","105/02/28"),
                new ReportParameter("Institution", InsName),
                new ReportParameter("Division", DivName)
            };
        }

    }
}
