using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewProductWindow {
    public class AddNewProductWindowViewModel : ViewModelBase {
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
                for (int i = 0; i < SourceStockTakingProducts.Count; i++) {
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
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand ProductSubmitCommand { get; set; }
        public RelayCommand ProductSearchCommand { get; set; }


        public string WarID { get; set; }
        public AddNewProductWindowViewModel(string warID) {
            GetControlMedicinesCommand = new RelayCommand(GetControlMedicinesAction);
            GetStockLessProductsCommand = new RelayCommand(GetStockLessProductsAction);
            AddProductCommand = new RelayCommand(AddProductAction);
            DeleteProductCommand = new RelayCommand(DeleteProductAction);
            ProductSubmitCommand = new RelayCommand(ProductSubmitAction);
            ProductSearchCommand = new RelayCommand(GetStockTakingProductByProNameAction);
            WarID = warID;
        }
        #region Function
        private void ProductSubmitAction() {
            Messenger.Default.Send(new NotificationMessage<StockTakingPlanProducts>(this, TargetStockTakingProducts, "GetProductSubmit"));
            TargetStockTakingProducts.Clear();
        }
        private void AddProductAction()
        {
            foreach (var s in SourceStockTakingProducts) {
                if (s.IsSelected && TargetStockTakingProducts.Count(t => t.ID == s.ID) == 0 )
                    TargetStockTakingProducts.Add(s);
            }

        }
        private void DeleteProductAction() {
            for (int i = 0; i < TargetStockTakingProducts.Count; i++) {
                if (TargetStockTakingProducts[i].IsSelected) {
                    TargetStockTakingProducts.Remove(TargetStockTakingProducts[i]);
                    i--;
                }  
            } 
        }
        private void GetControlMedicinesAction() {
            SourceStockTakingProducts = SourceStockTakingProducts.GetControlMedincines(WarID);
        }
        private void GetStockLessProductsAction() {
            SourceStockTakingProducts = SourceStockTakingProducts.GetStockLessProducts(WarID);
        }
        private void GetStockTakingProductByProNameAction() {
            SourceStockTakingProducts = SourceStockTakingProducts.GetStockTakingPlanProductByProName(ProductSearchName);
        }
        #endregion
    }
}
