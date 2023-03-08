using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Report.Accounts;

namespace His_Pos.NewClass.Accounts
{
    public class JournalMaster : ObservableObject
    {
        public string JouMas_ID
        {
            get => jouMas_ID;
            set
            {
                Set(() => JouMas_ID, ref jouMas_ID, value);
            }
        }
        private string jouMas_ID;
        public DateTime? JouMas_Date
        {
            get => jouMas_Date;
            set
            {
                Set(() => JouMas_Date, ref jouMas_Date, value);
            }
        }
        private DateTime? jouMas_Date;
        public string JouMas_Status
        {
            get => jouMas_Status;
            set
            {
                Set(() => JouMas_Status, ref jouMas_Status, value);
            }
        }
        private string jouMas_Status;
        public int JouMas_IsEnable
        {
            get => jouMas_IsEnable;
            set
            {
                Set(() => JouMas_IsEnable, ref jouMas_IsEnable, value);
            }
        }
        private int jouMas_IsEnable;
        public string JouMas_VoidReason
        {
            get => jouMas_VoidReason;
            set
            {
                Set(() => JouMas_VoidReason, ref jouMas_VoidReason, value);
            }
        }
        private string jouMas_VoidReason;
        public DateTime JouMas_InsertTime
        {
            get => jouMas_InsertTime;
            set
            {
                Set(() => JouMas_InsertTime, ref jouMas_InsertTime, value);
            }
        }
        private DateTime jouMas_InsertTime;
        public int JouMas_InsertEmpID
        {
            get => jouMas_InsertEmpID;
            set
            {
                Set(() => JouMas_InsertEmpID, ref jouMas_InsertEmpID, value);
            }
        }
        private int jouMas_InsertEmpID;
        public string JouMas_InsertEmpName
        {
            get => jouMas_InsertEmpName;
            set
            {
                Set(() => JouMas_InsertEmpName, ref jouMas_InsertEmpName, value);
            }
        }
        private string jouMas_InsertEmpName;
        public DateTime? JouMas_ModifyTime { get; set; }
        public int JouMas_ModifyEmpID
        {
            get => jouMas_ModifyEmpID;
            set
            {
                Set(() => JouMas_ModifyEmpID, ref jouMas_ModifyEmpID, value);
            }
        }
        private int jouMas_ModifyEmpID;
        public string JouMas_ModifyEmpName
        {
            get => jouMas_ModifyEmpName;
            set
            {
                Set(() => JouMas_ModifyEmpName, ref jouMas_ModifyEmpName, value);
            }
        }
        private string jouMas_ModifyEmpName;
        public string JouMas_Memo
        {
            get => jouMas_Memo;
            set
            {
                Set(() => JouMas_Memo, ref jouMas_Memo, value);
            }
        }
        private string jouMas_Memo;
        public int JouMas_Source
        {
            get => jouMas_Source;
            set
            {
                Set(() => JouMas_Source, ref jouMas_Source, value);
            }
        }//輸入來源
        private int jouMas_Source;
        public int DebitTotalAmount
        {
            get => debitTotalAmount;
            set
            {
                Set(() => DebitTotalAmount, ref debitTotalAmount, value);
            }
        }
        private int debitTotalAmount;//借方總金額
        public int CreditTotalAmount
        {
            get => creditTotalAmount;
            set
            {
                Set(() => CreditTotalAmount, ref creditTotalAmount, value);
            }
        }
        private int creditTotalAmount;//貸方總金額
        public JournalDetails DebitDetails
        {
            get => debitDetails;
            set
            {
                Set(() => DebitDetails, ref debitDetails, value);
                CalculateTotal("D");
            }
        }
        private JournalDetails debitDetails;//借方明細
        public JournalDetails CreditDetails
        {
            get => creditDetails;
            set
            {
                Set(() => CreditDetails, ref creditDetails, value);
                CalculateTotal("C");
            }
        }
        private JournalDetails creditDetails;//貸方明細
        public JournalDetail SelectedDebitDetail
        {
            get => selectedDebitDetail;
            set
            {
                Set(() => SelectedDebitDetail, ref selectedDebitDetail, value);
            }
        }
        private JournalDetail selectedDebitDetail;//當下選擇借方明細
        public JournalDetail SelectedCreditDetail
        {
            get => selectedCreditDetail;
            set
            {
                Set(() => SelectedCreditDetail, ref selectedCreditDetail, value);
            }
        }
        private JournalDetail selectedCreditDetail;//當下選擇貸方明細
        public void CalculateTotal(string type)
        {
            switch (type)
            {
                case "D":
                    debitTotalAmount = (int)DebitDetails.Sum(S => S.JouDet_Amount);
                    break;
                case "C":
                    creditTotalAmount = (int)CreditDetails.Sum(S => S.JouDet_Amount);
                    break;
            }
        }
    }
}
