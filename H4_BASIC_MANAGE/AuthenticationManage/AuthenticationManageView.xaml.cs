using His_Pos.Class.Authority;
using His_Pos.Class.Leave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using His_Pos.Class;

namespace His_Pos.H4_BASIC_MANAGE.AuthenticationManage
{
    /// <summary>
    /// AuthenticationManageView.xaml 的互動邏輯
    /// </summary>
    public partial class AuthenticationManageView : UserControl, INotifyPropertyChanged
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
        private Collection<AuthLeaveRecord> authLeaveRecords;

        public Collection<AuthLeaveRecord> AuthLeaveRecords
        {
            get { return authLeaveRecords; }
            set
            {
                authLeaveRecords = value;
                NotifyPropertyChanged("AuthLeaveRecords");
            }
        }

        public string AuthLeaveCheckedCount {
            get { return AuthLeaveRecords.Count(al => al.IsSelected).ToString(); }
        }

        public AuthenticationManageView()
        {
            InitializeComponent();
            DataContext = this;
            InitAuthStatus();

            InitAuthRecord();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void InitAuthRecord()
        {
            AuthLeaveRecords = AuthorityDb.GetLeaveRecord();
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

        private void AuthLeave_OnClick(object sender, RoutedEventArgs e)
        {
            NotifyPropertyChanged("AuthLeaveCheckedCount");

            AuthLeaveAllSelectCheckBox.IsChecked =
                AuthLeaveRecords.Count(al => al.IsSelected) == AuthLeaveRecords.Count;
        }

        private void AuthLeaveAllSelectCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            bool selectStatus = (bool)AuthLeaveAllSelectCheckBox.IsChecked;

            foreach (var authLeaveRecord in AuthLeaveRecords)
            {
                authLeaveRecord.IsSelected = selectStatus;
            }

            NotifyPropertyChanged("AuthLeaveCheckedCount");
        }

        private void LeaveConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            AuthorityDb.AuthLeaveConfirm(AuthLeaveRecords.Where(al => al.IsSelected).ToList());

            MessageWindow messageWindow = new MessageWindow("審核成功!", MessageType.SUCCESS);
            messageWindow.ShowDialog();

            InitAuthRecord();
            NotifyPropertyChanged("AuthLeaveCheckedCount");
        }
    }
}
