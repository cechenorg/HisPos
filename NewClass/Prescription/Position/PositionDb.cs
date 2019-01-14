using System.Data;

namespace His_Pos.NewClass.Prescription.Position
{
    public static class PositionDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[HISPOS_Develop].[Get].[Position]");
        }
    }
}
