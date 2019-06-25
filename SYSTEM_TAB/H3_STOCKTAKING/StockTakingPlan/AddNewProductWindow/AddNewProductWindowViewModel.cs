using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewProductWindow {
    public class AddNewProductWindowViewModel : ViewModelBase {
        private StockTakingProducts sourceStockTakingProducts = new StockTakingProducts();
        public StockTakingProducts SourceStockTakingProducts
        {
            get { return sourceStockTakingProducts; }
            set
            {
                Set(() => SourceStockTakingProducts, ref sourceStockTakingProducts, value);
            }
        }
        private StockTakingProducts targetStockTakingProducts;
        public StockTakingProducts TargetStockTakingProducts
        {
            get { return targetStockTakingProducts; }
            set
            {
                Set(() => TargetStockTakingProducts, ref targetStockTakingProducts, value);
            }
        }
        private bool allItemsAreChecked;
        public bool AllItemsAreChecked
        {
            get { return allItemsAreChecked; }
            set
            {
                Set(() => AllItemsAreChecked, ref allItemsAreChecked, value);
                for (int i = 0; i < SourceStockTakingProducts.Count; i++) {
                    SourceStockTakingProducts[i].IsSelected = value;
                }
            }
        }
        
        public RelayCommand GetControlMedicinesCommand { get; set; }
        public RelayCommand GetStockLessProductsCommand { get; set; }
         
        public string WarID { get; set; }
        public AddNewProductWindowViewModel(string warID) {
            GetControlMedicinesCommand = new RelayCommand(GetControlMedicinesAction);
            GetStockLessProductsCommand = new RelayCommand(GetStockLessProductsAction);
            WarID = warID;
        }
        #region Function
        private void GetControlMedicinesAction() {
            SourceStockTakingProducts = SourceStockTakingProducts.GetControlMedincines(WarID);
        }
        private void GetStockLessProductsAction() {
            SourceStockTakingProducts = SourceStockTakingProducts.GetControlMedincines(WarID);
        }
        
        #endregion
    }
}
