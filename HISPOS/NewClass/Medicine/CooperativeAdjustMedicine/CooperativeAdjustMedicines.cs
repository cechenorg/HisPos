using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.CooperativeAdjustMedicine
{
    public class CooperativeAdjustMedicines : ObservableCollection<CooperativeAdjustMedicine>
    {
        public CooperativeAdjustMedicines()
        {
        }

        public void GetDataByDate(DateTime sDate, DateTime eDate)
        {
            Clear();
            var table = CooperativeAdjustMedicineDb.GetDataByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new CooperativeAdjustMedicine(r));
            }
        }
    }
}