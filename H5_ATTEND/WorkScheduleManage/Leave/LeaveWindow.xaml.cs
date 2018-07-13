using System;
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
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.WorkScheduleManage.Leave
{
    /// <summary>
    /// LeaveWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LeaveWindow : Window
    {
        public ObservableCollection<UserIconData> UserIconDatas { get; }
        public LeaveWindow(ObservableCollection<UserIconData> users)
        {
            InitializeComponent();
            DataContext = this;

            UserIconDatas = users;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string errors = CheckInputData();

            if (!errors.Equals(""))
            {
                MessageWindow messageWindow = new MessageWindow(errors, MessageType.ERROR);
                messageWindow.ShowDialog();

                return;
            }


        }

        private string CheckInputData()
        {
            string error = "";
            


            return error;
        }
    }
}
