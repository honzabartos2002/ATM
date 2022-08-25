using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ATMSW
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        Invoice invoice = new Invoice();
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AtmData;Integrated Security=True";
        string pin;
        bool menuNav;
        string cardNum;
        
        public Menu(int id)
        {
            InitializeComponent();
            Update(id);
            inputPasswordBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Foo));
            
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) //povoluje zadat pouze čísla
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Foo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Update(int id)
        {
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT PIN, firstName, lastName, balance, cardNumber FROM [Client] WHERE Id=@ID", connection);
                command.Parameters.AddWithValue("@ID", id);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    nameLabel.Content = (string)reader["firstName"] + " " + (string)reader["lastName"];
                    pin = (string)reader["PIN"];
                    balanceLabel.Content = (decimal)reader["balance"];
                    IdLabel.Content = id;
                    cardNum = (string)reader["cardNumber"];
                }
                connection.Close();
            }
        }

        private void hide()
        {
            withdrawButton.Visibility = Visibility.Hidden;
            balanceButton.Visibility = Visibility.Hidden;
            pinButton.Visibility = Visibility.Hidden;
            expansesButton.Visibility = Visibility.Hidden;
            confirmButton.Visibility = Visibility.Hidden;
            withdrawComboBox.Visibility = Visibility.Hidden;
            infoLabel.Visibility = Visibility.Hidden;
            balanceStackPanel.Visibility = Visibility.Hidden;
            confirmButton.Visibility= Visibility.Hidden;
            withdrawComboBox.Visibility= Visibility.Hidden;
            inputPasswordBox.Visibility = Visibility.Hidden;
            
        }

        private void balanceButton_Click(object sender, RoutedEventArgs e)
        {
            hide();
            balanceStackPanel.Visibility= Visibility.Visible;
            infoLabel.Visibility= Visibility.Visible;
            backButton.Visibility= Visibility.Visible;
            infoLabel.Text = "Váš zůstatek činí:";
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            hide();
            withdrawButton.Visibility = Visibility.Visible;
            balanceButton.Visibility = Visibility.Visible;
            pinButton.Visibility = Visibility.Visible;
            expansesButton.Visibility = Visibility.Visible;
            backButton.Visibility = Visibility.Hidden;
            inputPasswordBox.Password = string.Empty;
            withdrawComboBox.SelectedIndex=0;
            Expanse1.Visibility= Visibility.Hidden;
            Expanse2.Visibility= Visibility.Hidden;
            Expanse3.Visibility= Visibility.Hidden;
            Expanse4.Visibility= Visibility.Hidden;
            Expanse5.Visibility= Visibility.Hidden;
            expansesStackPanel.Visibility= Visibility.Hidden;
            myScrollViewer.VerticalScrollBarVisibility=ScrollBarVisibility.Hidden;
            myScrollViewer.IsEnabled=false;
        }

        private void withdrawButton_Click(object sender, RoutedEventArgs e)
        {
            hide();
            menuNav = true;
            backButton.Visibility = Visibility.Visible;
            infoLabel.Visibility= Visibility.Visible;
            infoLabel.Text = "Zvolte částku, kterou chcete vybrat";
            withdrawComboBox.Visibility= Visibility.Visible;
            confirmButton.Visibility= Visibility.Visible;
        }

        private void pinButton_Click(object sender, RoutedEventArgs e)
        {
            hide();
            menuNav = false;
            backButton.Visibility= Visibility.Visible;
            infoLabel.Visibility= Visibility.Visible;
            infoLabel.Text = "Zadejte váš nový PIN kód";
            inputPasswordBox.Visibility= Visibility.Visible;
            confirmButton.Visibility = Visibility.Visible;
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.ShowDialog();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (menuNav) //výběr hotovosti
            {
                int withdraw = int.Parse(withdrawComboBox.Text);
                if (withdraw <= ((decimal)balanceLabel.Content))
                {
                    decimal newBalance = (decimal)balanceLabel.Content - withdraw;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("UPDATE [Client] SET balance=@newBalance WHERE Id=@id", connection);
                        command.Parameters.AddWithValue("@newBalance", newBalance);
                        command.Parameters.AddWithValue("@id", IdLabel.Content);
                        command.ExecuteNonQuery();
                        confirmButton.Visibility = Visibility.Hidden;
                        withdrawComboBox.Visibility = Visibility.Hidden;
                        withdrawComboBox.Text = string.Empty;
                        infoLabel.Text = "Děkujeme vám za výběr";
                        Update((int)IdLabel.Content);
                        if (MessageBox.Show("Přejete si vytisknout strzenku", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            invoice.PrintInvoice(withdraw, cardNum, (string)nameLabel.Content);
                        }

                        SqlCommand cmd = new SqlCommand("INSERT INTO [Payment] (Datetime, Amount, Title, Client) VALUES (@date, @amount, @title, @client)", connection);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@amount", withdraw);
                        cmd.Parameters.AddWithValue("@title", "Výběr z bankomatu");
                        cmd.Parameters.AddWithValue("@client", IdLabel.Content);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Nemůžete vybrat větši částku než máte na účtě. Váš aktuální zůstatek činí: " + balanceLabel.Content.ToString() + "CZK");
                }
            }

            else //změna PINu
            {
                string newPin = (string)inputPasswordBox.Password;
                if(newPin != null && newPin != string.Empty && newPin.Length==4)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("UPDATE [Client] SET PIN=@newPin WHERE Id=@id", connection);
                        command.Parameters.AddWithValue("@newPin", newPin);
                        command.Parameters.AddWithValue("@id", IdLabel.Content);
                        command.ExecuteNonQuery();
                        infoLabel.Text = "Váš PIN byl úspěšně změněn";
                        inputPasswordBox.Visibility = Visibility.Hidden;
                        inputPasswordBox.Password = string.Empty;
                        confirmButton.Visibility = Visibility.Hidden;
                        connection.Close();
                        Update((int)IdLabel.Content);
                    }
                }

                else
                {
                    MessageBox.Show("PIN kód musí být dlouhý 4 číslice");
                }
                
            }
        }

        private void expansesButton_Click(object sender, RoutedEventArgs e)
        {
            hide();
            backButton.Visibility = Visibility.Visible;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT TOP 5 Datetime, Amount, Title FROM [Payment] WHERE Client=@client ORDER BY Datetime DESC", connection);
                command.Parameters.AddWithValue("@client", IdLabel.Content);
                SqlDataReader reader = command.ExecuteReader();
                int i = 0;
                string[] datetimes = new string[5];
                string[] amounts = new string[5];
                string[] titles = new string[5];
                while (reader.Read() && i<5)
                {
                    datetimes.SetValue(reader.GetDateTime(0).ToString("dd.MM yyyy HH:mm:ss"), i);
                    amounts.SetValue(reader.GetValue(1).ToString(), i);
                    titles.SetValue(reader.GetString(2), i);
                    i++;
                }

                if (datetimes[0] != null)
                {
                    Expanse1.Content = "Datum: " + datetimes[0] + "\nČástka: " + amounts[0] + "Kč" + "\nNázev: " + titles[0];
                    Expanse1.Visibility = Visibility.Visible;
                    expansesStackPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    infoLabel.Visibility = Visibility.Visible;
                    infoLabel.Text = "Na vašem účtu nebyly dosud provedeny žádné platby";
                }
                if (datetimes[1] != null)
                {
                    Expanse2.Content = "Datum: " + datetimes[1] + "\nČástka: " + amounts[1] + "Kč" + "\nNázev: " + titles[1];
                    Expanse2.Visibility = Visibility.Visible;
                }
                if (datetimes[2] != null)
                {
                    Expanse3.Content = "Datum: " + datetimes[2] + "\nČástka: " + amounts[2] + "Kč" + "\nNázev: " + titles[2];
                    Expanse3.Visibility = Visibility.Visible;
                }
                if(datetimes[3] != null)
                {
                    Expanse4.Content = "Datum: " + datetimes[3] + "\nČástka: " + amounts[3] + "Kč" + "\nNázev: " + titles[3];
                    Expanse4.Visibility = Visibility.Visible;
                    myScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    myScrollViewer.IsEnabled= true;
                }
                if (datetimes[4] != null)
                {
                    Expanse5.Content = "Datum: " + datetimes[4] + "\nČástka: " + amounts[4] + "Kč" + "\nNázev: " + titles[4];
                    Expanse5.Visibility = Visibility.Visible;
                }
                connection.Close();
            }
        }
    }
}

