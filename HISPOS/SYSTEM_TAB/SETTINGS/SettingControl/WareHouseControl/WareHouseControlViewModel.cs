using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.WareHouse;
using System.Data;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.WareHouseControl
{
    public class WareHouseControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand AddNewWareHouseCommand { get; set; }
        public RelayCommand DeleteWareHouseCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private WareHouseSettings wareHouseCollection;

        public WareHouseSettings WareHouseCollection
        {
            get => wareHouseCollection;
            set { Set(() => WareHouseCollection, ref wareHouseCollection, value); }
        }

        public WareHouseSetting SelectedWareHouse { get; set; }

        #endregion ----- Define Variables -----

        public WareHouseControlViewModel()
        {
            RegisterCommands();
            InitData();
        }

        #region ----- Define Actions -----

        private void AddNewWareHouseAction()
        {
            AddWareHouseWindow addWareHouseWindow = new AddWareHouseWindow();
            addWareHouseWindow.ShowDialog();

            if (!addWareHouseWindow.IsNewNameConfirmed) return;

            bool isSuccess = false;
            MainWindow.ServerConnection.OpenConnection();

            DataTable dataTable = WareHouseDb.AddNewWareHouse(addWareHouseWindow.NewName.Trim());

            if (dataTable != null && dataTable.Rows.Count > 0)
                isSuccess = dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");

            MainWindow.ServerConnection.CloseConnection();

            if (!isSuccess)
                MessageWindow.ShowMessage("網路連線異常 新增失敗", MessageType.ERROR);

            InitData();
        }

        private void DeleteWareHouseAction()
        { 
            if (!SelectedWareHouse.CanDelete)
            {
                MessageWindow.ShowMessage("欲刪除之庫別庫存需清空!", MessageType.ERROR);
                return;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認刪除 {SelectedWareHouse.Name}", "刪除庫別");

            if ((bool)confirmWindow.DialogResult)
            {
                MainWindow.ServerConnection.OpenConnection();

                bool isSuccess = false;
                DataTable dataTable = WareHouseDb.DeleteWareHouse(SelectedWareHouse.ID);

                if (dataTable is null || dataTable.Rows.Count == 0)
                    isSuccess = false;
                else
                    isSuccess = dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
               
                MainWindow.ServerConnection.CloseConnection();

                if (isSuccess)
                    MessageWindow.ShowMessage("成功刪除!", MessageType.SUCCESS);
                else
                    MessageWindow.ShowMessage("網路連線異常 刪除失敗", MessageType.ERROR);
            }

            InitData();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            AddNewWareHouseCommand = new RelayCommand(AddNewWareHouseAction);
            DeleteWareHouseCommand = new RelayCommand(DeleteWareHouseAction);
        }

        private void InitData()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = WareHouseDb.GetWareHouseSettings();
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable is null || dataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路連線異常!", MessageType.ERROR);
                return;
            }

            WareHouseCollection = new WareHouseSettings(dataTable);
        }

        #endregion ----- Define Functions -----
    }
}