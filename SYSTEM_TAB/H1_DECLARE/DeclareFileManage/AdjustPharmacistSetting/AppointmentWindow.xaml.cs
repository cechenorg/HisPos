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
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    /// <summary>
    /// AppointmentWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AppointmentWindow : Window
    {
        private AppointmentViewModel appointmentViewModel;
        public AppointmentWindow(Action<Appointment> saveCallback,DateTime selected)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAppointmentWindow"))
                    Close();
            });
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            appointmentViewModel = new AppointmentViewModel(saveCallback,selected);
            DataContext = appointmentViewModel;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //Appointment appointment = new Appointment();
            ////appointment.MedicalPersonnel = subjectTbx.Text;
            //appointment.Date = selectedDate;

            //saveCallback(appointment);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
