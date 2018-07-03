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
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// Day.xaml 的互動邏輯
    /// </summary>
    public partial class Day : UserControl
    {
        public Day(string id)
        {
            InitializeComponent();

            labelDay.Content = id;
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
                //switch (checkBox.Name)
                //{
                //    case "Morning":
                //        MorningStack.Children.Remove();
                //        break;
                //    case "Noon":
                //        NoonStack.Children.Add(newUser);
                //        break;
                //    case "Evening":
                //        EveningStack.Children.Add(newUser);
                //        break;
                //}
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
    }
}
