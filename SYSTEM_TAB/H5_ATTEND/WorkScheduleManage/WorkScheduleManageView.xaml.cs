using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// WorkScheduleManageView.xaml 的互動邏輯
    /// </summary>
    public partial class WorkScheduleManageView : UserControl
    {
        //public class SpecialData
        //{
        //    public SpecialData(DataRow dataRow)
        //    {
        //        Day = dataRow["DAY"].ToString();
        //        Special = dataRow["HOLIDAY"].ToString();
        //        Remark = dataRow["REMARK"].ToString();
        //        Leave = dataRow["LEAVERECORD"].ToString();
        //    }

        //    public string Day { get; }
        //    public string Special { get; }
        //    public string Remark { get; }
        //    public string Leave { get; }
        //}

        //class Time
        //{
        //    public Time(DateTime date)
        //    {
        //        Year = date.Year;
        //        Month = date.Month;
        //        Day = date.Day;
        //    }
        //    public int Year;
        //    public int Month;
        //    public int Day;
        //}
        //private Time selectDateTime = new Time(DateTime.Now);

        //public static UserIconData CurrentUserIconData;

        //public ObservableCollection<WorkSchedule> WorkSchedules { get; set; }
        //public ObservableCollection<UserIconData> UserIconDatas { get; set; }

        //private bool IsFirst { get; set; } = true;

        //public static WorkScheduleManageView Instance;
        //public static bool DataChanged;

        //public WorkScheduleManageView()
        //{
        //    InitializeComponent();
        //    Instance = this;
        //    DataChanged = false;
        //    InitUserIconData();
        //    InitBasicData();
        //    InitCalendar();
        //    UpdateEndEditUi();
        //}

        //private void InitUserIconData()
        //{
        //    UserIconPreview allUserIconPreview = new UserIconPreview(new UserIconData());

        //    allUserIconPreview.MouseLeftButtonDown += UserIconPreviewFilterButtonDown;

        //    UserPreview.Children.Add(allUserIconPreview);

        //    allUserIconPreview.IsSelected = true;

        //   /// UserIconDatas = WorkScheduleDb.GetUserIconDatas();

        //    foreach (var userIconData in UserIconDatas)
        //    {
        //        UserIconPreview newUserIconPreview = new UserIconPreview(userIconData);

        //        newUserIconPreview.MouseLeftButtonDown += UserIconPreviewFilterButtonDown;

        //        UserPreview.Children.Add(newUserIconPreview);
        //    }

        //    UserCombo.ItemsSource = UserIconDatas;
        //    UserCombo.SelectedIndex = 0;
        //}

        //private void UserIconPreviewFilterButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        //{
        //    if (StartEdit()) return;

        //    UserIconPreview userIconPreview = sender as UserIconPreview;

        //    ClearSelectedUserIcon();

        //    userIconPreview.IsSelected = true;

        //    ShowSelectedUserIcon(userIconPreview.Id);
        //}

        //private void ShowSelectedUserIcon(string id = null)
        //{
        //    List<Day> days = GridCalendar.Children.OfType<Day>().ToList();

        //    foreach (var d in days)
        //    {
        //        d.ShowSelectedIcon(id);
        //    }
        //}

        //private bool StartEdit()
        //{
        //    return !StartScheduleBtn.IsEnabled;
        //}

        //private void ClearSelectedUserIcon()
        //{
        //    List<UserIconPreview> userIconPreviews = UserPreview.Children.OfType<UserIconPreview>().ToList();

        //    foreach (var u in userIconPreviews)
        //    {
        //        u.IsSelected = false;
        //    }
        //}

        //private void InitWorkSchedule()
        //{
        //    ///WorkSchedules = WorkScheduleDb.GetWorkSchedules(selectDateTime.Year.ToString(), selectDateTime.Month.ToString());

        //    if (WorkSchedules.Count == 0) return;

        //    CurrentUserIconData = UserIconDatas.Single(u => u.Id.Equals(WorkSchedules[0].Id));

        //    foreach (var workSchedule in WorkSchedules)
        //    {
        //        if (!workSchedule.Id.Equals(CurrentUserIconData.Id))
        //            CurrentUserIconData = UserIconDatas.Single(u => u.Id.Equals(workSchedule.Id));

        //        (GridCalendar.Children[Int32.Parse(workSchedule.Day) - 1] as Day).AddUserToStack(CurrentUserIconData, workSchedule.Period);
        //    }
        //}

        //private void InitBasicData()
        //{
        //    List<string> months = new List<string> { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" };
        //    List<string> years = new List<string>();
        //    int thisyear = DateTime.Now.Year;
        //    thisyear -= 50;
        //    for (int i = 0; i < 100; i++)
        //    {
        //        years.Add(thisyear.ToString() + "年");
        //        thisyear++;
        //    }
        //    ComboMonth.ItemsSource = months;
        //    ComboMonth.Text = DateTime.Now.Month + "月";
        //    ComboYear.ItemsSource = years;
        //    ComboYear.Text = DateTime.Now.Year + "年";

        //    IsFirst = false;
        //}

        //public void InitCalendar()
        //{
        //    if (IsFirst) return;

        //    GridCalendar.Children.Clear();
        //    DateTime TheMonthStart = new DateTime(selectDateTime.Year, selectDateTime.Month, 1);
        //    DateTime TheMonthEnd = new DateTime(selectDateTime.Year, selectDateTime.Month, DateTime.DaysInMonth(selectDateTime.Year, selectDateTime.Month));
        //    int beginDayCount = Int16.Parse(TheMonthStart.DayOfWeek.ToString("d"));
        //    int endDayCount = 6 - Int16.Parse(TheMonthEnd.DayOfWeek.ToString("d"));
        //    int wcount = 0;

        //    ///Collection<SpecialData> specialDates = WorkScheduleDb.GetSpecialData(selectDateTime.Year, selectDateTime.Month);

        //    while (TheMonthStart != TheMonthEnd.AddDays(1))
        //    {
        //        string today = TheMonthStart.DayOfWeek.ToString("d");
        //        if (today == "0") wcount++;

        //        SpecialData specialData = null;/// specialDates.SingleOrDefault(s => s.Day.Equals(TheMonthStart.Day.ToString()));

        //        Day day = new Day(TheMonthStart, specialData);

        //        Grid.SetRow(day, wcount);
        //        Grid.SetColumn(day, Convert.ToInt32(today));
        //        GridCalendar.Children.Add(day);

        //        TheMonthStart = TheMonthStart.AddDays(1);
        //    }

        //    FillBlankDay(beginDayCount, endDayCount);

        //    HighlightToday();

        //    InitWorkSchedule();
        //    InitLeave();
        //}

        //private void InitLeave()
        //{
        //    Collection<LeaveRecord> leaveRecords = null;/// LeaveDb.GetLeaveRecord(selectDateTime.Year.ToString(), selectDateTime.Month.ToString());

        //    if (leaveRecords.Count == 0) return;

        //    foreach (var leaveRecord in leaveRecords)
        //    {
        //        if (!leaveRecord.Id.Equals(CurrentUserIconData.Id))
        //            CurrentUserIconData = UserIconDatas.Single(u => u.Id.Equals(leaveRecord.Id));

        //        (GridCalendar.Children[Int32.Parse(leaveRecord.Day) - 1] as Day).AddUserToStack(CurrentUserIconData);
        //    }
        //}

        //private void FillBlankDay(int beginDayCount, int endDayCount)
        //{
        //    if (beginDayCount == 0) beginDayCount = 7;

        //    for(int x = 0; x < beginDayCount; x++)
        //    {
        //        StackPanel stackPanel = new StackPanel();
        //        stackPanel.Margin = new Thickness(2);
        //        stackPanel.Background = (Brush)FindResource("DarkShadow");

        //        Grid.SetRow(stackPanel, 0);
        //        Grid.SetColumn(stackPanel, x);
        //        GridCalendar.Children.Add(stackPanel);
        //    }

        //    int lastRow = 5;

        //    if (beginDayCount <= 4)
        //        lastRow = 4;

        //    for (int x = 0; x < endDayCount; x++)
        //    {
        //        StackPanel stackPanel = new StackPanel();
        //        stackPanel.Margin = new Thickness(2);
        //        stackPanel.Background = (Brush)FindResource("DarkShadow");

        //        Grid.SetRow(stackPanel, lastRow);
        //        Grid.SetColumn(stackPanel, 6 - x);
        //        GridCalendar.Children.Add(stackPanel);
        //    }

        //    if(lastRow == 4 || endDayCount == 0)
        //    {
        //        for (int x = 0; x < 7; x++)
        //        {
        //            StackPanel stackPanel = new StackPanel();
        //            stackPanel.Margin = new Thickness(2);
        //            stackPanel.Background = (Brush)FindResource("DarkShadow");

        //            Grid.SetRow(stackPanel, 5);
        //            Grid.SetColumn(stackPanel, x);
        //            GridCalendar.Children.Add(stackPanel);
        //        }
        //    }
        //}

        //private void HighlightToday()
        //{
        //    DateTime today = DateTime.Now;

        //    if (today.Year == selectDateTime.Year && today.Month == selectDateTime.Month)
        //    {
        //        (GridCalendar.Children[today.Day - 1] as Day).DayGrid.Background = (Brush)FindResource("GridSelected");
        //    }
        //}

        //private void comboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (ComboMonth.SelectedItem == null) return;
        //    selectDateTime.Month = Convert.ToInt32(ComboMonth.SelectedValue.ToString().Split('月')[0]);
        //    InitCalendar();
        //}

        //private void comboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (ComboYear.SelectedItem == null) return;
        //    selectDateTime.Year = Convert.ToInt32(ComboYear.SelectedValue.ToString().Split('年')[0]);
        //    InitCalendar();

        //}

        //private void UserCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox comboBox = sender as ComboBox;

        //    if (comboBox is null || comboBox.SelectedItem is null) return;

        //    CurrentUserIconData = comboBox.SelectedItem as UserIconData;
        //    UpdateStartEditUi();
        //}

        //private void StartSchedule_Click(object sender, RoutedEventArgs e)
        //{
        //    CurrentUserIconData = UserCombo.SelectedItem as UserIconData;

        //    DateTime today = DateTime.Now;

        //    if (today.Year == selectDateTime.Year && today.Month <= selectDateTime.Month)
        //        UpdateStartEditUi();
        //    else if (today.Year < selectDateTime.Year)
        //        UpdateStartEditUi();
        //    else if (today.Year == selectDateTime.Year && today.Month == selectDateTime.Month && today.Day != DateTime.DaysInMonth(selectDateTime.Year, selectDateTime.Month))
        //        UpdateStartEditUi();
        //    else
        //    {
        //        MessageWindow.ShowMessage("日期已過 無法排班!", Class.MessageType.ERROR);

        //    }
        //}

        //private void UpdateStartEditUi()
        //{
        //    ScheduleStack.Visibility = Visibility.Visible;

        //    ComboYear.IsEnabled = false;
        //    ComboMonth.IsEnabled = false;
        //    StartScheduleBtn.IsEnabled = false;
        //    DayOffBtn.IsEnabled = false;

        //    ClearSelectedUserIcon();
        //    (UserPreview.Children[0] as UserIconPreview).IsSelected = true;

        //    ShowSelectedUserIcon();

        //    DateTime today = DateTime.Now;

        //    List<Day> days;

        //    if (today.Year == selectDateTime.Year && today.Month == selectDateTime.Month)
        //    {
        //        days = GridCalendar.Children.OfType<Day>().Where(d => Int16.Parse(d.LabelDay.Content.ToString()) > today.Day).ToList();
        //    }
        //    else
        //    {
        //        days = GridCalendar.Children.OfType<Day>().ToList();
        //    }

        //    foreach (var d in days)
        //    {
        //        d.StartEdit();
        //    }
        //}

        //private void FinishSchedule_Click(object sender, RoutedEventArgs e)
        //{
        //    UpdateWorkSchedules();

        //    ///WorkScheduleDb.InsertWorkSchedules(WorkSchedules, selectDateTime.Year.ToString(), selectDateTime.Month.ToString());

        //    UpdateEndEditUi();
        //}

        //private void UpdateWorkSchedules()
        //{
        //    WorkSchedules.Clear();

        //    List<Day> days = GridCalendar.Children.OfType<Day>().ToList();

        //    foreach (var d in days)
        //    {
        //        d.GetWorkSchedules(WorkSchedules);
        //    }
        //}

        //private void UpdateEndEditUi()
        //{
        //    ScheduleStack.Visibility = Visibility.Collapsed;

        //    ComboYear.IsEnabled = true;
        //    ComboMonth.IsEnabled = true;
        //    StartScheduleBtn.IsEnabled = true;
        //    DayOffBtn.IsEnabled = true;

        //    List<Day> days = GridCalendar.Children.OfType<Day>().ToList();

        //    foreach (var d in days)
        //    {
        //        d.EndEdit();
        //    }
        //}

        //private void CancelSchedule_Click(object sender, RoutedEventArgs e)
        //{
        //    InitCalendar();
        //    UpdateEndEditUi();
        //}

        //private void DayOff_OnClick(object sender, RoutedEventArgs e)
        //{
        //    LeaveWindow leaveWindow = new LeaveWindow(UserIconDatas);

        //    leaveWindow.ShowDialog();

        //    if (leaveWindow.LeaveComplete)
        //    {
        //        MessageWindow.ShowMessage(leaveWindow.CompleteResult, (leaveWindow.CompleteResult.Contains("無法新增"))? MessageType.ERROR:MessageType.SUCCESS);

        //        AuthenticationManageView.DataChanged = true;
        //        InitCalendar();
        //    }
        //}
    }
}