﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using His_Pos.Interface;
using His_Pos.Properties;

namespace His_Pos.Class
{
    public class Customer : IPerson
    {
        #region --IPerson--
        public string Id { get; set; }
        public string Name { get; set; }
        public string IcNumber { get; set; }
        #endregion
        public string Qname { get; set; }
        public string Birthday { get; set; }
        public bool Gender { get; set; }
        public ContactInfo ContactInfo { get; set; }
    }
}
