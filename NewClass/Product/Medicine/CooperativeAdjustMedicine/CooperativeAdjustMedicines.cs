using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.CooperativeAdjustMedicine {
    public class CooperativeAdjustMedicines : ObservableCollection<CooperativeAdjustMedicine> {
        public CooperativeAdjustMedicines() {
             
        }
        public void GetDataByDate(DateTime sDate,DateTime eDate) {
            var table = CooperativeAdjustMedicineDb.GetDataByDate(sDate,eDate);
            foreach (DataRow r in table.Rows) {
                Add(new CooperativeAdjustMedicine(r));
            }
        }
    }
}
