﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    /// Day.xaml 的互動邏輯
    /// </summary>
    public partial class Day : UserControl, INotifyPropertyChanged
    {
        private bool isEditMode = false;
        public bool IsEditMode
        {
            get
            {
                return isEditMode;
            }
            set
            {
                isEditMode = value;
                NotifyPropertyChanged("IsEditMode");
            }
        }

        public Day(string id)
        {
            InitializeComponent();
            DataContext = this;

            labelDay.Content = id;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void Morning_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if(checkBox is null) return;

            if ((bool)checkBox.IsChecked)
            {
                UserIcon newUser = new UserIcon(WorkScheduleManageView.CurrentUserIconData);

                switch (checkBox.Name)
                {
                    case "Morning":
                        MorningStack.Children.Add(newUser);
                        break;
                    case "Noon":
                        NoonStack.Children.Add(newUser);
                        break;
                    case "Evening":
                        EveningStack.Children.Add(newUser);
                        break;
                }
            }
            else
            {
                switch (checkBox.Name)
                {
                    case "Morning":
                        RemoveUserIcon(MorningStack);
                        break;
                    case "Noon":
                        RemoveUserIcon(NoonStack);
                        break;
                    case "Evening":
                        RemoveUserIcon(EveningStack);
                        break;
                }
            }
        }
        
        public void AddUserToStack(UserIconData userIconData, string period)
        {
            switch (period)
            {
                case "M":
                    MorningStack.Children.Add(new UserIcon(userIconData));
                    break;
                case "N":
                    NoonStack.Children.Add(new UserIcon(userIconData));
                    break;
                case "E":
                    EveningStack.Children.Add(new UserIcon(userIconData));
                    break;
            }
        }

        public void DeleteUserFromStack(string id, string period)
        {
            //switch (period)
            //{
            //    case "M":
            //        MorningStack.Children.Add(new UserIcon(userIconData));
            //        break;
            //    case "N":
            //        NoonStack.Children.Add(new UserIcon(userIconData));
            //        break;
            //    case "E":
            //        EveningStack.Children.Add(new UserIcon(userIconData));
            //        break;
            //}
        }

        public void StartEdit()
        {
            IsEditMode = true;

            Morning.IsChecked = HasCurrentUser(MorningStack);
            Noon.IsChecked = HasCurrentUser(NoonStack);
            Evening.IsChecked = HasCurrentUser(EveningStack);
        }

        private bool? HasCurrentUser(StackPanel stack)
        {
            List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

            foreach(var userIcon in userIcons)
            {
                if (userIcon.Id.Equals(WorkScheduleManageView.CurrentUserIconData.Id)) return true;
            }

            return false;
        }

        private void RemoveUserIcon(StackPanel stack)
        {
            List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

            foreach (var userIcon in userIcons)
            {
                if (userIcon.Id.Equals(WorkScheduleManageView.CurrentUserIconData.Id))
                {
                    stack.Children.Remove(userIcon);
                    return;
                }
            }
        }

        internal void EndEdit()
        {
            IsEditMode = false ;
        }

        internal ObservableCollection<WorkSchedule> GetWorkSchedules(ObservableCollection<WorkSchedule> workSchedules)
        {
            AddWorkSchedules(MorningStack,ref workSchedules);
            AddWorkSchedules(NoonStack, ref workSchedules);
            AddWorkSchedules(EveningStack, ref workSchedules);

            return workSchedules;
        }

        private void AddWorkSchedules(StackPanel stack, ref ObservableCollection<WorkSchedule> workSchedules)
        {
            string day = labelDay.Content.ToString();
            string period = stack.Name.Substring(0,1);

            List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

            foreach (var userIcon in userIcons)
            {
                workSchedules.Add(new WorkSchedule(userIcon.Id, day, period));
            }
        }
    }

    public class IsEditableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)value)
            {
                return 20;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
