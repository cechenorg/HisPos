using System;
using System.ComponentModel;
using System.Data;
using His_Pos.Class;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos
{
    /// <summary>
    /// LoadingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LoadingWindow
    {
        public BackgroundWorker backgroundWorker = new BackgroundWorker();

        public LoadingWindow()
        {
            InitializeComponent();
        }

        public LoadingWindow(string message)
        {
            InitializeComponent();

            LoadingMessage.Content = message;

            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Close();
                }));
            };

            Show();
            backgroundWorker.RunWorkerAsync();
        }

        public void GetMedicineData(User userLogin)
        {
            LoadingMessage.Content = "Loading Medicine Data...";

            MainWindow mainWindow = new MainWindow(userLogin);
            
            backgroundWorker.DoWork += (s, o) => {
                var dd = new DbConnection(Settings.Default.SQL_global);
                MainWindow.MedicineDataTable = dd.ExecuteProc("[HIS_POS_DB].[GET].[MEDICINE]");
                MainWindow.View = new DataView(MainWindow.MedicineDataTable) { Sort = "HISMED_ID" };
            };

            backgroundWorker.RunWorkerCompleted += (s, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    mainWindow.Show();
                    Close();
                }));
            };
            backgroundWorker.RunWorkerAsync();
        }
        
    }
}
