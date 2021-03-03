using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage.Leave
{
    /// <summary>
    /// LeaveWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LeaveWindow : Window
    {
        public bool LeaveComplete = false;
        public string CompleteResult = "";
        ////public ObservableCollection<UserIconData> UserIconDatas { get; }
        ////public ObservableCollection<Class.Leave.Leave> LeaveTypes { get; }

        ////public LeaveWindow(ObservableCollection<UserIconData> users)
        ////{
        ////    InitializeComponent();
        ////    DataContext = this;

        ////    ///LeaveTypes = LeaveDb.GetLeaveType();
        ////    UserIconDatas = users;
        ////    UserName.SelectedIndex = 0;
        ////}

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string errors = CheckInputData();

            if (!errors.Equals(""))
            {
                MessageWindow.ShowMessage(errors, MessageType.ERROR);

                return;
            }

            if ((bool)LHour.IsChecked)
            {
                DateTime startDateTime = (DateTime)StartDate.SelectedDate;
                startDateTime = startDateTime.AddHours(((DateTime)StartTime.SelectedTime).Hour);
                startDateTime = startDateTime.AddMinutes(((DateTime)StartTime.SelectedTime).Minute);

                DateTime endDateTime = (DateTime)StartDate.SelectedDate;
                endDateTime = endDateTime.AddHours(((DateTime)EndTime.SelectedTime).Hour);
                endDateTime = endDateTime.AddMinutes(((DateTime)EndTime.SelectedTime).Minute);

                /// CompleteResult = LeaveDb.AddNewLeave((UserName.SelectedItem as UserIconData).Id, (DayOffType.SelectedItem as Class.Leave.Leave).Id, startDateTime, endDateTime, Note.Text);
            }
            else
            {
                if (EndDate.Text.Equals(""))
                    EndDate.SelectedDate = StartDate.SelectedDate;

                ///CompleteResult = LeaveDb.AddNewLeave((UserName.SelectedItem as UserIconData).Id, (DayOffType.SelectedItem as Class.Leave.Leave).Id, (DateTime)StartDate.SelectedDate, (DateTime)EndDate.SelectedDate, Note.Text);
            }

            LeaveComplete = true;
            Close();
        }

        private string CheckInputData()
        {
            string error = "";

            if (DayOffType.SelectedItem is null)
                error += "假別未填寫!\n";

            if (Note.Text.Equals(""))
                error += "原因未填寫!\n";

            if (StartDate.Text.Equals(""))
                error += "日期未填寫!\n";
            else
            {
                if ((bool)LHour.IsChecked)
                {
                    if (StartDate.SelectedDate < DateTime.Today)
                        error += "所選日期已過!\n";

                    if (StartTime.SelectedTime is null)
                        error += "起始時間未填寫!\n";

                    if (EndTime.SelectedTime is null)
                        error += "結束時間未填寫!\n";

                    if (EndTime.SelectedTime < StartTime.SelectedTime)
                        error += "結束時間小於起始時間!\n";
                }
                else
                {
                    if (StartDate.SelectedDate < DateTime.Today || EndDate.SelectedDate < DateTime.Today)
                        error += "所選日期已過!\n";
                    else if (EndDate.SelectedDate < StartDate.SelectedDate)
                        error += "結束日期小於起始日期!\n";
                }
            }

            return error;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton is null || LeaveDays is null || LeaveHours is null) return;

            if (radioButton.Tag.ToString().Equals("0"))
            {
                LeaveDays.Visibility = Visibility.Visible;
                LeaveHours.Visibility = Visibility.Collapsed;
            }
            else
            {
                LeaveDays.Visibility = Visibility.Collapsed;
                LeaveHours.Visibility = Visibility.Visible;
            }
        }
    }
}