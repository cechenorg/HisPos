using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryHis : CustomerHistoryDetail
    {
        public CustomerHistoryHis(string medicineName, string usage, string position, string dosage)
        {
            MedicineName = medicineName;
            Usage = usage;
            Position = position;
            Dosage = dosage;
        }

        public string MedicineName { get; }
        public string Usage { get; }
        public string Position { get; }
        public string Dosage { get; }
    }
}