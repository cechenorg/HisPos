using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    public static class SpecialTreatDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[SpecialTreatment]");
        }
    }
}