using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// Day.xaml 的互動邏輯
    /// </summary>
    public partial class Day : UserControl
    {
        //    public bool HasMessage
        //    {
        //        get { return (!ImportantMessage.Equals("")) || (!LeaveRecord.Equals("")); }
        //    }
        //    private DateTime ThisDay { get; }

        //    private short WorkScheduleCount { get; set; } = 0;
        //    private bool isEditMode = false;
        //    public bool IsEditMode
        //    {
        //        get
        //        {
        //            return isEditMode;
        //        }
        //        set
        //        {
        //            isEditMode = value;
        //            NotifyPropertyChanged("IsEditMode");
        //        }
        //    }

        //    public string ImportantMessage { get; set; } = "";

        //    public string LeaveRecord { get; set; } = "";

        //    public bool HasDayOff
        //    {
        //        get
        //        {
        //            if (DayOffStack.Children.OfType<UserIcon>().Count() == 0)
        //                return false;
        //            return true;
        //        }
        //    }

        //    public Day(DateTime date, WorkScheduleManageView.SpecialData specialData)
        //    {
        //        InitializeComponent();
        //        DataContext = this;
        //        ThisDay = date;
        //        LabelDay.Content = date.Day.ToString();

        //        if (specialData != null)
        //        {
        //            if(!specialData.Special.Equals(""))
        //                LabelDay.Foreground = Brushes.Red;

        //            SpecialDay.Text = specialData.Special;
        //            ImportantMessage = specialData.Remark;
        //            LeaveRecord = specialData.Leave;
        //        }

        //    }

        //    public event PropertyChangedEventHandler PropertyChanged;
        //    private void NotifyPropertyChanged(string info)
        //    {
        //        if (PropertyChanged != null)
        //        {
        //            PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        }
        //    }

        //    private void CheckBox_OnClick(object sender, RoutedEventArgs e)
        //    {
        //        CheckBox checkBox = sender as CheckBox;

        //        if(checkBox is null) return;

        //        if ((bool)checkBox.IsChecked)
        //        {
        //            UserIcon newUser = new UserIcon(WorkScheduleManageView.CurrentUserIconData);

        //            switch (checkBox.Name)
        //            {
        //                case "Morning":
        //                    AddUserToCorrectOrder(MorningStack, newUser);
        //                    break;
        //                case "Noon":
        //                    AddUserToCorrectOrder(NoonStack, newUser);
        //                    break;
        //                case "Evening":
        //                    AddUserToCorrectOrder(EveningStack, newUser);
        //                    break;
        //                case "Sleep":
        //                    AddUserToCorrectOrder(SleepStack, newUser);
        //                    break;
        //            }
        //            WorkScheduleCount++;
        //            CheckAllDay();
        //        }
        //        else
        //        {
        //            switch (checkBox.Name)
        //            {
        //                case "Morning":
        //                    RemoveUserIcon(MorningStack);
        //                    break;
        //                case "Noon":
        //                    RemoveUserIcon(NoonStack);
        //                    break;
        //                case "Evening":
        //                    RemoveUserIcon(EveningStack);
        //                    break;
        //                case "Sleep":
        //                    RemoveUserIcon(SleepStack);
        //                    break;
        //            }
        //            WorkScheduleCount--;
        //            CheckAllDay();
        //        }
        //    }

        //    internal void ShowSelectedIcon(string id)
        //    {
        //        ShowSelectedInStack(MorningStack, id);
        //        ShowSelectedInStack(NoonStack, id);
        //        ShowSelectedInStack(EveningStack, id);
        //        ShowSelectedInStack(SleepStack, id);
        //    }

        //    private void ShowSelectedInStack(StackPanel stack, string id)
        //    {
        //        List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

        //        foreach (var userIcon in userIcons)
        //        {
        //            if (id is null)
        //            {
        //                userIcon.ShowIcon();
        //                continue;
        //            }

        //            if (userIcon.Id.Equals(id))
        //                userIcon.ShowIcon();
        //            else
        //                userIcon.HideIcon();
        //        }
        //    }

        //    private void AddUserToCorrectOrder(StackPanel stack, UserIcon newUser)
        //    {
        //        if (stack.Children.Count == 0)
        //        {
        //            stack.Children.Add(newUser);
        //            return;
        //        }

        //        for (int x = 0; x < stack.Children.Count; x++)
        //        {
        //            if (Int32.Parse((stack.Children[x] as UserIcon).Id) < Int32.Parse(newUser.Id)) continue;

        //            stack.Children.Insert(x, newUser);
        //            return;
        //        }

        //        stack.Children.Add(newUser);
        //    }

        //    //public void AddUserToStack(UserIconData userIconData, string period = "")
        //    //{
        //    //    switch (period)
        //    //    {
        //    //        case "M":
        //    //            AddUserToCorrectOrder(MorningStack, new UserIcon(userIconData));
        //    //            break;
        //    //        case "N":
        //    //            AddUserToCorrectOrder(NoonStack, new UserIcon(userIconData));
        //    //            break;
        //    //        case "E":
        //    //            AddUserToCorrectOrder(EveningStack, new UserIcon(userIconData));
        //    //            break;
        //    //        case "S":
        //    //            AddUserToCorrectOrder(SleepStack, new UserIcon(userIconData));
        //    //            break;
        //    //        default:
        //    //            AddUserToCorrectOrder(DayOffStack, new UserIcon(userIconData));
        //    //            break;
        //    //    }
        //    //}

        //    public void StartEdit()
        //    {
        //        IsEditMode = true;

        //        WorkScheduleCount = 0;

        //        if (IsCurrentUserLeave())
        //        {
        //            Morning.IsEnabled = false;
        //            Noon.IsEnabled = false;
        //            Evening.IsEnabled = false;
        //            Sleep.IsEnabled = false;
        //            AllDayBtn.IsEnabled = false;
        //        }
        //        else
        //        {
        //            Morning.IsEnabled = true;
        //            Noon.IsEnabled = true;
        //            Evening.IsEnabled = true;
        //            Sleep.IsEnabled = true;
        //            AllDayBtn.IsEnabled = true;
        //        }

        //        Morning.IsChecked = HasCurrentUser(MorningStack);
        //        Noon.IsChecked = HasCurrentUser(NoonStack);
        //        Evening.IsChecked = HasCurrentUser(EveningStack);
        //        Sleep.IsChecked = HasCurrentUser(SleepStack);
        //        CheckAllDay();
        //    }

        //    private bool IsCurrentUserLeave()
        //    {
        //        return DayOffStack.Children.OfType<UserIcon>().Any(u => u.Id == WorkScheduleManageView.CurrentUserIconData.Id);
        //    }

        //    private void CheckAllDay()
        //    {
        //        if (WorkScheduleCount == 4)
        //            AllDayChecked();
        //        else
        //            NotAllDayChecked();
        //    }

        //    private void AllDayChecked()
        //    {
        //        AllDayBtn.Background = Brushes.DeepSkyBlue;
        //    }

        //    private void NotAllDayChecked()
        //    {
        //        AllDayBtn.Background = (Brush)FindResource("Shadow");
        //    }

        //    private bool? HasCurrentUser(StackPanel stack)
        //    {
        //        List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

        //        foreach(var userIcon in userIcons)
        //        {
        //            if (userIcon.Id.Equals(WorkScheduleManageView.CurrentUserIconData.Id))
        //            {
        //                WorkScheduleCount++;
        //                return true;
        //            }
        //        }

        //        return false;
        //    }

        //    private void RemoveUserIcon(StackPanel stack)
        //    {
        //        List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

        //        foreach (var userIcon in userIcons)
        //        {
        //            if (userIcon.Id.Equals(WorkScheduleManageView.CurrentUserIconData.Id))
        //            {
        //                stack.Children.Remove(userIcon);
        //                return;
        //            }
        //        }
        //    }

        //    internal void EndEdit()
        //    {
        //        IsEditMode = false;
        //    }

        //    //internal ObservableCollection<WorkSchedule> GetWorkSchedules(ObservableCollection<WorkSchedule> workSchedules)
        //    //{
        //    //    AddWorkSchedules(MorningStack,ref workSchedules);
        //    //    AddWorkSchedules(NoonStack, ref workSchedules);
        //    //    AddWorkSchedules(EveningStack, ref workSchedules);
        //    //    AddWorkSchedules(SleepStack, ref workSchedules);

        //    //    return workSchedules;
        //    //}

        //    //private void AddWorkSchedules(StackPanel stack, ref ObservableCollection<WorkSchedule> workSchedules)
        //    //{
        //    //    string day = LabelDay.Content.ToString();
        //    //    string period = stack.Name.Substring(0,1);

        //    //    List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

        //    //    foreach (var userIcon in userIcons)
        //    //    {
        //    //        workSchedules.Add(new WorkSchedule(userIcon.Id, day, period));
        //    //    }
        //    //}

        //    private void HorizontalScroll(object sender, MouseWheelEventArgs e)
        //    {
        //        ScrollViewer scrollViewer = sender as ScrollViewer;

        //        if(scrollViewer is null) return;

        //        if (e.Delta < 0)
        //            scrollViewer.LineRight();
        //        else
        //            scrollViewer.LineLeft();

        //        e.Handled = true;
        //    }

        //    private void AllDayBtn_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //    {
        //        if (WorkScheduleCount < 4)
        //        {
        //            UserIcon newUser;

        //            if (!(bool) Morning.IsChecked)
        //            {
        //                newUser = new UserIcon(WorkScheduleManageView.CurrentUserIconData);
        //                AddUserToCorrectOrder(MorningStack, newUser);
        //                Morning.IsChecked = true;
        //            }

        //            if (!(bool) Noon.IsChecked)
        //            {
        //                newUser = new UserIcon(WorkScheduleManageView.CurrentUserIconData);
        //                AddUserToCorrectOrder(NoonStack, newUser);
        //                Noon.IsChecked = true;
        //            }

        //            if (!(bool) Evening.IsChecked)
        //            {
        //                newUser = new UserIcon(WorkScheduleManageView.CurrentUserIconData);
        //                AddUserToCorrectOrder(EveningStack, newUser);
        //                Evening.IsChecked = true;
        //            }

        //            if (!(bool) Sleep.IsChecked)
        //            {
        //                newUser = new UserIcon(WorkScheduleManageView.CurrentUserIconData);
        //                AddUserToCorrectOrder(SleepStack, newUser);
        //                Sleep.IsChecked = true;
        //            }

        //            WorkScheduleCount = 4;
        //            CheckAllDay();
        //        }
        //        else
        //        {
        //            Morning.IsChecked = false;
        //            Noon.IsChecked = false;
        //            Evening.IsChecked = false;
        //            Sleep.IsChecked = false;

        //            RemoveUserIcon(MorningStack);
        //            RemoveUserIcon(NoonStack);
        //            RemoveUserIcon(EveningStack);
        //            RemoveUserIcon(SleepStack);
        //            WorkScheduleCount = 0;
        //            CheckAllDay();
        //        }
        //    }

        //    private void ChangeMessage_Click(object sender, RoutedEventArgs e)
        //    {
        //        EditMessageWindow editMessageWindow = new EditMessageWindow(ImportantMessage);
        //        editMessageWindow.ShowDialog();

        //        ImportantMessage = editMessageWindow.Message;

        //        ///WorkScheduleDb.SaveCalendarRemark(ThisDay, ImportantMessage);
        //    }

        //    private void ShowRemark(object sender, MouseEventArgs e)
        //    {
        //        DayRemark dayRemark = new DayRemark(ImportantMessage, LeaveRecord);

        //        Grid.SetRow(dayRemark, 0);
        //        Grid.SetColumn(dayRemark, 0);

        //        Grid.SetRowSpan(dayRemark, 5);
        //        Grid.SetColumnSpan(dayRemark, 3);

        //        DayGrid.Children.Add(dayRemark);
        //    }

        //    private void HideRemark(object sender, MouseEventArgs e)
        //    {
        //        DayGrid.Children.Remove(DayGrid.Children.OfType<DayRemark>().ToList()[0]);
        //    }
        //}

        //public class IsEditableConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if((bool)value)
        //        {
        //            return 20;
        //        }

        //        return 0;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        return null;
        //    }
        //}

        //public class IsAllDayShowConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if ((bool)value)
        //        {
        //            return Visibility.Visible;
        //        }

        //        return Visibility.Collapsed;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        return null;
        //    }
        //}

        //public class HasMessageConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if ((bool)value)
        //        {
        //            return Visibility.Visible;
        //        }

        //        return Visibility.Collapsed;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        return null;
        //    }
        //}

        //public class HasDayOffConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if ((bool)value)
        //        {
        //            return Visibility.Visible;
        //        }

        //        return Visibility.Collapsed;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        return null;
        //    }
    }
}