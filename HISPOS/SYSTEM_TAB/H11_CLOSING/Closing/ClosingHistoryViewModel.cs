using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos;
using His_Pos.NewClass.BalanceSheet;
using System;

namespace ClosingHistoryViewModelHis_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    public class ClosingHistoryViewModel : ViewModelBase
    {
        private ClosingHistory selectedHistory;

        public ClosingHistory SelectedHistory
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

        private ClosingHistories closingHistories;

        public ClosingHistories ClosingHistories
        {
            get => closingHistories;
            set
            {
                closingHistories = value;
                RaisePropertyChanged(nameof(ClosingHistories));
            }
        }

        private DateTime beginDate = DateTime.Today.AddMonths(-3);

        public DateTime BeginDate
        {
            get => beginDate;
            set
            {
                Set(() => BeginDate, ref beginDate, value);
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
        public RelayCommand DeleteStrikeHistory { get; set; }
        public RelayCommand SearchCommand { get; set; }

        public ClosingHistoryViewModel()
        {
            DeleteStrikeHistory = new RelayCommand(DeleteStrikeHistoryAction);
            SearchCommand = new RelayCommand(Init);
            ClosingHistories = new ClosingHistories();
            Init();
        }

        private void Init()
        {
            MainWindow.ServerConnection.OpenConnection();
            ClosingHistories.GetData(BeginDate, EndDate);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void DeleteStrikeHistoryAction()
        {
            /*ConfirmWindow cw = new ConfirmWindow("是否進行刪除", "確認");
            if (!(bool)cw.DialogResult) { return; }

            MainWindow.ServerConnection.OpenConnection();
            CashReportDb.DeleteStrikeHistory(SelectedHistory);
            Init();
            MainWindow.ServerConnection.CloseConnection();*/
        }
    }
}