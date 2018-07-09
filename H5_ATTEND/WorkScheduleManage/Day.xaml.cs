using System;
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

            LabelDay.Content = id;
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
                        AddUserToCorrectOrder(MorningStack, newUser);
                        break;
                    case "Noon":
                        AddUserToCorrectOrder(NoonStack, newUser);
                        break;
                    case "Evening":
                        AddUserToCorrectOrder(EveningStack, newUser);
                        break;
                    case "Sleep":
                        AddUserToCorrectOrder(SleepStack, newUser);
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
                    case "Sleep":
                        RemoveUserIcon(SleepStack);
                        break;
                }
            }
        }

        internal void ShowSelectedIcon(string id)
        {
            ShowSelectedInStack(MorningStack, id);
            ShowSelectedInStack(NoonStack, id);
            ShowSelectedInStack(EveningStack, id);
            ShowSelectedInStack(SleepStack, id);
        }

        private void ShowSelectedInStack(StackPanel stack, string id)
        {
            List<UserIcon> userIcons = stack.Children.OfType<UserIcon>().ToList();

            foreach (var userIcon in userIcons)
            {
                if (id is null)
                {
                    userIcon.ShowIcon();
                    continue;
                }

                if (userIcon.Id.Equals(id))
                    userIcon.ShowIcon();
                else
                    userIcon.HideIcon();
            }
        }

        private void AddUserToCorrectOrder(StackPanel stack, UserIcon newUser)
        {
            if (stack.Children.Count == 0)
            {
                stack.Children.Add(newUser);
                return;
            }

            for (int x = 0; x < stack.Children.Count; x++)
            {
                if (Int32.Parse((stack.Children[x] as UserIcon).Id) < Int32.Parse(newUser.Id)) continue;

                stack.Children.Insert(x, newUser);
                return;
            }

            stack.Children.Add(newUser);
        }

        public void AddUserToStack(UserIconData userIconData, string period)
        {
            switch (period)
            {
                case "M":
                    AddUserToCorrectOrder(MorningStack, new UserIcon(userIconData));
                    break;
                case "N":
                    AddUserToCorrectOrder(NoonStack, new UserIcon(userIconData));
                    break;
                case "E":
                    AddUserToCorrectOrder(EveningStack, new UserIcon(userIconData));
                    break;
                case "S":
                    AddUserToCorrectOrder(SleepStack, new UserIcon(userIconData));
                    break;
            }
        }

        public void StartEdit()
        {
            IsEditMode = true;

            Morning.IsChecked = HasCurrentUser(MorningStack);
            Noon.IsChecked = HasCurrentUser(NoonStack);
            Evening.IsChecked = HasCurrentUser(EveningStack);
            Sleep.IsChecked = HasCurrentUser(SleepStack);
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
            AddWorkSchedules(SleepStack, ref workSchedules);

            return workSchedules;
        }

        private void AddWorkSchedules(StackPanel stack, ref ObservableCollection<WorkSchedule> workSchedules)
        {
            string day = LabelDay.Content.ToString();
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
