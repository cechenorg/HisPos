using MenuUserControl;
using System.Collections.Generic;
using System.Data;
using His_Pos.Class;
using His_Pos.Class.Person;

namespace His_Pos
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {

        public static List<Feature> HisFeatures = new List<Feature>();
        public static List<Feature> PosFeatures = new List<Feature>();

        public static DataTable MedicineDataTable = new DataTable();
        public static DataTable OtcDataTable = new DataTable();
        public static User CurrentUser;

        private List<DockingWindow> _openWindows;
        public static DataView View;
    }
}
