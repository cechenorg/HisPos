using System.Collections.ObjectModel;

namespace His_Pos.NewClass.StockTaking.StockTaking.StockTakingPage
{
    public class StockTakingPages : ObservableCollection<StockTakingPage>
    {
        public StockTakingPages()
        {
        }

        public void AssignPages(StockTakingPlan.StockTakingPlan stockTakingPlan)
        {
            Clear();
            int count = stockTakingPlan.StockTakingProductCollection.Count;
            int amount = 30;
            int index = 1;
            while (count > 0)
            {
                if (30 > count && count > 0)
                    amount = count;
                Add(new StockTakingPage(index, amount, ChromeTabViewModel.ViewModelMainWindow.CurrentUser));
                count -= 30;
                index++;
            }
        }
    }
}