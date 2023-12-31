﻿using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryPrincipals : ObservableCollection<ManufactoryPrincipal>, ICloneable
    {
        private ManufactoryPrincipals()
        {
        }

        public ManufactoryPrincipals(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ManufactoryPrincipal(row));
            }
        }

        #region ----- Define Functions -----

        #region ///// Static Functions /////

       
        #endregion ///// Static Functions /////

        public void AddNewPrincipal()
        {
            Add(new ManufactoryPrincipal());
        }

        public void ResetData(ManufactoryPrincipals principals)
        {
            Clear();

            foreach (var principal in principals)
            {
                Add(principal.Clone() as ManufactoryPrincipal);
            }
        }

        public object Clone()
        {
            ManufactoryPrincipals principals = new ManufactoryPrincipals();

            foreach (var principal in Items)
                principals.Add(principal.Clone() as ManufactoryPrincipal);

            return principals;
        }

        #endregion ----- Define Functions -----
    }
}