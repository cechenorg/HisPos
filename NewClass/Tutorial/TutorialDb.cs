using System.Data;

namespace His_Pos.NewClass.Tutorial
{
    public static class TutorialDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Get].[Tutorial]");
        }
    }
}