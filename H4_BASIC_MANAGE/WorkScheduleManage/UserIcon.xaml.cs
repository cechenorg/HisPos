using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// UserIcon.xaml 的互動邏輯
    /// </summary>
    public partial class UserIcon : UserControl
    {
        public UserIcon(string name)
        {
            InitializeComponent();
            UserName.Content = name;
        }
    }
}
