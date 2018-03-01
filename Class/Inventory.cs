using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Inventory
    {
        public string ProductId { get; set; } //商品編號
        public string ProductName { get; set; } //商品名稱
        public string ProductCurrentTime { get; set; }//商品最近一次進貨時間
        public string ProductTotalAmount { get; set; } //庫存總量
        public string ProductSafeAmount { get; set; } //商品安全量
    }
}
