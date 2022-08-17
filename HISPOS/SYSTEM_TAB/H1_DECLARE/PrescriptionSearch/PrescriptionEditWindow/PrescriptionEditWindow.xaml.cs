using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.Service;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Prescription = His_Pos.NewClass.Prescription.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow
{
    /// <summary>
    /// PrescriptionEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionEditWindow : Window
    {
        private int prevRowIndex = -1;

        public delegate Point GetDragDropPosition(IInputElement theElement);

        public PrescriptionEditWindow()
        {
            InitializeComponent();
        }

        public PrescriptionEditWindow(Prescription p, string title)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("ClosePrescriptionEditWindow"))
                    Close();
            });
            Messenger.Default.Register<NotificationMessage>("FocusDivision", FocusDivision);
            Messenger.Default.Register<NotificationMessage>("FocusSubDisease", FocusSubDisease);
            Messenger.Default.Register<NotificationMessage>("FocusChronicTotal", FocusChronicTotal);
            Closing += (sender, e) => Messenger.Default.Unregister(this);
            PrescriptionMedicines.Drop += PrescriptionMedicines_Drop;
            DataContext = new PrescriptionEditViewModel(p, title);
            ShowDialog();
        }

        private void FocusChronicTotal(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionEditViewModel && msg.Notification.Equals("FocusChronicTotal"))
            {
                ChronicTotal.Focus();
                ChronicTotal.SelectionStart = 0;
            }
        }

        private void FocusSubDisease(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionEditViewModel && msg.Notification.Equals("FocusSubDisease"))
            {
                SecondDiagnosis.Focus();
                SecondDiagnosis.SelectionStart = 0;
            }
        }

        private void FocusDivision(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionEditViewModel && msg.Notification.Equals("FocusDivision"))
                DivisionCombo.Focus();
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        private void Division_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DivisionCombo.SelectedItem != null)
            {
                TreatDate.Focus();
                TreatDate.SelectionStart = 0;
            }
        }

        private void TreatDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainDiagnosis.Focus();
                MainDiagnosis.SelectionStart = 0;
            }
        }

        private void ChronicTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ChronicSequence.Focus();
                ChronicSequence.SelectionStart = 0;
            }
        }

        private void ChronicSequence_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PrescriptionCaseCombo.Focus();
            }
        }

        private void PrescriptionCaseCombo_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CopaymentCombo.Focus();
            }
        }

        private void CopaymentCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PaymentCategoryCombo.Focus();
            }
        }

        private void PaymentCategoryCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SpecialTreatCombo.Focus();
            }
        }

        private void SpecialTreatCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var dataGridTextBox = new List<TextBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID",
                    ref dataGridTextBox);
                dataGridTextBox[0].Focus();
                dataGridTextBox[0].SelectionStart = 0;
                PrescriptionMedicines.SelectedItem = PrescriptionMedicines.Items[0];
            }
        }

        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //按 Enter 下一欄
            if (e.Key != Key.Enter) return;
            TextBox t = sender as TextBox;
            e.Handled = true;
            MoveFocusNext(sender);
        }

        private void MoveFocusNext(object sender)
        {
            if (sender is TextBox box)
                box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (PrescriptionMedicines.CurrentCell.Column is null) return;

            var focusedCell =
                PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    while (child is ContentPresenter)
                    {
                        child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    }

                    if ((child is TextBox || child is TextBlock))
                        break;
                }
                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell =
                    PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is TextBox || firstChild is TextBlock) && firstChild.Focusable)
            {
                firstChild.Focus();
                if (firstChild is TextBox t)
                    t.SelectAll();
            }
        }

        private void InputTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            textBox.SelectAll();
            var textBoxList = new List<TextBox>();
            NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref textBoxList);
            var index = textBoxList.IndexOf((TextBox)sender);
            PrescriptionMedicines.SelectedItem = (PrescriptionMedicines.Items[index] as Medicine);
        }

        private void InputTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is Medicine med)) return;
            ((PrescriptionEditViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }

        private void MedicineID_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            if (PrescriptionMedicines.CurrentCell.Item is null) return;
            if (PrescriptionMedicines.CurrentCell.Item.ToString().Equals("{NewItemPlaceholder}") && !textBox.Text.Equals(string.Empty))
            {
                var itemsCount = PrescriptionMedicines.Items.Count;
                (DataContext as PrescriptionEditViewModel)?.AddMedicine.Execute(textBox.Text);
                textBox.Text = string.Empty;

                if (PrescriptionMedicines.Items.Count != itemsCount)
                    PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[PrescriptionMedicines.Items.Count - 2], PrescriptionMedicines.Columns[3]);
            }
            else if (PrescriptionMedicines.CurrentCell.Item is Medicine med)
            {
                if (!med.ID.Equals(textBox.Text))
                    ((PrescriptionEditViewModel)DataContext).AddMedicine.Execute(textBox.Text);

                var textBoxList = new List<TextBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID", ref textBoxList);
                var index = textBoxList.IndexOf((TextBox)sender);
                if (!((Medicine)PrescriptionMedicines.Items[index]).ID.Equals(textBox.Text))
                    textBox.Text = ((Medicine)PrescriptionMedicines.Items[index]).ID;

                PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[index], PrescriptionMedicines.Columns[3]);
            }
            PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;

            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            if (firstChild is TextBox)
                firstChild.Focus();
        }

        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                t.CaretIndex++;
            }
        }

        private void MedicineID_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox t)
            {
                t.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                    PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;
                }
            }
        }

        private void MedicalNumber_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void PrescriptionMedicines_Drop(object sender, DragEventArgs e)
        {
            if (prevRowIndex < 0)
                return;

            var index = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (index < 0)
                return;
            if (index == prevRowIndex)
                return;
            if (index == PrescriptionMedicines.Items.Count - 1)
                return;

            var medicines = ((PrescriptionEditViewModel)DataContext).EditedPrescription.Medicines;
            var movedMedicine = medicines[prevRowIndex];
            medicines.RemoveAt(prevRowIndex);
            medicines.Insert(index, movedMedicine);
            ((PrescriptionEditViewModel)DataContext).EditedPrescription.Medicines.ReOrder();
            ((PrescriptionEditViewModel)DataContext).IsEdit = true;
        }

        private void PrescriptionMedicines_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prevRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (prevRowIndex < 0)
                return;
            PrescriptionMedicines.SelectedIndex = prevRowIndex;

            if (!(PrescriptionMedicines.Items[prevRowIndex] is Medicine selectedEmp))
                return;

            var dragDropEffects = DragDropEffects.Move;

            if (DragDrop.DoDragDrop(PrescriptionMedicines, selectedEmp, dragDropEffects) != DragDropEffects.None)
            {
                PrescriptionMedicines.SelectedItem = selectedEmp;
            }
        }

        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            var posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
            var theMousePos = pos((IInputElement)theTarget);
            return posBounds.Contains(theMousePos);
        }

        private DataGridRow GetDataGridRowItem(int index)
        {
            if (PrescriptionMedicines.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return PrescriptionMedicines.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos)
        {
            var curIndex = -1;
            for (var i = 0; i < PrescriptionMedicines.Items.Count; i++)
            {
                var itm = GetDataGridRowItem(i);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }

        private void ChangeMedicineIDToMostPriced(object sender, MouseButtonEventArgs e)
        {
            PrescriptionMedicines_PreviewMouseLeftButtonDown(sender, e);
            ((PrescriptionEditViewModel)DataContext).ChangeMedicineIDToMostPriced.Execute(null);
        }

        private void BuckleAmount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            if (textBox.SelectedText.Length.Equals(textBox.Text.Length))
                ((PrescriptionEditViewModel)DataContext).ResetBuckleAmount.Execute(null);
            textBox.Focus();
        }
        private void PatientCellPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PatientCellPhone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.V)
            {
                e.Handled = true;
            }
        }
    }
}