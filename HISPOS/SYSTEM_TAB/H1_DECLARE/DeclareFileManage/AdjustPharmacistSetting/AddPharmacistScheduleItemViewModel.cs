using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AddPharmacistScheduleItemViewModel : ViewModelBase
    {
        private Action<PharmacistScheduleItem> saveCallback;
        private DateTime selectedDate;
        private Employees medicalPersonnels;

        public Employees MedicalPersonnels
        {
            get => medicalPersonnels;
            set
            {
                Set(() => MedicalPersonnels, ref medicalPersonnels, value);
            }
        }

        private CollectionViewSource medicalPersonnelsCollectionViewSource;

        public CollectionViewSource MedicalPersonnelsCollectionViewSource
        {
            get => medicalPersonnelsCollectionViewSource;
            set
            {
                Set(() => MedicalPersonnelsCollectionViewSource, ref medicalPersonnelsCollectionViewSource, value);
            }
        }

        private ICollectionView medicalPersonnelsCollectionView;

        public ICollectionView MedicalPersonnelsCollectionView
        {
            get => medicalPersonnelsCollectionView;
            set
            {
                Set(() => MedicalPersonnelsCollectionView, ref medicalPersonnelsCollectionView, value);
            }
        }

        public RelayCommand<object> Save { get; set; }
        public RelayCommand Cancel { get; set; }

        public AddPharmacistScheduleItemViewModel(Action<PharmacistScheduleItem> saveCallback, DateTime selected)
        {
            MedicalPersonnels = new Employees();
            MainWindow.ServerConnection.OpenConnection();
            MedicalPersonnels.GetEnablePharmacist(selected);
            MainWindow.ServerConnection.CloseConnection();
            MedicalPersonnelsCollectionViewSource = new CollectionViewSource { Source = MedicalPersonnels };
            MedicalPersonnelsCollectionView = MedicalPersonnelsCollectionViewSource.View;
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("IsLocal");
            MedicalPersonnelsCollectionView.GroupDescriptions.Add(groupDescription);
            MedicalPersonnelsCollectionView.SortDescriptions.Add(new SortDescription("IsLocal", ListSortDirection.Descending));
            selectedDate = selected;
            this.saveCallback = saveCallback;
            Save = new RelayCommand<object>(SaveAction);
            Cancel = new RelayCommand(CancelAction);
        }

        private void SaveAction(object selectedItems)
        {
            var selectPharmacists = (selectedItems as ObservableCollection<object>).Cast<Employee>().ToList();
            if (selectPharmacists.Count == 0)
            {
                MessageWindow.ShowMessage("請選擇新增藥師", MessageType.WARNING);
                return;
            }

            foreach (var selected in selectPharmacists)
            {
                var item = new PharmacistScheduleItem { MedicalPersonnel = new DeclareMedicalPersonnel(selected), Date = selectedDate, RegisterTime = DateTime.Now };
                saveCallback(item);
            }
            Messenger.Default.Send(new NotificationMessage("CloseAddPharmacistScheduleItemWindow"));
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAddPharmacistScheduleItemWindow"));
        }
    }
}