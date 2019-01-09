using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.Class.Product;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// CooperativePrescriptSelectWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativePrescriptSelectWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static CooperativePrescriptSelectWindow Instance;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<CooperativeClinic> cooperativeCollection;
        public ObservableCollection<CooperativeClinic> CooperativeCollection
        {
            get
            {
                return cooperativeCollection;
            }
            set
            {
                cooperativeCollection = value;
                NotifyPropertyChanged("CooperativeCollection");
            }
        }
        private ObservableCollection<CustomerDeclare> customerDeclaresCollection = new ObservableCollection<CustomerDeclare>();
        public ObservableCollection<CustomerDeclare> CustomerDeclaresCollection
        {
            get
            {
                return customerDeclaresCollection;
            }
            set
            {
                customerDeclaresCollection = value;
                NotifyPropertyChanged("CustomerDeclaresCollection");
            }
        }
        private ObservableCollection<Product> medicineInfo;
        public ObservableCollection<Product> MedicineInfo
        {
            get
            {
                return medicineInfo;
            }
            set
            {
                medicineInfo = value;
                NotifyPropertyChanged("MedicineInfo");
            }
        }
        public class CustomerDeclare{
            public CustomerDeclare() { }
            public CustomerDeclare(Prescription prescription) {
                DeclareData declareData = new DeclareData(prescription);
                DecMasId = string.Empty;
                HospitalName = declareData.Prescription.Treatment.MedicalInfo.Hospital.Name;
                DivName = declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Name;
                AdjustDate = declareData.Prescription.Treatment.TreatmentDate.AddYears(-1911).ToString("yyy/MM/dd");
                Point = declareData.D18TotalPoint.ToString();
                Medicines = declareData.Prescription.Medicines;
            }
            public CustomerDeclare(DataRow dataRow) {
                DecMasId = dataRow["HISDECMAS_ID"].ToString();
                HospitalName = dataRow["INS_NAME"].ToString();
                DivName = dataRow["HISDIV_NAME"].ToString();
                AdjustDate = Convert.ToDateTime(dataRow["HISDECMAS_TREATDATE"].ToString()).AddYears(-1911).ToString("yyy/MM/dd");
                Point = dataRow["HISDECMAS_TOTALPOINT"].ToString();
                Medicines = null;/// MedicineDb.GetDeclareMedicineByMasId(DecMasId);
            }  
            public string DecMasId { get; set; }
            public string HospitalName { get; set; }
            public string DivName { get; set; }
            public string AdjustDate { get; set; }
            public string Point { get; set; }

            public ObservableCollection<Product> medicines;
            public ObservableCollection<Product> Medicines
            {
                get
                {
                    return medicines;
                }
                set
                {
                    medicines = value;
                    NotifyPropertyChanged("Medicines");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private bool unPrescript = true;
        public bool UnPrescript
        {
            get => unPrescript;
            set
            {
                unPrescript = value;
                NotifyPropertyChanged("UnPrescript");
            }
        }
        private bool prescript = false;
        public bool Prescript
        {
            get => prescript;
            set
            {
                prescript = value;
                NotifyPropertyChanged("Prescript");
            }
        }
        private DateTime startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }
        private DateTime endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }
  
        public CooperativePrescriptSelectWindow()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            InitData();
        }
        private void InitData() {
            CooperativeCollection = WebApi.GetXmlByDate(MainWindow.CurrentPharmacy.Id, StartDate, EndDate);//MainWindow.CurrentPharmacy.Id
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate.Date <= EndDate.Date.AddDays(-3)) {
               /// MessageWindow.ShowMessage("日期區間只能為三天內^^",MessageType.ERROR);
               /// messageWindow.ShowDialog();
                return;
            }
            CooperativeCollection = WebApi.GetXmlByDate(MainWindow.CurrentPharmacy.Id, StartDate, EndDate);//MainWindow.CurrentPharmacy.Id
        }

        private void start_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox t)
            {
                t.SelectionStart = 0;
                t.SelectionLength = t.Text.Length;
            }
        }

        private void DataGridCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectitem = (sender as DataGrid).SelectedItem;
            if (selectitem is null) return;
            WebApi.UpdateIsReadByDeclareId(((CooperativeClinic)selectitem).DeclareId);
            ((CooperativeClinic)selectitem).IsRead = "已讀";
            CustomerDeclaresCollection = null;///DeclareDb.GetDeclareHistoryByCusIdnum(( (CooperativeClinic)selectitem).Prescription.Customer.IcCard.IcNumber); 
            CustomerDeclaresCollection.Insert(0,(new CustomerDeclare(((CooperativeClinic)selectitem).Prescription)));
            MedicineInfo = null;
            DataGridCooperativeClinic.SelectedIndex = 0;
        }

        private void UnPrescript_Checked(object sender, RoutedEventArgs e)
        {
            DataGridCustomer.Items.Filter = ((o) => {
                var temp = (CooperativeClinic)o;
                if (temp.IsRead == "未讀" && UnPrescript && (temp.Prescription.Customer.IcCard.IcNumber.Contains(Condition.Text) || string.IsNullOrEmpty(Condition.Text)))
                    return true;
                else if (temp.IsRead == "已讀" && Prescript && (temp.Prescription.Customer.IcCard.IcNumber.Contains(Condition.Text) || string.IsNullOrEmpty(Condition.Text)))
                    return true;
                else
                    return false;
            });
        }

        private void DataGridCooperativeClinic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectitem = (sender as DataGrid).SelectedItem;
            if (selectitem is null) return;
            MedicineInfo = ((CustomerDeclare)selectitem).Medicines;
        }

        private void DataGridCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = DataGridCustomer.SelectedItem;
            if (selectedItem is null) return;
            PrescriptionDec2View.Instance.SetValueByPrescription(((CooperativeClinic)selectedItem)); 
            Close();
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e) {
            if (DataGridCustomer.SelectedItem is null) return;
            CooperativeClinic selectItem = DataGridCustomer.SelectedItem as CooperativeClinic; 
            DeclareData declareData = new DeclareData(selectItem.Prescription);
            double medPoint = 0;
            int selfCost = 0;
            foreach (Product product in selectItem.Prescription.Medicines) {
                if(product is DeclareMedicine)
                {
                    var tempMed = ((DeclareMedicine)PrescriptionDec2View.Instance.DeclareMedicines.SingleOrDefault(med => med.Id == product.Id));
                    ((DeclareMedicine)product).Ingredient = tempMed.Ingredient;
                    ((DeclareMedicine)product).Indication = tempMed.Indication;
                    ((DeclareMedicine)product).SideEffect = tempMed.SideEffect;
                    if (!((DeclareMedicine)product).PaySelf)
                    { 
                        medPoint += tempMed.HcPrice * ((DeclareMedicine)product).Amount;
                    }
                    else {
                        selfCost += Convert.ToInt32(((DeclareMedicine)product).TotalPrice);
                    }

                }
            }
            
            NewFunction.PrintMedBag(selectItem.Prescription, declareData, medPoint, selfCost, selfCost, "合作", selfCost, null,null, Instance); 
        }
    }
}
