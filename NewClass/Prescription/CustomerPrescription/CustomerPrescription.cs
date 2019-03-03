using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription.CustomerPrescription
{
    public class CustomerPrescription:ObservableObject
    {
        public CustomerPrescription(DataRow r, PrescriptionSource source)
        {
            ID = r.Field<int>("ID");
            ReleaseIns = VM.GetInstitution(r.Field<string>("Ins_ID"));
            Division = VM.GetDivision(r.Field<string>("Div_ID"));
            TreatDate = r.Field<DateTime>("Tre_Date");
            AdjustDate = r.Field<DateTime>( "Adj_Date");
            Source = source;
        }
        public int ID { get; set; }
        public Institution ReleaseIns { get; set; }
        public Division Division { get; set; }
        public DateTime TreatDate { get; set; }
        public DateTime AdjustDate { get; set; }
        public PrescriptionSource Source { get; set; }
    }
}
