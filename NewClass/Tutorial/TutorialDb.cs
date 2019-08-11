using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
