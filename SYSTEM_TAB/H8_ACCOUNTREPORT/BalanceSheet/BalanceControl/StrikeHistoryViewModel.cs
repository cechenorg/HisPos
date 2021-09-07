using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using System;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class StrikeHistoryViewModel : ViewModelBase
    {
        private DateTime startDate = DateTime.Today;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime endDate = DateTime.Today;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private StrikeHistory selectedHistory;

        public StrikeHistory SelectedHistory
        {
            get => selectedHistory;
            set
            {
                if (selectedHistory != null)
                    selectedHistory.IsSelected = false;

                selectedHistory = value;
                RaisePropertyChanged(nameof(SelectedHistory));

                if (selectedHistory != null)
                    selectedHistory.IsSelected = true;
            }
        }

        private StrikeHistories strikeHistories;

        public StrikeHistories StrikeHistories
        {
            get => strikeHistories;
            set
            {
                strikeHistories = value;
                RaisePropertyChanged(nameof(StrikeHistories));
            }
        }

        public RelayCommand DeleteStrikeHistory { get; set; }
        public RelayCommand SearchStrikeHistory { get; set; }

        public StrikeHistoryViewModel()
        {
            DeleteStrikeHistory = new RelayCommand(DeleteStrikeHistoryAction);
            SearchStrikeHistory = new RelayCommand(SearchStrikeHistoryAction);
            StrikeHistories = new StrikeHistories();
            Init();
        }

        private void Init()
        {
            MainWindow.ServerConnection.OpenConnection();
            StrikeHistories.GetData();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void SearchStrikeHistoryAction()
        {
            StrikeHistories = new StrikeHistories();
            MainWindow.ServerConnection.OpenConnection();
            StrikeHistories.GetSelectData("", StartDate, EndDate);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void DeleteStrikeHistoryAction()
        {
            ConfirmWindow cw = new ConfirmWindow("是否進行刪除", "確認");
            if (!(bool)cw.DialogResult) { return; }

            MainWindow.ServerConnection.OpenConnection();
            CashReportDb.DeleteStrikeHistory(SelectedHistory);
            Init();
            MainWindow.ServerConnection.CloseConnection();
        }
    }
}