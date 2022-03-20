using GalaSoft.MvvmLight;
using His_Pos.NewClass;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.CustomerHistoryProduct;
using System;
using System.Data;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistory : ObservableObject
    {
        public CustomerHistory()
        {
        }

        public CustomerHistory(DataRow r)
        {
            switch (r.Field<string>("Type"))
            {
                case "Adjust":
                    Type = HistoryType.AdjustRecord;
                    break;
                    //case "Register":
                    //    Type = HistoryType.RegisterRecord;
                    //    CopyOnly = false;
                    //    break;
                    //case "Reserve":
                    //    Type = HistoryType.ReservedPrescription;
                    //    CopyOnly = false;
                    //    break;
            }
            TreatDate = r.Field<DateTime?>("TreatmentDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            SourceId = r.Field<int>("SourceId");
            InsName = r.Field<string>("Ins_Name").Length > 8 ? r.Field<string>("Ins_Name").Substring(0, 8) : r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            ChronicTotal = r.Field<byte?>("ChronicTotal");
            ChronicSeq = r.Field<byte?>("ChronicSequence");
            Status = r.Field<bool>("Status");
            switch (r.Field<string>("AdjustCaseID"))
            {
                case "0":
                    Case = "自費";
                    break;

                case "1":
                case "3":
                    Case = "一般";
                    break;

                case "2":
                    if (ChronicTotal is null)
                        Case = "慢箋";
                    else
                        Case = ChronicTotal + "-" + ChronicSeq;
                    break;
            }
        }

        public HistoryType Type { get; }
        public string InsName { get; }
        public string DivName { get; }
        public string Case { get; }
        public int SourceId { get; }
        public int? ChronicSeq { get; }
        public int? ChronicTotal { get; }
        public DateTime? TreatDate { get; } //日期
        public DateTime AdjustDate { get; } //日期
        public bool Status { get; }//已調劑處方:是否未過卡
        public CustomerHistoryProducts Products { get; set; }

        public Prescription.Prescription GetPrescriptionRefactoringByID()
        {
            return new Prescription.Prescription(PrescriptionDb.GetPrescriptionByID(SourceId).Rows[0], PrescriptionType.Normal);
        }
    }
}