using System.Windows;
using His_Pos.Class.Product;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// MedicineInfoWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineInfoWindow : Window
    {
        public MedicineInformation Med { get; set; }
        public MedicineInfoWindow(MedicineInformation medicineInformation)
        {
            InitializeComponent();
            Med = medicineInformation.DeepCloneViaJson();
            DataContext = this;
        }
    }
}
