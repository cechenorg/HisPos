using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage
{
    /// <summary>
    /// ManufactoryManageView.xaml 的互動邏輯
    /// </summary>
    public partial class ManufactoryManageView : UserControl
    {
        //private bool isFirst = true;

        //private ManageManufactory currentManufactory;
        //public ManageManufactory CurrentManufactory
        //{
        //    get { return currentManufactory; }
        //    set
        //    {
        //        currentManufactory = value;
        //        NotifyPropertyChanged("CurrentManufactory");
        //    }
        //}
        //public ObservableCollection<ManageManufactory> ManageManufactories { get; set; }

        public ManufactoryManageView()
        {
            InitializeComponent();
            //InitManufactory();
        }

        //private void InitManufactory()
        //{
        //    ///ManageManufactories = ManufactoryDb.GetManageManufactory();
        //    ManageManufactoryDataGrid.SelectedIndex = 0;
        //}

        //private void Manufactory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if((sender as DataGrid).SelectedItem is null) return;

        //    CurrentManufactory = ((sender as DataGrid).SelectedItem as ManageManufactory).Clone() as ManageManufactory;

        //    Notes.Document.Blocks.Clear();
        //    Notes.AppendText(CurrentManufactory.Note);

        //    UpdateUi();
        //    InitDataChanged();
        //}

        //private void UpdateUi()
        //{
        //    if (CurrentManufactory.ManufactoryPrincipals.Count > 0)
        //    {
        //        PrincipalDetail.IsEnabled = true;
        //        PrincipalDataGrid.SelectedIndex = 0;

        //        PrincipalDataGrid.Items.Filter = p => (p as ManufactoryPrincipal).IsEnable;
        //    }
        //    else
        //    {
        //        PrincipalDetail.IsEnabled = false;
        //        PrincipalDetail.DataContext = null;
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged(string info)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //    }
        //}

        //private void Principals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((sender as DataGrid).SelectedItem is null) return;

        //    bool isChanged = IsChangedLbl.Content.Equals("已修改");

        //    if (((sender as DataGrid).SelectedItem as ManufactoryPrincipal).ManufactoryPayOverviews is null)
        //        ;/// ((sender as DataGrid).SelectedItem as ManufactoryPrincipal).ManufactoryPayOverviews = ManufactoryDb.GetManufactoryPayOverview(((sender as DataGrid).SelectedItem as ManufactoryPrincipal).Id);

        //    PrincipalDetail.DataContext = (sender as DataGrid).SelectedItem;

        //    PayC.Children.OfType<RadioButton>().Single(r =>
        //        r.Tag.Equals((PrincipalDetail.DataContext as ManufactoryPrincipal).PayCondition)).IsChecked = true;

        //    PayT.Children.OfType<RadioButton>().Single(r =>
        //        r.Tag.Equals((PrincipalDetail.DataContext as ManufactoryPrincipal).PayType)).IsChecked = true;

        //    if (!isChanged) InitDataChanged();
        //}

        //private void Cancel_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if(ManageManufactoryDataGrid.SelectedItem is null) return;

        //    CurrentManufactory = (ManageManufactoryDataGrid.SelectedItem as ManageManufactory).Clone() as ManageManufactory;
        //    UpdateUi();
        //    InitDataChanged();
        //}

        //private void AddPrincipal_OnClick(object sender, RoutedEventArgs e)
        //{
        //    ManufactoryPrincipal newPrincipal = new ManufactoryPrincipal();

        //    CurrentManufactory.ManufactoryPrincipals.Add(newPrincipal);
        //    PrincipalDataGrid.SelectedItem = newPrincipal;
        //    PrincipalDataGrid.ScrollIntoView(newPrincipal);
        //    PrincipalDetail.IsEnabled = true;
        //    DataChanged();
        //}

        //private void DeletePrincipal_OnClick(object sender, RoutedEventArgs e)
        //{
        //    (PrincipalDetail.DataContext as ManufactoryPrincipal).IsEnable = false;
        //    UpdateUi();
        //    DataChanged();
        //}

        //private void DataChanged()
        //{
        //    if (isFirst) return;

        //    IsChangedLbl.Content = "已修改";
        //    IsChangedLbl.Foreground = Brushes.Red;

        //    ConfirmBtn.IsEnabled = true;
        //    CancelBtn.IsEnabled = true;
        //}

        //private void InitDataChanged()
        //{
        //    IsChangedLbl.Content = "未修改";
        //    IsChangedLbl.Foreground = Brushes.Black;

        //    ConfirmBtn.IsEnabled = false;
        //    CancelBtn.IsEnabled = false;
        //}

        //private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    DataChanged();
        //}

        //private void AddManufactory_Click(object sender, MouseButtonEventArgs e)
        //{
        //    ManageManufactory newManageManufactory = null;/// ManufactoryDb.AddNewManageManufactory();

        //    ManageManufactories.Add(newManageManufactory);

        //    ManageManufactoryDataGrid.SelectedItem = newManageManufactory;
        //    ManageManufactoryDataGrid.ScrollIntoView(newManageManufactory);
        //}

        //private void ConfirmChanged_OnClick(object sender, RoutedEventArgs e)
        //{
        //    int index = ManageManufactoryDataGrid.SelectedIndex;

        //    CurrentManufactory.Note = new TextRange(Notes.Document.ContentStart, Notes.Document.ContentEnd).Text;

        //    ManageManufactories[index] = CurrentManufactory;

        //    ManageManufactoryDataGrid.SelectedIndex = index;

        //   /// ManufactoryDb.UpdateManageManufactory(CurrentManufactory);
        //}

        //private void DeleteManufactory_Click(object sender, MouseButtonEventArgs e)
        //{
        //    ManageManufactory manufactory = ManageManufactoryDataGrid.SelectedItem as ManageManufactory;

        //    /// ManufactoryDb.DeleteManageManufactory(manufactory.Id);

        //    ManageManufactories.Remove(manufactory);

        //    ManageManufactoryDataGrid.SelectedIndex = 0;
        //    ManageManufactoryDataGrid.ScrollIntoView(ManageManufactoryDataGrid.SelectedItem);
        //}

        //private void ManufactoryManageView_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    isFirst = false;
        //}

        //private void PayCondition_CheckedChanged(object sender, RoutedEventArgs e)
        //{
        //    if(PrincipalDetail.DataContext is null) return;

        //    DataChanged();

        //    (PrincipalDetail.DataContext as ManufactoryPrincipal).PayCondition = (sender as RadioButton).Tag.ToString();
        //}

        //private void PayType_CheckedChanged(object sender, RoutedEventArgs e)
        //{
        //    if (PrincipalDetail.DataContext is null) return;

        //    DataChanged();

        //    (PrincipalDetail.DataContext as ManufactoryPrincipal).PayType = (sender as RadioButton).Tag.ToString();
        //}

        //private void Search_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (SearchName.Text.Equals(""))
        //    {
        //        ManageManufactoryDataGrid.Items.Filter = null;
        //        return;
        //    }

        //    ManageManufactoryDataGrid.Items.Filter = m => (m as ManageManufactory).Name.Contains(SearchName.Text)
        //                                                  || (m as ManageManufactory).NickName.Contains(SearchName.Text)
        //                                                  || (m as ManageManufactory).ManufactoryPrincipals.Count(p => p.Name.Contains(SearchName.Text)) > 0
        //                                                  || (m as ManageManufactory).ManufactoryPrincipals.Count(p => p.ResponsibleDepartment.Contains(SearchName.Text)) > 0;

        //    ManageManufactoryDataGrid.SelectedIndex = 0;
        //}
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataGrid dataGrid = sender as DataGrid;

            //if(dataGrid is null) return;

            //int index = dataGrid.Items.IndexOf(dataGrid.SelectedItem);

            //if (index != dataGrid.SelectedIndex)
            //{
            //    MessageWindow.ShowMessage("資料有異動　請先確認變更再切換供應商!", MessageType.ERROR);
            //    dataGrid.UnselectAllCells();
            //    dataGrid.SelectedItem = e.RemovedItems[0];
            //    e.Handled = true;
            //}
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dataGridRow = sender as DataGridRow;
            if (dataGridRow is null || (DataContext as ManufactoryManageViewModel).CurrentManufactory is null) return;

            if ((DataContext as ManufactoryManageViewModel).CurrentManufactory.IsDataChanged)
            {
                MessageWindow.ShowMessage("資料有異動　請先確認變更再切換供應商!", MessageType.ERROR);
                e.Handled = true;
            }
        }
    }
}