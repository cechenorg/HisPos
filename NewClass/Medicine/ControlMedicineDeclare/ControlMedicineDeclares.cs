using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.ControlMedicineDeclare {
    public class ControlMedicineDeclares  : ObservableCollection<ControlMedicineDeclare> {
        public ControlMedicineDeclares() {
        }
        public void GetData(DateTime sDate,DateTime eDate) {
            Clear();
            DataTable table = ControlMedicineDeclareDb.GetDataByDate(sDate,eDate);
            foreach (DataRow r in table.Rows) {
                Add(new ControlMedicineDeclare(r));
            }
        }
    }
}
