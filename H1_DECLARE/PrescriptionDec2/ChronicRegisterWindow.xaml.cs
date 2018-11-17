using His_Pos.Class;
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
                DecMasid = row["HISDECMAS_ID"].ToString();
                CusName = row["CUS_NAME"].ToString();
                HospitalName = row["INS_NAME"].ToString();
                DivisionName = row["HISDIV_NAME"].ToString();
                Status = row["CHRONIC_STATUS"].ToString();
                TreatmentDate = Convert.ToDateTime(row["HISDECMAS_TREATDATE"].ToString()).AddYears(-1911).ToString("yyy/MM/dd");

            }
            public bool IsRegister { get; set; } = true;
            public string DecMasid { get; set; }
            public string CusName { get; set; }
            public string HospitalName { get; set; }
            public string DivisionName { get; set; }
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
            InitData(DecMasId);
        }
        private void InitData(string DecMasId) {
            ChronicRegisterCollection = ChronicDb.GetChronicGroupById(DecMasId);
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonSubmmit_Click(object sender, RoutedEventArgs e)
        {
            foreach (ChronicRegister chronicRegister in ChronicRegisterCollection) {
                if (chronicRegister.IsRegister)
                    ChronicDb.UpdateRegisterStatus(chronicRegister.DecMasid);
            }
            Close();
        }
    }
}
