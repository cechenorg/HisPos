using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.ControlMedicineDetail
{
    public class ControlMedicineDetails : ObservableCollection<ControlMedicineDetail>
    {
        public ControlMedicineDetails()
        {
        }

        public void GetDataById(string medId, DateTime sDate, DateTime eDate, double stock, string warID)
        {
            double tempstock = stock;
            Clear();
            ControlMedicineDetail rowOne = new ControlMedicineDetail();
            rowOne.Date = sDate;
            rowOne.FinalStock = tempstock;
            rowOne.TypeName = "上次結存";
            rowOne.MedID = medId;
            Add(rowOne);
            DataTable table = ControlMedicineDetailDb.GetDataById(medId, sDate, eDate, warID);
            foreach (DataRow r in table.Rows)
            {
                ControlMedicineDetail controlMedicineDetail = new ControlMedicineDetail(r, tempstock, medId);
                tempstock = controlMedicineDetail.FinalStock;
                Add(controlMedicineDetail);
            }
        }

        public static ControlMedicineDetails GetDeclareData(DateTime sDate, DateTime eDate, List<string> warIDs)
        {
            ControlMedicineDetails controlMedicineDetails = new ControlMedicineDetails();
            DataTable table = ControlMedicineDetailDb.GetDeclareData(sDate, eDate, warIDs);
            foreach (DataRow r in table.Rows)
            {
                controlMedicineDetails.Add(new ControlMedicineDetail(r));
            }
            return controlMedicineDetails;
        }
    }
}