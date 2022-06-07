using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.AuthenticationManage
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
                Status = Boolean.Parse(dataRow["AUTH_STATUS"].ToString());
            }

            public string Name { get; }
            public bool Status { get; }
        }

        private Collection<AuthStatus> AuthStatuses { get; set; }
        //private Collection<AuthLeaveRecord> authLeaveRecords;

        //public Collection<AuthLeaveRecord> AuthLeaveRecords
        //{
        //    get { return authLeaveRecords; }
        //    set
        //    {
        //        authLeaveRecords = value;
        //        NotifyPropertyChanged("AuthLeaveRecords");
        //    }
        //}

        //public string AuthLeaveCheckedCount {
        //    get { return AuthLeaveRecords.Count(al => al.IsSelected).ToString(); }
        //}

        public static AuthenticationManageView Instance;
        public static bool DataChanged;

        public AuthenticationManageView()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            DataChanged = false;
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

        public void InitAuthRecord()
        {
            ///AuthLeaveRecords = AuthorityDb.GetLeaveRecord();
        }

        private void InitAuthStatus()
        {
            ///AuthStatuses = AuthorityDb.GetAuthStatus();

            AuthStatus leaveStatus = AuthStatuses.Single(a => a.Name.Equals("Leave"));
            LeaveToggle.IsChecked = leaveStatus.Status;
            SetLeaveUi((bool)LeaveToggle.IsChecked);
        }

        private void AuthLeaveToggle(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton is null) return;

            SetLeaveUi((bool)toggleButton.IsChecked);

            ///AuthorityDb.ChangeAuthLeaveStatus((bool)toggleButton.IsChecked);
        }

        private void SetLeaveUi(bool status)
        {
            LeaveGrid.IsEnabled = status;
            LeaveConfirmStack.IsEnabled = status;
        }

        private void AuthLeave_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateUi();

            //AuthLeaveAllSelectCheckBox.IsChecked =
            //    AuthLeaveRecords.Count(al => al.IsSelected) == AuthLeaveRecords.Count;
        }

        public void UpdateUi()
        {
            NotifyPropertyChanged("AuthLeaveCheckedCount");
        }

        private void AuthLeaveAllSelectCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            bool selectStatus = (bool)AuthLeaveAllSelectCheckBox.IsChecked;

            //foreach (var authLeaveRecord in AuthLeaveRecords)
            //{
            //    authLeaveRecord.IsSelected = selectStatus;
            //}

            UpdateUi();
        }

        private void LeaveConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            ///AuthorityDb.AuthLeaveConfirm(AuthLeaveRecords.Where(al => al.IsSelected).ToList());

            MessageWindow.ShowMessage("審核成功!", MessageType.SUCCESS);

            //WorkScheduleManageView.DataChanged = true;

            InitAuthRecord();
            UpdateUi();
        }
    }
}