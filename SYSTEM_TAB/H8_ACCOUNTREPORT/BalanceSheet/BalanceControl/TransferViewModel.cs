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
    public class TransferViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand<RelayCommand> StrikeCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string transferValue;
        private string target;

        public double MaxValue { get; set; } = 0;
        public string Target
        {
            get { return target; }
            set
            {
                target = value;
                RaisePropertyChanged(nameof(Target));
            }
        }
        public string TransferValue
        {
            get { return transferValue; }
            set
            {
                transferValue = value;
                RaisePropertyChanged(nameof(TransferValue));
            }
        }
        #endregion

        public TransferViewModel()
        {
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
        }

        #region ----- Define Actions -----
        private void StrikeAction(RelayCommand command)
        {
            if(!TransferValueIsValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = CashReportDb.StrikeBalanceSheet(Target.Equals("銀行")? StrikeTypeEnum.Bank : StrikeTypeEnum.Cash, BalanceSheetTypeEnum.Transfer, Double.Parse(transferValue), "");
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("轉帳成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("轉帳失敗", MessageType.ERROR);
            }

            TransferValue = "";
            command.Execute(null);
        }

        #endregion

        #region ----- Define Functions -----
        private bool TransferValueIsValid()
        {
            double temp;
            if (double.TryParse(TransferValue, out temp))
            {
                if (temp <= 0)
                {
                    MessageWindow.ShowMessage("轉帳金額不可小於等於0", MessageType.ERROR);
                    return false;
                }

                if (temp > MaxValue)
                {
                    MessageWindow.ShowMessage("轉帳金額超過餘額", MessageType.ERROR);
                    return false;
                }
            }
            else
            {
                MessageWindow.ShowMessage("輸入金額非數字", MessageType.ERROR);
                return false;
            }
            
            return true;
        }
        #endregion
    }
}
