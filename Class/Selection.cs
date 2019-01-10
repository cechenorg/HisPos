using System.ComponentModel;

namespace His_Pos.Class
{
    public abstract class Selection : INotifyPropertyChanged
    {
        protected Selection()
        {
            Id = "";
            Name = "";
        }
        private string id;
        public string Id
        {
            get => id;
            set
            {
                id = value;
                FullName = Id + " " +  Name;
                NotifyPropertyChanged("Id");
            }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                FullName = Id + " " + Name;
                NotifyPropertyChanged("Name");
            }
        }
        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                NotifyPropertyChanged("FullName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}