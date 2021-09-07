using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.Service;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Medicine = His_Pos.NewClass.Medicine.Base.Medicine;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    /// <summary>
    /// PrescriptionDeclareView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDeclareView : System.Windows.Controls.UserControl
    {
        private int prevRowIndex = -1;
        public static TextBox FromPOSCuslblcheck;

        public delegate Point GetDragDropPosition(IInputElement theElement);

        public PrescriptionDeclareView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>("FocusDivision", FocusDivision);
            Messenger.Default.Register<NotificationMessage>("FocusMedicalNumber", FocusMedicalNumber);
            Messenger.Default.Register<NotificationMessage>("FocusSubDisease", FocusSubDisease);
            Messenger.Default.Register<NotificationMessage>("FocusChronicTotal", FocusChronicTotal);
            Messenger.Default.Register<NotificationMessage>("FocusMainDisease", FocusMainDisease);
            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
            PrescriptionMedicines.Drop += PrescriptionMedicines_Drop;
            FromPOSCuslblcheck = this.tbFromPOS;
        }

        private void FocusMainDisease(NotificationMessage msg)
        {
            if (!(msg.Sender is PrescriptionDeclareViewModel) || !msg.Notification.Equals("FocusMainDisease")) return;
            MainDiagnosis.Focus();
            MainDiagnosis.SelectionStart = 0;
        }

        private void FocusDivision(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionDeclareViewModel && msg.Notification.Equals("FocusDivision"))
                DivisionCombo.Focus();
        }

        private void FocusMedicalNumber(NotificationMessage msg)
        {
            if (msg.Notification.Equals("FocusMedicalNumber"))
            {
                CheckPharmacistSelected();
            }
        }

        private void CheckPharmacistSelected()
        {
            if (DivisionCombo.SelectedIndex == 0 || DivisionCombo.SelectionBoxItem is null) return;
            if (PharmacistCombo.SelectedIndex < 0)
                PharmacistCombo.Focus();
            else
                MedicalNumber.Focus();
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        private void Division_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (PharmacistCombo.SelectedItem is null)
                        PharmacistCombo.Focus();
                    else
                    {
                        MedicalNumber.Focus();
                        MedicalNumber.SelectAll();
                    }
                    break;

                case Key.Left:
                    e.Handled = true;
                    ReleaseHospital.Focus();
                    break;
            }
        }

        private void MedicalNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    e.Handled = true;
                    break;

                case Key.Enter:
                    TreatDateTextBox.Focus();
                    TreatDateTextBox.SelectionStart = 0;
                    break;

                case Key.Left:
                    PharmacistCombo.Focus();
                    break;
            }
        }

        private void TreatDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                        AdjustDateTextBox.Focus();
                        AdjustDateTextBox.SelectionStart = 0;
                        e.Handled = true;
                        break;

                    case Key.Left:
                        MedicalNumber.Focus();
                        break;
                }
            }
        }

        private void AdjustDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is MaskedTextBox t)) return;
            switch (e.Key)
            {
                case Key.Enter:
                    t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                    MainDiagnosis.Focus();
                    MainDiagnosis.SelectionStart = 0;
                    e.Handled = true;
                    break;
                    //case Key.Left:
                    //    TreatDateTextBox.Focus();
                    //    break;
            }
        }

        private void FocusSubDisease(NotificationMessage msg)
        {
            if (!(msg.Sender is PrescriptionDeclareViewModel) || !msg.Notification.Equals("FocusSubDisease")) return;

            SecondDiagnosis.Focus();
            SecondDiagnosis.SelectionStart = 0;
        }

        private void FocusChronicTotal(NotificationMessage msg)
        {
            if (!(msg.Sender is PrescriptionDeclareViewModel) || !msg.Notification.Equals("FocusChronicTotal")) return;
            ChronicTotal.Focus();
            ChronicTotal.SelectionStart = 0;
        }

        private void MainDiagnosis_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                AdjustDateTextBox.Focus();
            }
        }

        private void SecondDiagnosis_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                MainDiagnosis.Focus();
            }
        }

        private void ChronicTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    ChronicSequence.Focus();
                    ChronicSequence.SelectionStart = 0;
                    break;

                case Key.Left:
                    SecondDiagnosis.Focus();
                    break;
            }
        }

        private void ChronicSequence_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    AdjustCombo.Focus();
                    break;

                case Key.Left:
                    ChronicTotal.Focus();
                    break;
            }
        }

        private void AdjustCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    PrescriptionCaseCombo.Focus();
                    break;

                case Key.Left:
                    e.Handled = true;
                    ChronicSequence.Focus();
                    break;
            }
        }

        private void PrescriptionCaseCombo_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    CopaymentCombo.Focus();
                    break;

                case Key.Left:
                    e.Handled = true;
                    AdjustCombo.Focus();
                    break;
            }
        }

        private void CopaymentCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    PaymentCategoryCombo.Focus();
                    break;

                case Key.Left:
                    e.Handled = true;
                    PrescriptionCaseCombo.Focus();
                    break;
            }
        }

        private void PaymentCategoryCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SpecialTreatCombo.Focus();
                    break;

                case Key.Left:
                    e.Handled = true;
                    CopaymentCombo.Focus();
                    break;
            }
        }

        private void SpecialTreatCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        var dataGridTextBox = new List<TextBox>();
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID",
                            ref dataGridTextBox);
                        dataGridTextBox[0].Focus();
                        dataGridTextBox[0].SelectionStart = 0;
                        PrescriptionMedicines.SelectedItem = PrescriptionMedicines.Items[0];
                        break;
                    }
                case Key.Left:
                    e.Handled = true;
                    PaymentCategoryCombo.Focus();
                    break;
            }
        }

        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //按 Enter 下一欄
            if (e.Key != Key.Enter && e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Right) return;
            e.Handled = true;
            if (sender is null) return;
            switch (e.Key)
            {
                case Key.Enter:
                    MoveFocusNext(sender, FocusNavigationDirection.Next);
                    break;

                case Key.Up:
                    MoveFocusNext(sender, FocusNavigationDirection.Up);
                    break;

                case Key.Down:
                    MoveFocusNext(sender, FocusNavigationDirection.Down);
                    break;

                case Key.Left:
                    MoveFocusNext(sender, FocusNavigationDirection.Left);
                    break;

                case Key.Right:
                    MoveFocusNext(sender, FocusNavigationDirection.Next);
                    break;
            }
        }

        private void MedicinePrice_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //按 Enter 下一欄
            if (e.Key != Key.Enter && e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Right) return;
            e.Handled = true;
            if (sender is null) return;
            switch (e.Key)
            {
                case Key.Enter:
                    MoveFocusNext(sender, FocusNavigationDirection.Next);
                    break;

                case Key.Up:
                    MoveFocusNext(sender, FocusNavigationDirection.Up);
                    break;

                case Key.Down:
                    MoveFocusNext(sender, FocusNavigationDirection.Down);
                    break;

                case Key.Right:
                    MoveFocusNext(sender, FocusNavigationDirection.Next);
                    break;
            }
        }

        private void MoveFocusNext(object sender, FocusNavigationDirection direction)
        {
            switch (sender)
            {
                case null:
                    return;

                case TextBox box:
                    if (direction.Equals(FocusNavigationDirection.Next))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    else if (direction.Equals(FocusNavigationDirection.Up))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                    else if (direction.Equals(FocusNavigationDirection.Left))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                    else
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    break;
            }
            var focusedCell = PrescriptionMedicines.CurrentCell.Column?.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
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

                focusedCell?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            }

            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
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

        private void MedicineTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var focusIndex = GetCurrentRowIndex(sender) + 1;
                FocusDataGridCell("MedicineID", PrescriptionMedicines, focusIndex);
            }
            else if (e.Key == Key.Up)
                MoveFocusNext(sender, FocusNavigationDirection.Up);
            else if (e.Key == Key.Down)
                MoveFocusNext(sender, FocusNavigationDirection.Down);
            else if (e.Key == Key.Left)
                MoveFocusNext(sender, FocusNavigationDirection.Left);
            /*else if (e.Key == Key.Right)
                MoveFocusNext(sender, FocusNavigationDirection.Next);*/
        }

        private int GetCurrentRowIndex(object sender)
        {
            switch (sender)
            {
                case TextBox textBox:
                    {
                        var temp = new List<TextBox>();
                        NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref temp);
                        for (var x = 0; x < temp.Count; x++)
                        {
                            if (temp[x].Equals(textBox))
                                return x;
                        }
                        break;
                    }
            }
            return -1;
        }

        private void FocusDataGridCell(string controlName, DataGrid focusGrid, int rowIndex)
        {
            var dataGridCells = new List<TextBox>();
            NewFunction.FindChildGroup(focusGrid, controlName, ref dataGridCells);
            if (controlName.Equals("MedicineID") && rowIndex >= dataGridCells.Count)
                rowIndex = dataGridCells.Count - 1;
            if (rowIndex >= dataGridCells.Count) return;
            dataGridCells[rowIndex].Focus();
            dataGridCells[rowIndex].SelectAll();
            focusGrid.SelectedIndex = rowIndex;
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
            ((PrescriptionDeclareViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
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
                (DataContext as PrescriptionDeclareViewModel)?.AddMedicine.Execute(textBox.Text);
                textBox.Text = string.Empty;

                if (PrescriptionMedicines.Items.Count != itemsCount)
                    PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[PrescriptionMedicines.Items.Count - 2], PrescriptionMedicines.Columns[4]);
            }
            else if (PrescriptionMedicines.CurrentCell.Item is Medicine med)
            {
                if (!med.ID.Equals(textBox.Text))
                    ((PrescriptionDeclareViewModel)DataContext).AddMedicine.Execute(textBox.Text);

                var textBoxList = new List<TextBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID", ref textBoxList);
                var index = textBoxList.IndexOf((TextBox)sender);
                if (!((Medicine)PrescriptionMedicines.Items[index]).ID.Equals(textBox.Text))
                    textBox.Text = ((Medicine)PrescriptionMedicines.Items[index]).ID;

                PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[index], PrescriptionMedicines.Columns[4]);
            }
            PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;

            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            if (firstChild is TextBox)
                firstChild.Focus();
        }

        private void MedicineIdPressDown(object sender, KeyEventArgs e, TextBox textBox)
        {
            e.Handled = true;
            switch (PrescriptionMedicines.CurrentCell.Item)
            {
                case Medicine _:
                    {
                        var textBoxList = new List<TextBox>();
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID", ref textBoxList);
                        var index = textBoxList.IndexOf((TextBox)sender) + 1;
                        PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[index], PrescriptionMedicines.Columns[2]);
                        break;
                    }
                default:
                    return;
            }

            PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;

            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            if (firstChild is TextBox)
                firstChild.Focus();
        }

        private void MedicineIdPressUp(object sender, KeyEventArgs e, TextBox textBox)
        {
            e.Handled = true;
            if (PrescriptionMedicines.CurrentCell.Item is null) return;
            var textBoxList = new List<TextBox>();
            NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID", ref textBoxList);
            var index = textBoxList.IndexOf((TextBox)sender) - 1;
            if (index >= 0)
                PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[index], PrescriptionMedicines.Columns[2]);
            PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;

            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            if (firstChild is TextBox)
                firstChild.Focus();
        }

        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            var t = sender as TextBox;
            switch (e.Key)
            {
                case Key.Decimal:
                    e.Handled = true;
                    if (t != null) t.CaretIndex++;
                    break;

                case Key.Subtract:
                    e.Handled = true;
                    break;

                default:
                    return;
            }
        }

        private void ShowPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row?.Item is null) return;
            if (!(row.Item is CustomerHistory)) return;
            ((PrescriptionDeclareViewModel)DataContext).ShowPrescriptionEditWindow.Execute(null);
        }

        private void Division_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckPharmacistSelected();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
            {
                e.Handled = true;
                return;
            }
            ((PrescriptionDeclareViewModel)DataContext).ScanPrescriptionQRCode.Execute(null);
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
            if (sender is TextBox tb)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                    if (PrescriptionMedicines.CurrentCell.Item != null) 
                    {
                        PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;
                    }
                }
            }
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

            var medicines = ((PrescriptionDeclareViewModel)DataContext).CurrentPrescription.Medicines;
            var movedMedicine = medicines[prevRowIndex];
            medicines.RemoveAt(prevRowIndex);
            medicines.Insert(index, movedMedicine);
            ((PrescriptionDeclareViewModel)DataContext).CurrentPrescription.Medicines.ReOrder();
        }

        private void PrescriptionMedicines_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is Medicine med)) return;
            prevRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (prevRowIndex < 0 || prevRowIndex >= PrescriptionMedicines.Items.Count - 1)
                return;
            PrescriptionMedicines.SelectedIndex = prevRowIndex;

            if (!(PrescriptionMedicines.Items[prevRowIndex] is Medicine selectedMed))
                return;

            var dragDropEffects = DragDropEffects.Move;

            if (DragDrop.DoDragDrop(PrescriptionMedicines, selectedMed, dragDropEffects) != DragDropEffects.None)
            {
                PrescriptionMedicines.SelectedItem = selectedMed;
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
                if (itm != null && IsTheMouseOnTargetRow(itm, pos))
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
            ((PrescriptionDeclareViewModel)DataContext).ChangeMedicineIDToMostPriced.Execute(null);
        }

        private void TextBox_SelectAll(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectAll();
        }

        private void PharmacistCombo_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        MedicalNumber.Focus();
                        break;

                    case Key.Left:
                        e.Handled = true;
                        DivisionCombo.Focus();
                        break;
                }
            }
        }

        private void BuckleAmount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            e.Handled = true;
            if (textBox.SelectedText.Length.Equals(textBox.Text.Length))
                ((PrescriptionDeclareViewModel)DataContext).ResetBuckleAmount.Execute(null);
            textBox.Focus();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as DataGridRow).DataContext is Medicine))
            {
                e.Handled = true;
            }
        }

        private void NoBuckleClick(object sender, RoutedEventArgs e)
        {
            if (PrescriptionMedicines.CurrentCell.Item is Medicine med)
            {
                PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;
            }
        }
    }
}