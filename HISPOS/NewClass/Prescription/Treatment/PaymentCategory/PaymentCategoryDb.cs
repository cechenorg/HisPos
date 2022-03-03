using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    public static class PaymentCategoryDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PaymentCategory]");
        }
    }
}