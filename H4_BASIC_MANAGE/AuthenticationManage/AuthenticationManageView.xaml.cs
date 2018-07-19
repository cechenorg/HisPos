using His_Pos.Class.Authority;
using His_Pos.Class.Leave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace His_Pos.H4_BASIC_MANAGE.AuthenticationManage
{
    /// <summary>
    /// AuthenticationManageView.xaml 的互動邏輯
    /// </summary>
    public partial class AuthenticationManageView : UserControl
    {
        public class AuthStatus
        {
            public AuthStatus(DataRow dataRow)
            {
                Name = dataRow["AUTH_NAME"].ToString();
                Status = Boolean.Parse( dataRow["AUTH_STATUS"].ToString());
            }

            public string Name { get; }
            public bool Status { get; }
        }

        Collection<AuthStatus> AuthStatuses { get; set; }

        public AuthenticationManageView()
        {
            InitializeComponent();

            InitAuthStatus();
        }

        private void InitAuthStatus()
        {
            AuthStatuses = AuthorityDb.GetAuthStatus();

            AuthStatus leaveStatus = AuthStatuses.Single(a => a.Name.Equals("Leave"));
            LeaveToggle.IsChecked = leaveStatus.Status;
            SetLeaveUi((bool)LeaveToggle.IsChecked);
        }

        private void AuthLeaveToggle(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton is null) return;
            
            SetLeaveUi((bool)toggleButton.IsChecked);

            AuthorityDb.ChangeAuthLeaveStatus((bool)toggleButton.IsChecked);
        }

        private void SetLeaveUi(bool status)
        {
            LeaveGrid.IsEnabled = status;
            LeaveConfirmStack.IsEnabled = status;
        }
    }
}
