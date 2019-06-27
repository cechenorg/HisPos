using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.StockTaking.StockTaking.StockTakingPage;
using His_Pos.NewClass.StockTaking.StockTakingPlan;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking
{
    public class StockTakingViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        private StockTakingType stockTakingType = StockTakingType.Choose;
        public StockTakingType StockTakingType
        {
            get { return stockTakingType; }
            set
            {
                Set(() => StockTakingType, ref stockTakingType, value);
            }
        }
        private int diffInventoryAmount;
        public int DiffInventoryAmount
        {
            get { return diffInventoryAmount; }
            set
            {
                Set(() => DiffInventoryAmount, ref diffInventoryAmount, value);
            }
        }
        private Employees employeeCollection = new Employees();
        public Employees EmployeeCollection
        {
            get { return employeeCollection; }
            set
            {
                Set(() => EmployeeCollection, ref employeeCollection, value);
            }
        }
        private StockTakingPages stockTakingPageCollection = new StockTakingPages();
        public StockTakingPages StockTakingPageCollection
        {
            get { return stockTakingPageCollection; }
            set {
                Set(() => StockTakingPageCollection, ref stockTakingPageCollection, value);
            }
        }
        private StockTakingPlans stockTakingPlanCollection ;
        public StockTakingPlans StockTakingPlanCollection
        {
            get { return stockTakingPlanCollection; }
            set { Set(() => StockTakingPlanCollection, ref stockTakingPlanCollection, value); }
        }
        private NewClass.StockTaking.StockTakingPlan.StockTakingPlan currentPlan;
        public NewClass.StockTaking.StockTakingPlan.StockTakingPlan CurrentPlan
        {
            get { return currentPlan; }
            set {
                Set(() => CurrentPlan, ref currentPlan, value);
                MainWindow.ServerConnection.OpenConnection();
                value?.GetPlanProducts();
                MainWindow.ServerConnection.CloseConnection();
                StockTakingPageCollection.AssignPages(CurrentPlan);
            }
        }
        private NewClass.StockTaking.StockTaking.StockTaking stockTakingResult = new NewClass.StockTaking.StockTaking.StockTaking();
        public NewClass.StockTaking.StockTaking.StockTaking StockTakingResult
        {
            get { return stockTakingResult; }
            set
            {
                Set(() => StockTakingResult, ref stockTakingResult, value); 
            }
        }
        private NewClass.StockTaking.StockTaking.StockTaking stockTakingReason = new NewClass.StockTaking.StockTaking.StockTaking();
        public NewClass.StockTaking.StockTaking.StockTaking StockTakingReason
        {
            get { return stockTakingReason; }
            set
            {
                Set(() => StockTakingReason, ref stockTakingReason, value);
            }
        }
        public RelayCommand AssignPageCommand { get; set; }
        public RelayCommand ClearStockTakingProductCommand { get; set; }
        public RelayCommand NextToResultPageCommand { get; set; }
        public RelayCommand NextToReasonPageCommand { get; set; }
        public RelayCommand FillUnTakingInventoryCommand { get; set; }
        public RelayCommand LastToChoosePageCommand { get; set; }
        public RelayCommand SetDiffInventoryAmountCommand { get; set; }
        public RelayCommand LastToResultPageCommand { get; set; }
        public RelayCommand CompleteStockTakingCommand { get; set; }

        public StockTakingViewModel() {
            RegisterCommand();

            MainWindow.ServerConnection.OpenConnection();
            StockTakingPlanCollection = StockTakingPlans.GetStockTakingPlans();
            MainWindow.ServerConnection.CloseConnection();
            if (StockTakingPlanCollection.Count > 0)
                CurrentPlan = StockTakingPlanCollection[0];

            EmployeeCollection.Init();
            for (int i = 0; i < EmployeeCollection.Count; i++) {
                if (!EmployeeCollection[i].IsLocal) {
                    EmployeeCollection.Remove(EmployeeCollection[i]);
                    i--;
                }
            }
            EmployeeCollection.Add(ViewModelMainWindow.CurrentUser);
        }
        private void CompleteStockTakingAction() {
            foreach (var s in StockTakingReason.StockTakingProductCollection) {
                StockTakingResult.StockTakingProductCollection.Single(st => st.ID == s.ID).Note = s.Note; 
            }
            StockTakingResult.WareHouse = CurrentPlan.WareHouse;
            StockTakingResult.InsertStockTaking();
            MessageWindow.ShowMessage("盤點完成",Class.MessageType.SUCCESS);
            StockTakingType = StockTakingType.Choose;
        }
        private void LastToResultPageAction() {
            StockTakingType = StockTakingType.Result;
        }
        private void NextToReasonPageAction() {
            StockTakingReason.StockTakingProductCollection.Clear();
            foreach (var s in StockTakingResult.StockTakingProductCollection) {
                if (s.ValueDiff != 0)
                    StockTakingReason.StockTakingProductCollection.Add(s);
            } 
            StockTakingType = StockTakingType.Reason;
        }
        private void SetDiffInventoryAmountAction() {
            DiffInventoryAmount = StockTakingResult.StockTakingProductCollection.Count(s => s.NewInventory != s.Inventory);
        }
        private void LastToChoosePageAction() {
            StockTakingPageCollection.AssignPages(CurrentPlan);
            StockTakingType = StockTakingType.Choose;
        }
        private void NextToResultPageAction() {
            StockTakingPageCollection.AssignPages(CurrentPlan);
            StockTakingResult.StockTakingProductCollection = StockTakingProducts.GetStockTakingPlanProducts(CurrentPlan.StockTakingProductCollection, CurrentPlan.WareHouse.ID);
            int index;
            int counti = 0;
            for (int i = 0; i < StockTakingPageCollection.Count; i++) {
                index = StockTakingPageCollection[i].Amount;
                while (index > 0) {
                    StockTakingResult.StockTakingProductCollection[counti].Employee = StockTakingPageCollection[i].Employee;
                    index--;
                    counti++;
                } 
            }
            SetDiffInventoryAmountAction();
            StockTakingType = StockTakingType.Result;
        }
        private void FillUnTakingInventoryAction() {
            for (int i = 0; i < StockTakingResult.StockTakingProductCollection.Count; i++)
            {
                StockTakingResult.StockTakingProductCollection[i].NewInventory = 
                    StockTakingResult.StockTakingProductCollection[i].Inventory < 0 ? 0 : StockTakingResult.StockTakingProductCollection[i].Inventory;
            }
            SetDiffInventoryAmountAction();
        }
        private void AssignPageAction()
        { 
            StockTakingPageCollection.AssignPages(CurrentPlan);
        }
        private void ClearStockTakingProductAction()
        {
            CurrentPlan.StockTakingProductCollection.Clear();
            StockTakingPageCollection.AssignPages(CurrentPlan); 
        }
        private void RegisterCommand() {
            AssignPageCommand = new RelayCommand(AssignPageAction);
            ClearStockTakingProductCommand = new RelayCommand(ClearStockTakingProductAction);
            NextToResultPageCommand = new RelayCommand(NextToResultPageAction);
            FillUnTakingInventoryCommand = new RelayCommand(FillUnTakingInventoryAction);
            LastToChoosePageCommand = new RelayCommand(LastToChoosePageAction);
            SetDiffInventoryAmountCommand = new RelayCommand(SetDiffInventoryAmountAction);
            NextToReasonPageCommand = new RelayCommand(NextToReasonPageAction);
            LastToResultPageCommand = new RelayCommand(LastToResultPageAction);
            CompleteStockTakingCommand = new RelayCommand(CompleteStockTakingAction);
        }
    }
}
