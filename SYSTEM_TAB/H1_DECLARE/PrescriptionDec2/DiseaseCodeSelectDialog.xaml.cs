using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using His_Pos.Class.DiseaseCode;
using His_Pos.Class.Person;
using JetBrains.Annotations;
using MoreLinq;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// DiseaseCodeSelectDialog.xaml 的互動邏輯
    /// </summary>
    public partial class DiseaseCodeSelectDialog : Window,INotifyPropertyChanged
    {
        private ObservableCollection<DiseaseCode9To10> diseaseCollection;

        public ObservableCollection<DiseaseCode9To10> DiseaseCollection
        {
            get => diseaseCollection;
            set
            {
                diseaseCollection = value;
                OnPropertyChanged(nameof(DiseaseCollection));
            }
        }
        public string Source { get; set; }
        public DiseaseCode9To10 SelectedDiseaseCode { get; set; }
        public DiseaseCodeSelectDialog(string id,string name)
        {
            InitializeComponent();
            DiseaseCollection = new ObservableCollection<DiseaseCode9To10>();
            Source = name;
            List<DiseaseCode9To10> distinctList = DiseaseCodeDb.GetDiseaseCodeById(id).DistinctBy(d => d.ICD10.Id).DistinctBy(d=>d.ICD9.Id).ToList();
            foreach (var d in distinctList)
            {
                DiseaseCollection.Add(d);
            }
            DataContext = this;
            Condition.Text = id;
            Condition.Focus();
            SearchResult.ItemsSource = DiseaseCollection;
            if(SearchResult.Items.Count == 1)
                SetSelectDiseaseCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Condition_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox)) return;
            var itemSourceList = new CollectionViewSource { Source = DiseaseCollection };
            var diseaseCodeList = itemSourceList.View;
            itemSourceList.Filter += DiseaseCodeFilter;
            SearchResult.ItemsSource = diseaseCodeList;
            if (diseaseCodeList.Cast<DiseaseCode9To10>().ToList().Count == 1)
                SelectedDiseaseCode = diseaseCodeList.Cast<DiseaseCode9To10>().ToList().ToList()[0];
            if (SearchResult.SelectedIndex == -1 && diseaseCodeList.Cast<DiseaseCode9To10>().ToList().Count > 0)
                SearchResult.SelectedIndex = 0;
        }

        private void DiseaseCodeFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is DiseaseCode9To10 obj)) return;
            e.Accepted = string.IsNullOrEmpty(Condition.Text) || obj.ICD10.Id.Contains(Condition.Text) || obj.ICD9.Id.Contains(Condition.Text);
        }
        private void DiseaseCode_Select_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetSelectDiseaseCode();
        }

        private void SetSelectDiseaseCode()
        {
            SelectedDiseaseCode = SearchResult.SelectedIndex == -1 ? DiseaseCollection[0] : DiseaseCollection[SearchResult.SelectedIndex];
            if (Source.Contains("Main"))
            {
                PrescriptionDec2View.Instance.CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id =
                    SelectedDiseaseCode.ICD10.Id;
                PrescriptionDec2View.Instance.CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Name =
                    SelectedDiseaseCode.ICD10.Name;
            }
            else
            {
                PrescriptionDec2View.Instance.CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id =
                    SelectedDiseaseCode.ICD10.Id;
                PrescriptionDec2View.Instance.CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Name =
                    SelectedDiseaseCode.ICD10.Name;
            }
            Close();
        }
    }
}
