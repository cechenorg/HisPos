using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
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
        public RelayCommand EditTypeCommand { get; set; }
        public RelayCommand DeleteTypeCommand { get; set; }
        #endregion

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
        #endregion

        public ProductTypeManageViewModel()
        {
            RegisterCommand();
            InitData();
        }

        #region ----- Define Actions -----
        private void AddTypeAction()
        {
            AddTypeWindow addTypeWindow = new AddTypeWindow(TypeManageCollection);
            addTypeWindow.ShowDialog();

            InitData();
        }
        private void EditTypeAction()
        {
            EditTypeWindow addTypeWindow = new EditTypeWindow();
            addTypeWindow.ShowDialog();

            InitData();
        }
        private void DeleteTypeAction()
        {
            if (CurrentType.ID == 0)
            {
                MessageWindow.ShowMessage("藥品無法被刪除", MessageType.ERROR);
                return;
            }

            DeleteTypeWindow deleteTypeWindow = new DeleteTypeWindow(CurrentType);
            deleteTypeWindow.ShowDialog();

            InitData();
        }
        #endregion

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
        #endregion
    }
}
