using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement
{
    /// <summary>
    /// NewIncomeStatementView.xaml 的互動邏輯
    /// </summary>
    public partial class NewIncomeStatementView : UserControl
    {
        private BackgroundWorker bgWorker = new BackgroundWorker();
        private int year;

        public NewIncomeStatementView()
        {
            InitializeComponent();
            tbYear.Text = DateTime.Now.Year.ToString();
            InitBackgroundWorker();
            InitData();
        }

        private void InitBackgroundWorker() 
        {
            bgWorker.DoWork += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() => { 
                    bi.BusyContent = "取得損益資料...";
                }));
                GetData(year);
            };

            bgWorker.RunWorkerCompleted += (sender, args) =>
            {
                bi.IsBusy = false;
            };
        }

        private void InitData()
        {
            int.TryParse(tbYear.Text, out year);

            bi.IsBusy = true;
            if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync();
        }

        private void GetData(int year) 
        {
            MainWindow.ServerConnection.OpenConnection();

            List<SqlParameter> paraCount = new List<SqlParameter>();
            paraCount.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraIncome = new List<SqlParameter>();
            paraIncome.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraExpanse = new List<SqlParameter>();
            paraExpanse.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraClosed = new List<SqlParameter>();
            paraClosed.Add(new SqlParameter("YEAR", year));

            DataTable count = MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCountByYear]", paraCount);
            DataTable income = MainWindow.ServerConnection.ExecuteProc("[Get].[IncomeByYear]", paraIncome);
            DataTable expanse = MainWindow.ServerConnection.ExecuteProc("[Get].[ExpanseByYear]", paraExpanse);
            DataTable closed = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsClosedByYear]", paraClosed);

            Dispatcher.BeginInvoke(new Action(() => {
                dgPreCount.ItemsSource = count.DefaultView;
                dgIncome.ItemsSource = income.DefaultView;
                dgExpanse.ItemsSource = expanse.DefaultView;
                dgDiff.ItemsSource = closed.DefaultView;
            }));

            MainWindow.ServerConnection.CloseConnection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitData();
        }

        private void dg_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta);
        }
    }
}
