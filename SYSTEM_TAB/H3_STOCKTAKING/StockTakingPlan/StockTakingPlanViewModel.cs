using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StockTaking.StockTakingPlan;

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
        #endregion

        #region ----- Define Variables -----
        private StockTakingPlans stockTakingPlanCollection;
        private NewClass.StockTaking.StockTakingPlan.StockTakingPlan currentPlan;

        public StockTakingPlans StockTakingPlanCollection
        {
            get { return stockTakingPlanCollection; }
            set { Set(() => StockTakingPlanCollection, ref stockTakingPlanCollection, value); }
        }
        public NewClass.StockTaking.StockTakingPlan.StockTakingPlan CurrentPlan
        {
            get { return currentPlan; }
            set { Set(() => CurrentPlan, ref currentPlan, value); }
        }
        #endregion

        public StockTakingPlanViewModel()
        {
            RegisterCommand();
            InitPlans();
        }
        
        #region ----- Define Actions -----
        private void AddPlanAction()
        {

        }
        private void DeletePlanAction()
        {

        }
        private void AddProductAction()
        {

        }
        private void DataChangedAction()
        {
            CancelChangeCommand.RaiseCanExecuteChanged();
            ConfirmChangeCommand.RaiseCanExecuteChanged();
        }
        private void ConfirmChangeAction()
        {
            CancelChangeCommand.RaiseCanExecuteChanged();
            ConfirmChangeCommand.RaiseCanExecuteChanged();
        }
        private void CancelChangeAction()
        {
            CancelChangeCommand.RaiseCanExecuteChanged();
            ConfirmChangeCommand.RaiseCanExecuteChanged();
        }
        #endregion

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
            return false;
        }
        private void InitPlans()
        {
            MainWindow.ServerConnection.OpenConnection();
            StockTakingPlanCollection = StockTakingPlans.GetStockTakingPlans();
            MainWindow.ServerConnection.CloseConnection();
        }
        #endregion
    }
}
