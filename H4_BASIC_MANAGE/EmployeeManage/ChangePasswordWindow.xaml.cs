using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using His_Pos.Class.Employee;

namespace His_Pos.H4_BASIC_MANAGE.EmployeeManage
{
    /// <summary>
    /// ChangePasswordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        #region ----- Define Variables -----

        private string EmpId { get; }
        private string EmpOldPassword { get; }
        private string EmpNewPassword { get; set; }
        #endregion

        public ChangePasswordWindow(string id)
        {
            InitializeComponent();
            EmpId = id;
            EmpOldPassword = EmployeeDb.GetEmployeePassword(EmpId);
        }

        private void ConfirmChangePassword_OnClick(object sender, RoutedEventArgs e)
        {
            if(!CheckPasswordValid()) return;

            EmployeeDb.SetEmployeePassword(EmpId, EmpNewPassword);

            MessageWindow messageWindow = new MessageWindow("更新密碼成功!", MessageType.SUCCESS);
            messageWindow.ShowDialog();

            Close();
        }

        private bool CheckPasswordValid()
        {
            if (!EmpOldPassword.Equals(OldPassword.Password))
            {
                MessageWindow messageWindow = new MessageWindow("舊密碼錯誤!", MessageType.ERROR);
                messageWindow.ShowDialog();
                return false;
            }

            if (!NewPassword.Password.Equals(ConfirmNewPassword.Password))
            {
                MessageWindow messageWindow = new MessageWindow("新密碼與確認密碼不同!", MessageType.ERROR);
                messageWindow.ShowDialog();
                return false;
            }

            if (NewPassword.Password.Length > 10 || NewPassword.Password.Length < 1)
            {
                MessageWindow messageWindow = new MessageWindow("新密碼長度需介於 1 ~ 10 之間!", MessageType.ERROR);
                messageWindow.ShowDialog();
                return false;
            }

            Regex passwordReg = new Regex("[a-zA-Z0-9]*");
            Match match = passwordReg.Match(NewPassword.Password);

            if (!match.Value.Equals(NewPassword.Password))
            {
                MessageWindow messageWindow = new MessageWindow("新密碼中不能有符號!", MessageType.ERROR);
                messageWindow.ShowDialog();
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
