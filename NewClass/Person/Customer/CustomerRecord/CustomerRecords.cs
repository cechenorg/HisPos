﻿using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerRecords : Collection<CustomerRecord>
    {
        public CustomerRecords()
        {

        }

        public CustomerRecords(int id)
        {
            var table = CustomerRecordDb.GetData(id);
            foreach (DataRow r in table.Rows)
            {
                Add(new CustomerRecord(r));
            }
        }
    }
}
