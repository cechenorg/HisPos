using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Report.PrescriptionPointDetail;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeEntry.PrescriptionPointDetail
{
   public class PrescriptionPointDetailViewModel :ViewModelBase
    {
        public int NormalPrescriptionCount { get; set; }
        public int ChronicPrescriptionCount { get; set; }

        private NewClass.Report.PrescriptionPointDetail.PrescriptionPointDetail selectItem;
        public NewClass.Report.PrescriptionPointDetail.PrescriptionPointDetail SelectItem
        {
            get => selectItem;
            set
            {
                Set(() => SelectItem, ref selectItem, value);
            }
        }
        private PrescriptionPointDetails prescriptionPointDetailCollection = new PrescriptionPointDetails();
        public PrescriptionPointDetails PrescriptionPointDetailCollection
        {
            get => prescriptionPointDetailCollection;
            set
            {
                Set(() => PrescriptionPointDetailCollection, ref prescriptionPointDetailCollection, value);
            } 
        }
        public PrescriptionPointDetailViewModel(DateTime date) {
            PrescriptionPointDetailCollection.GetData(date);
            NormalPrescriptionCount = PrescriptionPointDetailCollection.Count(p => p.AdjustCaseName != "慢性病連續處方調劑");
            ChronicPrescriptionCount = PrescriptionPointDetailCollection.Count(p => p.AdjustCaseName == "慢性病連續處方調劑");
        }
     
    }
}
