using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.AccountReport.InstitutionDeclarePoint
{
    public class InstitutionDeclarePoints : ObservableCollection<InstitutionDeclarePoint>
    {
        public InstitutionDeclarePoints()
        {
        }

        public void GetDataByDate(DateTime FirstDay, DateTime LastDay)
        {
            Clear();
            DataTable table = InstitutionDeclarePointDb.GetDataByMonth(FirstDay, LastDay);
            foreach (DataRow r in table.Rows)
            {
                Add(new InstitutionDeclarePoint(r));
            }
        }
    }
}