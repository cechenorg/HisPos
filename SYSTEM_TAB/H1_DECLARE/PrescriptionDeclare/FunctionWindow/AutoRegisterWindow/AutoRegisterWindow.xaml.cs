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
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.AutoRegisterWindow
{
    /// <summary>
    /// AutoRegisterWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AutoRegisterWindow : Window
    {
        public AutoRegisterWindow()
        {
            InitializeComponent();
        }

        public AutoRegisterWindow(Prescription current,Prescriptions registerList)
        {
            InitializeComponent();
            DataContext = new AutoRegisterViewModel(current,registerList);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "AutoRegisterSubmit":
                        DialogResult = true;
                        Close();
                        break;
                    case "AutoRegisterCancel":
                        DialogResult = false;
                        Close();
                        break;
                }
            });
            if(registerList.Count > 0)
                ShowDialog();
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }
    }
}
