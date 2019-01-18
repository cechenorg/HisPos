﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
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
        public RelayCommand ProductSelected { get; set; }
        #endregion

        #region ----- Define Variables -----
        private CollectionViewSource proStructCollectionViewSource;

        private CollectionViewSource ProStructCollectionViewSource
        {
            get => proStructCollectionViewSource;
            set
            {
                Set(() => ProStructCollectionViewSource, ref proStructCollectionViewSource, value);
            }
        }

        private ICollectionView proStructCollectionView;
        public ICollectionView ProStructCollectionView
        {
            get => proStructCollectionView;
            private set
            {
                Set(() => ProStructCollectionView, ref proStructCollectionView, value);
            }
        }
        public bool IsEditing { get; set; }
        public bool IsProductSelected { get; set; } = false;
        public bool HideDisableProduct { get; set; }
        public bool ShowOnlyThisManufactory { get; set; }
        private string searchString;
        public string SearchString
        {
            get => searchString;
            set
            {
                Set(() => SearchString, ref searchString, value);
            }
        }

        public ProductStruct SelectedProductStruct { get; set; }
        public ProductStructs ProductStructCollection { get; set; }
        private AddProductEnum addProEnum { get; set; }
        #endregion

        public AddProductViewModel(string searchString, AddProductEnum addProductEnum)
        {
            GetRelatedDataCommand = new RelayCommand(GetRelatedDataAction);
            ProductSelected = new RelayCommand(ProductSelectedAction);
            addProEnum = addProductEnum;
            SearchString = searchString;
            GetRelatedDataAction();
            switch (addProductEnum)
            {
                case AddProductEnum.PruductPurchase:
                    FilterCommand = new RelayCommand(ProductPurchaseFilterAction);
                    break;
                case AddProductEnum.AddMedicine:
                    FilterCommand = new RelayCommand(AddMedicineFilterAction);
                    break;
            }
        }

        private void ProductSelectedAction()
        {
            if (string.IsNullOrEmpty(SelectedProductStruct.ID)) return;
            Messenger.Default.Send(SelectedProductStruct, "SelectedProduct");
            Messenger.Default.Send(new NotificationMessage("CloseAddProductView"));
        }

        ~AddProductViewModel()
        {
            SearchString = string.Empty;
            Messenger.Default.Unregister(this);
        }

        #region ----- Define Actions -----
        private void GetRelatedDataAction()
        {
            if (SearchString.Length > 4)
            {
                IsEditing = false;
                HideDisableProduct = false;
                MainWindow.ServerConnection.OpenConnection();
                ProductStructCollection = ProductStructs.GetProductStructsBySearchString(SearchString);
                MainWindow.ServerConnection.CloseConnection();
                if(addProEnum == AddProductEnum.AddMedicine)
                    ProStructCollectionViewSource = new CollectionViewSource { Source = ProductStructCollection.OrderByDescending(p=>p.NHIPrice) };
                else
                {
                    ProStructCollectionViewSource = new CollectionViewSource { Source = ProductStructCollection };
                }
                ProStructCollectionView = ProStructCollectionViewSource.View;
                ProStructCollectionViewSource.Filter += FilterByProductEnable;
                switch (ProductStructCollection.Count)
                {
                    case 0:
                        MessageWindow.ShowMessage("查無此藥品", MessageType.WARNING);
                        break;
                    case 1:
                        SelectedProductStruct = ProductStructCollection[0];
                        ProductSelectedAction();
                        break;
                    default:
                        ProStructCollectionViewSource.View.MoveCurrentToFirst();
                        SelectedProductStruct = (ProductStruct)ProStructCollectionViewSource.View.CurrentItem;
                        break;
                }
            }
        }
        private void ProductPurchaseFilterAction()
        {

        }
        private void FocusUpDownAction(string direction)
        {

        }
        private void AddMedicineFilterAction()
        {
            if(HideDisableProduct)
                ProStructCollectionViewSource.Filter += FilterByProductEnable;
            else
            {
                ProStructCollectionViewSource.Filter -= FilterByProductEnable;
            }
        }
        private void FilterByProductEnable(object sender, FilterEventArgs e)
        {
            if (!(e.Item is ProductStruct src))
                e.Accepted = false;
            else
            {
                e.Accepted = true;
            }
            //else if (src.IsEnable)
            //    e.Accepted = true;
            //else
            //{
            //    e.Accepted = false;
            //}
        }
        #endregion

        #region ----- Define Functions -----

        #endregion

    }
}