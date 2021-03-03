using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Medicine = His_Pos.NewClass.Medicine.Base.Medicine;
using Prescription = His_Pos.NewClass.Prescription.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow
{
    /// <summary>
    /// QRCodeReveiveWindow.xaml 的互動邏輯
    /// </summary>
    public partial class QRCodeReceiveWindow : Window
    {
        private Prescription p;
        private System.Timers.Timer timer = new System.Timers.Timer(200);

        public QRCodeReceiveWindow()
        {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(InputIdle);
            p = new Prescription();
            QRCodeReceiver.Focus();
            ShowDialog();
        }

        private void InputIdle(object source, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                btnFinish.IsEnabled = true;
            });
        }

        private void QRCodeReceiver_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            btnFinish.IsEnabled = false;
            if (timer.Enabled)
            {
                timer.Interval = 200;
            }
            else
            {
                timer.Start();
            }
            /*int count = 0;
            if (e.Key == Key.Return)
            {
                count++;
                lblInfo.Content = $"已掃描 {count} 個QRcode";
            }*/
        }

        private void SetPrescriptionData()
        {
            try
            {
                var result = QRCodeReceiver.Text.Split(';');
                //var prescriptionCase = ViewModelMainWindow.GetPrescriptionCases(result[2]);
                //var medicineDays = result[9];
                SetPatient(result);
                SetTreatmentData(result);
                GetMedicines(result);
                SetMedicinesValue(result);
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage(e.Message, MessageType.ERROR);
            }
        }

        private void SetTreatmentData(IReadOnlyList<string> result)
        {
            SetAdjustCase(result);
            SetInstitution(result);
            SetDivision(result);
            SetTreatDateAndAdjustDate(result);
            SetCopayment(result);
            GetDisease(result);
            SetMedicalNumber(result);
        }

        private void SetPatient(IReadOnlyList<string> result)
        {
            var patientName = result[3].Replace("?", "");
            var patientIDNumber = result[4].Contains("*") ? "" : result[4];
            var patientBirthday = ParseBirthday(result);
            var c = new Customer { Name = patientName, Birthday = patientBirthday, IDNumber = patientIDNumber };
            if (!string.IsNullOrEmpty(c.IDNumber))
                c.Check();
            p.Patient = c;
        }

        private DateTime? ParseBirthday(IReadOnlyList<string> result)
        {
            var dateString = result[5];
            var dateStringLength = dateString.Length;
            var year = int.Parse(dateString.Substring(0, dateStringLength - 4)) + 1911;
            var month = int.Parse(dateString.Substring(dateStringLength - 4, 2));
            var date = int.Parse(dateString.Substring(dateStringLength - 2, 2));
            return new DateTime(year, month, date);
        }

        private void SetAdjustCase(IReadOnlyList<string> result)
        {
            var adjustCase = result[1];
            switch (adjustCase)
            {
                case "1":
                    p.AdjustCase = ViewModelMainWindow.GetAdjustCase("1");
                    break;

                case "2":
                    p.AdjustCase = ViewModelMainWindow.GetAdjustCase("2");
                    break;

                default:
                    p.AdjustCase = ViewModelMainWindow.GetAdjustCase("1");
                    break;
            }
        }

        private void SetInstitution(IReadOnlyList<string> result)
        {
            var institution = result[0].Substring(result[0].Length - 10, 10);
            p.Institution = ViewModelMainWindow.GetInstitution(institution);
        }

        private void SetDivision(IReadOnlyList<string> result)
        {
            var division = result[6];
            p.Division = ViewModelMainWindow.GetDivision(division);
        }

        private void SetMedicalNumber(IReadOnlyList<string> result)
        {
            var medicalNum = result[8];
            p.TempMedicalNumber = medicalNum;
        }

        private void SetTreatDateAndAdjustDate(IReadOnlyList<string> result)
        {
            var treDate = new DateTime(int.Parse(result[7].Substring(0, result[7].Length - 4)) + 1911, int.Parse(result[7].Substring(result[7].Length - 4, 2)), int.Parse(result[7].Substring(result[7].Length - 2, 2)));
            p.TreatDate = treDate;
            p.AdjustDate = DateTime.Today;
        }

        private void SetCopayment(IReadOnlyList<string> result)
        {
            var copayment = ViewModelMainWindow.GetCopayment(result[10]);
            p.Copayment = copayment ?? ViewModelMainWindow.GetCopayment("I21");
        }

        private void GetDisease(IReadOnlyList<string> result)
        {
            var diseases = new[] { result[11] };

            if (result[11].Contains("#"))
                diseases = result[11].Split('#');
            else if (result[11].Contains(","))
                diseases = result[11].Split(',');
            var mainDiseaseCode = diseases[0].Replace(".", "");
            p.MainDisease = DiseaseCode.GetDiseaseCodeByID(mainDiseaseCode);
            if (diseases.Length > 1)
            {
                var twoDiseaseCode = diseases[1].Replace(".", "");
                p.SubDisease = DiseaseCode.GetDiseaseCodeByID(twoDiseaseCode);
            }
        }

        private void GetMedicines(string[] result)
        {
            DataTable table = CreateMedicinesTable(result);
            for (var i = 0; i < table.Rows.Count; i++)
            {
                Medicine medicine;
                switch (table.Rows[i].Field<int>("DataType"))
                {
                    case 1:
                        medicine = new MedicineNHI(table.Rows[i]);
                        p.Medicines.Add(medicine);
                        break;

                    case 2:
                        medicine = new MedicineOTC(table.Rows[i]);
                        p.Medicines.Add(medicine);
                        break;

                    case 3:
                        medicine = new MedicineSpecialMaterial(table.Rows[i]);
                        p.Medicines.Add(medicine);
                        break;
                }
            }
        }

        private DataTable CreateMedicinesTable(string[] result)
        {
            var medIdList = CreateMedicineIDList(result);
            MainWindow.ServerConnection.OpenConnection();
            var table = MedicineDb.GetQRcodeMedicine(medIdList, p.WareHouse?.ID, p.AdjustDate);
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }

        private List<string> CreateMedicineIDList(string[] result)
        {
            var medIdList = new List<string>();
            for (var x = 14; x < result.Length - 1; x += 5)
            {
                if (result[x].Equals(string.Empty)) continue;
                var id = result[x];
                medIdList.Add(id);
            }
            return medIdList;
        }

        private void SetMedicinesValue(string[] result)
        {
            var i = 0;
            for (var x = 14; x < result.Length - 1; x += 5)
            {
                if (result[x].Equals(string.Empty)) continue;
                var dosage = double.Parse(result[x + 1].Replace("+", ""));
                var frequency = result[x + 2];
                var way = result[x + 3].ToUpper();
                var total = double.Parse(result[x + 4].Replace("+", ""));
                p.Medicines[i].Dosage = dosage;
                p.Medicines[i].UsageName = frequency.ToUpper();
                if (way.Contains("AC"))
                {
                    p.Medicines[i].UsageName += "AC";
                    way = way.Replace("AC", "").Replace("/", "").Replace(" ", "").Replace(",", "");
                }
                if (way.Contains("PC"))
                {
                    p.Medicines[i].UsageName += "PC";
                    way = way.Replace("PC", "").Replace("/", "").Replace(" ", "").Replace(",", "");
                }
                p.Medicines[i].PositionID = way;
                p.Medicines[i].Amount = total;
                if (p.Medicines[i].ID.EndsWith("00") || p.Medicines[i].ID.EndsWith("G0"))
                {
                    switch (p.Medicines[i].UsageName)
                    {
                        case "QD":
                        case "HS":
                        case "QN":
                        case "QM":
                        case "QMA":
                        case "QAM":
                        case "QDA":
                        case "QDAM":
                        case "QDPM":
                        case "QDHS":
                        case "QDAC":
                        case "QDPC":
                            p.Medicines[i].Days = UsagesFunction.GetDaysByUsage_QD(total, dosage);
                            break;

                        case "BD":
                        case "BID":
                        case "BIDA":
                        case "BIDHS":
                        case "BIDCC":
                        case "QPMHS":
                        case "QAMPM":
                        case "QAMHS":
                            p.Medicines[i].Days = UsagesFunction.GetDaysByUsage_BID(total, dosage);
                            break;

                        case "TID":
                        case "TIDA":
                        case "TIDHS":
                        case "TIDCC":
                        case "TIDAC":
                        case "TIDPC":
                        case "BIDACHS":
                            p.Medicines[i].Days = UsagesFunction.GetDaysByUsage_TID(total, dosage);
                            break;

                        case "QID":
                        case "QIDA":
                        case "QIDAC":
                        case "QIDPC":
                        case "QIDP":
                        case "QIDACHS":
                            p.Medicines[i].Days = UsagesFunction.GetDaysByUsage_QID(total, dosage);
                            break;

                        case "QOD":
                            p.Medicines[i].Days = UsagesFunction.GetDaysByUsage_QOD(total, dosage);
                            break;

                        case "PID":
                            p.Medicines[i].Days = UsagesFunction.GetDaysByUsage_PID(total, dosage);
                            break;
                    }
                }
                i++;
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            SetPrescriptionData();
            Close();
            Messenger.Default.Send(new NotificationMessage<Prescription>(this, p, "CustomerPrescriptionSelected"));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnFinish.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            if (e.Key == Key.Escape)
            {
                btnCancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }
}