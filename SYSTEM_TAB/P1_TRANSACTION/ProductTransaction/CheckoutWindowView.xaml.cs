using His_Pos.Class;
using His_Pos.FunctionWindow;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// CheckoutWindowView.xaml 的互動邏輯
    /// </summary>
    public partial class CheckoutWindowView : Window
    {
        private int Total;
        private int Cash;
        private int Voucher;
        private int CashCoupon;
        private int Card;
        private int Change;
        private string TaxNumber;
        private string CardNum1;
        private string CardNum2;
        private string CardNum3;
        private string CardNum4;
        private string Employee;

        private bool IsPayAmountEnough = false;
        private bool IsSelectCashier = false;

        private static readonly Regex _regex = new Regex("^[0-9]+$");
        private static bool IsTextAllowed(string text) { return _regex.IsMatch(text); }

        public CheckoutWindowView(int total, int linecount, int itemcount)
        {
            InitializeComponent();
            Total = total;
            lblTotal.Content = total.ToString();
            ChangeCount();
            lblLineCount.Content = linecount.ToString();
            lblItemCount.Content = itemcount.ToString();
            tbTaxNum.Focus();
        }

        private void SubmitCheckout() 
        {
            if (!IsPayAmountEnough)
            {
                MessageWindow.ShowMessage("支付金額不足！", MessageType.WARNING);
                return;
            }
            if (!IsSelectCashier)
            {
                MessageWindow.ShowMessage("尚未選擇結帳人員！", MessageType.WARNING);
                return;
            }

        }

        private void ChangeCount() 
        {
            int.TryParse(tbCash.Text, out int cash);
            int.TryParse(tbVoucher.Text, out int voucher);
            int.TryParse(tbCashCoupon.Text, out int cashcoupon);
            int.TryParse(tbCard.Text, out int card);
            int change = (cash + voucher + cashcoupon + card) - Total;
            tbChange.Content = change;
            if (change >= 0)
            {
                IsPayAmountEnough = true;
            }
            else 
            {
                IsPayAmountEnough = false;
            }
        }

        private void tbTaxNum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter) 
            {
                if (tb.Text.Length != 0 && tb.Text.Length != 8) 
                {
                    MessageWindow.ShowMessage("統一編號位數有誤", MessageType.WARNING);
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
                ChangeCount();
                tbVoucher.Focus();
            }
        }

        private void tbVoucher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                ChangeCount();
                tbCashCoupon.Focus();
            }
        }

        private void tbCashCoupon_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                ChangeCount();
                tbCard.Focus();
            }
        }

        private void tbCard_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                ChangeCount();
                tbCardNum1.Focus();
            }
        }

        private void tbEmployee_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSubmit.Focus();
            }
        }

        #region CardNum

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

        #endregion

        #region LostFocus

        private void tbCash_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { tb.Text = "0"; }
        }

        private void tbVoucher_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { tb.Text = "0"; }
        }

        private void tbCashCoupon_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { tb.Text = "0"; }
        }

        private void tbCard_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { tb.Text = "0"; }
        }

        #endregion

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            SubmitCheckout();
        }
    }
}
