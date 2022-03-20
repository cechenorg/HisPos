using GalaSoft.MvvmLight.Messaging;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage
{
    /// <summary>
    /// DeclareFileManageView.xaml 的互動邏輯
    /// </summary>
    public partial class DeclareFileManageView : UserControl
    {
        public DeclareFileManageView()
        {
            InitializeComponent();
        }

        private void DeclareFileManageView_OnLoaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(nameof(DeclareFileManageViewModel) + "GetPrescriptions"));
        }

        private void btnCloud_Info_Click(object sender, RoutedEventArgs e)
        {
            string url = @"https://medvpn.nhi.gov.tw/";
            Process pro = new Process();
            pro.StartInfo.FileName = "iexplore.exe";
            pro.StartInfo.Arguments = url;
            pro.Start();
        }
    }
}