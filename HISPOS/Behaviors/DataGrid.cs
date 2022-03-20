using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace His_Pos.Behaviors
{
    public class ScrollIntoView : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid)
            {
                DataGrid grid = (sender as DataGrid);

                grid.Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (grid.SelectedItem != null)
                    {
                        grid.UpdateLayout();
                        grid.ScrollIntoView(grid.SelectedItem, null);
                    }
                }));
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -=
                AssociatedObject_SelectionChanged;
        }
    }
}