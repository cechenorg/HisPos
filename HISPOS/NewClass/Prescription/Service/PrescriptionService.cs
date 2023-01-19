using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Medicine.MedBag;
using His_Pos.NewClass.Medicine.ReserveMedicine;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.ICCard;
using His_Pos.NewClass.Product.PrescriptionSendData;
using His_Pos.NewClass.StoreOrder;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.SameDeclareConfirmWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Employee = His_Pos.NewClass.Person.Employee.Employee;
using HisAPI = His_Pos.HisApi.HisApiFunction;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription.Service
{
    public abstract class PrescriptionService : ObservableObject
    {
        #region AbstractFunctions

        public abstract bool CheckPrescription(bool noCard, bool errorAdjust);

        public abstract bool CheckPrescriptionBeforeOrder(bool noCard, bool errorAdjust);

        public abstract bool CheckEditPrescription(bool hasCard);

        public abstract bool NormalAdjust();

        public abstract bool ErrorAdjust();

        public abstract bool DepositAdjust();

        public abstract bool Register();

        public abstract bool PrescribeAdjust();

        public abstract bool CheckCustomerSelected();

        #endregion AbstractFunctions

        public PrescriptionService()
        {
        }

        protected MedicinesSendSingdeViewModel vm { get; set; } = null;

        protected IEnumerable<Prescription> PrescriptionList { get; set; }
        protected Prescription Current { get; set; }
        protected Prescription TempPre { get; set; }
        protected Prescription TempPrint { get; set; }
        protected List<bool?> PrintResult { get; set; }

        #region Functions

        public static PrescriptionService CreateService(Prescription p)
        {
            var ps = PrescriptionServiceProvider.CreateService(p.Type);
            ps.Current = p;
            return ps;
        }

        protected bool CheckSameDeclare()
        {
            if (Current.IsPrescribe) return true;
            var table = PrescriptionDb.CheckSameDeclarePrescription(Current);
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

        public bool StartErrorAdjust()
        {
            MainWindow.ServerConnection.OpenConnection();
            var result = ErrorAdjust();
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public bool StartDepositAdjust()
        {
            MainWindow.ServerConnection.OpenConnection();
            var result = DepositAdjust();
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public bool StartRegister()
        {
            MainWindow.SingdeConnection.OpenConnection();
            MainWindow.ServerConnection.OpenConnection();
            var result = Register();
            MainWindow.ServerConnection.CloseConnection();
            MainWindow.SingdeConnection.CloseConnection();
            return result;
        }

        public bool StartPrescribeAdjust()
        {
            MainWindow.ServerConnection.OpenConnection();
            var result = PrescribeAdjust();
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public bool SetPharmacist(Employee selectedPharmacist, int prescriptionCount)
        {
            if (selectedPharmacist is null || string.IsNullOrEmpty(selectedPharmacist.IDNumber))
            {
                MessageWindow.ShowMessage(Resources.尚未選擇藥師, MessageType.ERROR);
                return false;
            }
            if (prescriptionCount >= 80)
            {
                var confirmMsg = Resources.調劑張數提醒 + prescriptionCount + "張，是否繼續調劑?";
                var confirm = new ConfirmWindow(confirmMsg, "調劑張數提醒", true);
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                var result = (bool)confirm.DialogResult;
                if (!result) return false;
            }
            Current.Pharmacist = selectedPharmacist;
            return true;
        }

        public bool SetRegisterPharmacist(Employee selectedPharmacist, int prescriptionCount)
        {
            if (selectedPharmacist is null || string.IsNullOrEmpty(selectedPharmacist.IDNumber))
            {
                MessageWindow.ShowMessage(Resources.尚未選擇藥師, MessageType.ERROR);
                return false;
            }
            /*if (prescriptionCount >= 80)
            {
                var confirmMsg = Resources.調劑張數提醒 + prescriptionCount + "張，是否繼續調劑?";
                var confirm = new ConfirmWindow(confirmMsg, "調劑張數提醒", true);
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                var result = (bool)confirm.DialogResult;
                if (!result) return false;
            }*/
            Current.Pharmacist = selectedPharmacist;
            return true;
        }

        public void SetPharmacistWithoutCheckCount(Employee selectedPharmacist)
        {
            Current.Pharmacist = selectedPharmacist;
        }

        public void SetCard(IcCard c)
        {
            Current.Card = c;
        }

        protected bool CheckAnonymousPatient()
        {
            if (!Current.IsPrescribe) return true;
            if (Current.Patient.ID > 0) return true;
            //Current.SetPrescribeAdjustCase();
            if (!Current.Patient.CheckData() && !Current.Patient.IsAnonymous())
            {
                if (Current.AdjustCase.ID == "0")
                {
                    Current.Patient = Customer.GetCustomerByCusId(0);
                    return true;
                }
                else
                {
                    var confirm = new ConfirmWindow("尚未選擇客戶，是否以匿名取代?", "資料格式錯誤或資料不完整");
                    Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                    if ((bool)confirm.DialogResult)
                    {
                        Current.Patient = Customer.GetCustomerByCusId(0);
                        return true;
                    }
                }

                return false;
            }
            return true;
        }

        protected bool CheckValidCustomer()
        {
            if (Current.Patient.CheckData())
            {
                if (Current.Patient.ID > 0) return true;
                var insertResult = Current.Patient.InsertData();
                return insertResult;
            }
            if (!string.IsNullOrEmpty(Current.Patient.Name) && Current.Patient.Name.Equals("匿名") || Current.Patient.ID > 0) return true;
            MessageWindow.ShowMessage("尚未選擇客戶", MessageType.ERROR);
            return false;
        }

        protected bool CheckMedicines()
        {
            var errorMsg = Current.CheckMedicinesRule();
            if (string.IsNullOrEmpty(errorMsg)) return true;
            MessageWindow.ShowMessage(errorMsg, MessageType.WARNING);
            return false;
        }

        protected bool CheckMedicalNumber()
        {
            if (string.IsNullOrEmpty(Current.TempMedicalNumber))
            {
                var medicalNumberEmptyConfirm = new ConfirmWindow("就醫序號尚未填寫，確認繼續?(\"否\"返回填寫，\"是\"繼續調劑)?", "卡序確認");
                Debug.Assert(medicalNumberEmptyConfirm.DialogResult != null, "medicalNumberEmptyConfirm.DialogResult != null");
                return (bool)medicalNumberEmptyConfirm.DialogResult;
            }
            return Current.TempMedicalNumber.Length != 4 ? NewFunction.CheckHomeCareMedicalNumber(Current.TempMedicalNumber, Current.AdjustCase) : NewFunction.CheckNotIntMedicalNumber(Current.TempMedicalNumber, Current.AdjustCase.ID, Current.ChronicSeq);
        }

        protected bool CheckAdjustAndTreatDate()
        {
            //if (notCheckPast10Days)
            //    return CheckTreatDate() && CheckAdjustDate();
            //return CheckTreatDate() && CheckAdjustDate() && CheckAdjustDatePast10Days();
            return CheckTreatDate() && CheckTreatDateValid() && CheckAdjustDate() && CheckAdjustDatePast() /*&& CheckAdjustDateFutureOutOfRange()*/;
        }

        protected bool CheckAdjustAndTreatDateFromEdit()
        {
            return CheckTreatDate() && CheckAdjustDate();
        }

        private bool CheckTreatDate()
        {
            switch (Current.TreatDate)
            {
                case null when Current.AdjustCase.ID.Equals("D"):
                    return true;

                case null:
                    MessageWindow.ShowMessage(Resources.TreatDateError, MessageType.WARNING);
                    return false;

                default:
                    return true;
            }
        }

        private bool CheckTreatDateValid()
        {
            if (DateTime.Compare((DateTime)Current.TreatDate, DateTime.Today) >= 0) return true;
            var ts1 = new TimeSpan(DateTime.Today.Ticks);
            var ts2 = new TimeSpan(((DateTime)Current.TreatDate).Ticks);
            var ts = ts1.Subtract(ts2).Duration();
            if (ts.Days < 180) return true;
            MessageWindow.ShowMessage("就醫日不在合理範圍，請確認資料。", MessageType.WARNING);
            return false;
        }

        private bool CheckAdjustDate()
        {
            if (Current.AdjustDate is null)
            {
                MessageWindow.ShowMessage(Resources.AdjustDateError, MessageType.WARNING);
                return false;
            }
            if (Current.AdjustCase.IsChronic()) return true;
            if (DateTime.Compare((DateTime)Current.AdjustDate, DateTime.Today) > 0)
            {
                MessageWindow.ShowMessage("非登錄慢箋調劑日不可超過今天", MessageType.WARNING);
                return false;
            }
            Debug.Assert(Current.TreatDate != null, "Current.TreatDate != null");
            var startDate = (DateTime)Current.TreatDate;
            var endDate = (DateTime)Current.AdjustDate;
            if (DateTimeExtensions.CountTimeDifferenceWithoutHoliday(startDate, endDate) > 3)
            {
                var adjustDateOutOfRange = new ConfirmWindow(Resources.PrescriptoinOutOfDate, "");
                Debug.Assert(adjustDateOutOfRange.DialogResult != null, "adjustDateOutOfRange.DialogResult != null");
                return (bool)adjustDateOutOfRange.DialogResult;
            }
            if (Current.ChronicSeq != null)
            {
                bool result = Current.CheckChronicAdjustDateValid();
            }
            return true;
        }

        private bool CheckAdjustDatePast()
        {
            if (Current.AdjustDate < Current.TreatDate)
            {
                MessageWindow.ShowMessage("調劑日不可小於就醫日", MessageType.WARNING);
                return false;
            }

            if (Current.AdjustDate >= DateTime.Today || VM.CurrentUser.ID == 1 || VM.CurrentUser.Authority == DomainModel.Enum.Authority.MasterPharmacist || VM.CurrentUser.Authority == DomainModel.Enum.Authority.PharmacyManager) return true;
            MessageWindow.ShowMessage("調劑日不可小於今天", MessageType.WARNING);
            return false;
        }

        private bool CheckAdjustDateFutureOutOfRange()
        {
            var startDate = (DateTime)Current.TreatDate;
            var endDate = (DateTime)Current.AdjustDate;
            var ts1 = new TimeSpan(endDate.Ticks);
            var ts2 = new TimeSpan(startDate.Ticks);
            var ts = ts1.Subtract(ts2).Duration();
            if (ts.Days < 180) return true;
            MessageWindow.ShowMessage("調劑日超出合理範圍", MessageType.WARNING);
            return false;
        }

        private bool CheckAdjustDatePast10Days()
        {
            if (DateTime.Compare(((DateTime)Current.AdjustDate).Date, DateTime.Today) >= 0) return true;
            var timeDiff = new TimeSpan(DateTime.Today.Ticks - ((DateTime)Current.AdjustDate).Ticks).TotalDays;
            if (timeDiff > 10)
            {
                MessageWindow.ShowMessage("處方調劑日已超過可過卡日(10日)，處方會被核刪，若是慢箋將影響病人下次看診領藥。如需以目前調劑日申報此處方或請使用異常結案或將調劑日改為今日以前十日內。", MessageType.ERROR);
                return false;
            }
            return true;
        }

        protected bool CheckNhiRules(bool noCard)
        {
            if (Current.IsPrescribe) return true;
            var error = Current.CheckPrescriptionRule(noCard);//檢查健保規則
            if (string.IsNullOrEmpty(error)) return true;
            MessageWindow.ShowMessage(error, MessageType.ERROR);
            return false;
        }

        protected bool CheckPrescribeRules()
        {
            if (!Current.IsPrescribe) return true;
            var error = Current.CheckPrescribeRule();
            if (string.IsNullOrEmpty(error)) return true;
            MessageWindow.ShowMessage(error, MessageType.ERROR);
            return false;
        }

        public bool PrintConfirm(bool manualPrint = false)
        {
            bool? focus = null;
            bool isSend = false;
            if (vm?.PrescriptionSendData != null)
            {
                var printSendData = vm.PrescriptionSendData;
                var allSendCount = printSendData.Count(p => p.SendAmount == p.TreatAmount);//全傳送
                var allPrepareCount = printSendData.Count(p => p.SendAmount == 0);
                if (printSendData.Count > allPrepareCount)
                    isSend=true;
                if (printSendData.Count == allSendCount)
                    focus = false;
                else if (printSendData.Count == allPrepareCount)
                    focus = true;
            }
            PrintResult = NewFunction.CheckPrint(Current, focus, isSend, manualPrint);
            var printMedBag = PrintResult[0];//是否印藥袋
            var printSingle = PrintResult[1];//是否多藥一袋
            var printReceipt = PrintResult[2];//是否印收據
            if (printMedBag is null || printReceipt is null)
                return false;
            if ((bool)printMedBag && printSingle is null)
                return false;
            return true;
        }

        public bool PrintConfirmDir()
        {
            bool? focus = null;
  
            PrintResult = NewFunction.CheckPrintDir(Current, focus);
            var printMedBag = PrintResult[0];
            var printSingle = PrintResult[1];
            var printReceipt = PrintResult[2];
            if (printMedBag is null || printReceipt is null)
                return false;
            if ((bool)printMedBag && printSingle is null)
                return false;
            return true;
        }

        public void CheckDailyUpload(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            if (Current.IsPrescribe) return;
            Debug.Assert(Current.PrescriptionStatus.IsCreateSign != null, "Current.PrescriptionStatus.IsCreateSign != null");
            if ((bool)Current.PrescriptionStatus.IsCreateSign)
                HisAPI.CreatDailyUploadData(Current, false);
            else
                HisAPI.CreatErrorDailyUploadData(Current, false, errorCode);
        }

        public static string BuildPatientTel(Prescription p)
        {
            string patientTel = string.Empty;
            if (!string.IsNullOrEmpty(p.Patient.CellPhone))
            {
                patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.CellPhone : p.Patient.CellPhone + "(" + p.Patient.ContactNote + ")";
                patientTel = string.IsNullOrEmpty(p.Patient.Line) ? patientTel : "@" + patientTel;
            }
            else
            {
                patientTel = string.IsNullOrEmpty(p.Patient.Tel) ? string.Empty : p.Patient.Tel;
                patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? patientTel : patientTel + "(" + p.Patient.ContactNote + ")";
                patientTel = string.IsNullOrEmpty(p.Patient.Line) ? patientTel : "@" + patientTel;
            }
            return patientTel;
        }
        public static IEnumerable<ReportParameter> CreateSingleMedBagParameter(MedBagMedicine m, Prescription p, string orderNumber, int medDays)
        {
            var adjustDate = DateTimeExtensions.ConvertToTaiwanCalendarChineseFormat(p.AdjustDate, true);
            var adjustDateNext = string.Empty;
            var treatReturn = string.Empty;
            if (p.CheckChronicSeqValid() && p.CheckChronicTotalValid())
            {
                if (p.ChronicSeq < p.ChronicTotal)
                {
                    var nextAdjust = ((DateTime)p.AdjustDate).AddDays(medDays);
                    adjustDateNext = DateTimeExtensions.ConvertToTaiwanCalendarChineseFormat(nextAdjust, true);
                }
                DateTime? treatReturnDate = ((DateTime)p.TreatDate).AddDays((medDays * (int)p.ChronicTotal));
                treatReturn = DateTimeExtensions.ConvertToTaiwanCalendarChineseFormat(treatReturnDate, true);
            }
            var cusGender = p.Patient.CheckGender();
            string patientTel = BuildPatientTel(p);
            return new List<ReportParameter>
                    {
                        new ReportParameter("OrderNumber",orderNumber),
                        new ReportParameter("PharmacyName_Id",$"{VM.CurrentPharmacy.Name}({VM.CurrentPharmacy.ID})"),
                        new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                        new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                        new ReportParameter("MedicalPerson",p.Pharmacist is null ?VM.CurrentPharmacy.GetPharmacist().Name:p.Pharmacist.Name),
                        new ReportParameter("PatientName", p.Patient.Name),
                        new ReportParameter("PatientGender_Birthday",$"{cusGender}/{DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true)}"),
                        new ReportParameter("AdjustDate", adjustDate),
                        new ReportParameter("AdjustDateNext", adjustDateNext),
                        new ReportParameter("TreatmentDateReturn", treatReturn),
                        new ReportParameter("RecId", " "), //病歷號
                        new ReportParameter("Division",p.Division is null ?string.Empty:p.Division.Name),
                        new ReportParameter("Hospital", p.Institution.Name),
                        new ReportParameter("PaySelf", (p.PrescriptionPoint.AmountSelfPay ?? 0).ToString()),
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
                        new ReportParameter("FreqRemark", m.FreqRemark),
                        new ReportParameter("MedicineDay", m.MedicineDays),
                        new ReportParameter("Amount", m.Total),
                        new ReportParameter("Form", m.Form),
                        new ReportParameter("PatientTel", patientTel),
                        new ReportParameter("ChronicTotal", p.ChronicTotal is null ? string.Empty : ((int)p.ChronicTotal).ToString()),
                        new ReportParameter("ChronicSeq", p.ChronicSeq is null ? string.Empty : ((int)p.ChronicSeq).ToString()),
                        new ReportParameter("AdjustMonth", p.AdjustMonth),
                        new ReportParameter("AdjustYear", p.AdjustYear),
                        new ReportParameter("AdjustDay", p.AdjustDay)
                    };
        }

        public static IEnumerable<ReportParameter> CreateMultiMedBagParameter(Prescription p)
        {
            var treatmentDate =
                DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var treatmentDateChi = string.Empty;
            if (!string.IsNullOrEmpty(treatmentDate))
            {
                var year = treatmentDate.Split('/')[0];
                var month = treatmentDate.Split('/')[1];
                var day = treatmentDate.Split('/')[2];
                treatmentDateChi = $"{year}年{month}月{day}日";
            }
            var cusGender = p.Patient.CheckGender();
            string patientTel = BuildPatientTel(p);
            return new List<ReportParameter>
            {
                new ReportParameter("PharmacyName_Id",$"{VM.CurrentPharmacy.Name}({VM.CurrentPharmacy.ID})"),
                new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                new ReportParameter("MedicalPerson",p.Pharmacist is null ?VM.CurrentPharmacy.GetPharmacist().Name:p.Pharmacist.Name),
                new ReportParameter("PatientName", p.Patient.Name),
                new ReportParameter("PatientGender_Birthday",$"{cusGender}/{DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true)}"),
                new ReportParameter("TreatmentDate", treatmentDateChi),
                new ReportParameter("Hospital", p.Institution.Name),
                new ReportParameter("PaySelf", (p.PrescriptionPoint.AmountSelfPay ?? 0).ToString()),
                new ReportParameter("ServicePoint", p.PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalPoint", p.PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentPoint",p.PrescriptionPoint.CopaymentPointPayable.ToString()),
                new ReportParameter("HcPoint", p.PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("MedicinePoint", p.PrescriptionPoint.MedicinePoint.ToString()),
                new ReportParameter("Division", p.Division is null ?string.Empty:p.Division.Name),
                new ReportParameter("PatientTel", patientTel)
            };
        }

        #endregion Functions

        public static IEnumerable<ReportParameter> CreateDepositSheetParameters(Prescription p)
        {
            var adjustDate =
                DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var dateString = DateTimeExtensions.ConvertDateStringSplitToChinese(adjustDate);
            var printTime = $"{adjustDate}({DateTime.Now.Hour}:{DateTime.Now.Minute})";
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
            var copaymentPoint = p.PrescriptionPoint.CopaymentPointPayable;
            var actualReceive = p.PrescriptionPoint.ActualReceive ?? 0;
            var birth = DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true);
            string patientName;
            if (string.IsNullOrEmpty(p.Patient.Name) || p.Patient.Name.Equals("匿名"))
                patientName = " ";
            else
                patientName = p.Patient.Name;
            if (string.IsNullOrEmpty(birth))
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
                        new ReportParameter("MedicineCost", (p.PrescriptionPoint.AmountSelfPay ?? 0).ToString()),
                        new ReportParameter("MedicalServiceCost", (p.PrescriptionPoint.AmountsPay - (p.PrescriptionPoint.AmountSelfPay ?? 0)).ToString()),
                        new ReportParameter("TotalMedicalCost","0"),
                        new ReportParameter("CopaymentCost", "0"),
                        new ReportParameter("HcPay", "0"),
                        new ReportParameter("SelfCost", (p.PrescriptionPoint.AmountSelfPay ?? 0).ToString()),
                        new ReportParameter("ActualReceive", (p.PrescriptionPoint.ActualReceive ?? 0).ToString()),
                        new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(p.PrescriptionPoint.ActualReceive ?? 0))
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
                new ReportParameter("SelfCost", (p.PrescriptionPoint.AmountSelfPay ?? 0).ToString()),
                new ReportParameter("ActualReceive", actualReceive.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(actualReceive))
            };
        }

        public void CreateDailyUploadData(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            if (Current.PrescriptionStatus.IsGetCard || errorCode != null)
            {
                if (Current.Card.IsGetMedicalNumber)
                    CreatePrescriptionSign();
                else
                    Current.PrescriptionStatus.IsCreateSign = false;
            }
            else
                Current.PrescriptionStatus.IsDeclare = false;
        }

        private void CreatePrescriptionSign()
        {
            Current.PrescriptionSign = HisAPI.WritePrescriptionData(Current);

            if (Current.WriteCardSuccess != 0)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var description = MainWindow.GetEnumDescription((ErrorCode)Current.WriteCardSuccess);
                    MessageWindow.ShowMessage("寫卡異常 " + Current.WriteCardSuccess + ":" + description, MessageType.WARNING);
                });
                Current.PrescriptionStatus.IsCreateSign = null;
            }
            else
                Current.PrescriptionStatus.IsCreateSign = true;
        }
        public static List<string> GetPrescriptionSign(Prescription p)
        {
            return HisAPI.WritePrescriptionData(p);
        }

        public void Print(bool noCard)
        {
            PrintMedBag();
            PrintReceipt(noCard);
        }
        public void PrintDir(bool noCard)
        {
            PrintMedBag();

        }



        private void PrintMedBag()
        {
            var printMedBag = (bool)PrintResult[0];
            if (printMedBag)
                CheckMedBagPrintMode();
        }

        private void PrintReceipt(bool noCard)
        {
            // ReSharper disable once PossibleInvalidOperationException
            var printReceipt = (bool)PrintResult[2];
            if (printReceipt)
                TempPre.PrintReceipt();
            if (noCard)
                TempPre.PrintDepositSheet();
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private void CheckMedBagPrintMode()
        {
            var reportFormat = Properties.Settings.Default.ReportFormat;
            if (TempPre.Institution != null)// && TempPre.Institution.ID == "3532082753"
            {
                TempPrint.Division.Name = "";
                var singleMode = (bool)PrintResult[1];
                if (singleMode)
                    TempPrint.PrintMedBagSingleMode();
                else
                    TempPrint.PrintMedBagMultiMode();
            }
            else if (reportFormat == MainWindow.GetEnumDescription((PrintFormat)0))
            {
                TempPre.PrintMedBagSingleModeByCE();
            }
            else
            {
                var singleMode = (bool)PrintResult[1];
                if (singleMode)
                    TempPre.PrintMedBagSingleMode();
                else
                    TempPre.PrintMedBagMultiMode();
            }
        }

        protected bool CheckChronicRegister()
        {
            if (Current.AdjustCase.IsChronic()) return true;
            MessageWindow.ShowMessage("一般箋處方不可登錄", MessageType.ERROR);
            return false;
        }

        public void SendOrder(MedicinesSendSingdeViewModel vm)
        {
            var printSendData = vm.PrescriptionSendData.DeepCloneViaJson();
            var tempPrintSendData = new PrescriptionSendDatas();
            tempPrintSendData.Clear();
            foreach (var printData in printSendData)
            {
                if (printData.IsCommon == false)
                    tempPrintSendData.Add(printData);
            }

            var sendData = vm.PrescriptionSendData;
            if (sendData.Count(s => s.SendAmount == 0) != sendData.Count)
            {
                if (!Current.PrescriptionStatus.IsSendToSingde)
                    Current.PrescriptionStatus.IsSendToSingde = PurchaseOrder.InsertPrescriptionOrder(Current, sendData);
                //紀錄訂單and送單
                else if (Current.PrescriptionStatus.IsSendToSingde)
                {
                    PurchaseOrder.UpdatePrescriptionOrder(Current, sendData);
                }//更新傳送藥健康
            }
            else
            {
                if (!string.IsNullOrEmpty(Current.OrderID))
                {
                    var removeSingdeOrder = StoreOrderDB.RemoveSingdeStoreOrderByID(Current.OrderID).Rows[0].Field<string>("RESULT").Equals("SUCCESS"); ;
                    if (!removeSingdeOrder)
                        NewFunction.ShowMessageFromDispatcher("處方訂單已出貨或網路異常，訂單更改失敗", MessageType.ERROR);
                    else
                    {
                        var dataTable = StoreOrderDB.RemoveStoreOrderToSingdeByID(Current.OrderID);
                        var removeLocalOrder = dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
                        if (!removeLocalOrder)
                            NewFunction.ShowMessageFromDispatcher("處方訂單更改失敗", MessageType.ERROR);
                    }
                }
            }
            //var selfcoSendCount = printSendData.Count(p => p.SendAmount > 0 && p.SendAmount < p.TreatAmount); //部分傳送
            //var selfallSendCount = printSendData.Count(p => p.SendAmount == p.TreatAmount);//全傳送

            var selfPrepareAmount = tempPrintSendData.Sum(_ => _.TreatAmount - _.SendAmount);
            var sendAmount = tempPrintSendData.Sum(_ => _.SendAmount);


            //有自備也有傳送則列印登錄明細
            if (selfPrepareAmount > 0 && sendAmount > 0)
            {
                var rptViewer = new ReportViewer();
                SetReserveMedicinesSheetReportViewer(rptViewer, tempPrintSendData);
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintReserve(rptViewer);
                });
            }
            Current.PrescriptionStatus.UpdateStatus(Current.ID);
        }

        public void SetReserveMedicinesSheetReportViewer(ReportViewer rptViewer, PrescriptionSendDatas prescriptionSendDatas)
        {
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new ReserveMedicines(prescriptionSendDatas);
            var json = JsonConvert.SerializeObject(medBagMedicines);
            var dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            List<ReportParameter> parameters;
            switch (Settings.Default.ReceiptForm)
            {
                case "一般":
                    rptViewer.LocalReport.ReportPath = @"RDLC\ReserveSheet_A5.rdlc";
                    parameters = CreateReserveMedicinesSheetParametersA5();
                    break;

                default:
                    rptViewer.LocalReport.ReportPath = @"RDLC\ReserveSheet.rdlc";
                    parameters = CreateReserveMedicinesSheetParameters();
                    break;
            }
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.LocalReport.DataSources.Clear();
            var rd = new ReportDataSource("ReserveMedicinesDataSet", dataTable);
            rptViewer.LocalReport.DataSources.Add(rd);
            rptViewer.LocalReport.Refresh();
        }

        private List<ReportParameter> CreateReserveMedicinesSheetParameters()
        {
            return new List<ReportParameter>
            {
                new ReportParameter("Type","登錄"),
                new ReportParameter("PatientName",Current.Patient.Name),
                new ReportParameter("PatientBirthday",((DateTime)Current.Patient.Birthday).AddYears(-1911).ToString("yyy-MM-dd")),
                new ReportParameter("PatientTel",Current.Patient.ContactNote),
                new ReportParameter("Institution", Current.Institution.Name),
                new ReportParameter("Division", Current.Division.Name),
                new ReportParameter("AdjustStart", $"{((DateTime)Current.AdjustDate).AddYears(-1911):yyy-MM-dd}"),
                new ReportParameter("AdjustEnd", $"{((DateTime)Current.AdjustDate).AddYears(-1911).AddDays(20):yyy-MM-dd}"),
                new ReportParameter("AdjustDay", ((DateTime)Current.AdjustDate).Day.ToString())
            };
        }

        private List<ReportParameter> CreateReserveMedicinesSheetParametersA5()
        {
            return new List<ReportParameter>
            {
                new ReportParameter("Type","登錄"),
                new ReportParameter("PatientName",Current.Patient.Name),
                new ReportParameter("PatientBirthday",((DateTime)Current.Patient.Birthday).AddYears(-1911).ToString("yyy-MM-dd")),
                new ReportParameter("PatientTel",Current.Patient.ContactNote),
                new ReportParameter("Institution", Current.Institution.Name),
                new ReportParameter("Division", Current.Division.Name),
                new ReportParameter("AdjustStart", $"{((DateTime)Current.AdjustDate).AddYears(-1911):yyy-MM-dd}"),
                new ReportParameter("AdjustEnd", $"{((DateTime)Current.AdjustDate).AddYears(-1911).AddDays(20):yyy-MM-dd}"),
                new ReportParameter("AdjustDay", ((DateTime)Current.AdjustDate).Day.ToString())
            };
        }

        public void SetMedicalNumberByErrorCode(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            Current.TempMedicalNumber = errorCode.ID;
            if (Current.AdjustCase.ID.Equals("2") && Current.ChronicSeq > 1)
            {
                Current.MedicalNumber = "IC0" + Current.ChronicSeq;
                Current.OriginalMedicalNumber = errorCode.ID;
            }
            else
            {
                Current.MedicalNumber = errorCode.ID;
            }
        }

        public void MakeUpComplete()
        {
            var deposit = Current.PrescriptionPoint.Deposit;
            MainWindow.ServerConnection.OpenConnection();
            Current.PrescriptionPoint.Deposit = 0;
            Current.PrescriptionStatus.SetNormalAdjustStatus();
            Current.Update();
            MainWindow.ServerConnection.CloseConnection();
            NewFunction.ShowMessageFromDispatcher($"補卡作業成功，退還押金{deposit}元", MessageType.SUCCESS);
        }

        public void CheckDailyUploadMakeUp(ErrorUploadWindowViewModel.IcErrorCode errorCode)
        {
            if (Current.IsPrescribe) return;
            if ((bool)Current.PrescriptionStatus.IsCreateSign)
                HisAPI.CreatDailyUploadData(Current, true);
            else
                HisAPI.CreatErrorDailyUploadData(Current, true, errorCode);
        }

        public void SetMedicalNumber()
        {
            if (Current.AdjustCase.ID.Equals("2") && Current.ChronicSeq > 1)
            {
                Current.MedicalNumber = "IC0" + Current.ChronicSeq;
                Current.OriginalMedicalNumber = Current.TempMedicalNumber;
            }
            else
            {
                Current.MedicalNumber = Current.TempMedicalNumber;
            }
        }

        public void CloneTempPre()
        {
            TempPre = (Prescription)Current.Clone();
            if (TempPre.Institution != null)//&& TempPre.Institution.ID == "3532082753"
            {
                TempPrint = (Prescription)Current.PrintClone();
            }
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public static void ShowPrescriptionEditWindow(int preID, PrescriptionType type = PrescriptionType.Normal)
        {
            Prescription selected;
            switch (type)
            {
                case PrescriptionType.ChronicReserve:
                    selected = GetReserveByID(preID);
                    var title = "預約瀏覽 ResMasID:" + selected.SourceId;
                    var edit = new ReservePrescriptionWindow(selected, title);
                    break;

                default:
                    selected = GetPrescriptionByID(preID, type);
                    CheckAdminLogin(selected);
                    break;
            }
        }

        private static void CheckAdminLogin(Prescription selected)
        {
            if (selected is null) return;
            if (VM.CurrentUser.Authority == DomainModel.Enum.Authority.Admin || VM.CurrentUser.IsPharmist() )
            {
                var title = "處方修改 PreMasID:" + selected.ID;
                var edit = new PrescriptionEditWindow(selected, title);
            }
            else
            {
                if (selected.CheckCanEdit())
                {
                    var title = "處方修改 PreMasID:" + selected.ID;
                    var editWindow = new PrescriptionEditWindow(selected, title);
                }
                else
                {
                    var title = "處方瀏覽 PreMasID:" + selected.ID;
                    var recordWindow = new PrescriptionRecordWindow(selected, title);
                }
            }
        }

        private static Prescription GetPrescriptionByID(int preID, PrescriptionType type)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = PrescriptionDb.GetPrescriptionByID(preID);
            if (table == null || table.Rows.Count == 0)
                return null;
            var r = table.Rows[0];
            var selected = new Prescription(r, type);
            selected.InsertTime = r.Field<DateTime?>("InsertTime");
            MainWindow.ServerConnection.CloseConnection();
            return !CheckPrescriptionEnable(r) ? null : selected;
        }

        private static Prescription GetReserveByID(int reserveID)
        {
            MainWindow.ServerConnection.OpenConnection();
            var r = PrescriptionDb.GetReservePrescriptionByID(reserveID).Rows[0];
            MainWindow.ServerConnection.CloseConnection();
            var selected = new Prescription(r, PrescriptionType.ChronicReserve);
            selected.Type = PrescriptionType.ChronicReserve;
            selected.AdjustDate = r.Field<DateTime>("AdjustDate");
            MainWindow.ServerConnection.CloseConnection();
            return selected;
        }

        private static bool CheckPrescriptionEnable(DataRow r)
        {
            if (r.Field<string>("IsEnable").Equals("0"))
            {
                MessageWindow.ShowMessage("處方已被刪除。", MessageType.ERROR);
                return false;
            }
            return true;
        }

        public void SetCreateSign()
        {
            Current.PrescriptionStatus.IsCreateSign = false;
        }
    }
}