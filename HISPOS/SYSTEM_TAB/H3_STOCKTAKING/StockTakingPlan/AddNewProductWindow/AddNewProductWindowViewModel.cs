using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.StockTaking.StockTakingPlanProduct;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewProductWindow
{
    public class AddNewProductWindowViewModel : ViewModelBase
    {
        private string productSearchName;

        public string ProductSearchName
        {
            get { return productSearchName; }
            set
            {
                Set(() => ProductSearchName, ref productSearchName, value);
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

        private StockTakingPlanProducts targetStockTakingProducts = new StockTakingPlanProducts();

        public StockTakingPlanProducts TargetStockTakingProducts
        {
            get { return targetStockTakingProducts; }
            set
            {
                Set(() => TargetStockTakingProducts, ref targetStockTakingProducts, value);
            }
        }

        private bool sourceallItemsAreChecked;

        public bool SourceallItemsAreChecked
        {
            get { return sourceallItemsAreChecked; }
            set
            {
                Set(() => SourceallItemsAreChecked, ref sourceallItemsAreChecked, value);
                for (int i = 0; i < SourceStockTakingProducts.Count; i++)
                {
                    SourceStockTakingProducts[i].IsSelected = value;
                }
            }
        }

        private bool targetallItemsAreChecked;

        public bool TargetallItemsAreChecked
        {
            get { return targetallItemsAreChecked; }
            set
            {
                Set(() => TargetallItemsAreChecked, ref targetallItemsAreChecked, value);
                for (int i = 0; i < TargetStockTakingProducts.Count; i++)
                {
                    TargetStockTakingProducts[i].IsSelected = value;
                }
            }
        }

        public RelayCommand GetControlMedicinesCommand { get; set; }
        public RelayCommand GetStockLessProductsCommand { get; set; }
        public RelayCommand GetMonthMedicinesCommand { get; set; }
        public RelayCommand GetOnTheFrameMedicinesCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand ProductSubmitCommand { get; set; }
        public RelayCommand ProductSearchCommand { get; set; }

        public string WarID { get; set; }

        public AddNewProductWindowViewModel(string warID, StockTakingPlanProducts takingPlanProducts)
        {
            GetControlMedicinesCommand = new RelayCommand(GetControlMedicinesAction);
            GetStockLessProductsCommand = new RelayCommand(GetStockLessProductsAction);
            GetOnTheFrameMedicinesCommand = new RelayCommand(GetOnTheFrameMedicinesAction);
            GetMonthMedicinesCommand = new RelayCommand(GetMonthMedicinesAction);
            AddProductCommand = new RelayCommand(AddProductAction);
            DeleteProductCommand = new RelayCommand(DeleteProductAction);
            ProductSubmitCommand = new RelayCommand(ProductSubmitAction);
            ProductSearchCommand = new RelayCommand(GetStockTakingProductByProNameAction);
            WarID = warID;
            TargetStockTakingProducts.Clear();
            foreach (var t in takingPlanProducts)
            {
                TargetStockTakingProducts.Add(t);
            }
        }

        #region Function

        private void ProductSubmitAction()
        {
            Messenger.Default.Send(new NotificationMessage<StockTakingPlanProducts>(this, TargetStockTakingProducts, "GetProductSubmit"));
        }

        private void AddProductAction()
        {
            for (int i = 0; i < SourceStockTakingProducts.Count; i++)
            {
                if (SourceStockTakingProducts[i].IsSelected && TargetStockTakingProducts.Count(t => t.ID == SourceStockTakingProducts[i].ID) == 0)
                {
                    TargetStockTakingProducts.Add(SourceStockTakingProducts[i]);
                    SourceStockTakingProducts.Remove(SourceStockTakingProducts[i]);
                    i--;
                }
            }
        }

        private void DeleteProductAction()
        {
            for (int i = 0; i < TargetStockTakingProducts.Count; i++)
            {
                if (TargetStockTakingProducts[i].IsSelected)
                {
                    SourceStockTakingProducts.Add(TargetStockTakingProducts[i]);
                    TargetStockTakingProducts.Remove(TargetStockTakingProducts[i]);
                    i--;
                }
            }
        }

        private void GetControlMedicinesAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetControlMedincines(WarID);
            RemoveSourceProInTarget();
        }

        private void GetStockLessProductsAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetStockLessProducts(WarID);
            RemoveSourceProInTarget();
        }

        private void GetMonthMedicinesAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetMonthMedicines(WarID);
            RemoveSourceProInTarget();
        }

        private void GetOnTheFrameMedicinesAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetOnTheFrameMedicines(WarID);
            RemoveSourceProInTarget();
        }

        private void GetStockTakingProductByProNameAction()
        {
            SourceStockTakingProducts = SourceStockTakingProducts.GetStockTakingPlanProductByProName(ProductSearchName, WarID);
            RemoveSourceProInTarget();
        }

        private void RemoveSourceProInTarget()
        {
            SourceallItemsAreChecked = false;
            for (int i = 0; i < SourceStockTakingProducts.Count; i++)
            {
                if (TargetStockTakingProducts.Count(t => t.ID == SourceStockTakingProducts[i].ID) > 0)
                {
                    SourceStockTakingProducts.Remove(SourceStockTakingProducts[i]);
                    i--;
                }
            }
        }

        #endregion Function
    }
}