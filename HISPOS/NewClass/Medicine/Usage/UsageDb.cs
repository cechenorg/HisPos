using System.Data;

namespace His_Pos.NewClass.Medicine.Usage
{
    public static class UsageDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Usage]");
        }
    }
}