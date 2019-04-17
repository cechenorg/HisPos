using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
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

        /// <summary>
        /// The list of appointments. This is a dependency property.
        /// </summary>
        public ObservableCollection<Appointment> Appointments
        {
            get => (ObservableCollection<Appointment>)GetValue(AppointmentsProperty);
            set => SetValue(AppointmentsProperty, value);
        }

        static MonthViewCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthViewCalendar), new FrameworkPropertyMetadata(typeof(MonthViewCalendar)));
        }

        public MonthViewCalendar() : base()
        {
            SetValue(AppointmentsProperty, new ObservableCollection<Appointment>());
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            var element = e.OriginalSource as FrameworkElement;

            if (element.DataContext is DateTime)
            {
                AppointmentWindow appointmentWindow = new AppointmentWindow
                (
                    (Appointment appointment) =>
                    {
                        Appointments.Add(appointment);
                        if (PropertyChanged != null)
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("Appointments"));
                        }
                    }
                );
                appointmentWindow.Show();
            }
        }
    }
}
