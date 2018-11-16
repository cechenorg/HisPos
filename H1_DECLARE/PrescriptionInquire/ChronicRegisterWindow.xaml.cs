using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace His_Pos.H1_DECLARE.PrescriptionInquire
{
    /// <summary>
    /// ChronicRegisterWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChronicRegisterWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public class ChronicRegister {

            public ChronicRegister(DataRow row) {
                DecMasid = row[""].ToString();
                CusName = row[""].ToString();
                HospitalName = row[""].ToString();
                Division = row[""].ToString();
                Status = row[""].ToString();
                TreatmentDate = row[""].ToString();

            }
            public string DecMasid { get; set; }
            public string CusName { get; set; }
            public string HospitalName { get; set; }
            public string Division { get; set; }
            public string Status { get; set; }
            public string TreatmentDate { get; set; }
        }
        private ObservableCollection<ChronicRegister> chronicRegisterCollection = new ObservableCollection<ChronicRegister>();
        public ObservableCollection<ChronicRegister> ChronicRegisterCollection
        {
            get
            {
                return chronicRegisterCollection;
            }
            set
            {
                chronicRegisterCollection = value;
                NotifyPropertyChanged("ChronicRegisterCollection");
            }
        }
        public ChronicRegisterWindow(string DecMasId)
        {
            InitializeComponent();
            DataContext = this;
        }
        private void InitData() {



        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonSubmmit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
