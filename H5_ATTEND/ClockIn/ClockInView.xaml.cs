﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using His_Pos.Class;
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.ClockIn
{
    /// <summary>
    /// ClockInView.xaml 的互動邏輯
    /// </summary>
    public partial class ClockInView : UserControl
    {
        public ObservableCollection<UserIconData> UserIconDatas { get; set; }

        public ClockInView()
        {
            InitializeComponent();

            InitTodayUser();
        }

        private void InitTodayUser()
        {
            UserIconDatas = WorkScheduleDb.GetTodayUsers();

            foreach (var userIconData in UserIconDatas)
            {
                ClockInUserIcon clockInUserIcon = new ClockInUserIcon(userIconData);
                clockInUserIcon.MouseLeftButtonDown += UserIcon_Click;

                UserIconStack.Children.Add(clockInUserIcon);
            }
        }

        private void UserIcon_Click(object sender, RoutedEventArgs e)
        {
            UserId.Text = (sender as ClockInUserIcon).Id;

            UserPassword.Focus();
        }

        private void UserId_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                UserPassword.Focus();
        }

        private void UserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UserClockIn();
        }

        private void ClockIn_Click(object sender, RoutedEventArgs e)
        {
            UserClockIn();
        }

        private void UserClockIn()
        {
            if (IsDataEmpty())
            {
                MessageWindow messageWindow = new MessageWindow("帳號或密碼未填!", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }

            string inout = InOutStack.Children.OfType<RadioButton>().Single(r => (bool) r.IsChecked).Tag.ToString();

            switch (WorkScheduleDb.UserClockIn(UserId.Text, UserPassword.Password, inout)) {
                case "上班打卡成功":
                    MessageWindow messageWindow = new MessageWindow("上班打卡成功!", MessageType.SUCCESS);
                    messageWindow.ShowDialog();
                    ClearUi();
                    break;
                case "您已經打過上班卡":
                    messageWindow = new MessageWindow("您已經打過上班卡", MessageType.ERROR);
                    messageWindow.ShowDialog();
                    UserId.Text = string.Empty;
                    UserPassword.Password = string.Empty;
                    break;
                case "您已經下班 無法打卡上班 已通知主管":
                    messageWindow = new MessageWindow("您已經下班 無法打卡上班 已通知主管", MessageType.ERROR);
                    messageWindow.ShowDialog();
                    UserId.Text = string.Empty;
                    UserPassword.Password = string.Empty;
                    break;
                case "下班打卡成功":
                    messageWindow = new MessageWindow("下班打卡成功!", MessageType.SUCCESS);
                    messageWindow.ShowDialog();
                    ClearUi();
                    break;
                case "打卡失敗":
                    messageWindow = new MessageWindow("帳號或密碼錯誤!", MessageType.ERROR);
                    messageWindow.ShowDialog();

                    UserPassword.Password = "";
                    UserPassword.Focus();
                    break;
            }
         
        }

        private void ClearUi()
        {
            UserId.Text = "";
            UserPassword.Password = "";
        }

        private bool IsDataEmpty()
        {
            return UserId.Text.Equals("") || UserPassword.Password.Equals("");
        }
    }
}
