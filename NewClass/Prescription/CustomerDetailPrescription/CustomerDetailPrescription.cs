using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.CustomerDetailPrescription
{
    public class CustomerDetailPrescription : ObservableObject
    {
        public CustomerDetailPrescription()
        {
        }

        public CustomerDetailPrescription(DataRow r)
        {
            TypeName = r.Field<string>("AdjustCaseType");
            ID = r.Field<int>("ID");
            InsName = r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            TreatMentDate = r.Field<DateTime>("TreatmentDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            ChronicStatus = r.Field<string>("ChronicStatus");
        }

        public string TypeName { get; set; }
        public int ID { get; set; }
        public string InsName { get; set; }
        public string DivName { get; set; }
        public DateTime TreatMentDate { get; set; }
        public DateTime AdjustDate { get; set; }
        public string ChronicStatus { get; set; }
    }
}