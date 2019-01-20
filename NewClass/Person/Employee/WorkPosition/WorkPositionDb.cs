using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Employee.WorkPosition
{
    public static class WorkPositionDb
    {
        public static DataTable GetData() {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[WorkPosition]");
        }
    }
}
