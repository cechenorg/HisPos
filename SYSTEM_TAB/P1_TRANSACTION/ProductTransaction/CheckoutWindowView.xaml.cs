using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        private DataTable EmployeeList;
        private int Total;
        private int paid;
        private string cardNumber = "";

        private int cash = 0;
        private int realcash = 0;
        private int voucher = 0;
        private int cashcoupon = 0;
        private int card = 0;
        private int change = 0;

        public int Cash => realcash;
        public int Voucher => voucher;
        public int CashCoupon => cashcoupon;
        public int Card => card;
        public int Paid => paid;
        public int Change => change;
        public string TaxNumber => tbTaxNum.Text;
        public string CardNumber => cardNumber;
        public string Employee => tbEmployee.Text;
        public string PayMethod => GetPayMethod();

        private bool IsPayAmountEnough = false;

        private static readonly Regex _regex = new Regex("^[0-9]+$");

        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }

        public CheckoutWindowView(int total, int linecount, int itemcount)
        {
            InitializeComponent();
            Total = total;
            lblTotal.Content = total.ToString();
            lblLineCount.Content = linecount.ToString();
            lblItemCount.Content = itemcount.ToString();
            GetEmployeeList();
            CardNumberControl();
            ChangeCount();
            tbTaxNum.Focus();
        }

        private void GetEmployeeList()
        {
            MainWindow.ServerConnection.OpenConnection();
            EmployeeList = MainWindow.ServerConnection.ExecuteProc("[POS].[GetEmployee]");
            MainWindow.ServerConnection.CloseConnection();
        }

        private bool IsEmployeeIDValid()
        {
            string empCashierID = tbEmployee.Text;
            if (empCashierID == string.Empty)
            {
                return false;
            }
            else 
            {
                bool contains = EmployeeList.AsEnumerable().Any(row => empCashierID == row.Field<string>("Emp_CashierID"));
                return contains;
            }
        }

        private string GetPayMethod()
        {
            List<string> list = new List<string>();
            bool CashParse = int.TryParse(tbCash.Text, out cash);
            bool VoucherParse = int.TryParse(tbVoucher.Text, out voucher);
            bool CashCouponParse = int.TryParse(tbCashCoupon.Text, out cashcoupon);
            bool CardParse = int.TryParse(tbCard.Text, out card);

            if (CashParse && realcash > 0)
            {
                list.Add("現金");
            }
            if (VoucherParse && voucher > 0)
            {
                list.Add("禮券");
            }
            if (CashCouponParse && cashcoupon > 0)
            {
                list.Add("現金券");
            }
            if (CardParse && card > 0)
            {
                list.Add("信用卡");
            }

            string[] arr = list.ToArray();
            string result = string.Join("/", arr);
            return result;
        }

        private void CardNumberControl()
        {
            if (!int.TryParse(tbCard.Text, out int tmp)) { return; }
            bool isCard = int.Parse(tbCard.Text) > 0 ? true : false;
            if (isCard)
            {
                tbCardNum1.IsEnabled = true;
                tbCardNum2.IsEnabled = true;
                tbCardNum3.IsEnabled = true;
                tbCardNum4.IsEnabled = true;
            }
            else
            {
                tbCardNum1.IsEnabled = false;
                tbCardNum2.IsEnabled = false;
                tbCardNum3.IsEnabled = false;
                tbCardNum4.IsEnabled = false;
            }
        }

        private bool CheckCardNumber()
        {
            string FullCard = tbCardNum1.Text + tbCardNum2.Text + tbCardNum3.Text + tbCardNum4.Text;
            if (FullCard.Length == 16)
            {
                cardNumber = FullCard;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ChangeCount()
        {
            int.TryParse(tbCash.Text, out cash);
            int.TryParse(tbVoucher.Text, out voucher);
            int.TryParse(tbCashCoupon.Text, out cashcoupon);
            int.TryParse(tbCard.Text, out card);

            int nochange = voucher + cashcoupon + card;
            if (nochange >= Total)
            {
                change = cash;
            }
            else
            {
                change = (cash + voucher + cashcoupon + card) - Total;
            }
            tbChange.Content = change;
            paid = cash + voucher + cashcoupon + card;
            realcash = cash - change;

            if (change >= 0)
            {
                IsPayAmountEnough = true;
            }
            else
            {
                IsPayAmountEnough = false;
            }
        }

        private void SubmitCheckout()
        {
            if (!IsPayAmountEnough)
            {
                MessageWindow.ShowMessage("支付金額不足！", MessageType.WARNING);
                return;
            }
            if (!IsEmployeeIDValid())
            {
                MessageWindow.ShowMessage("結帳人員輸入錯誤！", MessageType.WARNING);
                return;
            }
            int.TryParse(tbCard.Text, out int card);
            if (card > 0 && !CheckCardNumber())
            {
                MessageWindow.ShowMessage("信用卡號輸入有誤！", MessageType.WARNING);
                return;
            }
            if (card == 0)
            {
                cardNumber = "";
            }

            DialogResult = true;
        }

        private void TextBoxLostFocus(object sender)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { tb.Text = "0"; }
        }

        #region PreviewKeyDown

        private void tbTaxNum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter || e.Key == Key.Down)
            {
                if (tb.Text.Length != 0 && tb.Text.Length != 8)
                {
                    if (!IsTextAllowed(tb.Text))
                    {
                        MessageWindow.ShowMessage("統一編號輸入有誤", MessageType.WARNING);
                        return;
                    }
                    MessageWindow.ShowMessage("統一編號位數有誤", MessageType.WARNING);
                    return;
                }

                tbCash.Focus();
            }
        }

        private void tbCash_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Down)
            {
                ChangeCount();
                tbVoucher.Focus();
            }
            if (e.Key == Key.Up)
            {
                ChangeCount();
                tbTaxNum.Focus();
            }
        }

        private void tbVoucher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Down)
            {
                ChangeCount();
                tbCashCoupon.Focus();
            }
            if (e.Key == Key.Up)
            {
                ChangeCount();
                tbCash.Focus();
            }
        }

        private void tbCashCoupon_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Down)
            {
                ChangeCount();
                tbCard.Focus();
            }
            if (e.Key == Key.Up)
            {
                ChangeCount();
                tbVoucher.Focus();
            }
        }

        private void tbCard_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (tbCardNum1.IsEnabled)
            {
                if (e.Key == Key.Enter || e.Key == Key.Down)
                {
                    ChangeCount();
                    tbCardNum1.Focus();
                }
                if (e.Key == Key.Up)
                {
                    ChangeCount();
                    tbCashCoupon.Focus();
                }
            }
            else
            {
                if (e.Key == Key.Enter || e.Key == Key.Down)
                {
                    ChangeCount();
                    tbEmployee.Focus();
                }
                if (e.Key == Key.Up)
                {
                    ChangeCount();
                    tbCashCoupon.Focus();
                }
            }
        }

        private void tbCardNum1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Down)
            {
                tbEmployee.Focus();
            }
            if (e.Key == Key.Up)
            {
                tbCard.Focus();
            }
        }

        private void tbEmployee_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (tbCardNum1.IsEnabled)
            {
                if (e.Key == Key.Enter)
                {
                    btnSubmit.Focus();
                }
                if (e.Key == Key.Up)
                {
                    tbCardNum1.Focus();
                }
            }
            else
            {
                if (e.Key == Key.Enter)
                {
                    btnSubmit.Focus();
                }
                if (e.Key == Key.Up)
                {
                    tbCard.Focus();
                }
            }
        }

        #endregion PreviewKeyDown

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

        #endregion CardNum

        #region GotFocus

        private void tbCash_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void tbVoucher_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void tbCashCoupon_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void tbCard_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        #endregion GotFocus

        #region LostFocus

        private void tbCash_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxLostFocus(sender);
        }

        private void tbVoucher_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxLostFocus(sender);
        }

        private void tbCashCoupon_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxLostFocus(sender);
        }

        private void tbCard_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxLostFocus(sender);
            CardNumberControl();
        }

        #endregion LostFocus

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            ChangeCount();
            SubmitCheckout();
        }

        private void tbCard_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                CardNumberControl();
            }
        }
    }
}