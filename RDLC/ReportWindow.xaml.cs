using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;
using Brushes = System.Drawing.Brushes;

namespace His_Pos.RDLC
{
    /// <summary>
    /// ReportWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ReportWindow : Window
    {
        private IList<Stream> m_streams;
        private  int m_currentPageIndex;
        public ReportWindow(DataTable t)
        {
            InitializeComponent();
            m_streams = new List<Stream>();
            RptViewer.LocalReport.DataSources.Clear();
            RptViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("InventoryDataSet", t));
            RptViewer.LocalReport.ReportPath = @"..\..\RDLC\InventoryCheckSheet.rdlc";
            RptViewer.LocalReport.Refresh();
            RptViewer.ProcessingMode = ProcessingMode.Local;
            CreatePdf(RptViewer);
        }
        public static void CreatePdf(ReportViewer viewer)
        {
            var deviceInfo = "<DeviceInfo>" +
                             "  <OutputFormat>PDF</OutputFormat>" +
                             "  <PageWidth>" + 21 + "cm</PageWidth>" +
                             "  <PageHeight>" + 29.7 + "cm</PageHeight>" +
                             "  <MarginTop>0cm</MarginTop>" +
                             "  <MarginLeft>0cm</MarginLeft>" +
                             "  <MarginRight>0cm</MarginRight>" +
                             "  <MarginBottom>0cm</MarginBottom>" +
                             "</DeviceInfo>";
            var bytes = viewer.LocalReport.Render("PDF", deviceInfo);

            using (var fs = new FileStream("output.pdf", FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
