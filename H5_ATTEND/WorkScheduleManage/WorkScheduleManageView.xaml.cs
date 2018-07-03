﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.WorkScheduleManage
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

        public static UserIconData CurrentUserIconData;

        public ObservableCollection<WorkSchedule> WorkSchedules { get; set; }
        public ObservableCollection<UserIconData> UserIconDatas { get; set; }

        public WorkScheduleManageView()
        {
            InitializeComponent();
            InitUserIconData();
            InitBasicData();
            InitCalendar(selectDateTime);
        }

        private void InitUserIconData()
        {
            UserIconDatas = WorkScheduleDb.GetUserIconDatas();

            foreach(var userIconData in UserIconDatas)
            {
                UserPreview.Children.Add(new UserIconPreview(userIconData));
            }

            UserCombo.ItemsSource = UserIconDatas;
            UserCombo.SelectedIndex = 0;
        }

        private void InitWorkSchedule()
        {
            WorkSchedules = WorkScheduleDb.GetWorkSchedules(selectDateTime.Year.ToString(), selectDateTime.Month.ToString());

            if(WorkSchedules.Count == 0) return;

            CurrentUserIconData = UserIconDatas.Single(u => u.Id.Equals(WorkSchedules[0].Id));

            foreach (var workSchedule in WorkSchedules)
            {
                if( !workSchedule.Id.Equals(CurrentUserIconData.Id) )
                    CurrentUserIconData = UserIconDatas.Single(u => u.Id.Equals(workSchedule.Id));

                (GridCalendar.Children[Int32.Parse(workSchedule.Day) - 1] as Day).AddUserToStack(CurrentUserIconData, workSchedule.Period);
            }
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
            ComboMonth.ItemsSource = months; 
            ComboMonth.Text = DateTime.Now.Month + "月";
            ComboYear.ItemsSource = years;
            ComboYear.Text = DateTime.Now.Year + "年";
        }

        private void InitCalendar(Time selectDateTime) {
            GridCalendar.Children.Clear();
            DateTime TheMonthStart = new DateTime(selectDateTime.Year, selectDateTime.Month, 1);
            DateTime TheMonthEnd = new DateTime(selectDateTime.Year, selectDateTime.Month, DateTime.DaysInMonth(selectDateTime.Year, selectDateTime.Month));
            int wcount = 0;
            
            while (TheMonthStart != TheMonthEnd.AddDays(1)) {
                
                string today = TheMonthStart.DayOfWeek.ToString("d");
                if (today == "0" && TheMonthStart.Day.ToString() != "1") wcount++;

                Day day = new Day(TheMonthStart.Day.ToString());

                Grid.SetRow(day, wcount);
                Grid.SetColumn(day, Convert.ToInt32(today));
                GridCalendar.Children.Add(day);

                TheMonthStart = TheMonthStart.AddDays(1);
            }

            InitWorkSchedule();
        }

        private void comboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboMonth.SelectedItem == null) return;
            selectDateTime.Month = Convert.ToInt32(ComboMonth.SelectedValue.ToString().Split('月')[0]);
            InitCalendar(selectDateTime);
        }

        private void comboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboYear.SelectedItem == null) return;
            selectDateTime.Year = Convert.ToInt32(ComboYear.SelectedValue.ToString().Split('年')[0]);
            InitCalendar(selectDateTime);

        }

        private void UserCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox is null || comboBox.SelectedItem is null) return;

            CurrentUserIconData = comboBox.SelectedItem as UserIconData;
            UpdateStartEditUi();
        }

        private void StartSchedule_Click(object sender, RoutedEventArgs e)
        {
            CurrentUserIconData = UserCombo.SelectedItem as UserIconData;

            UpdateStartEditUi();
        }

        private void UpdateStartEditUi()
        {
            UserCombo.IsEnabled = true;
            FinishScheduleBtn.IsEnabled = true;
            ComboYear.IsEnabled = false;
            ComboMonth.IsEnabled = false;
            StartScheduleBtn.IsEnabled = false;

            List<Day> days = GridCalendar.Children.OfType<Day>().ToList();

            foreach (var d in days)
            {
                d.StartEdit();
            }
        }

        private void FinishSchedule_Click(object sender, RoutedEventArgs e)
        {


            UpdateEndEditUi();
        }

        private void UpdateEndEditUi()
        {
            UserCombo.IsEnabled = false;
            FinishScheduleBtn.IsEnabled = false;
            ComboYear.IsEnabled = true;
            ComboMonth.IsEnabled = true;
            StartScheduleBtn.IsEnabled = true;

            List<Day> days = GridCalendar.Children.OfType<Day>().ToList();

            foreach (var d in days)
            {
                d.EndEdit();
            }
        }
    }
}
