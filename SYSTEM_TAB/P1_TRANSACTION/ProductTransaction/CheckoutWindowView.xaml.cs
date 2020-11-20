using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// CheckoutWindowView.xaml 的互動邏輯
    /// </summary>
    public partial class CheckoutWindowView : Window
    {
        private int Total;
        private int Linecount;
        private int Itemcount;

        private string TaxNumber;
        private int Cash;
        private int Voucher;
        private int CashCoupon;
        private int Card;
        private string CardNumber;
        private int Change;
        private string Employee;

        public CheckoutWindowView(int total, int linecount, int itemcount)
        {
            InitializeComponent();
            Total = total;
            Linecount = linecount;
            Itemcount = itemcount;
            tbTaxNum.Text = total.ToString();
            lblLineCount.Content = linecount.ToString();
            lblItemCount.Content = itemcount.ToString();
            tbTaxNum.Focus();
        }

        private void SubmitCheckout() 
        {

        }

        private void ChangeCount() 
        {

        }

        private void tbTaxNum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter) 
            {
                if (tb.Text.Length != 8) 
                {
                    MessageWindow.ShowMessage("統一編號長度有誤", MessageType.WARNING);
                    return;
                }
                tbCash.Focus();
            }
        }

        private void tbCash_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                tbVoucher.Focus();
            }
        }

        private void tbVoucher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                tbCashCoupon.Focus();
            }
        }

        private void tbCashCoupon_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                tbCard.Focus();
            }
        }

        private void tbCard_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                tbCardNum1.Focus();
            }
        }

        private void tbCardNum1_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length == 4) 
            {
                tbCardNum2.Focus();
            }
        }

        private void tbCardNum2_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length == 4)
            {
                tbCardNum3.Focus();
            }
        }

        private void tbCardNum3_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length == 4)
            {
                tbCardNum4.Focus();
            }
        }

        private void tbCardNum4_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length == 4)
            {
                tbEmployee.Focus();
            }
        }

        private void tbEmployee_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                btnSubmit.Focus();
            }
        }
    }
}
