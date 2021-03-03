using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.AccountReport;
using His_Pos.Service.ExportService;
using JetBrains.Annotations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;

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

        private int year;

        public int Year
        {
            get => year;
            set
            {
                year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        public ExportIncomeStatementWindow()
        {
            InitializeComponent();
        }

        public ExportIncomeStatementWindow(int year)
        {
            InitializeComponent();
            DataContext = this;
            Year = year;
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
            Collection<object> tempCollection = new Collection<object> { Year };

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