using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.IndexReserve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.INDEX.ReserveSendConfirmWindow
{
    public class ReserveSendConfirmViewModel : ViewModelBase
    {
        private IndexReserves indexReserveCollection;
        public IndexReserves IndexReserveCollection
        {
            get => indexReserveCollection;
            set
            {
                Set(() => IndexReserveCollection, ref indexReserveCollection, value);
            }
        }
        private IndexReserve indexReserveSelectedItem;
        public IndexReserve IndexReserveSelectedItem
        {
            get => indexReserveSelectedItem;
            set
            {
                Set(() => IndexReserveSelectedItem, ref indexReserveSelectedItem, value);
            }
        }
        public ReserveSendConfirmViewModel(IndexReserves indexReserves) {
            IndexReserveCollection = indexReserves;
        }
    }
}
