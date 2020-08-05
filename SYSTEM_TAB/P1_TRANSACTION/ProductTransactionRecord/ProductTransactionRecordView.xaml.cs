using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionRecord
{
    /// <summary>
    /// ProductTransactionRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionRecordView : UserControl
    {
        public ProductTransactionRecordView()
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

        private void ShowSelectedPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {
        }

        private void StartDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void EndDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
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
        }

        private void AdjustCase_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Division.Focus();
        }

        private void Division_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void MedicineCondition_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void MedicineCondition_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}
