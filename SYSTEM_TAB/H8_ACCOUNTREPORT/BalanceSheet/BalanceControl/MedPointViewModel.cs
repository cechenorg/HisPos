using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class MedPointViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand<RelayCommand> StrikeCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeFinalCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private StrikeDatas strikeDatas;
        private StrikeData selectedData;

        public StrikeDatas StrikeDatas
        {
            get { return strikeDatas; }
            set
            {
                strikeDatas = value;
                RaisePropertyChanged(nameof(StrikeDatas));
            }
        }
        public StrikeData SelectedData
        {
            get { return selectedData; }
            set
            {
                selectedData = value;
                RaisePropertyChanged(nameof(SelectedData));
            }
        }
        #endregion

        public MedPointViewModel()
        {
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
            StrikeFinalCommand = new RelayCommand<RelayCommand>(StrikeFinalAction);
        }

        #region ----- Define Actions -----
        private void StrikeAction(RelayCommand command)
        {
            if (!StrikeValueIsValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = CashReportDb.StrikeBalanceSheet(StrikeTypeEnum.Bank, BalanceSheetTypeEnum.Transfer, 0, "");
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("轉帳成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("轉帳失敗", MessageType.ERROR);
            }

            command.Execute(null);
        }
        private void StrikeFinalAction(RelayCommand command)
        {

        }
        #endregion

        #region ----- Define Functions -----
        private bool StrikeValueIsValid()
        {
            return false;
        }
        #endregion
    }
}
