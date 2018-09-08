using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Interface;
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
        public ObservableCollection<Hospital> Hospitals { get; set; }
        public ObservableCollection<Division> Divisions { get; set; }
        public ObservableCollection<AdjustCase> AdjustCases { get; set; }
        public ObservableCollection<DeclareFileDdata> PrescriptionCollection { get; set; }
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
            DataContext = this;
            InitializeDeclareFiles();
            DeclareFileList.SelectedIndex = 0;
        }

        private void InitializeDeclareFiles()
        {
            var load = new LoadingWindow();
            load.GetDeclareFileData(this);
            load.Show();
            SelectedFile = new DeclareFile();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DeclareFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFile = (DeclareFile)(sender as DataGrid)?.SelectedItem;

            foreach (var ddata in SelectedFile.FileContent.Ddata)
            {
                SelectedFile.PrescriptionDdatas.Add(new DeclareFileDdata(ddata));
            }

            foreach (var d in SelectedFile.PrescriptionDdatas)
            {
                foreach (var errorList in SelectedFile.ErrorPrescriptionList.ErrorList)
                {
                    if (!errorList.PrescriptionId.Equals(d.DecId) || errorList.Error.Count <= 0) continue;
                    d.HasError = true;
                    d.CanDeclare = false;
                }
            }
            PrescriptionCollection = new ObservableCollection<DeclareFileDdata>(SelectedFile.PrescriptionDdatas);
            PrescriptionList.ItemsSource = PrescriptionCollection;
            if (ErrorDec.IsChecked == true)
            {
                foreach (var d in SelectedFile.PrescriptionDdatas)
                {
                    if (d.HasError)
                    {
                        d.CanDeclare = false;
                    }
                }
            }
            else
            {
                foreach (var d in SelectedFile.PrescriptionDdatas)
                {
                    d.CanDeclare = true;
                }
            }
        }

        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            ObservableCollection<Hospital> tempCollection = new ObservableCollection<Hospital>(Hospitals.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50).ToList());
            ReleasePalace.ItemsSource = tempCollection;
            ReleasePalace.PopulateComplete();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CreateDeclareFileClick(object sender, RoutedEventArgs e)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            var sDate = Start.Text == "" ? "" : "0" + taiwanCalendar.GetYear(Convert.ToDateTime(Start.Text)) + "/" + Convert.ToDateTime(Start.Text).ToString("MM/dd");

            var eDate = End.Text == "" ? "" : "0" + taiwanCalendar.GetYear(Convert.ToDateTime(End.Text)) + "/" + Convert.ToDateTime(End.Text).ToString("MM/dd");
            string adjustId = "";
            if (AdjustCaseCombo.Text != String.Empty)
                adjustId = AdjustCaseCombo.Text.Substring(0, 1);
            string insName = "";
            if (ReleasePalace.Text != String.Empty)
                insName = ReleasePalace.Text.Split(' ')[1];
            
        }

        private void SaveDeclareFileButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
