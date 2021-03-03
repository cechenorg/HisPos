using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.Customer.CustomerPrescriptionChanged;

namespace His_Pos.SYSTEM_TAB.INDEX.CustomerPrescriptionChangedWindow
{
    public class CustomerPrescriptionChangedViewModel : ViewModelBase
    {
        private CustomerPrescriptionChangeds customerPrescriptionChangedsCollection = new CustomerPrescriptionChangeds();

        public CustomerPrescriptionChangeds CustomerPrescriptionChangedsCollection
        {
            get => customerPrescriptionChangedsCollection;
            set
            {
                Set(() => CustomerPrescriptionChangedsCollection, ref customerPrescriptionChangedsCollection, value);
            }
        }

        public CustomerPrescriptionChangedViewModel(int cusID)
        {
            CustomerPrescriptionChangedsCollection.GetDataByCudID(cusID);
        }
    }
}