using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using His_Pos.Class.DiseaseCode;
using JetBrains.Annotations;

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

        public DiseaseCode9To10 SelectedDiseaseCode { get; set; }
        public DiseaseCodeSelectDialog(string id)
        {
            InitializeComponent();
            DiseaseCollection = DiseaseCodeDb.GetDiseaseCodeById(id);
            SearchResult.ItemsSource = DiseaseCollection;
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            SelectedDiseaseCode = SearchResult.SelectedIndex == -1 ? DiseaseCollection[0] : DiseaseCollection[SearchResult.SelectedIndex];
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
