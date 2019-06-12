using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.AccountReport.InstitutionDeclarePoint
{
    public class InstitutionDeclarePoints : ObservableCollection<InstitutionDeclarePoint>
    {
        public InstitutionDeclarePoints() {
        }
        public void GetDataByDate(DateTime dateTime) {
            Clear();
            DataTable table = InstitutionDeclarePointDb.GetDataByMonth(dateTime);
            foreach (DataRow r in table.Rows) {
                Add(new InstitutionDeclarePoint(r));
            }
        }
    }
}
