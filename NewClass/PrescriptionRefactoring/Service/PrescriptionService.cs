using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine.MedBag;
using His_Pos.NewClass.StoreOrder;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.SameDeclareConfirmWindow;
using Microsoft.Reporting.WinForms;
using Employee = His_Pos.NewClass.Person.Employee.Employee;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using HisAPI = His_Pos.HisApi.HisApiFunction;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public abstract class PrescriptionService:ObservableObject
    {
        #region AbstractFunctions
        public abstract bool CheckPrescription(bool noCard);
        public abstract bool NormalAdjust();
        public abstract void ErrorAdjust();
        public abstract void DepositAdjust();
        public abstract bool Register();
        public abstract void PrescribeAdjust();
        #endregion
        public PrescriptionService()
        {

        }

        protected PrescriptionService(Prescription p)
        {
            current = p;
            printResult = new List<bool?>();
        }
        protected Prescription current { get; set; }
        protected Prescription tempPre { get; set; }
        protected List<bool?> printResult { get; set; }
        #region Functions
        public static PrescriptionService CreateService(Prescription p)
        {
            var ps = PrescriptionServiceProvider.CreateService(p.Type);
            ps.current = p;
            return ps;
        }

        protected bool CheckSameDeclare()
        {
            if (current.IsPrescribe) return true;
            var table = PrescriptionDb.CheckSameDeclarePrescription(current);
            if (table.Rows.Count > 0)
            {
                var pres = new SameDeclarePrescriptions.SameDeclarePrescriptions();
                pres.AddItems(table);
                var window = new SameDeclareConfirmWindow(pres);
                var result = window.DialogResult;
                return !(result is null) && (bool)result;
            }
            return true;
        }

        public bool StartNormalAdjust()
        {
            MainWindow.ServerConnection.OpenConnection();
            var result = NormalAdjust();
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public void StartErrorAdjust()
        {
            MainWindow.ServerConnection.OpenConnection();
            ErrorAdjust();
            MainWindow.ServerConnection.CloseConnection();
        }

        public void StartDepositAdjust()
        {
            MainWindow.ServerConnection.OpenConnection();
            DepositAdjust();
            MainWindow.ServerConnection.CloseConnection();
        }

        public bool StartRegister()
        {
            MainWindow.ServerConnection.OpenConnection();
            var result = Register();
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public void StartPrescribeAdjust()
        {
            MainWindow.ServerConnection.OpenConnection();
            PrescribeAdjust();
            MainWindow.ServerConnection.CloseConnection();
        }

        public bool SetPharmacist(Employee selectedPharmacist,int prescriptionCount)
        {
            if (selectedPharmacist is null || string.IsNullOrEmpty(selectedPharmacist.IDNumber))
            {
                MessageWindow.ShowMessage(Resources.尚未選擇藥師, MessageType.ERROR);
                return false;
            }
            if (prescriptionCount >= 80)
            {
                var confirm = new ConfirmWindow(Resources.調劑張數提醒 + prescriptionCount + "張，是否繼續調劑?", "調劑張數提醒", true);
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                var result = (bool)confirm.DialogResult;
                if (!result) return false;
            }
            current.Pharmacist = selectedPharmacist;
            return true;
        }
        public void SetCard(IcCard c)
        {
            current.Card = c;
        }

        protected void CheckAnonymousPatient()
        {
            if (!current.IsPrescribe) return;
            if (!current.Patient.CheckData())
            {
                var confirm = new ConfirmWindow("尚未選擇客戶或資料不全，是否以匿名取代?", "");
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                if ((bool)confirm.DialogResult)
                    current.Patient = Customer.GetCustomerByCusId(0);
                current.SetPrescribeAdjustCase();
            }
            current.SetPrescribeAdjustCase();
        }

        protected bool CheckValidCustomer()
        {
            if (current.Patient.CheckData()) return true;
            MessageWindow.ShowMessage("尚未選擇客戶", MessageType.ERROR);
            return false;
        }

        protected bool CheckMedicines()
        {
            var errorMsg = current.Medicines.Check();
            if (string.IsNullOrEmpty(errorMsg)) return true;
            MessageWindow.ShowMessage(errorMsg, MessageType.WARNING);
            return false;
        }

        protected bool CheckMedicalNumber()
        {
            if (string.IsNullOrEmpty(current.TempMedicalNumber))
            {
                var medicalNumberEmptyConfirm = new ConfirmWindow("就醫序號尚未填寫，確認繼續?(\"否\"返回填寫，\"是\"繼續調劑)?", "卡序確認");
                Debug.Assert(medicalNumberEmptyConfirm.DialogResult != null, "medicalNumberEmptyConfirm.DialogResult != null");
                return (bool)medicalNumberEmptyConfirm.DialogResult;
            }

            if (current.TempMedicalNumber.Length != 4)
            {
                MessageWindow.ShowMessage("就醫序號長度錯誤，應為4碼", MessageType.ERROR);
                return false;
            }
            return true;
        }

        protected bool CheckAdjustAndTreatDate()
        {
            return CheckTreatDate() && CheckAdjustDate();
        }

        private bool CheckTreatDate()
        {
            if (!(current.TreatDate is null) || current.AdjustCase.ID.Equals("D")) return true;
            MessageWindow.ShowMessage(Resources.TreatDateError, MessageType.WARNING);
            return false;
        }

        private bool CheckAdjustDate()
        {
            if (current.AdjustDate is null)
            {
                MessageWindow.ShowMessage(Resources.AdjustDateError, MessageType.WARNING);
                return false;
            }
            var startDate = (DateTime)current.TreatDate;
            var endDate = (DateTime)current.AdjustDate;
            if (DateTimeExtensions.CountTimeDifferenceWithoutHoliday(startDate, endDate) > 3)
            {
                var adjustDateOutOfRange = new ConfirmWindow(Resources.PrescriptoinOutOfDate, "");
                Debug.Assert(adjustDateOutOfRange.DialogResult != null, "adjustDateOutOfRange.DialogResult != null");
                return (bool)adjustDateOutOfRange.DialogResult;
            }
            return true;
        }

        protected bool CheckNhiRules(bool noCard)
        {
            if (current.IsPrescribe) return true;
            var error = current.CheckPrescriptionRule(noCard);//檢查健保規則
            if (string.IsNullOrEmpty(error)) return true;
            MessageWindow.ShowMessage(error, MessageType.ERROR);
            return false;
        }

        protected bool CheckPrescribeRules()
        {
            if (!current.IsPrescribe) return true;
            var error = current.CheckPrescribeRule();
            if (string.IsNullOrEmpty(error)) return true;
            MessageWindow.ShowMessage(error, MessageType.ERROR);
            return false;
        }

        public bool PrintConfirm()
        {
            printResult = NewFunction.CheckPrint(current);
            var printMedBag = printResult[0];
            var printSingle = printResult[1];
            var printReceipt = printResult[2];
            if (printMedBag is null || printReceipt is null)
                return false;
            if ((bool)printMedBag && printSingle is null)
                return false;
            tempPre = (Prescription)current.Clone();
            return true;
        }

        protected void SavePatientData()
        {
            if (current.Patient.ID == 0 || current.Patient.ID == -1) return;
            MainWindow.ServerConnection.OpenConnection();
            current.Patient.Save();
            MainWindow.ServerConnection.CloseConnection();
        }

        public void CheckDailyUpload(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            if (current.IsPrescribe) return;
            if ((bool)current.PrescriptionStatus.IsCreateSign)
                HisAPI.CreatDailyUploadData(current, false);
            else
                HisAPI.CreatErrorDailyUploadData(current, false, errorCode);
        }

        public static IEnumerable<ReportParameter> CreateSingleMedBagParameter(MedBagMedicine m,Prescription p)
        {
            var treatmentDate = DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                   treatmentDate.Split('/')[2] + "日";
            var cusGender = p.Patient.CheckGender();
            string patientTel;
            if (!string.IsNullOrEmpty(p.Patient.CellPhone))
                patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.CellPhone : p.Patient.CellPhone + "(註)";
            else
            {
                if (!string.IsNullOrEmpty(p.Patient.Tel))
                    patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.Tel : p.Patient.Tel + "(註)";
                else
                    patientTel = p.Patient.ContactNote;
            }
            return new List<ReportParameter>
                    {
                        new ReportParameter("PharmacyName_Id",
                            VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.ID + ")"),
                        new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                        new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                        new ReportParameter("MedicalPerson",VM.CurrentPharmacy.GetPharmacist().Name),
                        new ReportParameter("PatientName", p.Patient.Name),
                        new ReportParameter("PatientGender_Birthday",(cusGender) + "/" + DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true)),
                        new ReportParameter("TreatmentDate", treatmentDateChi),
                        new ReportParameter("RecId", " "), //病歷號
                        new ReportParameter("Division",p.Division is null ?string.Empty:p.Division.Name),
                        new ReportParameter("Hospital", p.Institution.Name),
                        new ReportParameter("PaySelf", p.PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("ServicePoint", p.PrescriptionPoint.MedicalServicePoint.ToString()),
                        new ReportParameter("TotalPoint", p.PrescriptionPoint.TotalPoint.ToString()),
                        new ReportParameter("CopaymentPoint", p.PrescriptionPoint.CopaymentPointPayable.ToString()),
                        new ReportParameter("HcPoint", p.PrescriptionPoint.ApplyPoint.ToString()),
                        new ReportParameter("MedicinePoint", p.PrescriptionPoint.MedicinePoint.ToString(CultureInfo.InvariantCulture)),
                        new ReportParameter("MedicineId", m.Id),
                        new ReportParameter("MedicineName", m.Name),
                        new ReportParameter("MedicineChineseName", m.ChiName),
                        new ReportParameter("Ingredient", m.Ingredient),
                        new ReportParameter("Indication", m.Indication),
                        new ReportParameter("SideEffect", m.SideEffect),
                        new ReportParameter("Note", m.Note),
                        new ReportParameter("Usage", m.Usage),
                        new ReportParameter("MedicineDay", m.MedicineDays),
                        new ReportParameter("Amount", m.Total),
                        new ReportParameter("Form", m.Form),
                        new ReportParameter("PatientTel", patientTel)
                    };
        }
        public static IEnumerable<ReportParameter> CreateMultiMedBagParameter(Prescription p)
        {
            var treatmentDate =
                DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var treatmentDateChi = string.Empty;
            if (!string.IsNullOrEmpty(treatmentDate))
                treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                      treatmentDate.Split('/')[2] + "日";
            var cusGender = p.Patient.CheckGender();
            string patientTel;
            if (!string.IsNullOrEmpty(p.Patient.CellPhone))
                patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.CellPhone : p.Patient.CellPhone + "(註)";
            else
            {
                if (!string.IsNullOrEmpty(p.Patient.Tel))
                    patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.Tel : p.Patient.Tel + "(註)";
                else
                    patientTel = p.Patient.ContactNote;
            }
            return new List<ReportParameter>
            {
                new ReportParameter("PharmacyName_Id",
                    VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.ID + ")"),
                new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                new ReportParameter("MedicalPerson", VM.CurrentPharmacy.GetPharmacist().Name),
                new ReportParameter("PatientName", p.Patient.Name),
                new ReportParameter("PatientGender_Birthday",cusGender + "/" +DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true)),
                new ReportParameter("TreatmentDate", treatmentDateChi),
                new ReportParameter("Hospital", p.Institution.Name),
                new ReportParameter("PaySelf", p.PrescriptionPoint.AmountSelfPay.ToString()),
                new ReportParameter("ServicePoint", p.PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalPoint", p.PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentPoint",p.PrescriptionPoint.CopaymentPointPayable.ToString()),
                new ReportParameter("HcPoint", p.PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("MedicinePoint", p.PrescriptionPoint.MedicinePoint.ToString()),
                new ReportParameter("Division", p.Division is null ?string.Empty:p.Division.Name),
                new ReportParameter("PatientTel", patientTel)
            };
        }
        #endregion

        public static IEnumerable<ReportParameter> CreateDepositSheetParameters(Prescription p)
        {
            var adjustDate =
                DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var dateString = DateTimeExtensions.ConvertDateStringSplitToChinese(adjustDate);
            var printTime = adjustDate + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
            return new List<ReportParameter>
            {
                new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                new ReportParameter("PatientName", p.Patient.Name),
                new ReportParameter("AdjustDate", dateString),
                new ReportParameter("Deposit", p.PrescriptionPoint.Deposit.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(p.PrescriptionPoint.Deposit)),
                new ReportParameter("PrintTime", printTime)
            };
        }

        public static IEnumerable<ReportParameter> CreateReceiptParameters(Prescription p)
        {
            var adjustDate = DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var cusGender = p.Patient.CheckGender();
            var copaymentPoint = p.PrescriptionPoint.CopaymentPoint;
            var actualReceive = p.PrescriptionPoint.ActualReceive;
            var birth = DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true);
            if (p.PrescriptionStatus.IsVIP)
            {
                copaymentPoint = 0;
                actualReceive = p.PrescriptionPoint.ActualReceive - p.PrescriptionPoint.CopaymentPoint;
            }
            string patientName;
            if (string.IsNullOrEmpty(p.Patient.Name) || p.Patient.Name.Equals("匿名"))
                patientName = " ";
            else
                patientName = p.Patient.Name;
            if(string.IsNullOrEmpty(birth))
                birth = "  /  /  ";
            if (p.AdjustCase.ID.Equals("0"))
            {
                return new List<ReportParameter>
                    {
                        new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                        new ReportParameter("PatientName", patientName),
                        new ReportParameter("Gender", cusGender),
                        new ReportParameter("Birthday",birth),
                        new ReportParameter("AdjustDate", adjustDate),
                        new ReportParameter("Hospital", p.Institution.Name),
                        new ReportParameter("Doctor", " "), //病歷號
                        new ReportParameter("MedicalNumber"," "),
                        new ReportParameter("MedicineCost", p.PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("MedicalServiceCost", (p.PrescriptionPoint.AmountsPay - p.PrescriptionPoint.AmountSelfPay).ToString()),
                        new ReportParameter("TotalMedicalCost","0"),
                        new ReportParameter("CopaymentCost", "0"),
                        new ReportParameter("HcPay", "0"),
                        new ReportParameter("SelfCost", p.PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("ActualReceive", p.PrescriptionPoint.ActualReceive.ToString()),
                        new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(p.PrescriptionPoint.ActualReceive))
                    };
            }
            return new List<ReportParameter>
            {
                new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                new ReportParameter("PatientName", patientName),
                new ReportParameter("Gender", cusGender),
                new ReportParameter("Birthday",birth),
                new ReportParameter("AdjustDate", adjustDate),
                new ReportParameter("Hospital", string.IsNullOrEmpty(p.Institution.Name)?" ":p.Institution.Name),
                new ReportParameter("Doctor", " "), //病歷號
                new ReportParameter("MedicalNumber", string.IsNullOrEmpty(p.TempMedicalNumber)?" ":p.TempMedicalNumber),
                new ReportParameter("MedicineCost", p.PrescriptionPoint.MedicinePoint.ToString()),
                new ReportParameter("MedicalServiceCost", p.PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalMedicalCost",p.PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentCost", copaymentPoint.ToString()),
                new ReportParameter("HcPay", p.PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("SelfCost", p.PrescriptionPoint.AmountSelfPay.ToString()),
                new ReportParameter("ActualReceive", actualReceive.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(actualReceive))
            };
        }

        public void CreateDailyUploadData(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            if (current.PrescriptionStatus.IsGetCard || errorCode != null)
            {
                if (current.Card.IsGetMedicalNumber)
                    CreatePrescriptionSign();
                else
                    current.PrescriptionStatus.IsCreateSign = false;
            }
            else
                current.PrescriptionStatus.IsDeclare = false;
        }

        private void CreatePrescriptionSign()
        {
            current.PrescriptionSign = HisAPI.WritePrescriptionData(current);
            if (current.WriteCardSuccess != 0)
            {
                Application.Current.Dispatcher.Invoke(delegate {
                    var description = MainWindow.GetEnumDescription((ErrorCode)current.WriteCardSuccess);
                    MessageWindow.ShowMessage("寫卡異常 " + current.WriteCardSuccess + ":" + description, MessageType.WARNING);
                });
                current.PrescriptionStatus.IsCreateSign = null;
            }
            else
                current.PrescriptionStatus.IsCreateSign = true;
        }

        public void Print(bool noCard)
        {
            PrintMedBag();
            PrintReceipt(noCard);
        }

        private void PrintMedBag()
        {
            var printMedBag = (bool)printResult[0];
            if (printMedBag)
                CheckMedBagPrintMode();
        }

        private void PrintReceipt(bool noCard)
        {
            // ReSharper disable once PossibleInvalidOperationException
            var printReceipt = (bool)printResult[2];
            if (printReceipt)
                tempPre.PrintReceipt();
            if (noCard)
                tempPre.PrintDepositSheet();
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private void CheckMedBagPrintMode()
        {
            var singleMode = (bool)printResult[1];
            if (singleMode)
                tempPre.PrintMedBagSingleMode();
            else
                tempPre.PrintMedBagMultiMode();
        }

        protected bool CheckChronicRegister()
        {
            if (current.AdjustCase.IsChronic()) return true;
            MessageWindow.ShowMessage("一般箋處方不可登錄", MessageType.ERROR);
            return false;
        }

        protected void SendOrder(MedicinesSendSingdeViewModel vm)
        {
            if (current.PrescriptionStatus.IsSendOrder)
            {
                var sendData = vm.PrescriptionSendData;
                if (!current.PrescriptionStatus.IsSendToSingde)
                    current.PrescriptionStatus.IsSendToSingde = PurchaseOrder.InsertPrescriptionOrder(current, sendData);
                //紀錄訂單and送單
                else if (current.PrescriptionStatus.IsSendToSingde)
                {
                    PurchaseOrder.UpdatePrescriptionOrder(current, sendData);
                } //更新傳送藥健康
            }
            current.PrescriptionStatus.UpdateStatus(current.ID);
        }

        public void SetMedicalNumberByErrorCode(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            current.TempMedicalNumber = errorCode.ID;
            if (current.AdjustCase.ID.Equals("2") && current.ChronicSeq > 1)
            {
                current.MedicalNumber = "IC0" + current.ChronicSeq;
                current.OriginalMedicalNumber = errorCode.ID;
            }
            else
            {
                current.MedicalNumber = errorCode.ID;
            }
        }

        public void MakeUpComplete()
        {
            var deposit = current.PrescriptionPoint.Deposit;
            MainWindow.ServerConnection.OpenConnection();
            current.PrescriptionPoint.Deposit = 0;
            current.PrescriptionStatus.SetNormalAdjustStatus();
            current.Update();
            MainWindow.ServerConnection.CloseConnection();
            Application.Current.Dispatcher.Invoke((Action)delegate {
                MessageWindow.ShowMessage("補卡作業成功，退還押金" + deposit + "元", MessageType.SUCCESS);
            });
        }

        public void CheckDailyUploadMakeUp(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            if (current.IsPrescribe) return;
            if ((bool)current.PrescriptionStatus.IsCreateSign)
                HisAPI.CreatDailyUploadData(current, true);
            else
                HisAPI.CreatErrorDailyUploadData(current, true, errorCode);
        }

        public void SetMedicalNumber()
        {
            if (current.AdjustCase.ID.Equals("2") && current.ChronicSeq > 1)
            {
                current.MedicalNumber = "IC0" + current.ChronicSeq;
                current.OriginalMedicalNumber = current.TempMedicalNumber;
            }
            else
            {
                current.MedicalNumber = current.TempMedicalNumber;
            }
        }
    }
}
