using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Service;
using JetBrains.Annotations;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// CommonHospitalsWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CommonHospitalsWindow : Window,INotifyPropertyChanged
    {
        MessageWindow m;
        private Institutions _commonInstitutions;
        public Institutions CommonInstitutions
        {
            get => _commonInstitutions;
            set
            {
                _commonInstitutions = value;
                OnPropertyChanged(nameof(CommonInstitutions));
            }
        }
        private Institution _searchedInstitution;
        public Institution SearchedInstitution
        {
            get => _searchedInstitution;
            set
            {
                _searchedInstitution = value;
                OnPropertyChanged(nameof(SearchedInstitution));
            }
        }

        public CommonHospitalsWindow()
        {
            InitializeComponent();
            SearchedInstitution = new Institution();
            CommonInstitutions.GetCommon();
            DataContext = this;
        }

        private void Hospital_Select_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HospitalGrid.SelectedIndex < 0 || HospitalGrid.SelectedItem is null) return;
            var tmpHospital = CommonInstitutions[HospitalGrid.SelectedIndex].DeepCloneViaJson();
            /*
            tmpHospital.Division = NewFunction.CheckHospitalNameContainsDivision(tmpHospital.Name);
            PrescriptionDec2View.Instance.CurrentPrescription.Treatment.MedicalInfo.Hospital = tmpHospital;
            if (string.IsNullOrEmpty(tmpHospital.Division.Id))
            {
                PrescriptionDec2View.Instance.DivisionCombo.SelectedIndex = -1;
                Close();
            }
            else
            {
                var i = 0;
                foreach (var d in MainWindow.Divisions)
                {
                    if (d.Id.Equals(tmpHospital.Division.Id))
                    {
                        PrescriptionDec2View.Instance.DivisionCombo.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
                Close();
            }*/
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
