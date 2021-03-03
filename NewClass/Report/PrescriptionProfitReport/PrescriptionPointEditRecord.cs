using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionProfitReport
{
    public class PrescriptionPointEditRecord : ObservableObject
    {
        public PrescriptionPointEditRecord()
        {
        }

        public PrescriptionPointEditRecord(DataRow r)
        {
            ID = r.Field<int>("PreRec_ID");
            switch (r.Field<string>("AdjustCaseID"))
            {
                case "2":
                    TypeID = "2";
                    break;

                case "0":
                    TypeID = "4";
                    break;

                default:
                    TypeID = "1";
                    break;
            }
            MedicalServiceDifference = r.Field<int>("PreRec_MedicalServiceDifference");
            MedicineDifference = r.Field<int>("PreRec_MedicineDifference");
            PaySelfDifference = r.Field<int>("PreRec_PaySelfDifference");
            ProfitDifference = MedicalServiceDifference + MedicineDifference + PaySelfDifference;
        }

        private int id;

        public int ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }

        private string typeID;

        public string TypeID
        {
            get => typeID;
            set
            {
                Set(() => TypeID, ref typeID, value);
            }
        }

        private int medicalServiceDifference;

        public int MedicalServiceDifference
        {
            get => medicalServiceDifference;
            set
            {
                Set(() => MedicalServiceDifference, ref medicalServiceDifference, value);
            }
        }

        private int medicineDifference;

        public int MedicineDifference
        {
            get => medicineDifference;
            set
            {
                Set(() => MedicineDifference, ref medicineDifference, value);
            }
        }

        private int paySelfDifference;

        public int PaySelfDifference
        {
            get => paySelfDifference;
            set
            {
                Set(() => PaySelfDifference, ref paySelfDifference, value);
            }
        }

        private int profitDifference;

        public int ProfitDifference
        {
            get => profitDifference;
            set
            {
                Set(() => ProfitDifference, ref profitDifference, value);
            }
        }
    }
}