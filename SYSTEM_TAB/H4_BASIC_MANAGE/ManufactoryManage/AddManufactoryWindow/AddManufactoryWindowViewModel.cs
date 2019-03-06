using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Manufactory.ManufactoryManagement;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage.AddManufactoryWindow
{
    public class AddManufactoryWindowViewModel
    {
        #region ----- Define Commands -----
        public RelayCommand ConfirmAddManufactoryCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public string ManufactoryName { get; set; }
        public string ManufactoryNickName { get; set; }
        public string ManufactoryTelephone { get; set; }
        public string ManufactoryAddress { get; set; }
        #endregion

        public AddManufactoryWindowViewModel()
        {
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void ConfirmAddManufactoryAction()
        {
            if(!CheckNewDataValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ManufactoryDB.AddNewManufactory(ManufactoryName, ManufactoryNickName, ManufactoryTelephone, ManufactoryAddress);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0)
            {
                ManufactoryManageDetail manufactory = new ManufactoryManageDetail(dataTable.Rows[0]);
                Messenger.Default.Send(new NotificationMessage<ManufactoryManageDetail>(this, manufactory, nameof(AddManufactoryWindowViewModel)));
            }
            else
                MessageWindow.ShowMessage("網路異常 新增失敗!", MessageType.ERROR);

            Messenger.Default.Send(new NotificationMessage(this, "CloseAddManufactoryWindow"));
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            ConfirmAddManufactoryCommand = new RelayCommand(ConfirmAddManufactoryAction);
        }
        private bool CheckNewDataValid()
        {
            if (ManufactoryName.Trim().Equals(string.Empty))
            {
                MessageWindow.ShowMessage("供應商名稱為必填欄位!", MessageType.ERROR);
                return false;
            }

            return true;
        }
        #endregion
    }
}
