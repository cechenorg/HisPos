using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using His_Pos.Class.Declare;
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
