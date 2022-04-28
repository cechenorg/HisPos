using ClosedXML.Excel;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.CashReport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using His_Pos.NewClass.Report.PrescriptionDetailReport;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.NewTodayCashStockEntryReport
{
    public class PrintService
    {
        public static void PrintPrescriptionProfitDetailAction(DateTime StartDate, DateTime EndDate, ICollectionView PrescriptionDetailReportView)
        {
            if (PrescriptionDetailReportView is null) return;

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "處方毛利明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "_" + EndDate.ToString("yyyyMMdd") + "處方毛利明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("姓名,醫療院所,藥服(估),藥品,自費,耗用,毛利");
                        double sumMedicalServicePoint = 0;
                        double sumMedicalPoint = 0;
                        double sumPaySelfPoint = 0;
                        decimal sumMeduse = 0;
                        double sumProfit = 0;
                        foreach (var c in PrescriptionDetailReportView)
                        {
                            PrescriptionDetailReport presc = (PrescriptionDetailReport)c;
                            file.WriteLine($"{presc.CusName},{presc.InsName},{presc.MedicalServicePoint},{presc.MedicalPoint},{presc.PaySelfPoint},{presc.Meduse},{presc.Profit}");
                            sumMedicalServicePoint += presc.MedicalServicePoint;
                            sumMedicalPoint += presc.MedicalPoint;
                            sumPaySelfPoint += presc.PaySelfPoint;
                            sumMeduse += presc.Meduse;
                            sumProfit += presc.Profit;
                        }
                        file.WriteLine($"合計,,{sumMedicalServicePoint},{sumMedicalPoint},{sumPaySelfPoint},{sumMeduse},{sumProfit}");
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        public static void PrintCashPerDayAction(DateTime StartDate, DateTime EndDate, CashReport CashflowSelectedItem)
        {
            if (CashflowSelectedItem is null)
                return;
            DataTable table = CashReportDb.GetPerDayDataByDate(StartDate, EndDate, CashflowSelectedItem.TypeId);
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "金流存檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "_" + CashflowSelectedItem.TypeName + "金流存檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("日期,部分負擔,自費,自費調劑,押金,其他,總計");
                        int cCopayMentPrice = 0;
                        int cPaySelfPrice = 0;
                        int cAllPaySelfPrice = 0;
                        int cDepositPrice = 0;
                        int cOtherPrice = 0;
                        int cTotalPrice = 0;
                        foreach (DataRow s in table.Rows)
                        {
                            file.WriteLine($"{s.Field<DateTime>("date").AddYears(-1911).ToString("yyy/MM/dd")}," +
                                $"{s.Field<int>("CopayMentPrice")},{s.Field<int>("PaySelfPrice")},{s.Field<int>("AllPaySelfPrice")}" +
                                $",{s.Field<int>("DepositPrice")},{s.Field<int>("OtherPrice")},{s.Field<int>("TotalPrice")}");

                            cCopayMentPrice += s.Field<int>("CopayMentPrice");
                            cPaySelfPrice += s.Field<int>("PaySelfPrice");
                            cAllPaySelfPrice += s.Field<int>("AllPaySelfPrice");
                            cDepositPrice += s.Field<int>("DepositPrice");
                            cOtherPrice += s.Field<int>("OtherPrice");
                            cTotalPrice += s.Field<int>("TotalPrice");
                        }
                        file.WriteLine($"總計,{cCopayMentPrice},{cPaySelfPrice},{cAllPaySelfPrice},{cDepositPrice},{cOtherPrice},{cTotalPrice}");
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }

        public static void PrintTradeProfitDetail(DateTime StartDate, DateTime EndDate)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("typeId", "0"));
            parameters.Add(new SqlParameter("sDate", StartDate));
            parameters.Add(new SqlParameter("eDate", EndDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeProfitDetailReportByDateExcel]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "銷售明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "-" + EndDate.ToString("yyyyMMdd") + "銷售明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("銷售明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                var col1 = ws.Column("A");
                col1.Width = 10;
                var col2 = ws.Column("B");
                col2.Width = 15;
                var col3 = ws.Column("C");
                col3.Width = 25;
                var col4 = ws.Column("D");
                col4.Width = 25;
                var col5 = ws.Column("E");
                col5.Width = 10;
                var col6 = ws.Column("F");
                col6.Width = 10;
                var col7 = ws.Column("G");
                col7.Width = 10;
                var col8 = ws.Column("H");
                col8.Width = 10;
                var col9 = ws.Column("I");
                col9.Width = 10;

                ws.Cell(1, 1).Value = "銷售明細";
                ws.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "銷售時間";
                ws.Cell("B2").Value = "姓名";
                ws.Cell("C2").Value = "現金";
                ws.Cell("D2").Value = "刷卡";
                ws.Cell("E2").Value = "訂金沖銷";
                ws.Cell("F2").Value = "禮券";
                ws.Cell("G2").Value = "現金券";
                ws.Cell("H2").Value = "耗用";
                ws.Cell("I2").Value = "毛利";
                ws.Cell("J2").Value = "銷售員";

                var rangeWithData = ws.Cell(3, 1).InsertData(result.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                wb.SaveAs(fdlg.FileName);
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

        public static void PrintRewardDetail(DateTime StartDate, DateTime EndDate)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("sDate", StartDate));
            parameters.Add(new SqlParameter("eDate", EndDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[RewardDetailRecordByDateExcel]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "績效明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "-" + EndDate.ToString("yyyyMMdd") + "績效明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("績效明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                var col1 = ws.Column("A");
                col1.Width = 10;
                var col2 = ws.Column("B");
                col2.Width = 15;
                var col3 = ws.Column("C");
                col3.Width = 25;
                var col4 = ws.Column("D");
                col4.Width = 25;
                var col5 = ws.Column("E");
                col5.Width = 10;

                ws.Cell(1, 1).Value = "績效明細";
                ws.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "績效人員";
                ws.Cell("B2").Value = "銷售時間";
                ws.Cell("C2").Value = "商品代碼";
                ws.Cell("D2").Value = "商品名稱";
                ws.Cell("E2").Value = "績效";

                var rangeWithData = ws.Cell(3, 1).InsertData(result.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                wb.SaveAs(fdlg.FileName);
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
    }
}
