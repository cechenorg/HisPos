using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription.Search;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch
{
    /// <summary>
    /// PrescriptionSearchView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionSearchView : UserControl
    {
        public PrescriptionSearchView()
        {
            InitializeComponent();
        }

        private void MaskedTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EndDate.Focus();
                EndDate.SelectAll();
            }
        }

        private void EndDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PatientNameText.Focus();
                PatientNameText.SelectAll();
            }
        }

        private void PreSearchPharmacistCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PreSearchAdjustCaseCombo.Focus();
            }
        }

        private void PreSearchAdjustCaseCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton.Focus();
            }
        }

        private void PatientName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PatientIdNumberText.Focus();
            }
        }

        private void PatientIDNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PatientBirth.Focus();
            }
        }

        private void PatientBirthday_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PreSearchAdjustCaseCombo.Focus();
            }
        }

        private void ShowSelectedPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridRow row) || !(row.DataContext is PrescriptionSearchPreview pre)) return;
            ((PrescriptionSearchViewModel)DataContext).ShowPrescriptionEdit.Execute(pre.ID);
        }
    }
}
