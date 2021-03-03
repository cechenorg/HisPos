using His_Pos.Class;

namespace His_Pos.Interface
{
    public interface IProductDeclare
    {
        string ProductId { get; set; }
        string ProductName { get; set; }
        double Dosage { get; set; }
        string Usage { get; set; }
        string Position { get; set; }
        string Days { get; set; }
        double Amount { get; set; }
        double Inventory { get; set; }
        double HcPrice { get; set; }
        bool PaySelf { get; set; }
        double TotalPrice { get; set; }
        string ControlLevel { get; set; }
        string Forms { get; set; }
        string Ingredient { get; set; }
        string SideEffect { get; set; }
        string Indication { get; set; }
        InStock Stock { get; set; }
    }
}