using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Windows.Controls;
using Calendar = Microsoft.Windows.Controls.Calendar;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class MonthViewCalendar : Calendar, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public static DependencyProperty AppointmentsProperty =
            DependencyProperty.Register
            (
                "Appointments",
                typeof(ObservableCollection<Appointment>),
                typeof(Calendar)
            );

        public static DependencyProperty AppointmentProperty =
            DependencyProperty.Register
            (
                "Appointment",
                typeof(Appointment),
                typeof(Calendar)
            );
        private ObservableCollection<Appointment> appointments;
        /// <summary>
        /// The list of appointments. This is a dependency property.
        /// </summary>
        public ObservableCollection<Appointment> Appointments
        {
            get => appointments;
            set
            {
                appointments = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Appointments"));
            }
            //get => (ObservableCollection<Appointment>)GetValue(AppointmentsProperty);
            //set => SetValue(AppointmentsProperty, value);
        }

        private Appointment selectedAppointment;
        public Appointment SelectedAppointment
        {
            get => selectedAppointment;
            set
            {
                selectedAppointment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedAppointment"));
            }
        }

        static MonthViewCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthViewCalendar), new FrameworkPropertyMetadata(typeof(MonthViewCalendar)));
        }

        public MonthViewCalendar() : base()
        {
            //SetValue(AppointmentsProperty, new ObservableCollection<Appointment>());
            //DisplayDate = AdjustPharmacistViewModel.CurrentDate;
        }

        public MonthViewCalendar(DateTime declare) : base()
        {
            DateTime first = new DateTime(declare.AddMonths(1).Year, declare.Month, 1);
            DateTime last = new DateTime(declare.AddMonths(1).Year, declare.AddMonths(1).Month, 1).AddDays(-1);
            CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, first.AddDays(-1));
            CalendarDateRange cdr2 = new CalendarDateRange(last.AddDays(1), DateTime.MaxValue);
            BlackoutDates.Add(cdr);
            BlackoutDates.Add(cdr2);
            SetValue(AppointmentsProperty, new ObservableCollection<Appointment>());
            DisplayDate = declare;
            Appointments = new ObservableCollection<Appointment>();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            //var element = e.OriginalSource as FrameworkElement;

            //if (element.DataContext is DateTime)
            //{
            //    var appointmentWindow = new AppointmentWindow
            //    (
            //        appointment =>
            //        {
            //            Appointments.Add(appointment);
            //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Appointments"));
            //        }
            //        ,AdjustPharmacistViewModel.MySelectedDate
            //    );
            //    appointmentWindow.Show();
            //}
        }

        public void RemoveSelectedAppointment()
        {
            Appointments.Remove(SelectedAppointment);
        }
    }
}
