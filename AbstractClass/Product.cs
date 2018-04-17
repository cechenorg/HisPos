using System.ComponentModel;
using System.Data;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace His_Pos.AbstractClass
{
    public class Product
    {
        public Product(DataRow dataRow)
        {
            Id = dataRow["PRO_ID"].ToString();
            Name = dataRow["PRO_NAME"].ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
