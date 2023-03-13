using GalaSoft.MvvmLight;
using His_Pos.NewClass.Report.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Accounts
{
    public class JournalDetail : ObservableObject
    {
        private string jouDet_ID;
        public string JouDet_ID
        {
            get => jouDet_ID;
            set
            {
                Set(() => JouDet_ID, ref jouDet_ID, value);
            }
        }
        private string jouDet_Type;
        public string JouDet_Type
        {
            get => jouDet_Type;
            set
            {
                Set(() => JouDet_Type, ref jouDet_Type, value);
            }
        }
        private int jouDet_Number;
        public int JouDet_Number 
        {
            get => jouDet_Number;
            set
            {
                Set(() => JouDet_Number, ref jouDet_Number, value);
            }
        }
        private string jouDet_AcctLvl1;
        public string JouDet_AcctLvl1
        {
            get => jouDet_AcctLvl1;
            set
            {
                Set(() => JouDet_AcctLvl1, ref jouDet_AcctLvl1, value);
            }
        }
        private string jouDet_AcctLvl2;
        public string JouDet_AcctLvl2
        {
            get => jouDet_AcctLvl2;
            set
            {
                Set(() => JouDet_AcctLvl2, ref jouDet_AcctLvl2, value);
            }
        }
        private string jouDet_AcctLvl3;
        public string JouDet_AcctLvl3
        {
            get => jouDet_AcctLvl3;
            set
            {
                Set(() => JouDet_AcctLvl3, ref jouDet_AcctLvl3, value);
            }
        }
        private string jouDet_AcctLvl1Name;
        public string JouDet_AcctLvl1Name
        {
            get => jouDet_AcctLvl1Name;
            set
            {
                Set(() => JouDet_AcctLvl1Name, ref jouDet_AcctLvl1Name, value);
            }
        }
        private string jouDet_AcctLvl2Name;
        public string JouDet_AcctLvl2Name 
        {
            get => jouDet_AcctLvl2Name;
            set
            {
                Set(() => JouDet_AcctLvl2Name, ref jouDet_AcctLvl2Name, value);
            }
        }
        private string jouDet_AcctLvl3Name;
        public string JouDet_AcctLvl3Name
        {
            get => jouDet_AcctLvl3Name;
            set
            {
                Set(() => JouDet_AcctLvl3Name, ref jouDet_AcctLvl3Name, value);
            }
        }
        private int jouDet_Amount;
        public int JouDet_Amount
        {
            get => jouDet_Amount;
            set
            {
                Set(() => JouDet_Amount, ref jouDet_Amount, value);
            }
        }
        private string jouDet_Memo;
        public string JouDet_Memo
        {
            get => jouDet_Memo;
            set
            {
                Set(() => JouDet_Memo, ref jouDet_Memo, value);
            }
        }
        private IEnumerable<JournalAccount> accounts;
        public IEnumerable<JournalAccount> Accounts
        {
            get => accounts;
            set
            {
                Set(() => Accounts, ref accounts, value);
            }
        }
        private JournalAccount account;
        public JournalAccount Account
        {
            get => account;
            set
            {
                Set(() => Account, ref account, value);
            }
        }
        private string jouDet_Source;
        public string JouDet_Source
        {
            get => jouDet_Source;
            set
            {
                Set(() => JouDet_Source, ref jouDet_Source, value);
            }
        }
        private string jouDet_SourceID;
        public string JouDet_SourceID
        {
            get => jouDet_SourceID;
            set
            {
                Set(() => JouDet_SourceID, ref jouDet_SourceID, value);
            }
        }
        private string jouDet_WriteOffID;
        public string JouDet_WriteOffID
        {
            get => jouDet_WriteOffID;
            set
            {
                Set(() => JouDet_WriteOffID, ref jouDet_WriteOffID, value);
            }
        }
        private int jouDet_WriteOffNumber;
        public int JouDet_WriteOffNumber 
        {
            get => jouDet_WriteOffNumber;
            set
            {
                Set(() => JouDet_WriteOffNumber, ref jouDet_WriteOffNumber, value);
            }
        }
    }
}
