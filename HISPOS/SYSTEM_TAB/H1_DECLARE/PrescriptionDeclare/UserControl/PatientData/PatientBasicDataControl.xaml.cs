using His_Pos.NewClass.Person.Customer;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl.PatientData
{
    /// <summary>
    /// PatientBasicDataControl.xaml 的互動邏輯
    /// </summary>
    public partial class PatientBasicDataControl : System.Windows.Controls.UserControl
    {
        #region Patient

        public static readonly DependencyProperty PatientProperty =
            DependencyProperty.Register(
                "Patient",
                typeof(Customer),
                typeof(PatientBasicDataControl),
                new PropertyMetadata(null));

        public Customer Patient
        {
            get { return (Customer)GetValue(PatientProperty); }
            set
            {
                SetValue(PatientProperty, value);
            }
        }

        #endregion Patient

        #region Edited

        public static readonly DependencyProperty EditedProperty =
            DependencyProperty.Register(
                "Edited",
                typeof(bool),
                typeof(PatientBasicDataControl),
                new PropertyMetadata(null));

        public bool Edited
        {
            get { return (bool)GetValue(EditedProperty); }
            set
            {
                SetValue(EditedProperty, value);
            }
        }

        #endregion Edited

        public PatientBasicDataControl()
        {
            InitializeComponent();
        }

        private void PatientCellPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PatientCellPhone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.V)
            {
                e.Handled = true;
            }
        }
    }
}