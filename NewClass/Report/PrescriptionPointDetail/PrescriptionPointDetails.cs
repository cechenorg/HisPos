using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionPointDetail
{
   public class PrescriptionPointDetails : ObservableCollection<PrescriptionPointDetail>
    {
        public PrescriptionPointDetails() {

        }
        public void GetData(DateTime date) {
            DataTable table = PrescriptionPointDetailDb.GetDataByDate(date, ViewModelMainWindow.CooperativeInstitutionID);
            foreach (DataRow r in table.Rows) {
                Add(new PrescriptionPointDetail(r));
            }
        }
    }
}
