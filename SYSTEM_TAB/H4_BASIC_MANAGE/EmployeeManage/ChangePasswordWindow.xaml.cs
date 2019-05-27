using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage
{
    /// <summary>
    /// ChangePasswordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        #region ----- Define Variables -----

        private int EmpId { get; }
        private string EmpOldPassword { get; }
        private string EmpNewPassword { get; set; }
        #endregion

        public ChangePasswordWindow(Employee e)
        {
            InitializeComponent();
            EmpId = e.ID;
            MainWindow.ServerConnection.OpenConnection();
            EmpOldPassword = e.GetPassword();
            MainWindow.ServerConnection.CloseConnection();
            ShowDialog();
        }

        private void ConfirmChangePassword_OnClick(object sender, RoutedEventArgs e)
        {
            if(!CheckPasswordValid()) return;
            MainWindow.ServerConnection.OpenConnection();
            EmployeeDb.ChangePassword(EmpId, EmpNewPassword);
            MainWindow.ServerConnection.CloseConnection(); 
            MessageWindow.ShowMessage("更新密碼成功!", MessageType.SUCCESS);
             
            Close();
        }

        private bool CheckPasswordValid()
        {
            if (!EmpOldPassword.Equals(OldPassword.Password))
            {
                MessageWindow.ShowMessage("舊密碼錯誤!", MessageType.ERROR);
                
                return false;
            }

            if (!NewPassword.Password.Equals(ConfirmNewPassword.Password))
            {
                MessageWindow.ShowMessage("新密碼與確認密碼不同!", MessageType.ERROR);
                
                return false;
            }

            if (NewPassword.Password.Length > 10 || NewPassword.Password.Length < 1)
            {
                MessageWindow.ShowMessage("新密碼長度需介於 1 ~ 10 之間!", MessageType.ERROR);
                
                return false;
            }

            Regex passwordReg = new Regex("[a-zA-Z0-9]*");
            Match match = passwordReg.Match(NewPassword.Password);

            if (!match.Value.Equals(NewPassword.Password))
            {
                MessageWindow.ShowMessage("新密碼中不能有符號!", MessageType.ERROR);
                
                return false;
            }

            EmpNewPassword = NewPassword.Password;
            return true;
        }

        private void Password_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as PasswordBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
