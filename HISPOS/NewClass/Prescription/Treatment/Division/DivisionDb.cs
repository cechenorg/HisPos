using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public static class DivisionDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Division]");
        }
    }
}