﻿using System;
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
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
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
