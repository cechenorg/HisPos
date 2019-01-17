using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine
{
    public class Medicine:Product
    {
        public Medicine() : base()
        {

        }
        public Medicine(DataRow r) : base(r)
        {

        }
        private double amount;//總量
        public double Amount
        {
            get => amount;
            set
            {
                if (amount != value)
                {
                    Set(() => Amount, ref amount, value);
                }
            }
        }
        private double dosage;//每次用量
        public double Dosage
        {
            get => dosage;
            set
            {
                if (dosage != value)
                {
                    Set(() => Dosage, ref dosage, value);
                }
            }
        }
        private Usage.Usage usage;//用法
        public Usage.Usage Usage
        {
            get => usage;
            set
            {
                if (usage != value)
                {
                    Set(() => Usage, ref usage, value);
                }
            }
        }
        private Position.Position position;//用法
        public Position.Position Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    Set(() => Position, ref position, value);
                }
            }
        }
        private int days;
        public int Days
        {
            get => days;
            set
            {
                if (days != value)
                {
                    Set(() => Days, ref days, value);
                }
            }
        }
        private double price;
        public double Price
        {
            get => price;
            set
            {
                if (price != value)
                {
                    Set(() => Price, ref price, value);
                }
            }
        }
        private double nhiPrice;
        public double NHIPrice
        {
            get => nhiPrice;
            set
            {
                if (nhiPrice != value)
                {
                    Set(() => NHIPrice, ref nhiPrice, value);
                }
            }
        }
        private double totalPrice;
        public double TotalPrice
        {
            get => totalPrice;
            set
            {
                if (totalPrice != value)
                {
                    Set(() => TotalPrice, ref totalPrice, value);
                }
            }
        }
    }
}
