using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.StockTaking.StockTaking.StockTakingPage;
using His_Pos.NewClass.StockTaking.StockTakingPlan;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using His_Pos.NewClass.WareHouse;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewProductWindow;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

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
        private WareHouses wareHouses;
        public WareHouses WareHouses
        {
            get { return wareHouses; }
            set { Set(() => WareHouses, ref wareHouses, value); }
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
       
        private NewClass.StockTaking.StockTakingPlan.StockTakingPlan currentPlan = new NewClass.StockTaking.StockTakingPlan.StockTakingPlan();
        public NewClass.StockTaking.StockTakingPlan.StockTakingPlan CurrentPlan
        {
            get { return currentPlan; }
            set {
                Set(() => CurrentPlan, ref currentPlan, value); 
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
        private StockTakingProduct stockTakingResultProductSelected ;
        public StockTakingProduct StockTakingResultProductSelected
        {
            get { return stockTakingResultProductSelected; }
            set
            {
                Set(() => StockTakingResultProductSelected, ref stockTakingResultProductSelected, value);
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
        public RelayCommand ExportCsvCommand { get; set; }
        public RelayCommand AddNewProductCommand { get; set; }

        public StockTakingViewModel() {
            RegisterCommand();
            WareHouses = VM.WareHouses;
            CurrentPlan.WareHouse = WareHouses[0];
            EmployeeCollection.Init();
            for (int i = 0; i < EmployeeCollection.Count; i++) {
                if (!EmployeeCollection[i].IsLocal) {
                    EmployeeCollection.Remove(EmployeeCollection[i]);
                    i--;
                }
            }
            EmployeeCollection.Add(ViewModelMainWindow.CurrentUser);
        } 
        private void SetDiffInventoryAmountAction() {
            DiffInventoryAmount = StockTakingResult.StockTakingProductCollection.Count(s => s.NewInventory != s.Inventory); 
            if (StockTakingResultProductSelected is null) return;
            StockTakingResultProductSelected.IsUpdate = true;
        }
        private void LastToResultPageAction() {
            StockTakingType = StockTakingType.Result;
        }
        private void LastToChoosePageAction() {
            StockTakingPageCollection.AssignPages(CurrentPlan);
            StockTakingType = StockTakingType.Choose;
        }
        private void NextToReasonPageAction() {
            if (StockTakingResult.StockTakingProductCollection.Count(s => s.NewInventory < 0) > 0) {
                MessageWindow.ShowMessage("盤點量不可為負值!",Class.MessageType.ERROR);
                return;
            }

            if(DiffInventoryAmount == 0)  {
                ConfirmWindow confirmWindow = new ConfirmWindow("未有盤差品項 是否直接盤點?","盤點確認");
                if ((bool)confirmWindow.DialogResult) {
                    StockTakingResult.WareHouse = CurrentPlan.WareHouse;
                    StockTakingResult.InsertStockTaking();
                    MessageWindow.ShowMessage("盤點完成", Class.MessageType.SUCCESS);
                    StockTakingType = StockTakingType.Choose; 
                } 
                return;
            }

            StockTakingReason.StockTakingProductCollection.Clear();
            foreach (var s in StockTakingResult.StockTakingProductCollection) {
                if (s.ValueDiff != 0)
                    StockTakingReason.StockTakingProductCollection.Add(s);
            } 
            StockTakingType = StockTakingType.Reason;
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
        private void CompleteStockTakingAction() {
            foreach (var s in StockTakingReason.StockTakingProductCollection)
            {
                StockTakingResult.StockTakingProductCollection.Single(st => st.ID == s.ID).Note = s.Note;
            }
            StockTakingResult.WareHouse = CurrentPlan.WareHouse;
            StockTakingResult.InsertStockTaking();
            MessageWindow.ShowMessage("盤點完成", Class.MessageType.SUCCESS);
            StockTakingType = StockTakingType.Choose;
        }
        private void FillUnTakingInventoryAction() {
            for (int i = 0; i < StockTakingResult.StockTakingProductCollection.Count; i++)
            {
                if (!StockTakingResult.StockTakingProductCollection[i].IsUpdate) { 
                    StockTakingResult.StockTakingProductCollection[i].NewInventory =
                        StockTakingResult.StockTakingProductCollection[i].Inventory < 0 ? 0 : StockTakingResult.StockTakingProductCollection[i].Inventory;
                }
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
        private void ExportCsvAction() {
            StockTakingResult.StockTakingProductCollection = StockTakingProducts.GetStockTakingPlanProducts(CurrentPlan.StockTakingProductCollection, CurrentPlan.WareHouse.ID);
           
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = CurrentPlan.Name +  "盤點單";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") + CurrentPlan.Name + "盤點單";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("庫名," + CurrentPlan.WareHouse.Name);
                        file.WriteLine("商品代碼,藥品名稱,庫存,盤點量");
                        foreach (var s in StockTakingResult.StockTakingProductCollection)
                        {
                            file.WriteLine($"{s.ID},{s.FullName},{s.Inventory},");
                        }
                         
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                } 
            }
        }
        private void AddNewProductAction() {
            Messenger.Default.Register<NotificationMessage<StockTakingPlanProducts>>(this, GetProductSubmit);
            AddNewProductWindow addNewProductWindow = new AddNewProductWindow(CurrentPlan.WareHouse.ID, CurrentPlan.StockTakingProductCollection);
            Messenger.Default.Unregister<NotificationMessage<StockTakingPlanProducts>>(this, GetProductSubmit);

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
            ExportCsvCommand = new RelayCommand(ExportCsvAction);
            AddNewProductCommand = new RelayCommand(AddNewProductAction);
        }
    }
}
