using System;
using System.ComponentModel;
using System.Data;

namespace His_Pos.Class.Leave
{
    public class AuthLeaveRecord : INotifyPropertyChanged
    {
        public AuthLeaveRecord(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Name = dataRow["EMP_NAME"].ToString();
            LeaveType = dataRow["EMPLEVTYP_NAME"].ToString();
            Dates = dataRow["LEAVEDATE"].ToString();
            Notes = dataRow["EMPLEVREC_NOTE"].ToString();
            InsertTime = DateTime.Parse(dataRow["EMPLEVREC_INSERTDATE"].ToString());
            IsSelected = false;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string LeaveType { get; set; }
        public string Dates { get; set; }
        public string Notes { get; set; }
        public DateTime InsertTime { get; }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
