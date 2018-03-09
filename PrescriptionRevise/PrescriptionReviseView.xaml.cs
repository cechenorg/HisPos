using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.PrescriptionInquire;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.PrescriptionRevise
{
    /// <summary>
    /// PrescriptionRevise.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionReviseView : UserControl
    {
        public PrescriptionReviseView()
        {
            InitializeComponent();
            InitialReviseUiElement();
            DataContext = this;
            CultureInfo cag = new CultureInfo("zh-TW");
            cag.DateTimeFormat.Calendar = new TaiwanCalendar();
            Thread.CurrentThread.CurrentCulture = cag;
        }

        private void InitialReviseUiElement()
        {
            string[] caseCategory = { "1: 一般處方調劑", "2: 慢性病連續處方調劑", "3: 日劑藥費", "4: 肺結核個案DOTS執行服務費", "5: 協助辦理門診戒菸計畫", "D: 藥事居家照護" };

            for (int i = 0; i < 6; i++)
            {
                CaseCateCombo.Items.Add(caseCategory[i]);
            }

            var cui = new CultureInfo("zh-TW", true)
            {
                DateTimeFormat =
                {
                    Calendar = new TaiwanCalendar(),
                    ShortDatePattern = "yy/MM/dd",
                    DateSeparator = "/"
                }
            };
            Thread.CurrentThread.CurrentCulture = cui;
        }

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            Debug.Assert(row != null, nameof(row) + " != null");
            row.InputBindings.Add(new MouseBinding(ShowCustomDialogCommand,
                new MouseGesture() { MouseAction = MouseAction.LeftDoubleClick }));
        }

        private ICommand _showCustomDialogCommand;

        private ICommand ShowCustomDialogCommand
        {
            get
            {
                return _showCustomDialogCommand ?? (_showCustomDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunCustomFromVm()
                });
            }
        }

        private void RunCustomFromVm()
        {

        }
        
    }
}
