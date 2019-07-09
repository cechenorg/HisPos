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
using His_Pos.NewClass.PrescriptionRefactoring;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindowRefactoring
{
    /// <summary>
    /// PrescriptionRecordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionRecordWindow : Window
    {
        public PrescriptionRecordWindow()
        {
            InitializeComponent();
        }
        public PrescriptionRecordWindow(Prescription p)
        {
            InitializeComponent();
        }
    }
}
