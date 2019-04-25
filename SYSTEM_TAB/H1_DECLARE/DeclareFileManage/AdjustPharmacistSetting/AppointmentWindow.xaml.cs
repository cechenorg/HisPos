using System;
using System.Collections.Generic;
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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    /// <summary>
    /// AppointmentWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AppointmentWindow : Window
    {
        private Action<Appointment> saveCallback;
        private DateTime selectedDate;
        public AppointmentWindow(Action<Appointment> saveCallback,DateTime selected)
        {
            InitializeComponent();
            selectedDate = selected;
            this.saveCallback = saveCallback;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Appointment appointment = new Appointment();
            //appointment.MedicalPersonnel = subjectTbx.Text;
            appointment.Date = selectedDate;

            saveCallback(appointment);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
