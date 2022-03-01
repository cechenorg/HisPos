using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Medicine.ControlMedicineDetail
{
    public class ControlMedicineDetail : ObservableObject
    {
        public ControlMedicineDetail()
        {
        }

        public ControlMedicineDetail(DataRow r)
        {
            MedID = r.Field<string>("Pro_ID");
            BatchNumber = r.Field<string>("batch");
            PackageName = r.Field<string>("PackageName");
            Date = r.Field<DateTime>("Date");
            TypeName = r.Field<string>("Type");
            InputAmount = r.Field<double>("Amount");
            OutputAmount = r.Field<double>("Amount");
            ManufactoryName = r.Field<string>("ManName");
            ManufactoryControlMedicinesID = r.Field<string>("ManID");
        }

        public ControlMedicineDetail(DataRow r, double stock, string medID)
        {
            MedID = medID;
            Date = r.Field<DateTime>("Date");
            TypeName = r.Field<string>("InvRec_Type");
            InputAmount = r.Field<double>("InputAmount");
            OutputAmount = r.Field<double>("OutputAmount");
            BatchNumber = r.Field<string>("InvRec_BatchNumber");
            Description = r.Field<string>("Description");
            ManufactoryName = r.Field<string>("Man_Name");
            ManufactoryControlMedicinesID = r.Field<string>("Man_ControlMedicineID");
            if (TypeName != "調劑(未過卡)")
                FinalStock = stock + InputAmount + OutputAmount;
            else
                FinalStock = stock;
        }

        public DateTime Date { get; set; }
        public string MedID { get; set; }
        public string TypeName { get; set; }
        public double InputAmount { get; set; }
        public double OutputAmount { get; set; }
        public string BatchNumber { get; set; }
        public string PackageName { get; set; }
        public double FinalStock { get; set; }
        public string Description { get; set; }
        public string ManufactoryName { get; set; }
        public string ManufactoryControlMedicinesID { get; set; }
    }
}