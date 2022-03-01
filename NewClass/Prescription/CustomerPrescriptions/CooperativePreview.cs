using His_Pos.Database;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Medicine;
using His_Pos.NewClass.Medicine.PreviewMedicine;
using His_Pos.NewClass.Prescription.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace His_Pos.NewClass.Prescription.CustomerPrescriptions
{
    public class CooperativePreview : CusPrePreviewBase
    {
        public CooperativePreview(CooperativePrescription.Prescription c, DateTime treatDate, string sourceId, bool isRead) : base(c, treatDate, isRead)
        {
            Content = c;
            SourceID = sourceId;
        }
        public CooperativePreview(CooperativePrescription.Prescription c, DateTime treatDate, string sourceId, bool isRead, bool isPrint) : base(c, treatDate, isRead,isPrint)
        {
            Content = c;
            SourceID = sourceId;
        }
        public CooperativePrescription.Prescription Content { get; }
        public string SourceID { get; }

        public override void Print()
        {
            var printPre = CreatePrescription();
            var service = PrescriptionService.CreateService(printPre);
            service.CloneTempPre();
            if (service.PrintConfirm())
            {
                service.Print(false);
            }
        }
        public override void PrintDir()
        {
            var printPre = CreatePrescription();
            var service = PrescriptionService.CreateService(printPre);
            service.CloneTempPre();
            if (service.PrintConfirmDir())
            {
                service.PrintDir(true);
            }
          
            
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", printPre.SourceId);
             MainWindow.ServerConnection.ExecuteProc("[Set].[IsPrePrintByID]", parameterList);
            MainWindow.ServerConnection.CloseConnection();


        }
        public override NewClass.Prescription.Prescription CreatePrescription()
        {
            var pre = new NewClass.Prescription.Prescription(Content, TreatDate, SourceID, IsRead, IsPrint);
            PrescriptionDb.UpdateCooperativePrescriptionIsRead(pre.SourceId);

            pre.Medicines.CountSelfPay();
            pre.CountPrescriptionPoint();
            pre.CountSelfPay();
            pre.PrescriptionPoint.CountAmountsPay();
            return pre;
        }

        public override void GetMedicines()
        {
            var medicines = new PreviewMedicines();
            var tempList = CreateTempMedicines(null);
            foreach (var setItem in Medicines)
            {
                if (medicines.Count(m => m.ID.Equals(setItem.ID)) > 0) continue;
                var medList = tempList.Where(m => m.ID.Equals(setItem.ID));
                foreach (var item in medList)
                {
                    medicines.Add(item);
                }
            }
            Medicines.Clear();
            Medicines = new PreviewMedicines(medicines);
        }

        private PreviewMedicines CreateTempMedicines(DateTime? adjustDate)
        {
            var idList = CreateMedicineIDList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, "0", adjustDate);
            var tempList = new PreviewMedicines();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var r = table.Rows[i];
                var addedList = Medicines.Where(m => m.ID.Equals(r.Field<string>("Pro_ID")));
                foreach (var item in addedList)
                    tempList.Add(new PreviewMedicine(r, item));
            }
            return tempList;
        }

        private List<string> CreateMedicineIDList()
        {
            var idList = new List<string>();
            foreach (var med in Content.MedicineOrder.Item)
            {
                if (!idList.Contains(med.Id))
                    idList.Add(med.Id);
            }
            return idList;
        }
    }
}