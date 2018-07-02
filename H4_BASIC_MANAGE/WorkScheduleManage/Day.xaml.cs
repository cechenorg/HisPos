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
            if ((bool)(sender as CheckBox).IsChecked)
            {
                UserIcon newUser = new UserIcon("測");

                MorningStack.Children.Add(newUser);
            }
            else
            {
                //MorningStack.Children.Clear();
            }
        }

        private void Noon_OnClick(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                UserIcon newUser = new UserIcon("測");

                NoonStack.Children.Add(newUser);
            }
            else
            {
                NoonStack.Children.Clear();
            }
        }

        private void Evening_OnClick(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                UserIcon newUser = new UserIcon("測");

                EveningStack.Children.Add(newUser);
            }
            else
            {
                EveningStack.Children.Clear();
            }
        }
    }
}
