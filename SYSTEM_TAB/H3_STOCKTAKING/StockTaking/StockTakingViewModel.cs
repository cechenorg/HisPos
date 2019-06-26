using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.StockTaking.StockTaking.StockTakingPage;
using His_Pos.NewClass.StockTaking.StockTakingPlan;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking
{
    public class StockTakingViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
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
        
        public StockTakingViewModel() {
            MainWindow.ServerConnection.OpenConnection();
            StockTakingPlanCollection = StockTakingPlans.GetStockTakingPlans();
            MainWindow.ServerConnection.CloseConnection();
            EmployeeCollection.Init();
            for (int i = 0; i < EmployeeCollection.Count; i++) {
                if (!EmployeeCollection[i].IsLocal) {
                    EmployeeCollection.Remove(EmployeeCollection[i]);
                    i--;
                }
            }
            EmployeeCollection.Add(ViewModelMainWindow.CurrentUser);
        }
    }
}
