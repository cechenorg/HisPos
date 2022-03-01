using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos;
using His_Pos.NewClass.BalanceSheet;

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

        public RelayCommand DeleteStrikeHistory { get; set; }

        public ClosingHistoryViewModel()
        {
            DeleteStrikeHistory = new RelayCommand(DeleteStrikeHistoryAction);
            ClosingHistories = new ClosingHistories();
            Init();
        }

        private void Init()
        {
            MainWindow.ServerConnection.OpenConnection();
            ClosingHistories.GetData();
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