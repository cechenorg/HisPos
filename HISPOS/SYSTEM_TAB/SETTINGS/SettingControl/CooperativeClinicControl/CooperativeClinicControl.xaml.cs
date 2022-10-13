using His_Pos.Service;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.CooperativeClinicControl
{
    /// <summary>
    /// CooperativeClinicControl.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeClinicControl : UserControl
    {
        public CooperativeClinicControl()
        {
            InitializeComponent();
            DataContext = new CooperativeClinicControlViewModel();
        }

        private void StartDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
            }
        }
    }

   
    
}