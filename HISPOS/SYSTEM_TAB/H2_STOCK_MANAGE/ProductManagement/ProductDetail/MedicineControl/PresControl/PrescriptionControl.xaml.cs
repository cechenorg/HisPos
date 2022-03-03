using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Product.ProductManagement;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.PresControl
{
    /// <summary>
    /// PrescriptionControl.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionControl : UserControl
    {
        public PrescriptionControl()
        {
            InitializeComponent();
        }

        private void Prescription_OnClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (!(row?.Item is ProductRegisterPrescription)) return;

            switch (((ProductRegisterPrescription)row.Item).Type)
            {
                case ProductRegisterPrescriptionTypeEnum.REGISTER:
                    PrescriptionService.ShowPrescriptionEditWindow(((ProductRegisterPrescription)row.Item).ID, PrescriptionType.ChronicReserve);
                    break;

                case ProductRegisterPrescriptionTypeEnum.PRESCRIPTION:
                    PrescriptionService.ShowPrescriptionEditWindow(((ProductRegisterPrescription)row.Item).ID);
                    break;
            }
        }
    }
}