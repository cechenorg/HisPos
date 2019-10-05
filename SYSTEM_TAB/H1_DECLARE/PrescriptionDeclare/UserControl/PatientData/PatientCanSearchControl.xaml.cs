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
using His_Pos.NewClass.Person.Customer;
using His_Pos.Service;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl.PatientData
{
    /// <summary>
    /// PatientCanSearchControl.xaml 的互動邏輯
    /// </summary>
    public partial class PatientCanSearchControl : System.Windows.Controls.UserControl
    {
        #region GetCustomers
        public static readonly DependencyProperty GetCustomersProperty =
            DependencyProperty.Register(
                "GetCustomers",
                typeof(ICommand),
                typeof(PatientCanSearchControl),
                new PropertyMetadata(null));
        public ICommand GetCustomers
        {
            get { return (ICommand)GetValue(GetCustomersProperty); }
            set { SetValue(GetCustomersProperty, value); }
        }
        #endregion
        #region Patient
        public static readonly DependencyProperty PatientProperty =
            DependencyProperty.Register(
                "Patient",
                typeof(Customer),
                typeof(PatientCanSearchControl),
                new PropertyMetadata(null));
        public Customer Patient
        {
            get { return (Customer)GetValue(PatientProperty); }
            set { SetValue(PatientProperty, value); }
        }
        #endregion
        public PatientCanSearchControl()
        {
            InitializeComponent();
        }
        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        private void PatientBirthday_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }
    }
}
