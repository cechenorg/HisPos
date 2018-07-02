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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace His_Pos.H4_BASIC_MANAGE.WorkScheduleManage
{
    /// <summary>
    /// WorkScheduleManageView.xaml 的互動邏輯
    /// </summary>
    public partial class WorkScheduleManageView : UserControl
    {
        class Time
        {
           public Time(DateTime date)
            {
                Year = date.Year;
                Month = date.Month;
                Day = date.Day;
            }
            public  int Year;
            public int Month;
            public int Day;
        }
        private Time selectDateTime = new Time(DateTime.Now);
        public WorkScheduleManageView()
        {
            InitializeComponent();
            InitBasicData();
            InitCalendar(selectDateTime);
        }
        private void InitBasicData() {
            List<string> months = new List<string> { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"};
            List<string> years = new List<string>();
            int thisyear = DateTime.Now.Year;
            thisyear -= 50;
            for (int i = 0; i < 100; i++)
            {
                years.Add(thisyear.ToString() + "年");
                thisyear++;
            }
            comboMonth.ItemsSource = months; 
            comboMonth.Text = DateTime.Now.Month + "月";
            comboYear.ItemsSource = years;
            comboYear.Text = DateTime.Now.Year + "年";
        }
        private void InitCalendar(Time selectDateTime) {
            GridCalendar.Children.Clear();
            DateTime TheMonthStart = new DateTime(selectDateTime.Year, selectDateTime.Month, 1);
            DateTime TheMonthEnd = new DateTime(selectDateTime.Year, selectDateTime.Month, DateTime.DaysInMonth(selectDateTime.Year, selectDateTime.Month));
            int wcount = 1;
            Grid week = NewRow("0");
            GridCalendar.Children.Add(week);
            Grid.SetRow(week,0);
            Day day;
            while (TheMonthStart != TheMonthEnd.AddDays(1)) {
                string today = TheMonthStart.DayOfWeek.ToString("d");
                if (today == "0" && TheMonthStart.Day.ToString() != "1")
                {
                    week = NewRow(wcount.ToString());
                    GridCalendar.Children.Add(week);
                    Grid.SetRow(week,wcount);
                    wcount++;
                }
                day = NewDay(TheMonthStart.Day.ToString());
                week.Children.Add(day);
                Grid.SetColumn(day,Convert.ToInt32(today));
                TheMonthStart = TheMonthStart.AddDays(1);
            }
            labelSelectDate.Content = selectDateTime.Year + "年" + selectDateTime.Month + "月";
        }
        private Day NewDay(string id) {
           Day day = new Day(id);
            return day;
        }
        private Grid NewRow(string id) {
            Grid newGrid = new Grid();
            newGrid.Name = "row" + id;
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            return newGrid;
        }

        private void comboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboMonth.SelectedItem == null) return;
            selectDateTime.Month = Convert.ToInt32(comboMonth.SelectedValue.ToString().Split('月')[0]);
            InitCalendar(selectDateTime);
        }

        private void comboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboYear.SelectedItem == null) return;
            selectDateTime.Year = Convert.ToInt32(comboYear.SelectedValue.ToString().Split('年')[0]);
            InitCalendar(selectDateTime);
        }
    }
}
