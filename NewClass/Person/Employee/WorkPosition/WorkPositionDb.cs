using System.Data;

namespace His_Pos.NewClass.Person.Employee.WorkPosition
{
    public static class WorkPositionDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[WorkPosition]");
        }
    }
}