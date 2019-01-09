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
using His_Pos.Class.Product;
using His_Pos.Service;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// MedicineInfoWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineInfoWindow : Window
    {
        public MedicineInformation Med { get; set; }
        public MedicineInfoWindow(MedicineInformation medicineInformation)
        {
            InitializeComponent();
            Med = medicineInformation.DeepCloneViaJson();
            DataContext = this;
        }
    }
}
