using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Employee.WorkPosition
{
   public class WorkPositions: Collection<WorkPosition>
    {
        public WorkPositions() {
            Init();
        }
        public void Init() {
            DataTable table = WorkPositionDb.GetData();
            foreach (DataRow r in table.Rows) {
                Add(new WorkPosition(r));
            }
        }
    }
}
