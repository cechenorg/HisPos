
using His_Pos.NewClass.Prescription;
using System.Windows; 

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.MedicinesSendSingdeWindow
{
    /// <summary>
    /// MedicinesSendSingdeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedicinesSendSingdeWindow : Window
    {
        public MedicinesSendSingdeWindow(Prescription p)
        {
            InitializeComponent();
            DataContext = new MedicinesSendSingdeViewModel(p);
        }
        
    }
}
