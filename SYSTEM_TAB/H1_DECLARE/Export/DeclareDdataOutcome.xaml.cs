using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Usage;
using His_Pos.NewClass.Usage;
using His_Pos.Service;
using JetBrains.Annotations;
using Prescription = His_Pos.Class.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.Export
{
    /// <summary>
    /// DeclareDdataOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class DeclareDdataOutcome : Window, INotifyPropertyChanged
    {
        private bool _isFirst = true;
        public static DeclareDdataOutcome Instance;
        public ObservableCollection<Hospital> Hospitals { get; set; }
        public ObservableCollection<Division> Divisions { get; set; }
        public PrescriptionCases TreatmentCases { get; set; }
        public ObservableCollection<PaymentCategory> PaymentCategories { get; set; }
        public Copayments Copayments { get; set; }
        public ObservableCollection<AdjustCase> AdjustCases { get; set; }
        public ObservableCollection<MedicalPersonnel> MedicalPersonnels { get; set; }

        private ObservableCollection<Product> _prescriptionMedicines;

        public ObservableCollection<Product> PrescriptionMedicines
        {
            get => _prescriptionMedicines;
            set
            {
                _prescriptionMedicines = value;
                OnPropertyChanged(nameof(PrescriptionMedicines));
            }
        }

        private DeclareFileDdata _currentFile;

        public DeclareFileDdata CurrentFile
        {
            get => _currentFile;
            set
            {
                _currentFile = value;
                OnPropertyChanged(nameof(CurrentFile));
            }
        }

        public ObservableCollection<Usage> Usages { get; set; }
        private string _gender;

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        private Prescription _currentDeclareFileDdata;

        public Prescription CurrentPrescription
        {
            get => _currentDeclareFileDdata;
            set
            {
                _currentDeclareFileDdata = value;
                if(_currentDeclareFileDdata.Customer.IcNumber.Length > 1)
                    Gender = _currentDeclareFileDdata.Customer.IcNumber.Substring(1, 1).Equals("2") ? "女" : "男";
                OnPropertyChanged(nameof(CurrentPrescription));
            }
        }

        public DeclareDdataOutcome()
        {
            InitializeComponent();
        }

        private ObservableCollection<object> _medicines;

        public DeclareDdataOutcome(DeclareFileDdata d)
        {
            InitializeComponent();
            Instance = this;
            InitData(d);
            InitDataChanged();
            DataContext = this;
            OnPropertyChanged(nameof(CurrentPrescription));
        }

        private void InitData(DeclareFileDdata declareFileDdata)
        {
            if (ExportView.Instance is null) return;
            CurrentFile = declareFileDdata;
            CurrentPrescription = new Prescription(CurrentFile);
            PrescriptionMedicines = new ObservableCollection<Product>();
            foreach (var p in declareFileDdata.Dbody.Pdata)
            {
                if (!p.P1.Equals("1")) continue;
                var d = new DeclareMedicine();
                foreach (var medicine in ExportView.Instance.DeclareMedicinesData)
                {
                    if (p.P2.Equals(medicine.Id))
                        d = medicine;
                }

                if (string.IsNullOrEmpty(d.Id)) continue;

                d.Dosage = double.Parse(p.P3.TrimStart('0'));
                d.UsageName = p.P4;
                d.Position = p.P5;
                d.Amount = double.Parse(p.P7);

                if(p.P8.TrimStart('0').StartsWith("."))
                    d.HcPrice = 0.00;

                d.TotalPrice = double.Parse(p.P8.TrimStart('0'))* double.Parse(p.P7);
                d.Days = p.P11;
                PrescriptionMedicines.Add(d);
            }
        }

        private void Text_TextChanged(object sender, EventArgs e)
        {
            DataChanged();
        }

        private void DataChanged()
        {
            if (_isFirst) return;

            Changed.Content = "已修改";
            Changed.Foreground = Brushes.Red;

            ButtonImportXml.IsEnabled = true;
        }

        private void InitDataChanged()
        {
            Changed.Content = "未修改";
            Changed.Foreground = Brushes.Black;

            ButtonImportXml.IsEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MedicineTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.Text.Length;
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;

            if (selectedItem is IDeletable)
            {
                if (PrescriptionMedicines.Contains(selectedItem))
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";

                PrescriptionSet.SelectedItem = selectedItem;
                return;
            }

            PrescriptionSet.SelectedIndex = PrescriptionMedicines.Count;

        }


        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow)?.Item;

            if (leaveItem is IDeletable) (leaveItem as IDeletable).Source = string.Empty;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearMedicine((DeclareMedicine)PrescriptionMedicines[PrescriptionSet.SelectedIndex]);
            DataChanged();
            PrescriptionMedicines.RemoveAt(PrescriptionSet.SelectedIndex);
        }

        private void ClearMedicine(DeclareMedicine med)
        {
            med.PaySelf = false;
            med.Cost = 0;
            med.TotalPrice = 0;
            med.Amount = 0;
            med.CountStatus = string.Empty;
            med.FocusColumn = string.Empty;
            med.Usage = new Usage();
            med.Days = string.Empty;
            med.Position = string.Empty;
            med.Source = string.Empty;
            med.Dosage = 0;
        }

        private void MedicineCodeAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;
            DataChanged();
            if (medicineCodeAuto is null) return;
            if (medicineCodeAuto.SelectedItem is null)
            {
                if (medicineCodeAuto.Text != string.Empty &&
                    ((ObservableCollection<object>) medicineCodeAuto.ItemsSource).Count != 0 &&
                    medicineCodeAuto.Text.Length >= 4)
                {
                    medicineCodeAuto.SelectedItem = (medicineCodeAuto.ItemsSource as ObservableCollection<object>)?[0];
                }
                else
                    return;
            }

            var declareMedicine = (DeclareMedicine) (medicineCodeAuto.SelectedItem as DeclareMedicine)?.Clone();
            var currentRow = GetCurrentRowIndex(sender);

            if (PrescriptionMedicines.Count > 0)
            {
                if (PrescriptionMedicines.Count == currentRow)
                {
                    PrescriptionMedicines.Add(declareMedicine);
                    medicineCodeAuto.Text = string.Empty;
                }
                else
                {
                    var d = declareMedicine;
                    if (d == null) return;
                    PrescriptionMedicines[currentRow].Id = d.Id;
                    PrescriptionMedicines[currentRow].Name = d.Name;
                }
            }
            else
            {
                PrescriptionMedicines.Add(declareMedicine);
                medicineCodeAuto.Text = string.Empty;
            }
        }

        private int GetCurrentRowIndex(object sender)
        {
            switch (sender)
            {
                case TextBox textBox:
                {
                    var temp = new List<TextBox>();

                    NewFunction.FindChildGroup<TextBox>(PrescriptionSet, textBox.Name, ref temp);

                    for (var x = 0; x < temp.Count; x++)
                        if (temp[x].Equals(textBox))
                            return x;
                    break;
                }
                case CheckBox checkBox:
                {
                    var temp = new List<CheckBox>();

                    NewFunction.FindChildGroup<CheckBox>(PrescriptionSet, checkBox.Name, ref temp);

                    for (var x = 0; x < temp.Count; x++)
                        if (temp[x].Equals(checkBox))
                            return x;
                    break;
                }
                case AutoCompleteBox _:
                {
                    var temp = new List<AutoCompleteBox>();
                    var autoCompleteBox = sender as AutoCompleteBox;
                    NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionSet, autoCompleteBox.Name, ref temp);
                    for (var x = 0; x < temp.Count; x++)
                        if (temp[x].Equals(sender))
                            return x;
                    break;
                }
            }

            return -1;
        }

        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            if (!(sender is AutoCompleteBox medicineCodeAuto)) return;

            var result = ExportView.Instance.DeclareMedicinesData.Where(x =>
                x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower())).Take(50).Select(x => x);
            _medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = _medicines;
            medicineCodeAuto.ItemFilter = MedicineFilter;
            medicineCodeAuto.PopulateComplete();
        }

        public AutoCompleteFilterPredicate<object> MedicineFilter
        {
            get
            {
                return (searchText, obj) =>
                    !((obj as DeclareMedicine)?.Id is null) && (((DeclareMedicine) obj).Id.ToLower().Contains(searchText.ToLower())
                                                                || ((DeclareMedicine) obj).ChiName.ToLower().Contains(searchText.ToLower()) ||
                                                                ((DeclareMedicine) obj).EngName.ToLower().Contains(searchText.ToLower()));
            }
        }

        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            TextBox t = sender as TextBox;
            DependencyObject dpobj = sender as DependencyObject;
            string name = dpobj.GetValue(FrameworkElement.NameProperty) as string;
            if (name.Equals("Usage"))
            {
                foreach (var u in ViewModelMainWindow.Usages)
                {
                    if (t.Text.Equals(u.QuickName))
                        t.Text = u.Name;
                }
            }
            e.Handled = true;
            MoveFocusNext(sender);
        }

        private void MoveFocusNext(object sender)
        {
            if (sender is TextBox box)
                box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (PrescriptionSet.CurrentCell.Column is null) return;

            var focusedCell = PrescriptionSet.CurrentCell.Column.GetCellContent(PrescriptionSet.CurrentCell.Item);
            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    while (child is ContentPresenter)
                    {
                        child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    }
                    if ((child is TextBox || child is AutoCompleteBox || child is CheckBox || child is TextBlock))
                        break;
                }
                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell = PrescriptionSet.CurrentCell.Column.GetCellContent(PrescriptionSet.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is TextBox || firstChild is AutoCompleteBox) && firstChild.Focusable)
                firstChild.Focus();
        }

        private void Dosage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.Text.Length;
        }

        private void NullTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox t && string.IsNullOrEmpty(t.Text))
                t.Text = "0";
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isFirst = false;
        }

        private void ButtonUpdatePrescriptionXml_Click(object sender, RoutedEventArgs e)
        {
            MessageWindow m;
            CurrentPrescription.Medicines = PrescriptionMedicines;
            CurrentPrescription.EList.Error = new List<Error>();
            CurrentPrescription.EList.Error.Clear();
            CurrentPrescription.EList.Error = CurrentPrescription.CheckPrescriptionData();

            if (CurrentPrescription.EList.Error.Count == 0)
            {
                var declareData = new DeclareData(CurrentPrescription) {DecMasId = CurrentFile.DecId};

                var xmlStr = declareData.SerializeObject<Ddata>();
                var ddata = XmlService.Deserialize<Ddata>(xmlStr);
                var declareFileDdata = new DeclareFileDdata(ddata) {DecId = CurrentFile.DecId};
                for (var i = 0; i < ExportView.Instance.SelectedFile.FileContent.Ddata.Count; i++)
                {
                    if (!ExportView.Instance.SelectedFile.FileContent.Ddata[i].DecId
                        .Equals(declareFileDdata.DecId)) continue;
                    ExportView.Instance.SelectedFile.FileContent.Ddata[i].Dhead = declareFileDdata.Dhead;
                    ExportView.Instance.SelectedFile.FileContent.Ddata[i].Dbody = declareFileDdata.Dbody;
                    ExportView.Instance.SelectedFile.PrescriptionDdatas[i].Dhead = declareFileDdata.Dhead;
                    ExportView.Instance.SelectedFile.PrescriptionDdatas[i].Dbody = declareFileDdata.Dbody;
                }
                ///DeclareFileDb.SetDeclareFileByPharmacyId(ExportView.Instance.SelectedFile, Convert.ToDateTime(ExportView.Instance.SelectedFile.DeclareDate), declareData, DeclareFileType.DECLAREFILE_UPDATE);
                MessageWindow.ShowMessage("處方申報資料修改成功", MessageType.SUCCESS);   
                
                InitDataChanged();
                Close();
            }
            else
            {
                var errorMessage = "處方資料錯誤 : \n";
                foreach (var error in CurrentPrescription.EList.Error)
                {
                    errorMessage += error.Content + "\n";
                }
                MessageWindow.ShowMessage(errorMessage, MessageType.ERROR);
                
            }
        }
    }
}
