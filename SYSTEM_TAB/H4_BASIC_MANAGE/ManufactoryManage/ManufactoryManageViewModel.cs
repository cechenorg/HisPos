using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory.ManufactoryManagement;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage.AddManufactoryWindow;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage
{
    public class ManufactoryManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand AddManufactoryCommand { get; set; }
        public RelayCommand DeleteManufactoryCommand { get; set; }
        public RelayCommand AddManufactoryPrincipalCommand { get; set; }
        public RelayCommand DeleteManufactoryPrincipalCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        #region ///// Search Variables /////

        public string SearchManufactoryName { get; set; } = "";
        public string SearchPrincipalName { get; set; } = "";

        #endregion ///// Search Variables /////

        private ManufactoryManageDetail currentManufactory;
        private ManufactoryManageDetail currentManufactoryBackUp;
        private ManufactoryManageDetails manufactoryManageCollection;
        private CurrentManufactoryTypeEnum currentManufactoryType = CurrentManufactoryTypeEnum.NONE;

        public ManufactoryManageDetail CurrentManufactory
        {
            get { return currentManufactory; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetManufactoryDetailData();
                MainWindow.ServerConnection.CloseConnection();
                currentManufactoryBackUp = value?.Clone() as ManufactoryManageDetail;
                Set(() => CurrentManufactory, ref currentManufactory, value);

                if (CurrentManufactory is null)
                    CurrentManufactoryType = CurrentManufactoryTypeEnum.NONE;
                else if (CurrentManufactory.ID == "0")
                    CurrentManufactoryType = CurrentManufactoryTypeEnum.SINGDE;
                else
                    CurrentManufactoryType = CurrentManufactoryTypeEnum.NORMAL;

                if (CurrentManufactory is null) return;

                CurrentManufactory.IsDataChanged = false;
                CancelChangeCommand.RaiseCanExecuteChanged();
                ConfirmChangeCommand.RaiseCanExecuteChanged();
            }
        }

        public ManufactoryManageDetails ManufactoryManageCollection
        {
            get { return manufactoryManageCollection; }
            set { Set(() => ManufactoryManageCollection, ref manufactoryManageCollection, value); }
        }

        public CurrentManufactoryTypeEnum CurrentManufactoryType
        {
            get { return currentManufactoryType; }
            set { Set(() => CurrentManufactoryType, ref currentManufactoryType, value); }
        }

        #endregion ----- Define Variables -----

        public ManufactoryManageViewModel()
        {
            RegisterCommand();
            SearchAction();
        }

        #region ----- Define Actions -----

        private void SearchAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            ManufactoryManageCollection = ManufactoryManageDetails.GetManufactoryManageDetailsBySearchCondition(SearchManufactoryName, SearchPrincipalName);
            MainWindow.ServerConnection.CloseConnection();

            if (ManufactoryManageCollection.Count > 0)
                CurrentManufactory = ManufactoryManageCollection[0];
            else
                MessageWindow.ShowMessage("無符合條件項目", MessageType.ERROR);
        }

        private void AddManufactoryAction()
        {
            Messenger.Default.Register<NotificationMessage<ManufactoryManageDetail>>(this, GetNewManufactory);
            AddManufactoryWindow.AddManufactoryWindow addManufactoryWindow = new AddManufactoryWindow.AddManufactoryWindow();
            addManufactoryWindow.ShowDialog();
            Messenger.Default.Unregister(this);
        }

        private void DeleteManufactoryAction()
        {
            if (CurrentManufactory.ID.Equals("0"))
            {
                MessageWindow.ShowMessage("杏德生技有限公司無法刪除!", MessageType.WARNING);
                return;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認刪除供應商?\n(刪除後無法復原)", "");

            if ((bool)confirmWindow.DialogResult)
            {
                MainWindow.ServerConnection.OpenConnection();
                bool isSuccess = CurrentManufactory.DeleteManufactory();
                MainWindow.ServerConnection.CloseConnection();

                if (isSuccess)
                {
                    ManufactoryManageCollection.Remove(CurrentManufactory);
                    MessageWindow.ShowMessage("刪除成功!", MessageType.SUCCESS);
                }
                else
                    MessageWindow.ShowMessage("刪除失敗 請稍後重試!", MessageType.ERROR);
            }
        }

        private void AddManufactoryPrincipalAction()
        {
            CurrentManufactory.AddManufactoryPrincipal();
            DataChangedAction();
        }

        private void DeleteManufactoryPrincipalAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認刪除負責人?", "");

            if ((bool)confirmWindow.DialogResult)
            {
                CurrentManufactory.DeleteManufactoryPrincipal();
                DataChangedAction();
            }
        }

        private void DataChangedAction()
        {
            if (CurrentManufactory is null) return;

            CurrentManufactory.IsDataChanged = true;
            CancelChangeCommand.RaiseCanExecuteChanged();
            ConfirmChangeCommand.RaiseCanExecuteChanged();
        }

        private void ConfirmChangeAction()
        {
            if (!CurrentManufactory.CheckUpdateDataValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認修改資料?", "");

            if ((bool)confirmWindow.DialogResult)
            {
                MainWindow.ServerConnection.OpenConnection();
                bool isSuccess = CurrentManufactory.UpdateManufactoryDetail();
                MainWindow.ServerConnection.CloseConnection();

                if (isSuccess)
                    MessageWindow.ShowMessage("更新成功!", MessageType.SUCCESS);
                else
                {
                    MessageWindow.ShowMessage("更新失敗 請稍後重試!", MessageType.ERROR);
                    return;
                }
            }

            CurrentManufactory.IsDataChanged = false;
            CancelChangeCommand.RaiseCanExecuteChanged();
            ConfirmChangeCommand.RaiseCanExecuteChanged();
        }

        private void CancelChangeAction()
        {
            CurrentManufactory.ResetData(currentManufactoryBackUp);
            RaisePropertyChanged(nameof(CurrentManufactory));

            CurrentManufactory.IsDataChanged = false;
            CancelChangeCommand.RaiseCanExecuteChanged();
            ConfirmChangeCommand.RaiseCanExecuteChanged();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommand()
        {
            SearchCommand = new RelayCommand(SearchAction);
            AddManufactoryCommand = new RelayCommand(AddManufactoryAction);
            DeleteManufactoryCommand = new RelayCommand(DeleteManufactoryAction);
            AddManufactoryPrincipalCommand = new RelayCommand(AddManufactoryPrincipalAction);
            DeleteManufactoryPrincipalCommand = new RelayCommand(DeleteManufactoryPrincipalAction);
            DataChangedCommand = new RelayCommand(DataChangedAction);
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsManufactoryDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsManufactoryDataChanged);
        }

        private bool IsManufactoryDataChanged()
        {
            if (CurrentManufactory is null) return false;

            return CurrentManufactory.IsDataChanged;
        }

        #region ///// Messenger Functions /////

        private void GetNewManufactory(NotificationMessage<ManufactoryManageDetail> notification)
        {
            if (notification.Sender is AddManufactoryWindowViewModel && notification.Notification.Equals(nameof(AddManufactoryWindowViewModel)))
            {
                ManufactoryManageCollection.Add(notification.Content);
                CurrentManufactory = notification.Content;
            }
        }

        #endregion ///// Messenger Functions /////

        #endregion ----- Define Functions -----
    }
}