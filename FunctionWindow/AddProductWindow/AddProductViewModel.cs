using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    public class AddProductViewModel : ViewModelBase
    {
        #region ----- Define Command -----
        public RelayCommand GetRelatedDataCommand { get; set; }
        public RelayCommand FilterCommand { get; set; }
        public RelayCommand<string> FocusUpDownCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public bool IsEditing { get; set; }
        public bool IsProductSelected { get; set; } = false;
        public bool HideDisableProduct { get; set; }
        public bool ShowOnlyThisManufactory { get; set; }
        public string SearchString { get; set; }
        public ProductStruct SelectedProductStruct { get; set; }
        public ProductStructs ProductStructCollection { get; set; }
        #endregion

        public AddProductViewModel(string searchString, AddProductEnum addProductEnum)
        {
            SearchString = searchString;

            GetRelatedDataCommand = new RelayCommand(GetRelatedDataAction);

            switch (addProductEnum)
            {
                case AddProductEnum.PruductPurchase:
                    FilterCommand = new RelayCommand(ProductPurchaseFilterAction);
                    break;
            }
        }

        #region ----- Define Actions -----
        private void GetRelatedDataAction()
        {
            if (SearchString.Length > 4)
            {
                IsEditing = false;
                MainWindow.ServerConnection.OpenConnection();
                ProductStructCollection = ProductStructs.GetProductStructsBySearchString(SearchString);
                MainWindow.ServerConnection.CloseConnection();
            }
        }
        private void ProductPurchaseFilterAction()
        {

        }
        private void FocusUpDownAction(string direction)
        {

        }
        #endregion

        #region ----- Define Functions -----

        #endregion

    }
}
