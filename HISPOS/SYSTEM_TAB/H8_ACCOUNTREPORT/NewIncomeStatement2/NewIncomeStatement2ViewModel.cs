using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Report;
using His_Pos.NewClass.Report.IncomeStatement;
using His_Pos.NewClass.StoreOrder;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using ClosedXML.Excel;
using His_Pos.FunctionWindow;
using His_Pos.Class;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement2
{
    internal class NewIncomeStatement2ViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        private int _year = DateTime.Today.Year;

        public int Year
        {
            get => _year;
            set
            {
                Set(() => Year, ref _year, value);
            }
        }

        private ObservableCollection<IncomeStatementDisplayData> _incomeStatementData;

        public ObservableCollection<IncomeStatementDisplayData> IncomeStatementData
        {
            get => _incomeStatementData;
            set
            {
                Set(() => IncomeStatementData, ref _incomeStatementData, value);
            }
        }

        private IncomeStatementDisplayData _selectedIncomeStatementData;

        public IncomeStatementDisplayData SelectedIncomeStatementData
        {
            get => _selectedIncomeStatementData;
            set
            {
                Set(() => SelectedIncomeStatementData, ref _selectedIncomeStatementData, value);
            }
        }


        public ICommand SearchCommand { get; set; }
        public ICommand YearMinusCommand { get; set; }
        public ICommand YearAddCommand { get; set; }

        public ICommand OpenDetailCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        

        public NewIncomeStatement2ViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            YearAddCommand = new RelayCommand(YearAdd);
            YearMinusCommand = new RelayCommand(YearMinus);
            OpenDetailCommand = new RelayCommand(OpenDetail);
            ExportCommand = new RelayCommand(ExportAction);
            Search();
        }

        private void OpenDetail()
        {
            if(SelectedIncomeStatementData is null)
                return;

            var expenseDatas = ReportService.GetIncomeStatementDetail(_year, Convert.ToString(_selectedIncomeStatementData.TypeID), Convert.ToString(_selectedIncomeStatementData.AccID)).ToList();


            if (expenseDatas.Count > 0)
            {
                AccountDetailWindow accountDetailWindow = new AccountDetailWindow()
                {
                    DataContext = new AccountDetailViewModel(expenseDatas)
                };
                accountDetailWindow.ShowDialog();
            }
          
        }

        private void YearAdd()
        {
            Year++;
            Search();
        }

        private void YearMinus()
        {
            Year--;
            Search();
        }


        private void Search()
        {

            var expenseDatas = ReportService.GetIncomeStatement(_year);
            GetExpenseData(expenseDatas);

        }

        private void GetExpenseData(IEnumerable<IncomeStatementRawData> rawData)
        {
            IncomeStatementData = new ObservableCollection<IncomeStatementDisplayData>();
          
            foreach (var typeName in rawData.Select(_ => _.ISType).Distinct())
            {

                var typeID = rawData.First(_ => _.ISType == typeName).ISTypeNo;

                IncomeStatementDisplayData typeIncomeData = new IncomeStatementDisplayData() { Name = typeName,TypeID = typeID };
                IncomeStatementData.Add(typeIncomeData);

                foreach (var groupNo in rawData.Where(_ => _.ISType == typeName).Select(_ => _.ISGroupNo).Distinct())
                {

                    if (groupNo == 0)
                    {
                        typeIncomeData.DisplayLayerCount = 2;
                        foreach (var accID in rawData.Where(_ => _.ISType == typeName && _.ISGroupNo == groupNo).Select(_ => _.AcctID).Distinct())
                        {
                            string accName = rawData.First(_ => _.AcctID == accID).AcctName;
                            IncomeStatementDisplayData accIncomeData = new IncomeStatementDisplayData()
                            {
                                Name = accName,AccID = accID, TypeID = typeIncomeData.TypeID

                            };
                            typeIncomeData.Childs.Add(accIncomeData);

                            var accfilterData = rawData.Where(_ => _.AcctID == accID).OrderBy(_ => _.MM);

                            foreach (var fdata in accfilterData)
                            {
                                accIncomeData.MonthlyValues[fdata.MM - 1] = fdata.AcctValue;
                            }
                        }
                        for (int i = 0; i < 12; i++)
                        {
                            typeIncomeData.MonthlyValues[i] = typeIncomeData.Childs.Sum(_ => _.MonthlyValues[i]);
                        }
                    }
                    else
                    {
                        typeIncomeData.DisplayLayerCount = 3;
                        string groupName = rawData.First(_ => _.ISType == typeName && _.ISGroupNo == groupNo).ISGroup;
                        IncomeStatementDisplayData groupIncomeData = new IncomeStatementDisplayData() { Name = groupName, };
                        typeIncomeData.Childs.Add(groupIncomeData);

                        foreach (var accID in rawData.Where(_ => _.ISType == typeName && _.ISGroupNo == groupNo).Select(_ => _.AcctID).Distinct())
                        {
                            string accName = rawData.First(_ => _.AcctID == accID).AcctName;
                            IncomeStatementDisplayData accIncomeData = new IncomeStatementDisplayData() { Name = accName, AccID = accID, TypeID = typeIncomeData.TypeID };
                            groupIncomeData.Childs.Add(accIncomeData);

                            var accfilterData = rawData.Where(_ => _.AcctID == accID).OrderBy(_ => _.MM);

                            foreach (var fdata in accfilterData)
                            {
                                accIncomeData.MonthlyValues[fdata.MM - 1] = fdata.AcctValue;
                            }
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            groupIncomeData.MonthlyValues[i] = groupIncomeData.Childs.Sum(_ => _.MonthlyValues[i]);
                        }
                    }

                   

                }
                for (int i = 0; i < 12; i++)
                {
                    typeIncomeData.MonthlyValues[i] = typeIncomeData.Childs.Sum(_ => _.MonthlyValues[i]);
                }
            }

            IncomeStatementDisplayData totalSumData = new IncomeStatementDisplayData() { Name = "總和",DisplayLayerCount = 1};

            for (int i = 0; i < 12; i++)
            {
                totalSumData.MonthlyValues[i] = IncomeStatementData.Select(_ => _.MonthlyValues[i]).Sum();
            }
            IncomeStatementData.Add(totalSumData);

            
        }
        private void ExportAction()
        {
            IEnumerable<IncomeStatementDetailData> exportData = ReportService.GetIncomeStatementDetail(Year, "0");
            if (exportData is null || exportData.Count() == 0)
                return;

            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog
            {
                Title = "損益表",
                InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath,
                Filter = "XLSX檔案|*.xlsx",
                FileName = string.Format("損益表{0}", Year),
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                IXLWorksheet ws = wb.Worksheets.Add("損益表");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                ws.Cell("A1").Value = "年";
                ws.Cell("B1").Value = "月";
                ws.Cell("C1").Value = "會計科目種類";
                ws.Cell("D1").Value = "會計科目";
                ws.Cell("E1").Value = "會計科目代號";
                ws.Cell("F1").Value = "會計科目子代號";
                ws.Cell("G1").Value = "會計科目名稱";
                ws.Cell("H1").Value = "金額";

                DataTable table = new DataTable();
                table.Columns.Add(new DataColumn("YYYY",typeof(int)));
                table.Columns.Add(new DataColumn("MM", typeof(string)));
                table.Columns.Add(new DataColumn("ISType", typeof(string)));
                table.Columns.Add(new DataColumn("ISGroup", typeof(string)));
                table.Columns.Add(new DataColumn("AcctLvl2", typeof(string)));
                table.Columns.Add(new DataColumn("AcctLvl3", typeof(string)));
                table.Columns.Add(new DataColumn("JouDet_AcctName", typeof(string)));
                table.Columns.Add(new DataColumn("AcctValue", typeof(int)));
                foreach (IncomeStatementDetailData item in exportData)
                {
                    DataRow dr = table.NewRow();
                    dr["YYYY"] = item.YYYY;
                    dr["MM"] = Convert.ToString(item.MM).PadLeft(2, '0');
                    dr["ISType"] = item.ISType;
                    dr["ISGroup"] = item.ISGroup;
                    dr["AcctLvl2"] = item.AcctLvl2;
                    dr["AcctLvl3"] = item.AcctLvl3;
                    dr["JouDet_AcctName"] = item.JouDet_AcctName;
                    dr["AcctValue"] = item.AcctValue.ToString();
                    table.Rows.Add(dr);
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
    }
}
