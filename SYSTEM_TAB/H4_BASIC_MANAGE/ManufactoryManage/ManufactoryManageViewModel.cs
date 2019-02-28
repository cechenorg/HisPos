using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
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
        public ManufactoryManageDetail CurrentManufactory { get; set; }
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

        }
        private void DeleteManufactoryAction()
        {

        }
        private void AddManufactoryPrincipalAction()
        {

        }
        private void DeleteManufactoryPrincipalAction()
        {

        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        private void ConfirmChangeAction()
        {
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
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
        #endregion
    }
}
