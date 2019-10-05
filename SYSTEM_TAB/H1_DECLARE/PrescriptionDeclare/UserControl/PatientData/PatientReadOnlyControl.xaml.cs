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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl.PatientData
{
    /// <summary>
    /// PatientReadOnlyControl.xaml 的互動邏輯
    /// </summary>
    public partial class PatientReadOnlyControl : System.Windows.Controls.UserControl
    {
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
        public PatientReadOnlyControl()
        {
            InitializeComponent();
        }
    }
}
