using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    public static class AdjustCaseDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[AdjustCase]");
        }
    }
}