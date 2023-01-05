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
        public string JouMas_ID { get; set; }
        public DateTime? JouMas_Date
        {
            get => jouMas_Date;
            set
            {
                Set(() => JouMas_Date, ref jouMas_Date, value);
            }
        }
        private DateTime? jouMas_Date;
        public string JouMas_Status { get; set; }
        public int JouMas_IsEnable { get; set; }
        public string JouMas_VoidReason { get; set; }
        public DateTime JouMas_InsertTime { get; set; }
        public int JouMas_InsertEmpID { get; set; }
        public string JouMas_InsertEmpName { get; set; }
        public DateTime? JouMas_ModifyTime { get; set; }
        public int JouMas_ModifyEmpID { get; set; }
        public string JouMas_ModifyEmpName { get; set; }
        public string JouMas_Memo { get; set; }
        public int JouMas_Source { get; set; }//輸入來源
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
