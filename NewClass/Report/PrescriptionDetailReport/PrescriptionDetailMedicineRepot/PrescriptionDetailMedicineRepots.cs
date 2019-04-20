using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot
{
   public class PrescriptionDetailMedicineRepots : ObservableCollection<PrescriptionDetailMedicineRepot>
    {
        public PrescriptionDetailMedicineRepots() { }

        public void GerDataById(int Id) {
            Clear();
            DataTable table = PrescriptionDetailMedicineRepotDb.GetDataById(Id);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionDetailMedicineRepot(r));
            }
        }
    }
}
