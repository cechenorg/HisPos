using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using JetBrains.Annotations;

namespace His_Pos.H6_DECLAREFILE.Export
{
    /// <inheritdoc />
    /// <summary>
    /// ExportView.xaml 的互動邏輯
    /// </summary>
    public partial class ExportView : UserControl,INotifyPropertyChanged
    {
        public ObservableCollection<DeclareFile> DeclareFiles { get; set; }
        public ObservableCollection<Division> Divisions { get; set; }

        private DeclareFile _selectedFile;
        public DeclareFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        private Ddata _selectedPrescription;
        public Ddata SelectedPrescription
        {
            get => _selectedPrescription;
            set
            {
                _selectedPrescription = value;
                OnPropertyChanged(nameof(SelectedPrescription));
            }
        }

        public ExportView()
        {
            InitializeComponent();
            InitializeDeclareFiles();
        }

        private void InitializeDeclareFiles()
        {
            var load = new LoadingWindow();
            load.GetDeclareFileData(this);
            load.Show();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
