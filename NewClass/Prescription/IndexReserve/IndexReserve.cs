using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
            Id = r.Field<int>("Id");
            CusId = r.Field<int>("Cus_ID");
            CusName = r.Field<string>("Cus_Name");
            InsName = r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            TreatDate = r.Field<DateTime>("TreatmentDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            PhoneNote = r.Field<string>("Cus_UrgentNote"); 
            switch (r.Field<string>("MedPrepareStatus")) {
                case "N":
                    PrepareStatusName = "未處理";
                    break;
                case "D":
                    PrepareStatusName = "已備藥";
                    break;
                case "F":
                    PrepareStatusName = "不備藥";
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
        public string CusName { get; set; }
        public string InsName { get; set; }
        public string DivName { get; set; }
        public DateTime TreatDate { get; set; }
        public DateTime AdjustDate { get; set; }
        public string PhoneNote { get; set; }
        private string prepareStatus;
        public string PrepareStatus {
            get => prepareStatus;
            set
            {
                Set(() => PrepareStatus, ref prepareStatus, value);
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
        private string prepareStatusName;
        public string PrepareStatusName
        { 
            get => prepareStatusName;
            set
            {
                prepareStatusName = value;
                switch (PrepareStatusName)
                {
                    case "未處理":
                        PrepareStatus = "N";
                        break;
                    case "已備藥":
                        PrepareStatus = "D";
                        break;
                    case "不備藥":
                        PrepareStatus = "F";
                        break;
                }
                Set(() => PrepareStatusName, ref prepareStatusName, value);
            }
        }
        public void SaveStatus() {
            IndexReserveDb.Save(Id, PhoneCallStatus, PrepareStatus);
        }
    }
}
