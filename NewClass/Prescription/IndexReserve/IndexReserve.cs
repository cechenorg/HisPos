using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
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
            IndexReserveDb.Save(Id, PhoneCallStatus, PrepareMedStatus);
        }
        public void GetIndexDetail() {
            IndexReserveDetailCollection.GetDataById(Id);
        }
    }
}
