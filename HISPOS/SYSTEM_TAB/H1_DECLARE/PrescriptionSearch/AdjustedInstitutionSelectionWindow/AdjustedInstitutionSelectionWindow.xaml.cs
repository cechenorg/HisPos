using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.AdjustedInstitutionSelectionWindow
{
    /// <summary>
    /// AdjustedInstitutionSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AdjustedInstitutionSelectionWindow : Window
    {
        public AdjustedInstitutionSelectionWindow()
        {
            InitializeComponent();
        }

        public AdjustedInstitutionSelectionWindow(PrescriptionSearchInstitutions institutions)
        {
            InitializeComponent();
            DataContext = new AdjustedInstitutionSelectionViewModel(institutions,this);
            ShowDialog();
        }
    }
}