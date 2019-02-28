using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory.ManufactoryManagement;

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
        #endregion

        #region ----- Define Variables -----
        private bool isDataChanged;
        private ManufactoryManageDetail currentManufactory;
        private ManufactoryManageDetail currentManufactoryBackUp;

        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set
            {
                Set(() => IsDataChanged, ref isDataChanged, value);
                CancelChangeCommand.RaiseCanExecuteChanged();
                ConfirmChangeCommand.RaiseCanExecuteChanged();
            }
        }
        public ManufactoryManageDetail CurrentManufactory
        {
            get { return currentManufactory; }
            set
            {
                if (IsDataChanged)
                {
                    MessageWindow.ShowMessage("資料有異動　請先確認變更再切換供應商!", MessageType.ERROR);
                    return;
                }

                MainWindow.ServerConnection.OpenConnection();
                value?.GetManufactoryDetailData();
                MainWindow.ServerConnection.CloseConnection();
                currentManufactoryBackUp = value?.Clone() as ManufactoryManageDetail;
                Set(() => CurrentManufactory, ref currentManufactory, value);
            }
        }
        public ManufactoryManageDetails ManufactoryManageCollection { get; set; }
        #endregion

        public ManufactoryManageViewModel()
        {
            RegisterCommand();
        }

        #region ----- Define Actions -----
        private void SearchAction()
        {

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
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認刪除供應商?\n(刪除後無法復原)", "");

            if ((bool) confirmWindow.DialogResult)
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
        }
        private void DeleteManufactoryPrincipalAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認刪除負責人?", "");

            if ((bool)confirmWindow.DialogResult)
                CurrentManufactory.DeleteManufactoryPrincipal();
        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        private void ConfirmChangeAction()
        {
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
                    CurrentManufactory.ResetData(currentManufactoryBackUp);
                }
            }

            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            CurrentManufactory.ResetData(currentManufactoryBackUp);

            IsDataChanged = false;
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            SearchCommand = new RelayCommand(SearchAction);
            AddManufactoryCommand = new RelayCommand(SearchAction);
            DeleteManufactoryCommand = new RelayCommand(SearchAction);
            AddManufactoryPrincipalCommand = new RelayCommand(SearchAction);
            DeleteManufactoryPrincipalCommand = new RelayCommand(SearchAction);
            DataChangedCommand = new RelayCommand(SearchAction);
            ConfirmChangeCommand = new RelayCommand(SearchAction, IsManufactoryDataChanged);
            CancelChangeCommand = new RelayCommand(SearchAction, IsManufactoryDataChanged);
        }
        private bool IsManufactoryDataChanged()
        {
            return IsDataChanged;
        }

        #region ///// Messenger Functions /////
        private void GetNewManufactory(NotificationMessage<ManufactoryManageDetail> notification)
        {

        }
        #endregion

        #endregion
    }
}
