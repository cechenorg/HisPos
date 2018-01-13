using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using His_Pos.PrescriptionInquire;

namespace His_Pos.PrescriptionDec
{
    /// <summary>
    /// PrescriptionDecView.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class PrescriptionDecView
    {
        public PrescriptionDecView()
        {
            InitializeComponent();
            StratClock();
        }

        private void TickEvent(Object sender, EventArgs e)
        {
            PrescriptionClock.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        }

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            row.InputBindings.Add(new MouseBinding(InsertChronicDataCommand,
                new MouseGesture() {MouseAction = MouseAction.LeftDoubleClick}));
        }

        private ICommand _insertChronicDataCommand;

        private ICommand InsertChronicDataCommand
        {
            get
            {
                return _insertChronicDataCommand ?? (_insertChronicDataCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunCustomFromVm()
                });
            }
        }

        private void RunCustomFromVm()
        {

        }

        private void StratClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TickEvent;
            timer.Start();
        }
    }
}
