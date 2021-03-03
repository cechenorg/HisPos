using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.WareHouse;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewPlanWindow
{
    public class AddNewPlanWindowViewModel : ObservableObject
    {
        private NewClass.StockTaking.StockTakingPlan.StockTakingPlan stockTakingPlan = new NewClass.StockTaking.StockTakingPlan.StockTakingPlan();

        public NewClass.StockTaking.StockTakingPlan.StockTakingPlan StockTakingPlan
        {
            get { return stockTakingPlan; }
            set
            {
                Set(() => StockTakingPlan, ref stockTakingPlan, value);
            }
        }

        private WareHouses wareHouseCollection;

        public WareHouses WareHouseCollection
        {
            get { return wareHouseCollection; }
            set
            {
                Set(() => WareHouseCollection, ref wareHouseCollection, value);
            }
        }

        public RelayCommand InsertStockPlanCommand { get; set; }

        public AddNewPlanWindowViewModel()
        {
            WareHouseCollection = WareHouses.GetWareHouses();
            StockTakingPlan.WareHouse = WareHouseCollection[0];
            InsertStockPlanCommand = new RelayCommand(InsertStockPlanAction);
        }

        #region function

        private void InsertStockPlanAction()
        {
            StockTakingPlan.NewStockTakingPlan();
            MessageWindow.ShowMessage("新增成功!", Class.MessageType.SUCCESS);
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseAddNewPlanWindow"));
        }

        #endregion function
    }
}