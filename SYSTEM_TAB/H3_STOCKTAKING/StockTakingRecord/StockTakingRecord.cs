using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StockTaking.StockTaking;
using System;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingRecord
{
    public class StockTakingRecord : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        private int changeProductCount;

        public int ChangeProductCount
        {
            get { return changeProductCount; }
            set
            {
                Set(() => ChangeProductCount, ref changeProductCount, value);
            }
        }

        private int totalProductCount;

        public int TotalProductCount
        {
            get { return totalProductCount; }
            set
            {
                Set(() => TotalProductCount, ref totalProductCount, value);
            }
        }

        private StockTakings stockTakingCollection = new StockTakings();

        public StockTakings StockTakingCollection
        {
            get { return stockTakingCollection; }
            set
            {
                Set(() => StockTakingCollection, ref stockTakingCollection, value);
            }
        }

        private NewClass.StockTaking.StockTaking.StockTaking stockTakingSelectedItem;

        public NewClass.StockTaking.StockTaking.StockTaking StockTakingSelectedItem
        {
            get { return stockTakingSelectedItem; }
            set
            {
                Set(() => StockTakingSelectedItem, ref stockTakingSelectedItem, value);
                value?.GetStockTakingProductbyID();
                SetStockProducts();
            }
        }

        private NewClass.StockTaking.StockTakingProduct.StockTakingProducts unChangeProductCollection = new NewClass.StockTaking.StockTakingProduct.StockTakingProducts();

        public NewClass.StockTaking.StockTakingProduct.StockTakingProducts UnChangeProductCollection
        {
            get { return unChangeProductCollection; }
            set
            {
                Set(() => UnChangeProductCollection, ref unChangeProductCollection, value);
            }
        }

        private NewClass.StockTaking.StockTakingProduct.StockTakingProducts changeProductCollection = new NewClass.StockTaking.StockTakingProduct.StockTakingProducts();

        public NewClass.StockTaking.StockTakingProduct.StockTakingProducts ChangeProductCollection
        {
            get { return changeProductCollection; }
            set
            {
                Set(() => ChangeProductCollection, ref changeProductCollection, value);
            }
        }

        private DateTime? startDate = DateTime.Today;

        public DateTime? StartDate
        {
            get { return startDate; }
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime? endDate = DateTime.Today;

        public DateTime? EndDate
        {
            get { return endDate; }
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private string proID;

        public string ProID
        {
            get { return proID; }
            set
            {
                Set(() => ProID, ref proID, value);
            }
        }

        private string proName;

        public string ProName
        {
            get { return proName; }
            set
            {
                Set(() => ProName, ref proName, value);
            }
        }

        public RelayCommand SearchCommand { get; set; }

        public StockTakingRecord()
        {
            SearchCommand = new RelayCommand(SearchAction);
            SearchAction();
        }

        #region Function

        private void SearchAction()
        {
            StockTakingCollection = StockTakingCollection.GetStockTakingByCondition(StartDate, EndDate, ProID, ProName);
            if (StockTakingCollection.Count > 0)
                StockTakingSelectedItem = StockTakingCollection[0];
        }

        private void SetStockProducts()
        {
            UnChangeProductCollection.Clear();
            ChangeProductCollection.Clear();
            if (StockTakingSelectedItem is null) return;
            foreach (var s in StockTakingSelectedItem.StockTakingProductCollection)
            {
                if (s.NewInventory != s.Inventory)
                    ChangeProductCollection.Add(s);
                else
                    UnChangeProductCollection.Add(s);
            }
            ChangeProductCount = ChangeProductCollection.Count;
            TotalProductCount = StockTakingSelectedItem.StockTakingProductCollection.Count;
        }

        #endregion Function
    }
}