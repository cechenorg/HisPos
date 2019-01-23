using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.MedicinesSendSingdeWindow
{
    /// <summary>
    /// MedicinesSendSingdeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedicinesSendSingdeWindow : Window
    {
        public MedicinesSendSingdeWindow()
        {
            InitializeComponent();
        }
        public class PrescriptionSendData : INotifyPropertyChanged
        {
            public PrescriptionSendData()//Product declareMedicine)
            {
               // MedId = declareMedicine.Id;
               // MedName = declareMedicine.Name;
               // Stock = "";/// ChronicDb.GetResidualAmountById(MedId); 
               // TreatAmount = declareMedicine is DeclareMedicine ? ((DeclareMedicine)declareMedicine).Amount.ToString() : ((PrescriptionOTC)declareMedicine).Amount.ToString();
               // SendAmount = declareMedicine is DeclareMedicine ? TreatAmount : "0";
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

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<PrescriptionSendData> prescription = new ObservableCollection<PrescriptionSendData>();
        public ObservableCollection<PrescriptionSendData> Prescription
        {
            get => prescription;
            set
            {
                prescription = value;
                NotifyPropertyChanged("Prescription");
            }
        }
        public static ChronicSendToServerWindow Instance;
        private string DecMasId = string.Empty;
      

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonSubmmit_Click(object sender, RoutedEventArgs e)
        {
           
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
