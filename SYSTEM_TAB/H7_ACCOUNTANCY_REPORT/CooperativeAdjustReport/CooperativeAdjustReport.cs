using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product.Medicine.CooperativeAdjustMedicine;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeAdjustReport
{
    public class CooperativeAdjustReport: TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        #region Command
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand CurrentMonthSearchCommand { get; set; }
        #endregion
       
        private CooperativeAdjustMedicines cooperativeAdjustMedCollection = new CooperativeAdjustMedicines();
        public CooperativeAdjustMedicines CooperativeAdjustMedCollection
        {
            get { return cooperativeAdjustMedCollection; }
            set
            {
                Set(() => CooperativeAdjustMedCollection, ref cooperativeAdjustMedCollection, value); 
            }
        }

        private DateTime sDateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime SDateTime
        {
            get { return sDateTime; }
            set
            {
                Set(() => SDateTime, ref sDateTime, value);
            }
        }
        private DateTime eDateTime = DateTime.Now;
        public DateTime EDateTime
        {
            get { return eDateTime; }
            set
            {
                Set(() => EDateTime, ref eDateTime, value);
            }
        }
        public CooperativeAdjustReport() {
            SearchCommand = new RelayCommand(SearchAction);
            CurrentMonthSearchCommand = new RelayCommand(CurrentMonthSearchAction);
        }
        #region Action
        public void SearchAction() {
            CooperativeAdjustMedCollection.GetDataByDate(SDateTime,EDateTime);
        }
        public void CurrentMonthSearchAction() {

        }
        #endregion
    }
}
