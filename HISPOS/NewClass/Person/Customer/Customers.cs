﻿using His_Pos.NewClass.Prescription.ImportDeclareXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customers : ObservableCollection<Customer>
    {
        public Customers()
        {
        }

        public void SearchCustomers(string idNumber, string name, string cellPhone, string tel, DateTime? birth)
        {
            var cusList = CustomerDb.SearchCustomers(idNumber, name, cellPhone, tel, birth);
            foreach (Customer r in cusList)
            {
                Add(r);
            }
        }

        public void GetDataByNameOrBirth(string name, DateTime? date, string idNumber, string phoneNumber)
        {
            Clear();
            var cusList = CustomerDb.GetDataByNameOrBirth(name, date, idNumber, phoneNumber);
            foreach (Customer r in cusList)
            {
                Add(r);
            }
        }

        public void GetCustomerByCusId(int ID)
        {
            Clear();
            var customer = CustomerDb.GetCustomerByCusId(ID);
            Add(customer);
        }

        public Customers SetCustomersByPrescriptions(List<ImportDeclareXml.Ddata> ddatas)
        {
            DataTable table = CustomerDb.SetCustomersByPrescriptions(ddatas);
            Customers customers = new Customers();
            foreach (DataRow r in table.Rows)
            {
                customers.Add(new Customer(r));
            }
            return customers;
        }

        public void GetTodayEdited()
        {
            var cusList = CustomerDb.GetTodayEdited();
            foreach (Customer r in cusList)
            {
                Add(r);
            }
        }
    }
}