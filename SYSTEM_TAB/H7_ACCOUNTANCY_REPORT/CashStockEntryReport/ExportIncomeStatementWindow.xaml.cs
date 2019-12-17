using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.AccountReport;
using His_Pos.Service.ExportService;
using JetBrains.Annotations;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport
{
    /// <summary>
    /// ExportIncomeStatementWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ExportIncomeStatementWindow : Window, INotifyPropertyChanged
    {
        private string templateFile = "";
        public string TemplateFile
        {
            get { return templateFile; }
            set
            {
                templateFile = value;
                OnPropertyChanged(nameof(TemplateFile));
            }
        }
        public ExportIncomeStatementWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ChooseFile_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "選擇範本";
                dialog.InitialDirectory = ".\\";
                dialog.Filter = "xlsx files (*.xlsx)|*.xlsx";

                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    TemplateFile = dialog.FileName;
                    return;
                }
            }

            TemplateFile = "";
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Collection<object> tempCollection = new Collection<object>() { new List<object> { "" } };

            if (TemplateFile.Equals(""))
                templateFile = @"NewClass\AccountReport\年度損益表.xlsx"; 

            MainWindow.ServerConnection.OpenConnection();
            ExportExcelService service = new ExportExcelService(tempCollection, new ExportIncomeStatementTemplate(), templateFile);
            bool isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\損益表{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
            MainWindow.ServerConnection.CloseConnection();

            if (isSuccess)
                MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
            else
                MessageWindow.ShowMessage("匯出失敗 請稍後再試", MessageType.ERROR);

            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
