using His_Pos.NewClass.Product;
using ZeroFormatter;

namespace His_Pos.NewClass.OfflineDataSet
{
    [ZeroFormattable]
    public class Product
    {
        public Product()
        {
        }

        public Product(ProductStruct product)
        {
            ID = product.ID;
            ChineseName = product.ChineseName;
            EnglishName = product.EnglishName;
            FullName = product.FullName;
            NHIPrice = product.NHIPrice;
            SellPrice = product.SellPrice;
            ControlLevel = product.ControlLevel;
            IsCommon = product.IsCommon;
            IsFrozen = product.IsFrozen;
            IsEnable = product.IsEnable;
        }

        [Index(0)]
        public virtual string ID { get; set; }

        [Index(1)]
        public virtual string ChineseName { get; set; }

        [Index(2)]
        public virtual string EnglishName { get; set; }

        [Index(3)]
        public virtual string FullName { get; set; }

        [Index(4)]
        public virtual double NHIPrice { get; set; }

        [Index(5)]
        public virtual double SellPrice { get; set; }

        [Index(6)]
        public virtual int? ControlLevel { get; set; }

        [Index(7)]
        public virtual bool IsCommon { get; set; }

        [Index(8)]
        public virtual bool IsFrozen { get; set; }

        [Index(9)]
        public virtual bool IsEnable { get; set; }
    }
}