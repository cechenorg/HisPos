﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.Class;

namespace His_Pos.NewClass.Report.Accounts
{
    public class AccountsDetailReports : ObservableObject
    {
        public AccountsDetailReports(DataRow r)
        {

            Name = r.Field<string>("Date");
            Value = r.Field<decimal>("Value");
            ID= r.Field<string>("ID");
            StrikeValue = r.Field<decimal>("StrikeValue");

        }
        public AccountsDetailReports(string name, decimal value , string id)
        {

            Name = name;
            Value = value;
            ID = id;

        }

        public AccountsDetailReports()
        {
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }
        private string iD;
        public string ID
        {
            get => iD;
            set
            {
                Set(() => Name, ref iD, value);
            }
        }
        private decimal valuee;
        public decimal Value
        {
            get => valuee;
            set
            {
                Set(() => Value, ref valuee, value);
            }
        }
        private decimal strikeValue;
        public decimal StrikeValue
        {
            get => strikeValue;
            set
            {
                Set(() => StrikeValue, ref strikeValue, value);
            }
        }
    }
}
