﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;
using His_Pos.Service;
using Newtonsoft.Json;

namespace His_Pos.Class.Product
{
    public class InventoryObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string BatchNum { get; set; }
        public double Amount { get; set; }
        public double Stock { get; set; }
        public string ValidityPeriod { get; set; }
        public string Location { get; set; }
        public InventoryObject(string id, string name,  string category,string batchNum,double amount, double stock,string validity,string location)
        {
            Id = id;
            Name = name;
            Category = category;
            BatchNum = batchNum;
            Amount = amount;
            Stock = stock;
            ValidityPeriod = validity;
            Location = location;
        }
    }

    public class Batch
    {
        public string Number { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public Batch(string number, double amount, double price)
        {
            Number = number;
            Amount = amount;
            Price = price;
        }
    }

    public static class InventoryChecking
    {
        public static DataTable t  = new DataTable("InventoryDataSet");

        public static List<InventoryObject> InventoryObjectList = new List<InventoryObject>();

        public static List<Batch> BatchNumbersList = new List<Batch>();

        public static List<InventoryObject> GetInventoryObjectList()
        {
            return InventoryObjectList;
        }

        public static List<Batch> GetBatchNumberList()
        {
            return BatchNumbersList;
        }

        public static void MergeData(this DataTable table, IEnumerable<object> data, string segName)
        {
            var json = JsonConvert.SerializeObject(data);
            var t = JsonConvert.DeserializeObject<DataTable>(json);
            table.MergeData(t, segName);
        }

        public static void MergeData(this DataTable table, DataTable toAdd, string segName)
        {
            var segFldName = $"Is{segName}";
            toAdd.Columns.Add(segFldName, typeof(bool));
            foreach (DataColumn c in toAdd.Columns)
            {
                if (!table.Columns.Contains(c.ColumnName))
                    table.Columns.Add(c.ColumnName, c.DataType);
            }
            foreach (DataRow row in toAdd.Rows)
            {
                row[segFldName] = true;
                var newRow = table.NewRow();
                foreach (DataColumn c in toAdd.Columns)
                    newRow[c.ColumnName] = row[c.ColumnName];
                table.Rows.Add(newRow);
            }
        }
    }

}
