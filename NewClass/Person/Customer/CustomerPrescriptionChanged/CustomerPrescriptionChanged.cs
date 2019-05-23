using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.Customer.CustomerPrescriptionChanged
{
    public class CustomerPrescriptionChanged: ObservableObject
    {
        public CustomerPrescriptionChanged(DataRow r) {
            Message = r.Field<string>("Message");
            Time = r.Field<string>("Time"); 
        }
        public string Message { get; set; }

        public string Time { get; set; }
    }
}
