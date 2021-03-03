using His_Pos.NewClass.Prescription.Search;
using His_Pos.Service;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

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

        //private void EndDate_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        PatientNameText.Focus();
        //        PatientNameText.SelectAll();
        //    }
        //}

        //private void PreSearchPharmacistCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        PreSearchAdjustCaseCombo.Focus();
        //    }
        //}

        //private void PreSearchAdjustCaseCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        SearchButton.Focus();
        //    }
        //}

        //private void PatientName_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        PatientIdNumberText.Focus();
        //    }
        //}

        //private void PatientIDNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        PatientBirth.Focus();
        //    }
        //}

        //private void PatientBirthday_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        PreSearchAdjustCaseCombo.Focus();
        //    }
        //}

        private void ShowSelectedPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridRow row) || !(row.DataContext is PrescriptionSearchPreview pre)) return;
            ((PrescriptionSearchViewModel)DataContext).ShowPrescriptionEdit.Execute(pre.ID);
        }

        private void StartDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                EndDate.Focus();
                EndDate.SelectionStart = 0;
            }
        }

        private void EndDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                PatientCondition.Focus();
            }
        }

        private void PatientCondition_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchPatientText.Focus();
        }

        private void PatientCondition_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchPatientText.Focus();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDate.Focus();
            StartDate.SelectionStart = 0;
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartDate.Focus();
        }

        private void SearchPatientText_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Birthday.Focus();
        }

        private void Birthday_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                AdjustCase.Focus();
            }
        }

        private void AdjustCase_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Division.Focus();
        }

        private void Division_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MedicineCondition.Focus();
        }

        private void MedicineCondition_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchMedicineText.Focus();
        }

        private void MedicineCondition_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchMedicineText.Focus();
        }
    }
}