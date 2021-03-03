using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.AutoRegisterWindow
{
    /// <summary>
    /// AutoRegisterWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AutoRegisterWindow : Window
    {
        public bool RegisterResult { get; set; }

        public AutoRegisterWindow()
        {
            InitializeComponent();
        }

        public AutoRegisterWindow(Prescription current, Prescriptions registerList)
        {
            InitializeComponent();
            DataContext = new AutoRegisterViewModel(current, registerList);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "AutoRegisterSubmit":
                        RegisterResult = true;
                        Close();
                        break;

                    case "AutoRegisterCancel":
                        RegisterResult = false;
                        Close();
                        break;
                }
            });
            if (registerList.Count > 0)
                ShowDialog();
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void AdjustDateTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Down && e.Key != Key.Up) return;
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
            }
        }

        private void MoveFocusNext(object sender, FocusNavigationDirection direction)
        {
            switch (sender)
            {
                case null:
                    return;

                case MaskedTextBox box:
                    if (direction.Equals(FocusNavigationDirection.Next))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    else if (direction.Equals(FocusNavigationDirection.Up))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                    else
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    break;
            }
            var focusedCell = ReserveGrid.CurrentCell.Column?.GetCellContent(ReserveGrid.CurrentCell.Item);
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
                focusedCell = ReserveGrid.CurrentCell.Column.GetCellContent(ReserveGrid.CurrentCell.Item);
            }

            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is MaskedTextBox || firstChild is TextBlock) && firstChild.Focusable)
            {
                firstChild.Focus();
                if (firstChild is MaskedTextBox t)
                    t.SelectAll();
            }
        }
    }
}