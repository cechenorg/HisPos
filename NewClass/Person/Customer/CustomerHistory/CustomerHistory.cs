using System;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.CustomerHistoryProduct;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistory:ObservableObject
    {
        public CustomerHistory() { }
        public CustomerHistory(DataRow r)
        {
            switch (r.Field<string>("Type")) {
                case "Adjust":
                    Type = HistoryType.AdjustRecord;
                    break;
                case "Register":
                    Type = HistoryType.RegisterRecord;
                    break;
                case "Reserve":
                    Type = HistoryType.ReservedPrescription;
                    break; 
            }
            
            AdjustDate = r.Field<DateTime>("AdjustDate");
            SourceId = r.Field<int>("SourceId");
            InsName = r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            if (InsName.Length > 8)
                Title = InsName.Substring(0, 8) + " " + DivName;
            else
                Title = InsName + " " + DivName;
            Status = r.Field<bool>("Status");
        } 
        public HistoryType Type { get; }
        public string InsName { get; }
        public string DivName { get; }
        public int SourceId { get; }
        public DateTime AdjustDate { get; } //日期
        public string Title { get; }//標題
        public bool Status { get; }//已調劑處方:是否未過卡 已登錄處方:是否傳送藥健康 預約:無
        public CustomerHistoryProducts Products { get; set; }

        public Prescription.Prescription GetPrescriptionByID()
        {
            return new Prescription.Prescription(PrescriptionDb.GetPrescriptionByID(SourceId).Rows[0], PrescriptionSource.Normal);
        }

        public Prescription.Prescription GetReservePrescriptionByID()
        {
            return new Prescription.Prescription(PrescriptionDb.GetReservePrescriptionByID(SourceId).Rows[0], PrescriptionSource.ChronicReserve);
        }
    }
}
