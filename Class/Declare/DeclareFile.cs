﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace His_Pos.Class.Declare
{
    public class DeclareFile:INotifyPropertyChanged
    {
        public DeclareFile() { }

        public DeclareFile(DataRow row)
        {
            Prescriptions = new ObservableCollection<Prescription>();
            DdataCollection = new ObservableCollection<Ddata>();
            Id = row["FILE_ID"].ToString();
            DeclareTime = row["DECLARE_TIME"].ToString();
            if(!string.IsNullOrEmpty(row["CHRONIC_COUNT"].ToString()))
                ChronicCount = int.Parse(row["CHRONIC_COUNT"].ToString());
            if (!string.IsNullOrEmpty(row["NORMAL_COUNT"].ToString()))
                NormalCount = int.Parse(row["NORMAL_COUNT"].ToString());
            if (!string.IsNullOrEmpty(row["ERROR_COUNT"].ToString()))
                ErrorCount = int.Parse(row["ERROR_COUNT"].ToString());
            if (!string.IsNullOrEmpty(row["TOTALPOINT"].ToString()))
                TotalPoint = double.Parse(row["TOTALPOINT"].ToString());
        }

        public string Id { get; set; }

        private string _declareTime;

        public string DeclareTime
        {
            get => _declareTime;
            set
            {
                _declareTime = value; 
                OnPropertyChanged(nameof(DeclareTime));
            }
        }

        private int _chronicCount;

        public int ChronicCount
        {
            get => _chronicCount;
            set
            {
                _chronicCount = value;
                OnPropertyChanged(nameof(ChronicCount));
            }
        }
        private int _normalCount;
        public int NormalCount
        {
            get => _normalCount;
            set
            {
                _normalCount = value;
                OnPropertyChanged(nameof(NormalCount));
            }
        }

        private int _errorCount;
        public int ErrorCount
        {
            get => _errorCount;
            set
            {
                _errorCount = value;
                OnPropertyChanged(nameof(ErrorCount));
            }
        }

        private double _totalPoint;
        public double TotalPoint
        {
            get => _totalPoint;
            set
            {
                _totalPoint = value;
                OnPropertyChanged(nameof(TotalPoint));
            }
        }


        
        public ObservableCollection<Prescription> Prescriptions { get; set; }

        public ObservableCollection<Ddata> DdataCollection { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private string SerializeToDdata(string xmlStr)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(Tdata));
        //    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlStr));
        //    Tdata tData = (Tdata)serializer.Deserialize(memStream);
        //}
    }
}
