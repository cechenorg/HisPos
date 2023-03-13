using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Accounts
{
    public class LedgerDetail : ObservableObject
    {
        public string CurPha_Name
        {
            get => curPha_Name;
            set
            {
                Set(() => CurPha_Name, ref curPha_Name, value);
            }
        }
        private string curPha_Name;
        public DateTime JouMas_Date
        {
            get => jouMas_Date;
            set
            {
                Set(() => JouMas_Date, ref jouMas_Date, value);
            }
        }
        private DateTime jouMas_Date;
        public string JouMas_Source
        {
            get => jouMas_Source;
            set
            {
                Set(() => JouMas_Source, ref jouMas_Source, value);
            }
        }
        private string jouMas_Source;
        public string JouMas_ID
        {
            get => jouMas_ID;
            set
            {
                Set(() => JouMas_ID, ref jouMas_ID, value);
            }
        }
        private string jouMas_ID;
        public string JouDet_Type
        {
            get => jouDet_Type;
            set
            {
                Set(() => JouDet_Type, ref jouDet_Type, value);
            }
        }
        private string jouDet_Type;
        public int JouDet_Number
        {
            get => jouDet_Number;
            set
            {
                Set(() => JouDet_Number, ref jouDet_Number, value);
            }
        }
        private int jouDet_Number;
        public string JouDet_AcctLvl1
        {
            get => jouDet_AcctLvl1;
            set
            {
                Set(() => JouDet_AcctLvl1, ref jouDet_AcctLvl1, value);
            }
        }
        private string jouDet_AcctLvl1;
        public string JouDet_AcctLvl2
        {
            get => jouDet_AcctLvl2;
            set
            {
                Set(() => JouDet_AcctLvl2, ref jouDet_AcctLvl2, value);
            }
        }
        private string jouDet_AcctLvl2;
        public string JouDet_AcctLvl3
        {
            get => jouDet_AcctLvl3;
            set
            {
                Set(() => JouDet_AcctLvl3, ref jouDet_AcctLvl3, value);
            }
        }
        private string jouDet_AcctLvl3;
        public string AcctID
        {
            get => acctID;
            set
            {
                Set(() => AcctID, ref acctID, value);
            }
        }
        private string acctID;
        public string JouDet_AcctName
        {
            get => jouDet_AcctName;
            set
            {
                Set(() => JouDet_AcctName, ref jouDet_AcctName, value);
            }
        }
        private string jouDet_AcctName;
        public int JouDet_Amount
        {
            get => jouDet_Amount;
            set
            {
                Set(() => JouDet_Amount, ref jouDet_Amount, value);
            }
        }
        private int jouDet_Amount;
        public int DAmount
        {
            get => dAmount;
            set
            {
                Set(() => DAmount, ref dAmount, value);
            }
        }
        private int dAmount;
        public int CAmount
        {
            get => cAmount;
            set
            {
                Set(() => CAmount, ref cAmount, value);
            }
        }
        private int cAmount;
        public int Balance
        {
            get => balance;
            set
            {
                Set(() => Balance, ref balance, value);
            }
        }
        private int balance;
        public string JouDet_Memo
        {
            get => jouDet_Memo;
            set
            {
                Set(() => JouDet_Memo, ref jouDet_Memo, value);
            }
        }
        private string jouDet_Memo;
        public string JouDet_Source
        {
            get => jouDet_Source;
            set
            {
                Set(() => JouDet_Source, ref jouDet_Source, value);
            }
        }
        private string jouDet_Source;
        public string JouDet_SourceID
        {
            get => jouDet_SourceID;
            set
            {
                Set(() => JouDet_SourceID, ref jouDet_SourceID, value);
            }
        }
        private string jouDet_SourceID;
        public string JouMas_Memo
        {
            get => jouMas_Memo;
            set
            {
                Set(() => JouMas_Memo, ref jouMas_Memo, value);
            }
        }
        private string jouMas_Memo;
        public DateTime JouMas_InsertTime
        {
            get => jouMas_InsertTime;
            set
            {
                Set(() => JouMas_InsertTime, ref jouMas_InsertTime, value);
            }
        }
        private DateTime jouMas_InsertTime;
        public string InsertEmpName
        {
            get => insertEmpName;
            set
            {
                Set(() => InsertEmpName, ref insertEmpName, value);
            }
        }
        private string insertEmpName;
        public DateTime? JouMas_ModifyTime
        {
            get => jouMas_ModifyTime;
            set
            {
                Set(() => JouMas_ModifyTime, ref jouMas_ModifyTime, value);
            }
        }
        private DateTime? jouMas_ModifyTime;
        public string ModifyEmpName
        {
            get => modifyEmpName;
            set
            {
                Set(() => ModifyEmpName, ref modifyEmpName, value);
            }
        }
        private string modifyEmpName;
    }
}
