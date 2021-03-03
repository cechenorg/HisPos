using His_Pos.AbstractClass;
using His_Pos.Class;

namespace His_Pos.Interface
{
    public interface IProductReturn
    {
        string Note { get; set; }
        InStock Stock { get; set; }
        string BatchNumber { get; set; }
        double BatchLimit { get; set; }

        void CopyFilledData(Product product);
    }
}