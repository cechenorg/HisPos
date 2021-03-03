using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage.EmployeeChangePasswordWindow
{
    public class EmployeeChangePasswordWindowViewModel : ViewModelBase
    {
        #region Var

        private string oldPassword;

        public string OldPassword
        {
            get => oldPassword;
            set
            {
                Set(() => OldPassword, ref oldPassword, value);
            }
        }

        private string newPassword;

        public string NewPassword
        {
            get => newPassword;
            set
            {
                Set(() => NewPassword, ref newPassword, value);
            }
        }

        private string confirmPassword;

        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                Set(() => ConfirmPassword, ref confirmPassword, value);
            }
        }

        private Employee employeeSelected;

        public Employee EmployeeSelected
        {
            get => employeeSelected;
            set
            {
                Set(() => EmployeeSelected, ref employeeSelected, value);
            }
        }

        public RelayCommand SubmitCommand { get; set; }

        #endregion Var

        public EmployeeChangePasswordWindowViewModel(Employee e)
        {
            EmployeeSelected = e;
            SubmitCommand = new RelayCommand(SubmitAction);
        }

        private void SubmitAction()
        {
            if (OldPassword != EmployeeSelected.Password)
            {
                MessageWindow.ShowMessage("舊密碼錯誤!", Class.MessageType.ERROR);
                return;
            }
            else if (ConfirmPassword != NewPassword)
            {
                MessageWindow.ShowMessage("新密碼確認錯誤!", Class.MessageType.ERROR);
                return;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow("是否修改密碼?", "密碼修改確認");
            if ((bool)confirmWindow.DialogResult)
            {
                EmployeeSelected.Password = NewPassword;
                EmployeeSelected.Update();
                MessageWindow.ShowMessage("修改成功!", Class.MessageType.SUCCESS);
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseEmployeeChangePasswordWindow"));
            }
        }
    }
}