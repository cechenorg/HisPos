using His_Pos.Class;
using His_Pos.Class.Product;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.H1_DECLARE.PrescriptionDec2 {
    /// <summary>
    /// ChronicSendToServerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChronicSendToServerWindow : Window , INotifyPropertyChanged {

        public class PrescriptionSendData:INotifyPropertyChanged {
            public PrescriptionSendData(DeclareMedicine declareMedicine) {
                MedId = declareMedicine.Id;
                MedName = declareMedicine.Name;
                Stock = ChronicDb.GetResidualAmountById(MedId);
                TreatAmount = declareMedicine.Amount.ToString();
                SendAmount = TreatAmount;
            }

            public string MedId { get; set; }
            public string MedName { get; set; }
            public string Stock { get; set; }
            public string TreatAmount { get; set; }
            public string SendAmount { get; set; }
            private string source;

            public string Source
            {
                get { return source; }
                set
                {
                    source = value;
                    NotifyPropertyChanged("Source");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      
        private ObservableCollection<PrescriptionSendData> prescription = new ObservableCollection<PrescriptionSendData>();
        public ObservableCollection<PrescriptionSendData> Prescription {
            get => prescription;
            set
            {
                prescription = value;
                NotifyPropertyChanged("Prescription");
            }
        }
        public static ChronicSendToServerWindow Instance;
        private string DecMasId = string.Empty;
        public ChronicSendToServerWindow(ObservableCollection<DeclareMedicine> medicines,string decMasId) {
            InitializeComponent();
            foreach (DeclareMedicine row in medicines) {
                PrescriptionSendData prescription = new PrescriptionSendData(row);
                if (!row.IsBuckle) 
                    prescription.SendAmount = "0";

                
                Prescription.Add(prescription);
            }
            DecMasId = decMasId;
            DataContext = this;
            Instance = this;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ButtonSubmmit_Click(object sender, RoutedEventArgs e) {
            bool checkAllZero = true;
            foreach (PrescriptionSendData prescriptionSendData in Prescription) {
                if (prescriptionSendData.SendAmount != "0") 
                    checkAllZero = false;
                
                if (Convert.ToInt32(prescriptionSendData.SendAmount) > Convert.ToInt32(prescriptionSendData.TreatAmount)) {
                    MessageWindow messageWindow = new MessageWindow("傳輸量不可大於調劑量", MessageType.ERROR, true);
                    messageWindow.ShowDialog();
                    return;
                } 
            }
            if (checkAllZero)
            {
                MessageWindow messageWindow = new MessageWindow("傳輸量不可為0", MessageType.ERROR, true);
                messageWindow.ShowDialog();
                return;
            }
            PrescriptionDec2View.Instance.IsSend = true;
            PrescriptionDec2View.Instance.PrescriptionSendData = Prescription;
            ChronicDb.InsertChronicDetail(Prescription,DecMasId);
            Close();
        }
        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Prescription.RemoveAt(MedicinesList.SelectedIndex);
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;
                if (Prescription.Contains(selectedItem))
                    (selectedItem as PrescriptionSendData).Source = "/Images/DeleteDot.png"; 
                MedicinesList.SelectedItem = selectedItem; 
        } 

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow)?.Item;
             (leaveItem as PrescriptionSendData).Source = string.Empty;
        }
    }
}
