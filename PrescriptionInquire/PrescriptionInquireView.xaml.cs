using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.Properties;
using His_Pos.Service;
using ComboBox = System.Windows.Controls.ComboBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.PrescriptionInquire
{

    public partial class PrescriptionInquireView : UserControl
    {
        public ObservableCollection<PrescriptionOverview> prescriptionOverview = new ObservableCollection<PrescriptionOverview>();
        
        private Function f = new Function();
        public PrescriptionInquireView()
        {
            InitializeComponent();

            //CultureInfo cag = new CultureInfo("zh-TW");
            //cag.DateTimeFormat.Calendar = new TaiwanCalendar();
            //Thread.CurrentThread.CurrentCulture = cag;

            LoadAdjustCases();
            LoadHospitalData();
        }

        private void showInquireOutcome(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (PrescriptionOverview)(sender as DataGridRow).Item;
            PrescriptionInquireOutcome prescriptionInquireOutcome = new PrescriptionInquireOutcome(PrescriptionDB.GetDeclareDataById(selectedItem.DECMAS_ID));
            prescriptionInquireOutcome.Show();
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            PrescriptionSet.SelectedItem = (sender as DataGridRow).Item;
        }

        /*
        *載入原處方案件類別
        */
        private void LoadAdjustCases()
        {
            foreach (var adjustCase in AdjustCaseDb.AdjustCaseList)
            {
                AdjustCaseCombo.Items.Add(adjustCase.Id + ". " + adjustCase.Name);
            }
        }

        /*
         *載入醫療院所資料
         */
        private void LoadHospitalData()
        {
            var institutions = new Institutions();
            institutions.GetData();
            ReleasePalace.ItemsSource = institutions.InstitutionsCollection;
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            prescriptionOverview.Clear();

            var sDate = Convert.ToDateTime(start.SelectedDate);
            var eDate = Convert.ToDateTime(end.SelectedDate);
            var patientName = PatientName.Text;
            string adjustId = "";

            if (AdjustCaseCombo.Text != String.Empty)
                adjustId = AdjustCaseCombo.Text.Substring(0, 1);
            
            prescriptionOverview = PrescriptionDB.GetPrescriptionOverviewBySearchCondition(sDate, eDate, patientName, adjustId);
            PrescriptionSet.ItemsSource = prescriptionOverview;
        }
    }
}
