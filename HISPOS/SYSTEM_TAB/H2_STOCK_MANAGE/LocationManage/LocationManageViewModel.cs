using His_Pos.ChromeTabViewModel;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    internal class LocationManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        /*public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----

        public RelayCommand AddTypeCommand { get; set; }
        public RelayCommand EditTypeCommand { get; set; }
        public RelayCommand DeleteTypeCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private ProductTypeManageMasters typeManageCollection;
        private ProductTypeManageMaster currentType;
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

        #endregion ----- Define Variables -----

        public LocationManageViewModel()
        {
            RegisterCommand();
            InitData();
        }

        #region ----- Define Actions -----

        private void AddTypeAction()
        {
            AddLocationWindow addTypeWindow = new AddLocationWindow();
            addTypeWindow.ShowDialog();

            InitData();
        }
        private void EditTypeAction()
        {
            if (CurrentType.ID == 0)
            {
                MessageWindow.ShowMessage("藥品類別無法編輯", MessageType.ERROR);
                return;
            }

            EditLocationWindow editTypeWindow = new EditLocationWindow();
            editTypeWindow.ShowDialog();

            InitData();
        }
        private void DeleteTypeAction()
        {
            if (CurrentType.ID == 0)
            {
                MessageWindow.ShowMessage("藥品無法被刪除", MessageType.ERROR);
                return;
            }

            DeleteLocationWindow deleteTypeWindow = new DeleteLocationWindow();
            deleteTypeWindow.ShowDialog();

            InitData();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommand()
        {
            AddTypeCommand = new RelayCommand(AddTypeAction);
            EditTypeCommand = new RelayCommand(EditTypeAction);
            DeleteTypeCommand = new RelayCommand(DeleteTypeAction);
        }
        private void InitData()
        {
            MainWindow.ServerConnection.OpenConnection();
            TypeManageCollection = ProductTypeManageMasters.GetProductTypeMasters();
            MainWindow.ServerConnection.CloseConnection();

            CurrentType = TypeManageCollection[0];
        }

        #endregion ----- Define Functions -----

        */
    }
}