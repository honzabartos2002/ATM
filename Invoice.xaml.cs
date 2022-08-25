using System;
using System.Windows.Controls;

namespace ATMSW
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class Invoice : Page
    {
        public Invoice()
        {
            InitializeComponent();
        }

        public void PrintInvoice(int amount, string cardNumber, string client)
        {
            datetimeLabel.Content=DateTime.Now.ToString("dddd, dd.MMMM yyyy, HH:mm:ss");
            amountLabel.Content=amount;
            clientLabel.Content = client;
            cardnumberLabel.Content=Encode(cardNumber);
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(print, "Invoice");
                }
            }
            finally
            {
                this.IsEnabled=true;
            }
        }

        public string Encode(string cardNumber)
        {
            char[] ch = cardNumber.ToCharArray();
            if(cardNumber.Length <= 4)
            {
                for (int i = 0; i < (cardNumber.Length - 2); i++)
                {
                    ch[i] = '*';
                }
            }
            for (int i = 0; i < (cardNumber.Length-4); i++)
            {
                ch[i] = '*';
            }
            string newstring = new string(ch);
            return newstring;
            
        }
    }
}
