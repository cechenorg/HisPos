using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.Employee;

namespace His_Pos.NewClass.StockTaking.StockTaking.StockTakingPage
{
    public class StockTakingPage : ObservableObject
    {
        public StockTakingPage(int index, int amount, Employee emp)
        {
            Index = index;
            Amount = amount;
            Employee = emp;
        }

        public int Index { get; set; }
        public int Amount { get; set; }
        public Employee Employee { get; set; }
    }
}