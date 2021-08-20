using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.StockTaking.StockTakingPlanProduct;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using His_Pos.NewClass.WareHouse;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewProductWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
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

        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }

        private string busyContent;

        public string BusyContent
        {
            get => busyContent;
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
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

        private string productSearchName;

        public string ProductSearchName
        {
            get { return productSearchName; }
            set
            {
                Set(() => ProductSearchName, ref productSearchName, value);
            }
        }

        private bool isAlphabetCheck;

        public bool IsAlphabetCheck
        {
            get { return isAlphabetCheck; }
            set
            {
                Set(() => IsAlphabetCheck, ref isAlphabetCheck, value);
            }
        }

        private bool sourceallItemsAreChecked;

        public bool SourceallItemsAreChecked
        {
            get { return sourceallItemsAreChecked; }
            set
            {
                Set(() => SourceallItemsAreChecked, ref sourceallItemsAreChecked, value);
                for (int i = 0; i < WordsView.Count; i++)
                {
                    WordsView[i].IsSelected = value;
                }
            }
        }

        private StockTakingPlanProduct sourceStockTakingProductSelectedItem;

        public StockTakingPlanProduct SourceStockTakingProductSelectedItem
        {
            get { return sourceStockTakingProductSelectedItem; }
            set
            {
                Set(() => SourceStockTakingProductSelectedItem, ref sourceStockTakingProductSelectedItem, value);
            }
        }

        private StockTakingPlanProducts sourceStockTakingProducts = new StockTakingPlanProducts();

        public StockTakingPlanProducts SourceStockTakingProducts
        {
            get { return sourceStockTakingProducts; }
            set
            {
                Set(() => SourceStockTakingProducts, ref sourceStockTakingProducts, value);
            }
        }

        private NewClass.StockTaking.StockTakingPlan.StockTakingPlan currentPlan = new NewClass.StockTaking.StockTakingPlan.StockTakingPlan();

        public NewClass.StockTaking.StockTakingPlan.StockTakingPlan CurrentPlan
        {
            get { return currentPlan; }
            set
            {
                Set(() => CurrentPlan, ref currentPlan, value);
            }
        }

        private StockTakingPlanProduct stockTakingPlanProductSelected;

        public StockTakingPlanProduct StockTakingPlanProductSelected
        {
            get { return stockTakingPlanProductSelected; }
            set
            {
                Set(() => StockTakingPlanProductSelected, ref stockTakingPlanProductSelected, value);
                for (int i = 0; i < CurrentPlan.StockTakingProductCollection.Count; i++)
                {
                    CurrentPlan.StockTakingProductCollection[i].IsSelected = false;
                }
                if (value is null) return;
                value.IsSelected = true;
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

        private StockTakingProduct stockTakingResultProductSelected;

        public StockTakingProduct StockTakingResultProductSelected
        {
            get { return stockTakingResultProductSelected; }
            set
            {
                Set(() => StockTakingResultProductSelected, ref stockTakingResultProductSelected, value);
            }
        }

        private double resultInitTotalPrice;

        public double ResultInitTotalPrice
        {
            get { return resultInitTotalPrice; }
            set
            {
                Set(() => ResultInitTotalPrice, ref resultInitTotalPrice, value);
                ResultDiffTotalPrice = ResultFinalTotalPrice - ResultInitTotalPrice;
            }
        }

        private double resultDiffTotalPrice;

        public double ResultDiffTotalPrice
        {
            get { return resultDiffTotalPrice; }
            set
            {
                Set(() => ResultDiffTotalPrice, ref resultDiffTotalPrice, value);
            }
        }

        private double resultFinalTotalPrice;

        public double ResultFinalTotalPrice
        {
            get { return resultFinalTotalPrice; }
            set
            {
                Set(() => ResultFinalTotalPrice, ref resultFinalTotalPrice, value);
                ResultDiffTotalPrice = ResultFinalTotalPrice - ResultInitTotalPrice;
            }
        }
        /**7.27 盤點篩選 **/
        private string stockTakingTypeSelectItem = "全部";

        public string StockTakingTypeSelectItem
        {
            get => stockTakingTypeSelectItem;
            set
            {
                Set(() => StockTakingTypeSelectItem, ref stockTakingTypeSelectItem, value);
            }
        }
        private string stockTakingCommonSelectItem = "全部";

        public string StockTakingCommonSelectItem
        {
            get => stockTakingCommonSelectItem;
            set
            {
                Set(() => StockTakingCommonSelectItem, ref stockTakingCommonSelectItem, value);
            }
        }
        private string whereSelectItem = "全部";

        public string WhereSelectItem
        {
            get => whereSelectItem;
            set
            {
                Set(() => WhereSelectItem, ref whereSelectItem, value);
            }
        }


        private List<string> stockTakingTypeString;

        public List<string> StockTakingTypeString
        {
            get => stockTakingTypeString;
            set
            {
                Set(() => StockTakingTypeString, ref stockTakingTypeString, value);
            }
        }
        private List<string> stockTakingCommonString;

        public List<string> StockTakingCommonString
        {
            get => stockTakingCommonString;
            set
            {
                Set(() => StockTakingCommonString, ref stockTakingCommonString, value);
            }
        }
        private List<string> whereString;

        public List<string> WhereString
        {
            get => whereString;
            set
            {
                Set(() => WhereString, ref whereString, value);
            }
        }


        private ObservableCollection<StockTakingPlanProduct> wordsView;

        public ObservableCollection<StockTakingPlanProduct> WordsView
        {
            get => wordsView;
            set
            {
                Set(() => WordsView, ref wordsView, value);
            }
        }
      



        /**7.27 盤點篩選 **/
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
        public RelayCommand ShowStockResultMedicineDetailCommand { get; set; }
        public RelayCommand ShowStockPlanMedicineDetailCommand { get; set; }
        public RelayCommand ShowSourceMedicineDetailCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand GetControlMedicinesCommand { get; set; }
        public RelayCommand GetStockLessProductsCommand { get; set; }
        public RelayCommand GetMonthMedicinesCommand { get; set; }
        public RelayCommand GetOnTheFrameMedicinesCommand { get; set; }
        public RelayCommand ProductSearchCommand { get; set; }
        public RelayCommand WarHouseChangedCommand { get; set; }
        public RelayCommand PlanProductCommand { get; set; }
        public RelayCommand StockTakingTypeChangedCommand { get; set; }
        public RelayCommand StockTakingCommonSChangedCommand { get; set; }
        public RelayCommand WhereChangedCommand { get; set; }

        public StockTakingViewModel()
        {
            RegisterCommand();
            WordsView = new ObservableCollection<StockTakingPlanProduct>();
            WareHouses = VM.WareHouses;
            CurrentPlan.WareHouse = WareHouses[0];
            EmployeeCollection.Init();
            for (int i = 0; i < EmployeeCollection.Count; i++)
            {
                if (!EmployeeCollection[i].IsLocal)
                {
                    EmployeeCollection.Remove(EmployeeCollection[i]);
                    i--;
                }
            }
            EmployeeCollection.Add(ViewModelMainWindow.CurrentUser);

            StockTakingCommonString = new List<string>() { "全部", "常備", "非常備" };
            StockTakingTypeString = new List<string>() { "全部", "藥品", "OTC",  };
            WhereString = new List<string>() { "全部", "一般箋", "慢箋", "自費調劑" };

        }

        private void SetDiffInventoryAmountAction()
        {
            DiffInventoryAmount = StockTakingResult.StockTakingProductCollection.Count(s => s.NewInventory != s.OnTheFrame);
            SetResultFinalTotalPrice();
            if (StockTakingResultProductSelected is null) return;
            StockTakingResultProductSelected.IsUpdate = true;
        }

        private void LastToResultPageAction()
        {
            StockTakingType = StockTakingType.Result;
        }

        private void LastToChoosePageAction()
        {
            StockTakingType = StockTakingType.Choose;
        }

        private void NextToReasonPageAction()
        {
            if (StockTakingResult.StockTakingProductCollection.Count(s => s.NewInventory < 0) > 0)
            {
                MessageWindow.ShowMessage("盤點量不可為負值!", MessageType.ERROR);
                return;
            }
            else if (StockTakingResult.StockTakingProductCollection.Count(s => s.IsUpdate == false) > 0)
            {
                MessageWindow.ShowMessage("有品項尚未盤點", MessageType.ERROR);
                return;
            }

            if (DiffInventoryAmount == 0)
            {
                ConfirmWindow confirmWindow = new ConfirmWindow("未有盤差品項 是否直接盤點?", "盤點確認");
                if ((bool)confirmWindow.DialogResult)
                {
                    CompleteStockTakingAction();
                }
                return;
            }

            StockTakingReason.StockTakingProductCollection.Clear();
            foreach (var s in StockTakingResult.StockTakingProductCollection)
            {
                if (s.ValueDiff != 0)
                    StockTakingReason.StockTakingProductCollection.Add(s);
            }
            CurrentPlan.StockTakingProductCollection.Clear();
            StockTakingType = StockTakingType.Reason;
        }

        private void NextToResultPageAction()
        {
            if (CurrentPlan.StockTakingProductCollection.Count(s => s.IsError == true) > 0)
            {
                MessageWindow.ShowMessage("有橘底商品為異常品項 藥袋量大於總庫存 不可盤點", MessageType.ERROR);
                return;
            }
            StockTakingResult.StockTakingProductCollection = StockTakingProducts.GetStockTakingPlanProducts(CurrentPlan.StockTakingProductCollection, CurrentPlan.WareHouse.ID);
            ResultFinalTotalPrice = StockTakingResult.StockTakingProductCollection.Sum(s => s.NewInventoryTotalPrice);
            StockTakingType = StockTakingType.Result;
        }

        private void CompleteStockTakingAction()
        {
            if (StockTakingReason.StockTakingProductCollection.Count(s => string.IsNullOrEmpty(s.Note.Trim())) > 0)
            {
                MessageWindow.ShowMessage("盤差原因不可為空!", MessageType.WARNING);
                return;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow("確認是否盤點?", "盤點最後確認");
            if (!(bool)confirmWindow.DialogResult) return;

            StockTakingProducts temp = StockTakingProducts.GetStockTakingPlanProducts(CurrentPlan.StockTakingProductCollection, CurrentPlan.WareHouse.ID);
            for (int i = 0; i < StockTakingResult.StockTakingProductCollection.Count; i++)
            {
                if (temp.Count(t => t.InvID == StockTakingResult.StockTakingProductCollection[i].InvID) == 1)
                    StockTakingResult.StockTakingProductCollection[i].MedBagAmount = temp.Single(t => t.InvID == StockTakingResult.StockTakingProductCollection[i].InvID).MedBagAmount;//重新確認藥袋量
                StockTakingResult.StockTakingProductCollection[i].NewInventory += StockTakingResult.StockTakingProductCollection[i].MedBagAmount;  //架上量 + 藥袋量 = 總盤點量
            }
            //更新盤點原因
            foreach (var s in StockTakingReason.StockTakingProductCollection)
            {
                StockTakingResult.StockTakingProductCollection.Single(st => st.ID == s.ID).Note = s.Note;
            }
            StockTakingResult.WareHouse = CurrentPlan.WareHouse;
            StockTakingResult.InsertStockTaking("盤點單盤點");
            MessageWindow.ShowMessage("盤點完成", Class.MessageType.SUCCESS);
            StockTakingType = StockTakingType.Choose;
        }

        private void FillUnTakingInventoryAction()
        {
            for (int i = 0; i < StockTakingResult.StockTakingProductCollection.Count; i++)
            {
                if (!StockTakingResult.StockTakingProductCollection[i].IsUpdate)
                {
                    StockTakingResult.StockTakingProductCollection[i].NewInventory =
                        StockTakingResult.StockTakingProductCollection[i].Inventory < 0 ? 0 : StockTakingResult.StockTakingProductCollection[i].OnTheFrame;
                    StockTakingResult.StockTakingProductCollection[i].GetStockTakingTotalPrice(CurrentPlan.WareHouse.ID);
                    StockTakingResult.StockTakingProductCollection[i].IsUpdate = true;
                }
            }
            DiffInventoryAmount = StockTakingResult.StockTakingProductCollection.Count(s => s.NewInventory != s.OnTheFrame);
            ResultFinalTotalPrice = StockTakingResult.StockTakingProductCollection.Sum(s => s.NewInventoryTotalPrice);
        }

        private void ClearStockTakingProductAction()
        {
            CurrentPlan.StockTakingProductCollection.Clear();
        }

        private void ExportCsvAction()
        {
            StockTakingResult.StockTakingProductCollection = StockTakingProducts.GetStockTakingPlanProducts(CurrentPlan.StockTakingProductCollection, CurrentPlan.WareHouse.ID);

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = CurrentPlan.Name + "盤點單";
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
                        file.WriteLine("商品代碼,藥品名稱,庫存,架上量,盤點量");
                        foreach (var s in StockTakingResult.StockTakingProductCollection)
                        {
                            file.WriteLine($"{s.ID},{s.FullName},{s.Inventory},{s.OnTheFrame},");
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

        private void AddNewProductAction()
        {
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
            ResultInitTotalPrice = CurrentPlan.StockTakingProductCollection.Sum(s => s.TotalPrice);
        }

        private void SetResultFinalTotalPrice()
        {
            if (StockTakingResultProductSelected is null) return;
            StockTakingResultProductSelected.GetStockTakingTotalPrice(CurrentPlan.WareHouse.ID);
            ResultFinalTotalPrice = StockTakingResult.StockTakingProductCollection.Sum(s => s.NewInventoryTotalPrice);
        }

        private void ShowStockPlanMedicineDetailAction()
        {
            if (StockTakingPlanProductSelected is null) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { StockTakingPlanProductSelected.ID, CurrentPlan.WareHouse.ID }, "ShowProductDetail"));
        }

        private void ShowSourceMedicineDetailAction()
        {
            if (SourceStockTakingProductSelectedItem is null) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { SourceStockTakingProductSelectedItem.ID, CurrentPlan.WareHouse.ID }, "ShowProductDetail"));
            TypeChangedAction();
        }

        private void ShowStockResultMedicineDetailAction()
        {
            if (StockTakingResultProductSelected is null) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { StockTakingResultProductSelected.ID, CurrentPlan.WareHouse.ID }, "ShowProductDetail"));
            TypeChangedAction();
        }

        private void DeleteProductAction()
        {
            if (StockTakingPlanProductSelected is null) return;
            CurrentPlan.StockTakingProductCollection.Remove(StockTakingPlanProductSelected);
            ResultInitTotalPrice = CurrentPlan.StockTakingProductCollection.Sum(s => s.TotalPrice);
        }

        private void GetControlMedicinesAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetControlMedincines(CurrentPlan.WareHouse.ID);
            RemoveSourceProInTarget();
            TypeChangedAction();
        }

        private void GetStockLessProductsAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetStockLessProducts(CurrentPlan.WareHouse.ID);
            RemoveSourceProInTarget();
            TypeChangedAction();
        }

        private void GetMonthMedicinesAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetMonthMedicines(CurrentPlan.WareHouse.ID);
            RemoveSourceProInTarget();
            TypeChangedAction();
        }

        private void GetOnTheFrameMedicinesAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetOnTheFrameMedicines(CurrentPlan.WareHouse.ID);
            RemoveSourceProInTarget();
            TypeChangedAction();
        }

        private void GetStockTakingProductByProNameAction()
        {
            if (IsAlphabetCheck)
                SourceStockTakingProducts = SourceStockTakingProducts.GetStockTakingPlanProductByProName(ProductSearchName + "%", CurrentPlan.WareHouse.ID);
            else
                SourceStockTakingProducts = SourceStockTakingProducts.GetStockTakingPlanProductByProName("%" + ProductSearchName + "%", CurrentPlan.WareHouse.ID);
            RemoveSourceProInTarget();
            TypeChangedAction();
        }

        private void RemoveSourceProInTarget()
        {
            SourceallItemsAreChecked = false;
            for (int i = 0; i < SourceStockTakingProducts.Count; i++)
            {
                if (CurrentPlan.StockTakingProductCollection.Count(t => t.ID == SourceStockTakingProducts[i].ID) > 0)
                {
                    WordsView.Remove(SourceStockTakingProducts[i]);
                    SourceStockTakingProducts.Remove(SourceStockTakingProducts[i]);
                    i--;
                }
            }
        }

        private void AddProductAction()
        {
            for (int i = 0; i < SourceStockTakingProducts.Count; i++)
            {
                if (SourceStockTakingProducts[i].IsSelected && CurrentPlan.StockTakingProductCollection.Count(t => t.ID == SourceStockTakingProducts[i].ID) == 0)
                {
                    SourceStockTakingProducts[i].IsSelected = false;
                    if (SourceStockTakingProducts[i].IsError)
                    {
                        MessageWindow.ShowMessage($"{SourceStockTakingProducts[i].FullName} 庫存不足藥袋需求量 架上量為0 不可盤點", MessageType.ERROR);
                        continue;
                    }

                    CurrentPlan.StockTakingProductCollection.Add(SourceStockTakingProducts[i]);
                    WordsView.Remove(SourceStockTakingProducts[i]);
                    SourceStockTakingProducts.Remove(SourceStockTakingProducts[i]);
                    i--;
                }
            }
            ResultInitTotalPrice = CurrentPlan.StockTakingProductCollection.Sum(s => s.TotalPrice);
        }

        private void PlanProductAction()
        {
            CurrentPlan.StockTakingProductCollection.Clear();
            ResultInitTotalPrice = CurrentPlan.StockTakingProductCollection.Sum(s => s.TotalPrice);
        }

        private void WarHouseChangedAction()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "切換庫別中...";
                CurrentPlan.StockTakingProductCollection = CurrentPlan.StockTakingProductCollection.GetProductByWarID(CurrentPlan.WareHouse.ID);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                ResultInitTotalPrice = CurrentPlan.StockTakingProductCollection.Sum(s => s.TotalPrice);
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void RegisterCommand()
        {
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
            ShowStockPlanMedicineDetailCommand = new RelayCommand(ShowStockPlanMedicineDetailAction);
            ShowStockResultMedicineDetailCommand = new RelayCommand(ShowStockResultMedicineDetailAction);
            ShowSourceMedicineDetailCommand = new RelayCommand(ShowSourceMedicineDetailAction);
            DeleteProductCommand = new RelayCommand(DeleteProductAction);
            AddProductCommand = new RelayCommand(AddProductAction);
            GetControlMedicinesCommand = new RelayCommand(GetControlMedicinesAction);
            GetStockLessProductsCommand = new RelayCommand(GetStockLessProductsAction);
            GetOnTheFrameMedicinesCommand = new RelayCommand(GetOnTheFrameMedicinesAction);
            GetMonthMedicinesCommand = new RelayCommand(GetMonthMedicinesAction);
            ProductSearchCommand = new RelayCommand(GetStockTakingProductByProNameAction);
            WarHouseChangedCommand = new RelayCommand(WarHouseChangedAction);
            StockTakingTypeChangedCommand = new RelayCommand(TypeChangedAction);
            PlanProductCommand = new RelayCommand(PlanProductAction);
            StockTakingCommonSChangedCommand = new RelayCommand(TypeChangedAction);
            WhereChangedCommand = new RelayCommand(TypeChangedAction);
        }

        private void TypeChangedAction()
        {
        if (StockTakingCommonSelectItem == "全部" && StockTakingTypeSelectItem == "藥品")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => ( p.Type == 1))) ;

            }
            else if (StockTakingCommonSelectItem == "常備" && StockTakingTypeSelectItem == "全部")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => ( p.IsCommon == true)));
            }
            else if (StockTakingCommonSelectItem == "全部" && StockTakingTypeSelectItem == "OTC")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => ( p.Type == 2)));
            }
            else if (StockTakingCommonSelectItem == "非常備" && StockTakingTypeSelectItem == "全部")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => (p.IsCommon == false && p.Type == 1))); 
            }
            else if (StockTakingCommonSelectItem == "非常備" && StockTakingTypeSelectItem == "OTC")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => (p.IsCommon == false && p.Type == 2)));
            }
            else if (StockTakingCommonSelectItem == "全部" && StockTakingTypeSelectItem == "全部")
            {
                WordsView = (StockTakingPlanProducts)SourceStockTakingProducts;
            }
            else if (StockTakingCommonSelectItem == "非常備" && StockTakingTypeSelectItem == "藥品")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => (p.IsCommon == false && p.Type == 1)));
            }
            else if (StockTakingCommonSelectItem == "常備" && StockTakingTypeSelectItem == "藥品")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => (p.IsCommon == true && p.Type == 1)));
            }
            else if (StockTakingCommonSelectItem == "常備" && StockTakingTypeSelectItem == "OTC")
            {
                WordsView = new ObservableCollection<StockTakingPlanProduct> (SourceStockTakingProducts.Where(p => (p.IsCommon == true && p.Type == 2)));
            }


        }

    }
}