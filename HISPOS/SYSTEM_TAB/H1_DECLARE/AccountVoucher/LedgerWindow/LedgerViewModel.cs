using ClosedXML.Excel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Accounts;
using His_Pos.NewClass.Report.Accounts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.LedgerWindow
{
    public class LedgerViewModel : ViewModelBase
    {
        public LedgerViewModel()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param">0:日記帳 1:明細帳</param>
        public LedgerViewModel(string param)
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            ExportCommand = new RelayCommand(ExportAction);
            LedgerType = param.Equals("0") ? 0 : 1;
            GetData(param);
        }
        /// <summary>
        /// 0:日記帳 1:明細帳
        /// </summary>
        public int LedgerType
        {
            get => ledgerType;
            set
            {
                Set(() => LedgerType, ref ledgerType, value);
            }
        }
        private int ledgerType;
        public string Title
        {
            get => title;
            set
            {
                Set(() => Title, ref title, value);
            }
        }
        private string title = "日記帳";
        public Visibility IsDayLedger//日記帳顯示
        {
            get => isDayLedger;
            set
            {
                Set(() => IsDayLedger, ref isDayLedger, value);
            }
        }
        private Visibility isDayLedger = Visibility.Collapsed;
        public Visibility IsDetailLedger//明細帳顯示
        {
            get => isDetailLedger;
            set
            {
                Set(() => IsDetailLedger, ref isDetailLedger, value);
            }
        }
        private Visibility isDetailLedger = Visibility.Collapsed;
        public DateTime BeginDate
        {
            get => beginDate;
            set
            {
                Set(() => BeginDate, ref beginDate, value);
            }
        }
        private DateTime beginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private DateTime endDate = DateTime.Today;
        public IEnumerable<JournalAccount> Accounts
        {
            get => accounts;
            set
            {
                Set(() => Accounts, ref accounts, value);
            }
        }
        private IEnumerable<JournalAccount> accounts;
        public JournalAccount Account
        {
            get => account;
            set
            {
                Set(() => Account, ref account, value);
            }
        }
        private JournalAccount account;
        public string KeyWord
        {
            get => keyWord;
            set
            {
                Set(() => KeyWord, ref keyWord, value);
            }
        }
        private string keyWord;
        public IEnumerable<LedgerDetail> Details
        {
            get => details;
            set
            {
                Set(() => Details, ref details, value);
            }
        }
        private IEnumerable<LedgerDetail> details;
        public LedgerDetail Detail
        {
            get => detail;
            set
            {
                Set(() => Detail, ref detail, value);
            }
        }
        private LedgerDetail detail;
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public void SubmitAction()
        {
            string acct1 = string.Empty;
            string acct2 = string.Empty; 
            string acct3 = string.Empty;
            if (LedgerType == 1)
            {
                if (Account is null || string.IsNullOrEmpty(Account.AcctFullName))
                {
                    MessageWindow.ShowMessage("明細帳需選擇會計科目!", MessageType.ERROR);
                    return;
                }
            }
            if (Account != null)
            {
                acct1 = Convert.ToString(Account.acctLevel1);
                acct2 = Convert.ToString(Account.acctLevel2);
                acct3 = Convert.ToString(Account.acctLevel3);
            }
            if (Account == null && LedgerType == 1)
            {
                return;
            }
            Details = AccountsDb.GetLedgerData(LedgerType, BeginDate, EndDate, acct1, acct2, acct3, KeyWord);
            if (LedgerType == 1)
            {
                int first = AccountsDb.GetLedgerFirst(BeginDate, acct1, acct2, acct3);
                List<string> posNum = new List<string>() { "1", "5", "6", "8" };
                if (posNum.Contains(acct1))
                {
                    foreach (LedgerDetail item in Details)
                    {
                        item.Balance = first + item.DAmount - item.CAmount;
                        first = item.Balance;
                    }
                }
                else
                {
                    foreach (LedgerDetail item in Details)
                    {
                        item.Balance = first - item.DAmount + item.CAmount;
                        first = item.Balance;
                    }
                }
            }
        }

        public void ExportAction()
        {
            //SubmitAction();
            if (Details is null || Details.Count() == 0)
                return;

            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = Title;
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = string.Format("{0}_{1}_{2}", Title, BeginDate.ToString("yyyyMMdd"), EndDate.ToString("yyyyMMdd"));
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add(Title);
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                List<string> header_ch_name = new List<string>();
                List<string> header_en_name = new List<string>();
                if (LedgerType == 0)
                {
                    header_ch_name = new List<string>() { "藥局名稱", "傳票日期", "單據來源", "傳票號碼", "借方/貸方", "項次", "會計科目代號1", "會計科目代號2", "會計科目代號3", "會計科目完整名稱", "金額", "摘要", "來源單號", "備註", "登錄時間", "登錄人", "最後修改時間", "最後修改人" };
                    header_en_name = new List<string>() { "CurPha_Name", "JouMas_Date", "JouMas_Source", "JouMas_ID", "JouDet_Type", "JouDet_Number", "JouDet_AcctLvl1", "JouDet_AcctLvl2", "JouDet_AcctLvl3", "JouDet_AcctName", "JouDet_Amount", "JouDet_Memo", "JouDet_SourceID", "JouMas_Memo", "JouMas_InsertTime", "InsertEmpName", "JouMas_ModifyTime", "ModifyEmpName" };
                }
                else
                {
                    header_ch_name = new List<string>() { "藥局名稱", "傳票日期", "單據來源", "傳票號碼", "會計科目代號1", "會計科目代號2", "會計科目代號3", "會計科目完整名稱", "借方金額", "貸方金額", "餘額", "摘要", "來源單號", "備註", "登錄時間", "登錄人", "最後修改時間", "最後修改人" };
                    header_en_name = new List<string>() { "CurPha_Name", "JouMas_Date", "JouMas_Source", "JouMas_ID", "JouDet_AcctLvl1", "JouDet_AcctLvl2", "JouDet_AcctLvl3", "JouDet_AcctName", "DAmount", "CAmount", "Balance", "JouDet_Memo", "JouDet_SourceID", "JouMas_Memo", "JouMas_InsertTime", "InsertEmpName", "JouMas_ModifyTime", "ModifyEmpName" };
                }

                DataTable table = new DataTable();
                int count = 65 + header_ch_name.Count();
                Type type = new LedgerDetail().GetType();
                PropertyInfo[] properties = type.GetProperties();

                for (int i = 65; i < count; i++)
                {
                    string alpha = ((char)i).ToString();
                    ws.Cell(string.Format("{0}1", alpha)).Value = header_ch_name[i - 65];
                    Type colType = properties.Where(w => w.Name.Equals(header_en_name[i - 65])).ToList()[0].PropertyType;
                    if (colType.IsGenericType)
                    {
                        table.Columns.Add(new DataColumn(header_en_name[i - 65], colType.GenericTypeArguments[0]));
                    }
                    else
                    {
                        table.Columns.Add(new DataColumn(header_en_name[i - 65], colType));
                    }
                }

                foreach (LedgerDetail item in Details)
                {
                    DataRow newRow = table.NewRow();
                    foreach (PropertyInfo info in properties)
                    {
                        if (table.Columns.Contains(info.Name))
                        {
                            if (info.PropertyType.Name == "DateTime")
                            {
                                if (Convert.ToDateTime(info.GetValue(item)).ToString("HHmm").Equals("0000"))
                                    newRow[info.Name] = Convert.ToDateTime(info.GetValue(item)).ToString("yyyy/MM/dd");
                                else
                                    newRow[info.Name] = Convert.ToDateTime(info.GetValue(item)).ToString("yyyy/MM/dd HH:mm");
                                
                            }
                            else
                            {
                                var data = info.GetValue(item);
                                if (data != null)
                                {
                                    newRow[info.Name] = info.GetValue(item);
                                }
                                else
                                {
                                    newRow[info.Name] = DBNull.Value;
                                }
                            }
                        }
                    }
                    table.Rows.Add(newRow);
                }

                IXLRange rangeWithData = ws.Cell(2, 1).InsertData(table.AsEnumerable());
                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                ws.Columns().AdjustToContents();//欄位寬度根據資料調整
                try
                {
                    wb.SaveAs(fdlg.FileName);
                    ConfirmWindow cw = new ConfirmWindow("是否開啟檔案", "確認");
                    if ((bool)cw.DialogResult)
                    {
                        myProcess.StartInfo.UseShellExecute = true;
                        myProcess.StartInfo.FileName = fdlg.FileName;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        private void GetData(string param)
        {
            Title = param.Equals("0") ? "日記帳" : "明細帳";
            IsDayLedger = param.Equals("0") ? Visibility.Visible : Visibility.Collapsed; ;
            IsDetailLedger = param.Equals("1") ? Visibility.Visible : Visibility.Collapsed; ;
            Accounts = AccountsDb.GetJournalAccount("ALL");
            JournalAccount empty = new JournalAccount();
            Accounts.ToList().Add(empty);
            Account = new JournalAccount();
        }
    }
}
