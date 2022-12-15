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
        public string JouDet_ID { get; set; }
        public string JouDet_Type { get; set; }
        public string JouDet_Number { get; set; }
        public string JouDet_AcctLvl1 { get; set; }
        public string JouDet_AcctLvl2 { get; set; }
        public string JouDet_AcctLvl3 { get; set; }
        public string JouDet_AcctLvl1Name { get; set; }
        public string JouDet_AcctLvl2Name { get; set; }
        public string JouDet_AcctLvl3Name { get; set; }
        public int JouDet_Amount
        {
            get => jouDet_Amount;
            set
            {
                Set(() => JouDet_Amount, ref jouDet_Amount, value);
            }
        }
        private int jouDet_Amount;
        public string JouDet_Memo { get; set; }
        public IEnumerable<JournalAccount> Accounts { get; set; }
        public JournalAccount Account { get; set; }
    }
}
