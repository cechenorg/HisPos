﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.Class.Manufactory
{
    public class Manufactory : ICloneable
    {
        public Manufactory()
        {
        }

        public Manufactory(DataRow row, DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.MANUFACTORY:
                    Address = row["MAN_ADDR"].ToString();
                    Telphone = row["MAN_TEL"].ToString();
                    Fax = row["MAN_FAX"].ToString();
                    break;
            }
            
            Id = row["MAN_ID"].ToString();
            Name = row["MAN_NAME"].ToString();
        }

        public string Id { get; set; }
        public string Name{ get; set; }
        public string Address{ get; set; }
        public string Telphone{ get; set; }
        public string Fax { get; set; }
        public object Clone()
        {
            Manufactory manufactory = new Manufactory();

            manufactory.Id = Id;
            manufactory.Address = Address;
            manufactory.Fax = Fax;
            manufactory.Name = Name;
            manufactory.Telphone = Telphone;

            return manufactory;
        }
    }
}
