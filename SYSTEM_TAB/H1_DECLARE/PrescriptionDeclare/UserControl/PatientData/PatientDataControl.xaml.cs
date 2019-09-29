using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using JetBrains.Annotations;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl.PatientData
{
    /// <summary>
    /// PatientDataControl.xaml 的互動邏輯
    /// </summary>
    public partial class PatientDataControl
    {
        #region GetCustomers
        public static readonly DependencyProperty GetCustomersProperty =
            DependencyProperty.Register(
                "GetCustomers",
                typeof(ICommand),
                typeof(PatientDataControl),
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
                typeof(PatientDataControl),
                new PropertyMetadata(null));
        public Customer Patient
        {
            get { return (Customer)GetValue(PatientProperty); }
            set
            {
                SetValue(PatientProperty, value);
            }
        }
        #endregion
        #region Patient
        public static readonly DependencyProperty CanSearchProperty =
            DependencyProperty.Register(
                "CanSearch",
                typeof(bool),
                typeof(PatientDataControl),
                new PropertyMetadata(null));
        public bool CanSearch
        {
            get { return (bool)GetValue(CanSearchProperty); }
            set
            {
                SetValue(CanSearchProperty, value);
            }
        }
        #endregion
        public PatientDataControl()
        {
            InitializeComponent();
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }
    }
}
