using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StockTaking.StockTakingPlan;
using His_Pos.NewClass.StockTaking.StockTakingPlanProduct;
using His_Pos.NewClass.WareHouse;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan
{
    public class StockTakingPlanViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----

        public RelayCommand AddPlanCommand { get; set; }
        public RelayCommand DeletePlanCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private StockTakingPlans stockTakingPlanCollection;
        private NewClass.StockTaking.StockTakingPlan.StockTakingPlan currentPlan;
        private StockTakingPlanProduct stockTakingPlanSelectProduct;
        private bool isDataChanged;
        private WareHouses wareHouses;

        public WareHouses WareHouses
        {
            get { return wareHouses; }
            set { Set(() => WareHouses, ref wareHouses, value); }
        }

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

        public StockTakingPlanProduct StockTakingPlanSelectProduct
        {
            get { return stockTakingPlanSelectProduct; }
            set
            {
                Set(() => StockTakingPlanSelectProduct, ref stockTakingPlanSelectProduct, value);
            }
        }

        public StockTakingPlans StockTakingPlanCollection
        {
            get { return stockTakingPlanCollection; }
            set { Set(() => StockTakingPlanCollection, ref stockTakingPlanCollection, value); }
        }

        public NewClass.StockTaking.StockTakingPlan.StockTakingPlan CurrentPlan
        {
            get { return currentPlan; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetPlanProducts();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentPlan, ref currentPlan, value);
            }
        }

        #endregion ----- Define Variables -----

        public StockTakingPlanViewModel()
        {
            RegisterCommand();
            InitPlans();
        }

        #region ----- Define Actions -----

        private void AddPlanAction()
        {
            AddNewPlanWindow.AddNewPlanWindow addNewPlanWindow = new AddNewPlanWindow.AddNewPlanWindow();
            MainWindow.ServerConnection.OpenConnection();
            StockTakingPlanCollection = StockTakingPlans.GetStockTakingPlans();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void DeletePlanAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否刪除此盤點計畫?", "盤點計畫刪除");
            if ((bool)confirmWindow.DialogResult)
            {
                CurrentPlan.Delete();
                StockTakingPlanCollection.Remove(CurrentPlan);
            }
            MessageWindow.ShowMessage("刪除成功", MessageType.SUCCESS);
        }

        private void AddProductAction()
        {
            Messenger.Default.Register<NotificationMessage<StockTakingPlanProducts>>(this, GetProductSubmit);
            AddNewProductWindow.AddNewProductWindow addNewProductWindow = new AddNewProductWindow.AddNewProductWindow(CurrentPlan.WareHouse.ID, CurrentPlan.StockTakingProductCollection);
            Messenger.Default.Unregister<NotificationMessage<StockTakingPlanProducts>>(this, GetProductSubmit);
            IsDataChanged = true;
        }

        private void GetProductSubmit(NotificationMessage<StockTakingPlanProducts> notificationMessage)
        {
            if (notificationMessage.Notification == "GetProductSubmit")
                AddProducts(notificationMessage.Content);
        }

        private void AddProducts(StockTakingPlanProducts stockTakingProducts)
        {
            CurrentPlan.StockTakingProductCollection.Clear();
            foreach (var s in stockTakingProducts)
            {
                if (CurrentPlan.StockTakingProductCollection.Count(c => c.ID == s.ID) == 0)
                    CurrentPlan.StockTakingProductCollection.Add(s);
            }
        }

        private void DataChangedAction()
        {
            IsDataChanged = true;
        }

        private void ConfirmChangeAction()
        {
            IsDataChanged = false;
            CurrentPlan.Update();
            MessageWindow.ShowMessage("更新盤點計畫成功", MessageType.SUCCESS);
        }

        private void CancelChangeAction()
        {
            IsDataChanged = false;
            CurrentPlan.GetPlanProducts();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommand()
        {
            AddPlanCommand = new RelayCommand(AddPlanAction);
            DeletePlanCommand = new RelayCommand(DeletePlanAction);
            AddProductCommand = new RelayCommand(AddProductAction);
            DataChangedCommand = new RelayCommand(DataChangedAction);
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsPlanDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsPlanDataChanged);
        }

        private bool IsPlanDataChanged()
        {
            return IsDataChanged;
        }

        private void InitPlans()
        {
            MainWindow.ServerConnection.OpenConnection();
            StockTakingPlanCollection = StockTakingPlans.GetStockTakingPlans();
            MainWindow.ServerConnection.CloseConnection();

            if (StockTakingPlanCollection != null && StockTakingPlanCollection.Count > 0)
                CurrentPlan = StockTakingPlanCollection[0];
        }

        #endregion ----- Define Functions -----
    }
}