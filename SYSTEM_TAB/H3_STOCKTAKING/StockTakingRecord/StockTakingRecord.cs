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

        private StockTakings stockTakingCollection = new StockTakings();
        public StockTakings StockTakingCollection
        {
            get { return stockTakingCollection; }
            set
            {
                Set(() => StockTakingCollection, ref stockTakingCollection, value); 
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
        public StockTakingRecord() {
            SearchCommand = new RelayCommand(SearchAction);
        }
        #region Function
        private void SearchAction() {
            StockTakingCollection = StockTakingCollection.GetStockTakingByCondition(StartDate, EndDate,ProID,ProName);
        }
        #endregion
    }
}
