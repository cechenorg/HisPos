using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    public static class PrescriptionCaseDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCase]");
        }
    }
}