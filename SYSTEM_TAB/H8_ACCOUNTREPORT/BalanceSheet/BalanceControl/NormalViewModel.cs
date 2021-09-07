using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class NormalViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand InsertCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeCommand { get; set; }

        public RelayCommand DetailChangeCommand { get; set; }
        public RelayCommand<RelayCommand> StrikeFinalCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private string transferValue;
        private int strikeValue;
        private string target;
        public string IDClone;

        public string SelectClone;
        public string SelectDetailClone;
        public bool IsFirst = true;
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

        public int StrikeValue
        {
            get { return strikeValue; }
            set
            {
                strikeValue = value;
                RaisePropertyChanged(nameof(StrikeValue));
            }
        }

        private AccountsReport accData;

        public AccountsReport AccData
        {
            get => accData;
            set
            {
                Set(() => AccData, ref accData, value);
            }
        }

        private AccountsDetailReport accDataDetail;

        public AccountsDetailReport AccDataDetail
        {
            get => accDataDetail;
            set
            {
                Set(() => AccDataDetail, ref accDataDetail, value);
            }
        }

        private AccountsDetailReports selectedDetail;

        public AccountsDetailReports SelectedDetail
        {
            get => selectedDetail;
            set
            {
                Set(() => SelectedDetail, ref selectedDetail, value);
            }
        }

        private string selectedDetailCopy;

        public string SelectedDetailCopy
        {
            get => selectedDetailCopy;
            set
            {
                Set(() => SelectedDetailCopy, ref selectedDetailCopy, value);
            }
        }

        private AccountsReports selected;

        public AccountsReports Selected
        {
            get => selected;
            set
            {
                Set(() => Selected, ref selected, value);
            }
        }

        private string selectedCopy;

        public string SelectedCopy
        {
            get => selectedCopy;
            set
            {
                Set(() => SelectedCopy, ref selectedCopy, value);
            }
        }

        private List<AccountsReports> selectedType;

        public List<AccountsReports> SelectedType
        {
            get => selectedType;
            set
            {
                Set(() => SelectedType, ref selectedType, value);
            }
        }

        private AccountsReports selectedBank;

        public AccountsReports SelectedBank
        {
            get => selectedBank;
            set
            {
                Set(() => SelectedBank, ref selectedBank, value);
            }
        }

        private int selectedindex;

        public int Selectedindex
        {
            get => selectedindex;
            set
            {
                Set(() => Selectedindex, ref selectedindex, value);
            }
        }

        private int selectedDetailindex;

        public int SelectedDetailindex
        {
            get => selectedDetailindex;
            set
            {
                Set(() => SelectedDetailindex, ref selectedDetailindex, value);
            }
        }

        #endregion ----- Define Variables -----

        public NormalViewModel(string ID)
        {
            AccDataDetail = new AccountsDetailReport();
            SelectedType = new List<AccountsReports>();
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            SelectedType = new List<AccountsReports>();
            if (ID != "001001")
            {
                SelectedType.Add(new AccountsReports("現金", 0, "001001"));
            }
            foreach (DataRow c in result.Rows)
            {
                SelectedType.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
            }
            AccData = new AccountsReport();
            IDClone = ID;
            Init();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);

            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
            DetailChangeCommand = new RelayCommand(DetailChangeAction);
            StrikeFinalCommand = new RelayCommand<RelayCommand>(StrikeFinalAction);
        }

        private void StrikeFinalAction(RelayCommand command)
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("請選擇科目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail == null)
            {
                MessageWindow.ShowMessage("請選擇細目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail.Name == "20000101" || SelectedDetail.ID == "0")
            {
                MessageWindow.ShowMessage("不得結案該項目", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行結案?", "確認");
            if (!(bool)cw.DialogResult) { return; }


            if (SelectedDetail.StrikeValue != 0)
            {
                PREStrikeAction();
            }


            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Emp", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("Detail", SelectedDetail.ID));
            parameters.Add(new SqlParameter("ID", Selected.ID));
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeclareClosed]", parameters);
            MessageWindow.ShowMessage("結案成功", MessageType.SUCCESS);
            MainWindow.ServerConnection.CloseConnection();
            StrikeValue = 0;
            DetailChangeAction();
            Init();
            command.Execute(null);
        }

        private void DetailChangeAction()
        {
            if (Selected == null) { return; }

            AccDataDetail = new AccountsDetailReport();
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", Selected.ID));
            DataTable Data = new DataTable();
            Data = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetailDetailReport]", parameters);
            int index = 0;
            int nowindex = 0;
            foreach (DataRow r in Data.Rows)
            {
                if (r["ID"].ToString() == SelectDetailClone)
                {
                    nowindex = index;
                }
                AccDataDetail.Add(new AccountsDetailReports(r));
                index++;
            }
            //SelectedDetailCopy = SelectDetailClone;
            MainWindow.ServerConnection.CloseConnection();
            SelectedDetailindex = nowindex;
        }

        public NormalViewModel()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            SelectedType = new List<AccountsReports>();
            SelectedType.Add(new AccountsReports("現金", 0, "001001"));
            foreach (DataRow c in result.Rows)
            {
                SelectedType.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
            }
            AccData = new AccountsReport();
            InsertCommand = new RelayCommand(InsertAction);
            DeleteCommand = new RelayCommand(DeleteAction);
        }

        public void Init()
        {
            AccData = new AccountsReport();
            Selectedindex = -1;
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", IDClone));
            DataTable Data = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetail]", parameters);
            int index = 0;
            int nowindex = 0;
            if (Data.Rows.Count > 0)
            {
                foreach (DataRow r in Data.Rows)
                {
                    AccData.Add(new AccountsReports(r));

                    if (r["ID"].ToString() == SelectClone)
                    {
                        nowindex = index;
                    }
                    index++;
                }
                Selectedindex = nowindex;
                if (IsFirst == true)
                {
                    IsFirst = false;
                    Selected = AccData[0];
                    DetailChangeAction();
                }
            }
            MainWindow.ServerConnection.CloseConnection();
        }

        public void DeleteAction()
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("錯誤", MessageType.ERROR);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", Selected.ID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[AccountsDetailDelete]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("刪除成功", MessageType.SUCCESS);
            Selected = null;
            Init();
        }

        public void InsertAction()
        {
            if (TransferValue == "" || TransferValue == null)
            {
                MessageWindow.ShowMessage("請輸入名稱", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行新增科目?", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", IDClone));
            parameters.Add(new SqlParameter("NAME", TransferValue));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccounts]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("新增成功", MessageType.SUCCESS);
            TransferValue = "";
            Init();
        }

        public void StrikeAction(RelayCommand command)
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("請選擇科目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail.StrikeValue == 0 || SelectedDetail == null)
            {
                MessageWindow.ShowMessage("不得為零", MessageType.ERROR);
                return;
            }
            if (SelectedBank == null)
            {
                MessageWindow.ShowMessage("請選擇沖帳目標", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行科目沖帳?", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", SelectedDetail.StrikeValue));
            parameters.Add(new SqlParameter("TYPE", SelectedDetail.ID));
            parameters.Add(new SqlParameter("NOTE", Selected.Name));
            parameters.Add(new SqlParameter("TARGET", SelectedBank.ID));
            parameters.Add(new SqlParameter("SOURCE_ID", Selected.ID));
            MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheetByAccount]", parameters);
            MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            MainWindow.ServerConnection.CloseConnection();
            if (SelectedDetail.StrikeValue == SelectedDetail.Value)
            {
            }
            else
            {
                SelectDetailClone = SelectedDetail.ID;
                SelectClone = Selected.ID;
            }
            StrikeValue = 0;
            DetailChangeAction();
            Init();

            command.Execute(null);
        }

        public void PREStrikeAction()
        {
            if (Selected == null)
            {
                MessageWindow.ShowMessage("請選擇科目", MessageType.ERROR);
                return;
            }
            if (SelectedDetail.StrikeValue == 0 || SelectedDetail == null)
            {
                MessageWindow.ShowMessage("不得為零", MessageType.ERROR);
                return;
            }
            if (SelectedBank == null)
            {
                MessageWindow.ShowMessage("請選擇沖帳目標", MessageType.ERROR);
                return;
            }
            ConfirmWindow cw = new ConfirmWindow("是否進行科目沖帳?", "確認");
            if (!(bool)cw.DialogResult) { return; }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", SelectedDetail.StrikeValue));
            parameters.Add(new SqlParameter("TYPE", SelectedDetail.ID));
            parameters.Add(new SqlParameter("NOTE", Selected.Name));
            parameters.Add(new SqlParameter("TARGET", SelectedBank.ID));
            parameters.Add(new SqlParameter("SOURCE_ID", Selected.ID));
            MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheetByAccount]", parameters);
            MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            MainWindow.ServerConnection.CloseConnection();
            if (SelectedDetail.StrikeValue == SelectedDetail.Value)
            {
            }
            else
            {
                SelectDetailClone = SelectedDetail.ID;
                SelectClone = Selected.ID;
            }
            StrikeValue = 0;
            DetailChangeAction();
            Init();

        }
    }
}