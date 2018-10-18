using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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

namespace His_Pos.SystemSettings.SettingControl
{
    /// <summary>
    /// PrinterControl.xaml 的互動邏輯
    /// </summary>
    public partial class PrinterControl : UserControl
    {
        public PrinterControl()
        {
            InitializeComponent();

            //foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            //{
            //    MessageBox.Show(printer);
            //}

            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                
            }
        }
    }
}
