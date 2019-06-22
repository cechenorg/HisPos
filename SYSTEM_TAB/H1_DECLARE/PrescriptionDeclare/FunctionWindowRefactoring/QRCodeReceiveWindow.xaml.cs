using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Product.Medicine;
using Medicine = His_Pos.NewClass.MedicineRefactoring.Medicine;
using Prescription = His_Pos.NewClass.PrescriptionRefactoring.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring
{
    /// <summary>
    /// QRCodeReveiveWindow.xaml 的互動邏輯
    /// </summary>
    public partial class QRCodeReceiveWindow : Window
    {
        public QRCodeReceiveWindow()
        {
            InitializeComponent();
            QRCodeReceiver.Focus();
            ShowDialog();
        }

        private void QRCodeReceiver_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Console.WriteLine(QRCodeReceiver.Text);
                var p = new Prescription();
                var temp = QRCodeReceiver.Text;
                var result = temp.Split(';');
                var institution = result[0].Substring(result[0].Length - 10, 10);
                var adjustCase = result[1];
                var preCase = ViewModelMainWindow.GetPrescriptionCases(result[2]);
                var cusName = result[3].Replace("?", "");
                var IDNum = result[4].Contains("*") ? "" : result[4];
                var birth = new DateTime(int.Parse(result[5].Substring(0, result[5].Length - 4)) + 1911, int.Parse(result[5].Substring(result[5].Length - 4, 2)), int.Parse(result[5].Substring(result[5].Length - 2, 2)));
                var division = result[6];
                var treDate = new DateTime(int.Parse(result[7].Substring(0, result[7].Length - 4)) + 1911, int.Parse(result[7].Substring(result[7].Length - 4, 2)), int.Parse(result[7].Substring(result[7].Length - 2, 2)));
                var medicalNum = result[8];
                var preDays = result[9];
                var copayment = ViewModelMainWindow.GetCopayment(result[10]);
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
                p.Institution = ViewModelMainWindow.GetInstitution(institution);
                p.Division = ViewModelMainWindow.GetDivision(division);
                var c = new Customer {Name = cusName, Birthday = birth, IDNumber = IDNum};
                if (!string.IsNullOrEmpty(c.IDNumber))
                    c.Check();
                p.Patient = c;
                p.TreatDate = treDate;
                p.TempMedicalNumber = medicalNum;
                p.AdjustDate = DateTime.Today;
                p.Copayment = copayment ?? ViewModelMainWindow.GetCopayment("I21");
                var dArray = new[] { result[11] };

                if (result[11].Contains("#"))
                    dArray = result[11].Split('#');
                else if (result[11].Contains(","))
                    dArray = result[11].Split(',');

                var mainDiseaseCode = dArray[0].Replace(".", "");
                p.MainDisease = DiseaseCode.GetDiseaseCodeByID(mainDiseaseCode);
                if (dArray.Length > 1)
                {
                    var twoDiseaseCode = dArray[1].Replace(".", "");
                    p.SubDisease = DiseaseCode.GetDiseaseCodeByID(twoDiseaseCode);
                }
                var medIdList = new List<string>();
                for (var x = 14; x < result.Length - 1; x += 5)
                {
                    if (result[x].Equals(string.Empty)) continue;
                    var id = result[x];
                    medIdList.Add(id);
                }

                var table = MedicineDb.GetMedicinesBySearchIds(medIdList, p.WareHouse?.ID, p.AdjustDate);
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    Medicine medicine;
                    switch (table.Rows[i].Field<int>("DataType"))
                    {
                        case 1:
                            medicine = new NewClass.MedicineRefactoring.MedicineNHI(table.Rows[i]);
                            p.Medicines.Add(medicine);
                            break;
                        case 2:
                            medicine = new NewClass.MedicineRefactoring.MedicineOTC(table.Rows[i]);
                            p.Medicines.Add(medicine);
                            break;
                        case 3:
                            medicine = new NewClass.MedicineRefactoring.MedicineSpecialMaterial(table.Rows[i]);
                            p.Medicines.Add(medicine);
                            break;
                    }
                }

                var j = 0;
                for (var x = 14; x < result.Length - 1; x += 5)
                {
                    if (result[x].Equals(string.Empty)) continue;

                    var amount = double.Parse(result[x + 1].Replace("+", ""));
                    var frequency = result[x + 2];
                    var way = result[x + 3];
                    var total = double.Parse(result[x + 4].Replace("+", ""));
                    p.Medicines[j].Dosage = amount;
                    p.Medicines[j].UsageName = frequency;
                    if (way.Contains("AC"))
                    {
                        p.Medicines[j].UsageName += "AC";
                        way = way.Replace("AC", "").Replace("/", "").Replace(" ", "").Replace(",", "");
                    }
                    if (way.Contains("PC"))
                    {
                        p.Medicines[j].UsageName += "PC";
                        way = way.Replace("PC", "").Replace("/", "").Replace(" ", "").Replace(",", "");
                    }
                    p.Medicines[j].PositionID = way;
                    p.Medicines[j].Amount = total;
                    j++;
                }
                Close();
                Messenger.Default.Send(new NotificationMessage<Prescription>(this, p, "CustomerPrescriptionSelected"));
            }
        }
    }
}
