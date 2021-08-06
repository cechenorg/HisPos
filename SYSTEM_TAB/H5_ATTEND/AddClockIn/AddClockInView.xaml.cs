using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.AddClockIn
{
    /// <summary>
    /// AddClockInView.xaml 的互動邏輯
    /// </summary>
    public partial class AddClockInView : UserControl
    {
        public AddClockInView()
        {            
            StartClock();
            InitializeComponent();
            Account.Focus();
        }


        private void Account_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(Account.Text))
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        Password.Focus();
                        break;


                }
            }
        }
        private void PassWord_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty((sender as System.Windows.Controls.PasswordBox)?.Password))           
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        AddClockIn.Focus();
                        break;

                        //case Key.Left:
                        //    AddClockIn.Focus();
                        //    break;
                }
            }
        }

        private DispatcherTimer ShowTimer;

        private void StartClock()
        {
            ShowTimer = new System.Windows.Threading.DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起個Timer一直獲取當前時間
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();
        }
        //show timer by_songgp
        public void ShowCurTimer(object sender, EventArgs e)
        {
            //"星期"+DateTime.Now.DayOfWeek.ToString(("d"))

            //獲得年月日
            this.SystemTime.Text = DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日
            this.SystemTime.Text += " ";
            //獲得星期幾
            this.SystemTime.Text += DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            this.SystemTime.Text += " ";
            //獲得時分秒
            this.SystemTime.Text += DateTime.Now.ToString("HH:mm:ss");
            //System.Diagnostics.Debug.Print("this.ShowCurrentTime {0}", this.ShowCurrentTime);
        }

    }
}
