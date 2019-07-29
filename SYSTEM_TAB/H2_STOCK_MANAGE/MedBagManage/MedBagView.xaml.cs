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
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.MedBagManage;
using His_Pos.NewClass.Prescription.Service;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.MedBagManage
{
    /// <summary>
    /// MedBagView.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagView : UserControl
    {
        public MedBagView()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (!(row?.Item is MedBagPrescriptionStruct)) return;

            PrescriptionService.ShowPrescriptionEditWindow(((MedBagPrescriptionStruct)row.Item).ID);
        }

        private void Reserve_Click(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (!(row?.Item is MedBagPrescriptionStruct)) return;

            PrescriptionService.ShowPrescriptionEditWindow(((MedBagPrescriptionStruct)row.Item).ID, PrescriptionType.ChronicReserve);
        }
    }
}
