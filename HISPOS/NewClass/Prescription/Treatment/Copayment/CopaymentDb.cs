using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    public static class CopaymentDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Copayment]");
        }
    }
}