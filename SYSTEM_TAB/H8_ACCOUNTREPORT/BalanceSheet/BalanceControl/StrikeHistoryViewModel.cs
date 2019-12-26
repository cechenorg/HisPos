using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashFlow;
using His_Pos.NewClass.Report.CashReport;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class StrikeHistoryViewModel : ViewModelBase
    {
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
        private string source;
        public RelayCommand DeleteStrikeHistory { get; set; }
        public StrikeHistoryViewModel()
        {
            
        }

        public StrikeHistoryViewModel(string source)
        {
            DeleteStrikeHistory = new RelayCommand(DeleteStrikeHistoryAction);
            this.source = source;
            Init();
        }

        private void Init()
        {
            MainWindow.ServerConnection.OpenConnection();
            StrikeHistories = new StrikeHistories();
            StrikeHistories.GetData(source);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void DeleteStrikeHistoryAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            CashReportDb.DeleteStrikeHistory(SelectedHistory);
            Init();
            MainWindow.ServerConnection.CloseConnection();
        }
    }
}
