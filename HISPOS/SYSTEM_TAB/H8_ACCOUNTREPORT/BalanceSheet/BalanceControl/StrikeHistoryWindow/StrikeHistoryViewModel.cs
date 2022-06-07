using ClosedXML.Excel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class StrikeHistoryViewModel : ViewModelBase
    {
        private static DateTime now = DateTime.Now;
        private DateTime startDate = new DateTime(now.Year, now.Month, 1);

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime endDate = now;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private string type = "";

        public string Type
        {
            get => type;
            set
            {
                Set(() => Type, ref type, value);
            }
        }

        private string sujectString = "";

        public string SujectString
        {
            get => sujectString;
            set
            {
                Set(() => SujectString, ref sujectString, value);
            }
        }

        private string accountString = "";

        public string AccountString
        {
            get => accountString;
            set
            {
                Set(() => AccountString, ref accountString, value);
            }
        }

        private string emp = "";

        public string Emp
        {
            get => emp;
            set
            {
                Set(() => Emp, ref emp, value);
            }
        }

        private string keywords;

        public string KeyWords
        {
            get => keywords;
            set
            {
                Set(() => KeyWords, ref keywords, value);
            }
        }

        private DataTable typeTable;

        public DataTable TypeTable
        {
            get => typeTable;
            set
            {
                Set(() => TypeTable, ref typeTable, value);
            }
        }

        private DataTable sujectTable;

        public DataTable SujectTable
        {
            get => sujectTable;
            set
            {
                Set(() => SujectTable, ref sujectTable, value);
            }
        }

        private DataTable accountTable;

        public DataTable AccountTable
        {
            get => accountTable;
            set
            {
                Set(() => AccountTable, ref accountTable, value);
            }
        }

        private DataTable empTable;

        public DataTable EmpTable
        {
            get => empTable;
            set
            {
                Set(() => EmpTable, ref empTable, value);
            }
        }

        private DataTable resultTable;

        public DataTable ResultTable
        {
            get => resultTable;
            set
            {
                Set(() => ResultTable, ref resultTable, value);
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
        public RelayCommand PrintHistory { get; set; }
        public StrikeHistoryViewModel()
        {
            DeleteStrikeHistory = new RelayCommand(DeleteStrikeHistoryAction);
            SearchStrikeHistory = new RelayCommand(SearchStrikeHistoryAction);
            PrintHistory = new RelayCommand(PrintAction);
            StrikeHistories = new StrikeHistories();
            GetData();
        }
         
        private void SearchStrikeHistoryAction()
        {
            GetData();
        }

        private void DeleteStrikeHistoryAction()
        {
            ConfirmWindow cw = new ConfirmWindow("是否進行刪除", "確認");
            if (!(bool)cw.DialogResult) { return; }

            MainWindow.ServerConnection.OpenConnection();
            CashReportDb.DeleteStrikeHistory(SelectedHistory);
            GetData();
            MainWindow.ServerConnection.CloseConnection();
        }


        private void PrintAction()
        {
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "沖帳明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = "沖帳明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("沖帳明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                var col1 = ws.Column("A");
                col1.Width = 30;
                var col2 = ws.Column("B");
                col2.Width = 30;
                var col3 = ws.Column("C");
                col3.Width = 30;
                var col4 = ws.Column("D");
                col4.Width = 30;
                var col5 = ws.Column("E");
                col5.Width = 30;
                var col6 = ws.Column("F");
                col6.Width = 30;
                var col7 = ws.Column("G");
                col7.Width = 30;
                var col8 = ws.Column("H");
                col8.Width = 30;
                var col9 = ws.Column("I");
                col9.Width = 30;
                var col10 = ws.Column("J");
                col10.Width = 30;
                var col11 = ws.Column("K");
                col11.Width = 30;
                ws.Cell(1, 1).Value = "沖帳明細";
                ws.Range(1, 1, 1, 9).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "帳戶";
                ws.Cell("B2").Value = "流水號";
                ws.Cell("C2").Value = "科目名稱";
                ws.Cell("D2").Value = "金額";
                ws.Cell("E2").Value = "科目代號";
                ws.Cell("F2").Value = "來源";
                ws.Cell("G2").Value = "時間";
                ws.Cell("H2").Value = "備註";
                ws.Cell("I2").Value = "啟用";
                ws.Cell("J2").Value = "類別";
                ws.Cell("K2").Value = "人員";
                var rangeWithData = ws.Cell(3, 1).InsertData(ResultTable.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                //wb.SaveAs(fdlg.FileName);
                try
                {
                    wb.SaveAs(fdlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = (fdlg.FileName);
                myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.StartInfo.Verb = "print";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void GetData()
        {
            StrikeHistories = new StrikeHistories();
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("type", Type));
            parameters.Add(new SqlParameter("sdate", StartDate));
            parameters.Add(new SqlParameter("edate", EndDate));
            parameters.Add(new SqlParameter("SujectString", SujectString));
            parameters.Add(new SqlParameter("AccountString", AccountString));
            parameters.Add(new SqlParameter("Category", DBNull.Value));
            parameters.Add(new SqlParameter("Emp", Emp));
            parameters.Add(new SqlParameter("KeyWord", KeyWords));
            DataSet result = MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[StrikeHistoriesByCondition]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Tables.Count == 5)
            {
                StrikeHistories.GetSelectData(result.Tables[0]);

                DataRow dr1 = result.Tables[1].NewRow();
                dr1[0] = "";
                result.Tables[1].Rows.InsertAt(dr1, 0);
                TypeTable = result.Tables[1];

                DataRow dr2 = result.Tables[2].NewRow();
                dr2[0] = "";
                result.Tables[2].Rows.InsertAt(dr2, 0);
                SujectTable = result.Tables[2];

                DataRow dr3 = result.Tables[3].NewRow();
                dr3[0] = "";
                result.Tables[3].Rows.InsertAt(dr3, 0);
                AccountTable = result.Tables[3];

                DataRow dr4 = result.Tables[4].NewRow();
                dr4[0] = "";
                result.Tables[4].Rows.InsertAt(dr4, 0);
                EmpTable = result.Tables[4];

                ResultTable = result.Tables[0];
            }
        }
    }
}