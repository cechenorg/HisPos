using His_Pos.Class;
using System.Windows.Media.Imaging;

namespace His_Pos.Interface
{
    public interface IInventory
    {
        string Id { get; set; }
        string Name { get; set; }
        InStock Stock { get; set; }
        string Location { get; set; }
        bool Status { get; set; }
        BitmapImage TypeIcon { get; set; }
        string StockValue { get; set; }
        string Note { get; set; }
        string WareHouseId { get; set; }
        string WareHouse { get; set; }
        string BarCode { get; set; }
    }
}