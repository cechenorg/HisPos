using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Prescription;
using His_Pos.Service;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow
{
    /// <summary>
    /// PrescriptionRecordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionRecordWindow : Window
    {
        private int prevRowIndex = -1;
        public delegate Point GetDragDropPosition(IInputElement theElement);

        public PrescriptionRecordWindow()
        {
            InitializeComponent();
        }

        public PrescriptionRecordWindow(Prescription p, string title)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("ClosePrescriptionEditWindow"))
                    Close();
            });
            Closing += (sender, e) => Messenger.Default.Unregister(this);
            PrescriptionMedicines.Drop += PrescriptionMedicines_Drop;
            DataContext = new PrescriptionEditViewModel(p, title);
            ShowDialog();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is Medicine med)) return;
            ((PrescriptionEditViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
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

        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                t.CaretIndex++;
            }
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
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
            if (index == PrescriptionMedicines.Items.Count)
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

        private void DateMaskedTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }

        private void BuckleAmount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            if (textBox.SelectedText.Length.Equals(textBox.Text.Length))
                ((PrescriptionEditViewModel)DataContext).ResetBuckleAmount.Execute(null);
            textBox.Focus();
        }
        private void BuckleAmount_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            if (textBox.SelectedText.Length.Equals(textBox.Text.Length))
                ((PrescriptionEditViewModel)DataContext).ClearBuckleAmount.Execute(null);
        }
    }
}