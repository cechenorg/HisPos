using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.Person.MedicalPerson;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AdjustPharmacistViewModel : ViewModelBase
    {
        public static MonthViewCalendar monthViewCalendar;
        public MonthViewCalendar MonthViewCalendar
        {
            get => monthViewCalendar;
            set
            {
                Set(() => MonthViewCalendar, ref monthViewCalendar, value);
            }
        }

        public string DeclareMonth => (CurrentDate.Year - 1911) + "年" + CurrentDate.Month + "月";
        public static DateTime CurrentDate { get; set; }
        private static DateTime first { get; set; }
        private static DateTime last { get; set; }
        private static DateTime _selectedDate;
        public static DateTime MySelectedDate
        {
            get => _selectedDate;
            set
            {
                var temp = _selectedDate;
                if (value >= first && value <= last)
                {
                    _selectedDate = value;
                }
                else
                {
                    _selectedDate = temp;
                }
            }
        }
        private DateTime _displayDate;
        public DateTime MyDisplayDate
        {
            get => _displayDate;
            set
            {
                if (value >= first && value <= last)
                {
                    _displayDate = value;
                }
                else
                {
                    _displayDate = CurrentDate;
                }
            }
        }
        public RelayCommand DeleteAppointment { get; set; }
        public RelayCommand AddAppointment { get; set; }
        public static MedicalPersonnels MedicalPersonnels { get; set; }
        public AdjustPharmacistViewModel(DateTime declare)
        {
            DeleteAppointment = new RelayCommand(DeleteAppointmentAction);
            AddAppointment = new RelayCommand(AddAppointmentAction);
            CurrentDate = declare;
            MyDisplayDate = declare;
            first = new DateTime(declare.AddMonths(1).Year, declare.Month, 1);
            last = new DateTime(declare.AddMonths(1).Year, declare.AddMonths(1).Month, 1).AddDays(-1);
            MedicalPersonnels = new MedicalPersonnels(false);
            MainWindow.ServerConnection.OpenConnection();
            MedicalPersonnels.GetEnablePharmacist(first,last);
            MainWindow.ServerConnection.CloseConnection();
            MonthViewCalendar = new MonthViewCalendar(declare);
        }

        private void AddAppointmentAction()
        {
            //if (MySelectedDate != null)
            //{
            //    var appointmentWindow = new AppointmentWindow
            //    (
            //        appointment =>
            //        {
            //            MonthViewCalendar.Appointments.Add(appointment);
            //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Appointments"));
            //        }, MySelectedDate
            //    );
            //    appointmentWindow.Show();
            //}
        }

        private void DeleteAppointmentAction()
        {
            MonthViewCalendar.Appointments.Remove(MonthViewCalendar.SelectedAppointment);
        }
    }
}
