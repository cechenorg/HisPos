using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.ControlMedicineDetail
{
    public class ControlMedicineDetails : ObservableCollection<ControlMedicineDetail>
    {
        public ControlMedicineDetails( ) {
          
        }
        public void GetDataById(string medId,DateTime sDate,DateTime eDate) {
            Clear();
            DataTable table = ControlMedicineDetailDb.GetDataById(medId, sDate,eDate);
            foreach (DataRow r in table.Rows) {
                Add(new ControlMedicineDetail(r));
            }
        }
    }
}
