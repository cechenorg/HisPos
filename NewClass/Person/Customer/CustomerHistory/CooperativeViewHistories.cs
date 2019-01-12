using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass.Product;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CooperativeViewHistories:Collection<CooperativeViewHistory>
    {
        public CooperativeViewHistories(int customerID)
        {
            Init(customerID);
        }

        public void Init(int customerID)
        {
            var table = CooperativeViewHistoryDb.GetData(customerID);
            //foreach (DataRow r in table.Rows)
            //{
            //    Add(new CooperativeViewHistory(r));
            //}
            var c1 = new CooperativeViewHistory();
            c1.Institution = "翰群骨科診所";
            c1.Division = "骨科";
            c1.AdjustDate = new DateTime(2019,1,1);
            c1.TotalPoint = 100;
            c1.Products = new Products();
            Product.Product p = new Product.Product();
            p.ID = "AC123456789";
            p.ChineseName = "測試藥品1";
            p.EnglishName = "TestMedicine1";
            Product.Product p1 = new Product.Product();
            p1.ID = "BC123456789";
            p1.ChineseName = "測試藥品2";
            p1.EnglishName = "TestMedicine2";
            c1.Products.Add(p);
            c1.Products.Add(p1);
            var c2 = new CooperativeViewHistory();
            c2.Institution = "翰群骨科診所";
            c2.Division = "骨科";
            c2.AdjustDate = new DateTime(2019, 1, 2);
            c2.TotalPoint = 123;
            Product.Product p2 = new Product.Product();
            p2.ID = "AD00000000";
            p2.ChineseName = "測試藥品3";
            p2.EnglishName = "TestMedicine3";
            Product.Product p3 = new Product.Product();
            p3.ID = "BD00000000";
            p3.ChineseName = "測試藥品4";
            p3.EnglishName = "TestMedicine4";
            c2.Products.Add(p2);
            c2.Products.Add(p3);
            var c3 = new CooperativeViewHistory();
            c3.Institution = "翰群骨科診所";
            c3.Division = "骨科";
            c3.AdjustDate = new DateTime(2019, 1, 3);
            c3.TotalPoint = 456;
            Product.Product p4 = new Product.Product();
            p4.ID = "AD98765432";
            p4.ChineseName = "測試藥品5";
            p4.EnglishName = "TestMedicine5";
            c3.Products.Add(p4);
            Add(c1);
            Add(c2);
            Add(c3);
        }
    }
}
