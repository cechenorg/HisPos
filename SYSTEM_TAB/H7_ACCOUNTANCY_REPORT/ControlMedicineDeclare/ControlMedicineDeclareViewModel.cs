using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.WareHouse;
using System;
using System.ComponentModel;
using System.Windows.Data;
using His_Pos.NewClass.Medicine.ControlMedicineDeclare;
using His_Pos.NewClass.Medicine.ControlMedicineDetail;
using System.Windows.Forms;
using System.IO;
using System.Text;
using His_Pos.FunctionWindow;
using His_Pos.Class;
using System.Linq;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare
{
    public class ControlMedicineDeclareViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region Var
        private DateTime sDateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime SDateTime
        {
            get { return sDateTime; }
            set
            {
                Set(() => SDateTime, ref sDateTime, value);
            }
        }
        private DateTime eDateTime = DateTime.Now;
        public DateTime EDateTime
        {
            get { return eDateTime; }
            set
            {
                Set(() => EDateTime, ref eDateTime, value);
            }
        }
        private CollectionViewSource controlCollectionViewSource;
        private CollectionViewSource ControlCollectionViewSource
        {
            get => controlCollectionViewSource;
            set
            {
                Set(() => ControlCollectionViewSource, ref controlCollectionViewSource, value); 
            }
        }

        private ICollectionView controlCollectionView;
        public ICollectionView ControlCollectionView
        {
            get => controlCollectionView;
            private set
            {
                Set(() => ControlCollectionView, ref controlCollectionView, value);
            }
        }


        private ControlMedicineDeclares controlMedicineDeclares = new ControlMedicineDeclares();
        public ControlMedicineDeclares ControlMedicineDeclares
        {
            get { return controlMedicineDeclares; }
            set
            {
                Set(() => ControlMedicineDeclares, ref controlMedicineDeclares, value);
            }
        }
        private NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare  selectItem;
        public NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare SelectItem
        {
            get { return selectItem; }
            set
            {
                Set(() => SelectItem, ref selectItem, value);
            }
        }
        private ControlMedicineDetails controlMedicineDetailsCollection = new ControlMedicineDetails();
        public ControlMedicineDetails ControlMedicineDetailsCollection
        {
            get { return controlMedicineDetailsCollection; }
            set
            {
                Set(() => ControlMedicineDetailsCollection, ref controlMedicineDetailsCollection, value);
            }
        }
        private ControlMedicineDetails controlMedicineBagDetailsCollection = new ControlMedicineDetails();
        public ControlMedicineDetails ControlMedicineBagDetailsCollection
        {
            get { return controlMedicineBagDetailsCollection; }
            set
            {
                Set(() => ControlMedicineBagDetailsCollection, ref controlMedicineBagDetailsCollection, value);
            }
        }
        private WareHouse selectedWareHouse;
        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set
            {
                Set(() => SelectedWareHouse, ref selectedWareHouse, value); 
            }
        }
        public WareHouses WareHouseCollection { get; set; } = WareHouses.GetWareHouses();
        #endregion
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand SearchCommand { get; set; } 
        public RelayCommand WareHouseSelectionChangedCommand { get; set; }
        public RelayCommand PrintMaserCommand { get; set; }
        public RelayCommand PrintDetailCommand { get; set; }
        public RelayCommand ExportControlMedDeclareFileCommand { get; set; }
        public RelayCommand ControlMedEditCommand { get; set; }

        public ControlMedicineDeclareViewModel()
        {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
            SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
            SearchCommand = new RelayCommand(SearchAction);
            WareHouseSelectionChangedCommand = new RelayCommand(WareHouseSelectionChangedAction);
            PrintMaserCommand = new RelayCommand(PrintMaserAction);
            PrintDetailCommand = new RelayCommand(PrintDetailAction);
            ExportControlMedDeclareFileCommand = new RelayCommand(ExportControlMedDeclareFileAction);
            ControlMedEditCommand = new RelayCommand(ControlMedEditAction);
            SelectedWareHouse = WareHouseCollection[0];
            SearchAction();
        }
        private void ControlMedEditAction() {
            ControlMedicineEditWindow.ControlMedicineEditWindow controlMedicineEditWindow = new ControlMedicineEditWindow.ControlMedicineEditWindow(SelectItem.ID,SelectedWareHouse.ID);    
        }
        private void PrintDetailAction() {
            if (ControlMedicineDetailsCollection is null) return;

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "管制藥品收支結存簿冊";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = SDateTime.ToString("yyyyMMdd") + "_" + EDateTime.ToString("yyyyMMdd") + "_" + SelectItem.ID + "管制藥品收支結存簿冊";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("管制藥品收支結存簿冊");
                        file.WriteLine($"健保碼,{SelectItem.ID}");
                        file.WriteLine($"藥品名稱,{SelectItem.FullName}");
                        file.WriteLine("日期,收支原因,收入數量,批號,支出數量,結存數量,備註");
                        foreach (var c in ControlMedicineDetailsCollection)
                        { 
                            file.WriteLine($"{c.Date},{c.TypeName},{c.InputAmount},{c.BatchNumber},{c.OutputAmount},{c.FinalStock},{c.Description}");
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        private void PrintMaserAction() {
            if (ControlCollectionView is null) return; 

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "管制藥品收支結存簿冊";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = SDateTime.ToString("yyyyMMdd") + "_" + EDateTime.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "管制藥品收支結存簿冊主檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine(SDateTime.ToString("yyyyMMdd") + "_" + EDateTime.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "管制藥品收支結存簿冊");
                        file.WriteLine($"庫別:{SelectedWareHouse.Name}");
                        file.WriteLine("級別,健保碼,名稱,進貨,支出,結存");
                        foreach (var c in ControlCollectionView) {
                            var conMed = ((NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare)c);
                            file.WriteLine($"{conMed.IsControl},{conMed.ID},{conMed.FullName},{conMed.GetValue},{conMed.PayValue},{conMed.FinalValue}");
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        private void WareHouseSelectionChangedAction() {
            ControlCollectionViewSource.Filter += Filter;
        }
        private void SearchAction()
        {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
            SelectItem = ControlMedicineDeclares[0];
            SelectionChangedAction();
            ControlCollectionViewSource = new CollectionViewSource { Source = ControlMedicineDeclares };
            ControlCollectionView = ControlCollectionViewSource.View;
            ControlCollectionViewSource.Filter += Filter;
           
        }
        private void SelectionChangedAction()
        {
            if (SelectItem == null) return;
            ControlMedicineDetails temp = new ControlMedicineDetails();
            temp.GetDataById(SelectItem.ID, SDateTime, EDateTime, SelectItem.InitStock,SelectedWareHouse.ID);
            ControlMedicineBagDetailsCollection.Clear();
            ControlMedicineDetailsCollection.Clear();
            foreach (ControlMedicineDetail c in temp)
            {
                switch (c.TypeName)
                {
                    case "調劑(未過卡)":
                        ControlMedicineBagDetailsCollection.Add(c);
                        break;
                    default:
                        ControlMedicineDetailsCollection.Add(c);
                        break;
                }
            }
        }
        private void Filter(object sender, FilterEventArgs e) {
            if (e.Item is null) return;
            if (!(e.Item is NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare src))
                e.Accepted = false;
            NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare controlMedicineDeclare = ((NewClass.Medicine.ControlMedicineDeclare.ControlMedicineDeclare)e.Item);
            e.Accepted = controlMedicineDeclare.WareHouse.ID == SelectedWareHouse.ID; 
        }
        private void ExportControlMedDeclareFileAction() {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "管制藥品批次申報檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Txt檔案|*.txt";
            fdlg.FileName = SDateTime.ToString("yyyy") + ViewModelMainWindow.CurrentPharmacy.Name + "管制藥品批次申報檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        ControlMedicineDetails controlMedicineDetails = ControlMedicineDetails.GetDeclareData(SDateTime,EDateTime,SelectedWareHouse.ID);
                      
                        int index = 1;
                        foreach (var c in controlMedicineDetails)
                        {   
                                string isgetpay = controlMedicineDetails.Count(ta => ta.MedID == c.MedID) > 1 ? "Y" : "N";
                                file.WriteLine($"" +
                                     $"{index}," +
                                     $"P," +
                                     $"{c.MedID}," +
                                     $"," +
                                     $"{isgetpay}," +
                                     $"{c.PackageName}," +
                                     $"{c.Date.AddYears(-1911).ToString("yyyMMdd")},{c.TypeName}," +
                                     $"{c.InputAmount},{c.InputAmount},,,,,,,,,,,,,,,,,,");
                                index++;  
                        }
                           
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
    }
}
