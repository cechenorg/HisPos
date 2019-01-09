using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.FunctionWindow;
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
        public ObservableCollection<Hospital> HospitalCollection { get; set; }
        private ObservableCollection<Hospital> _commonHospitalsCollection;
        public ObservableCollection<Hospital> CommonHospitalsCollection
        {
            get => _commonHospitalsCollection;
            set
            {
                _commonHospitalsCollection = value;
                OnPropertyChanged(nameof(CommonHospitalsCollection));
            }
        }
        private Hospital _searchedHospital;
        public Hospital SearchedHospital
        {
            get => _searchedHospital;
            set
            {
                _searchedHospital = value;
                OnPropertyChanged(nameof(SearchedHospital));
            }
        }

        public CommonHospitalsWindow()
        {
            InitializeComponent();
            SearchedHospital = new Hospital();
            HospitalCollection = new ObservableCollection<Hospital>(MainWindow.Hospitals);
            CommonHospitalsCollection = new ObservableCollection<Hospital>(MainWindow.Hospitals.Where(h=>h.Common));
            DataContext = this;
        }

        private void Hospital_Select_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HospitalGrid.SelectedIndex < 0 || HospitalGrid.SelectedItem is null) return;
            var tmpHospital = CommonHospitalsCollection[HospitalGrid.SelectedIndex].DeepCloneViaJson();
            tmpHospital.Division = NewFunction.CheckHospitalNameContainsDivision(tmpHospital.Name);
            PrescriptionDec2View.Instance.CurrentPrescription.Treatment.MedicalInfo.Hospital =
                tmpHospital;
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
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddCommon_Click(object sender, RoutedEventArgs e)
        {
            SearchedHospital.Common = true;
            foreach (var c in CommonHospitalsCollection)
            {
                if (!c.Id.Equals(SearchedHospital.Id)) continue;
                MessageWindow.ShowMessage("無法新增，該常用醫療院所已存在", MessageType.ERROR);
                
                return;
            }
            CommonHospitalsCollection.Add(SearchedHospital);
            foreach (var h in MainWindow.Hospitals)
            {
                if (h.Id.Equals(SearchedHospital.Id))
                {
                    h.Common = true;
                }
            }
           /// HospitalDb.UpdateCommonHospitalById(SearchedHospital.Id, SearchedHospital.Common);
        }

        private void DeleteCommon_Click(object sender, RoutedEventArgs e)
        {
            SearchedHospital.Common = false;
            var find = false;
            var deleteIndex = 0;
            foreach (var c in CommonHospitalsCollection)
            {
                if (c.Id.Equals(SearchedHospital.Id))
                {
                    find = true;
                    break;
                }
                deleteIndex++;
            }
            if (!find)
            {
                MessageWindow.ShowMessage("無法刪除，查無對應常用醫療院所", MessageType.ERROR);
                
                return;
            }
            foreach (var h in MainWindow.Hospitals)
            {
                if (!h.Id.Equals(SearchedHospital.Id)) continue;
                h.Common = false;
                break;
            }
            CommonHospitalsCollection.RemoveAt(deleteIndex);
            ///HospitalDb.UpdateCommonHospitalById(SearchedHospital.Id, SearchedHospital.Common);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox t)) return;
            if (HospitalCollection.Count(h => h.Id.Contains(t.Text)) == 1)
            {
                SearchedHospital = HospitalCollection.SingleOrDefault(h => h.Id.Equals(t.Text)).DeepCloneViaJson();
                return;
            }
            SearchedHospital = new Hospital();
        }
    }
}
