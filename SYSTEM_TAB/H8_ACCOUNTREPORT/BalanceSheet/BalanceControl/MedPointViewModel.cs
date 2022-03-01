using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using System;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class MedPointViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand<RelayCommand> StrikeCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeFinalCommand { get; set; }

        #endregion ----- Define Commands -----

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

        #endregion ----- Define Variables -----

        public MedPointViewModel()
        {
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
            StrikeFinalCommand = new RelayCommand<RelayCommand>(StrikeFinalAction);
        }

        #region ----- Define Actions -----

        private void StrikeAction(RelayCommand command)
        {
            if (!StrikeValueIsValid()) return;
            if (SelectedData.SelectedType.ID == null) { MessageWindow.ShowMessage("請選擇正確的沖帳對象", MessageType.ERROR); return; }
            ConfirmWindow cw = new ConfirmWindow("是否進行沖帳?", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = CashReportDb.StrikeBalanceSheet(SelectedData.SelectedType.ID, BalanceSheetTypeEnum.MedPoint, Double.Parse(SelectedData.StrikeValue), SelectedData.Name);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("沖帳失敗", MessageType.ERROR);
            }

            command.Execute(null);
        }

        private void StrikeFinalAction(RelayCommand command)
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認結案", "", false);

            if (!(bool)confirmWindow.DialogResult) return;

            DateTime dateTime = new DateTime(Convert.ToInt32(SelectedData.Name.Substring(0, 4)), Convert.ToInt32(SelectedData.Name.Substring(4)), 01);
            dateTime = dateTime.AddMonths(1).AddDays(-1);

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = CashReportDb.SetDeclareDoneMonth(dateTime);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("結案成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("結案失敗", MessageType.ERROR);
            }

            command.Execute(null);
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private bool StrikeValueIsValid()
        {
            double temp;
            if (double.TryParse(SelectedData.StrikeValue, out temp))
            {
                if (temp <= 0)
                {
                    MessageWindow.ShowMessage("不可小於等於0!", MessageType.ERROR);
                    return false;
                }

                if (temp > SelectedData.Value)
                {
                    MessageWindow.ShowMessage("不可大於原金額!", MessageType.ERROR);
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

        #endregion ----- Define Functions -----
    }
}