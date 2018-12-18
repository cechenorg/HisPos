using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace His_Pos.H1_DECLARE.PrescriptionDec2 {
    /// <summary>
    /// MedHistoryWebWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedHistoryWebWindow : Window {
        [ComVisible(true)]
        public class ScriptHelper {
            public Action<string> ProcMessage = null;
            public void SendMessage(string msg) {
                if (ProcMessage != null)
                    ProcMessage(msg);
            }
        }
        private ScriptHelper sh = new ScriptHelper();

        public MedHistoryWebWindow() {
            InitializeComponent(); 
            Uri uri = new Uri("http://singde.com.tw/ClickOnce/publish.htm");
            Web.Navigate(uri);
        }

        private void Web_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e) {
            //加入ProcMessage事件，處理傳回字串
            sh.ProcMessage = (s) =>
            {
                MessageWindow messageWindow = new MessageWindow(s, Class.MessageType.SUCCESS);
                messageWindow.ShowDialog();
            };
            //執行Script，取得熱門新聞，注意: 必須要在網頁載入完成後才可呼叫
            Web.InvokeScript("eval", @"
                 alert('hack');");
        }
         
        private void Web_Loaded(object sender, RoutedEventArgs e) {
            //指定ObjectForScripting，網頁可透過window.external存取該物件
            Web.ObjectForScripting = sh;
        }
    }
    
}
