using His_Pos.NewClass.Product;
using His_Pos.Service;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace His_Pos.Behaviors
{
    public class ToNextTextBoxBehavior : Behavior<TextBox>
    {
        #region ----- Override Attach -----

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObjectOnKeyDown;
        }

        #endregion ----- Override Attach -----

        #region ----- Define Events -----

        private void AssociatedObjectOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                MoveFocusNext(sender);
            }
        }

        private void MoveFocusNext(object sender)
        {
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            DataGrid dataGrid = NewFunction.FindParent<DataGrid>(sender as TextBox);

            if (Keyboard.FocusedElement is Button)
                (Keyboard.FocusedElement as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (dataGrid.CurrentCell.Column is null) return;

            var focusedCell = dataGrid.CurrentCell.Column.GetCellContent(dataGrid.CurrentCell.Item);

            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                    if (!(child is Image))
                        break;
                }

                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                focusedCell = dataGrid.CurrentCell.Column.GetCellContent(dataGrid.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

            if (firstChild is TextBox)
            {
                firstChild.Focus();
                FocusRow(firstChild as TextBox, dataGrid);
            }
        }

        private void FocusRow(TextBox textBox, DataGrid dataGrid)
        {
            List<TextBox> textBoxs = new List<TextBox>();
            NewFunction.FindChildGroup(dataGrid, textBox.Name, ref textBoxs);

            int index = textBoxs.IndexOf(textBox);

            dataGrid.SelectedItem = (dataGrid.Items[index] as Product);
        }

        #endregion ----- Define Events -----
    }
}