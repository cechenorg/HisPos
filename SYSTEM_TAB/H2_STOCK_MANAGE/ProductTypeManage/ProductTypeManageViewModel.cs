using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.ProductType;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    class ProductTypeManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----
        public RelayCommand AddTypeCommand { get; set; }
        public RelayCommand DeleteTypeCommand { get; set; }
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private ProductTypeManageMasters typeManageCollection;
        private ProductTypeManageMaster currentType;
        private bool isDataChanged;

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
        public ProductTypeManageMasters TypeManageCollection
        {
            get { return typeManageCollection; }
            set { Set(() => TypeManageCollection, ref typeManageCollection, value); }
        }
        public ProductTypeManageMaster CurrentType
        {
            get { return currentType; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetTypeDetails();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentType, ref currentType, value);
            }
        }
        #endregion

        public ProductTypeManageViewModel()
        {
            RegisterCommand();
            InitData();
        }

        #region ----- Define Actions -----
        private void AddTypeAction()
        {
            AddTypeWindow addTypeWindow = new AddTypeWindow();
            addTypeWindow.ShowDialog();

            InitData();
        }
        private void DeleteTypeAction()
        {
            DeleteTypeWindow deleteTypeWindow = new DeleteTypeWindow();
            deleteTypeWindow.ShowDialog();

            InitData();
        }
        private void ConfirmChangeAction()
        {
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            IsDataChanged = false;
        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            AddTypeCommand = new RelayCommand(AddTypeAction);
            DeleteTypeCommand = new RelayCommand(DeleteTypeAction);
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsTypeDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsTypeDataChanged);
            DataChangedCommand = new RelayCommand(DataChangedAction);
        }
        private void InitData()
        {
            MainWindow.ServerConnection.OpenConnection();
            TypeManageCollection = ProductTypeManageMasters.GetProductTypeMasters();
            MainWindow.ServerConnection.CloseConnection();

            CurrentType = TypeManageCollection[0];
        }
        private bool IsTypeDataChanged()
        {
            return IsDataChanged;
        }
        #endregion
    }
}
