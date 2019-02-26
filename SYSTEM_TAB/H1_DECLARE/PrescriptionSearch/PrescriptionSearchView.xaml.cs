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
            Messenger.Default.Register<NotificationMessage>("FocusPreSearchPharmacistCombo", FocusPharmacistCombo);
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
                ReleaseHospital.Focus();
                ReleaseHospital.SelectAll();
                e.Handled = true;
            }
        }
        private void FocusPharmacistCombo(NotificationMessage msg)
        {
            if (msg.Notification.Equals("FocusPreSearchPharmacistCombo"))
            {
                PreSearchPharmacistCombo.Focus();
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

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PatientBirth.Focus();
            }
        }

        private void ShowSelectedPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row?.Item is null) return;
            if (!(row.Item is PrescriptionSearchPreview)) return;
            Messenger.Default.Send(new NotificationMessage(nameof(PrescriptionSearchView)+"ShowPrescriptionEditWindow"));
        }
    }
}
