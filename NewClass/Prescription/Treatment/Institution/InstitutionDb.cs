using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public static class InstitutionDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Institution]");
        }

        public static DataTable GetCommonInstitutions()
        {
            var table = new DataTable();
            return table;
        }
    }
}
