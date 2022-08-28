using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Media;

namespace ATMSW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AtmData;Integrated Security=True";
        public MainWindow()
        {
            InitializeComponent();
            cardNum.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Foo));
            code.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Foo));
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) //povoluje zadat pouze čísla
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Foo(object sender, ExecutedRoutedEventArgs e) //zabraňuje uživateli do políčka cokoliv zkopírovaného vložit
        {
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cardNum.Text) || string.IsNullOrEmpty(code.Password))
            {
                MessageBox.Show("Vyplňte prosím všechna políčka");
            }
            else
                Login();
        }

        private void Login() //metoda v případě zadání správných údajů přihlásí uživatele
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT PIN, Id FROM [Client] WHERE cardNumber=@CN", connection);
                command.Parameters.AddWithValue("@CN", cardNum.Text);

                object o = command.ExecuteScalar(); 
                bool valueExists = o != null && o != DBNull.Value; //zjistí pokud existuje uživatel se zadaným číslem karty
                if (valueExists)
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if ((string)reader["PIN"] == code.Password)
                        {   
                            Menu menu = new Menu((int)reader["Id"]);
                            this.Close();
                            menu.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Zadané údaje jsou chybné");
                        }
                    }
                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Uživatel s tímto číslem karty v našem systému neexistuje. Zkontrolujte prosím zadané údaje.");
                }
            }

        }
        private void beep() //obě vstupní políčka vydávají zvuk při změně obsahu
        {
            SoundPlayer player = new SoundPlayer(Properties.Resources.atmBeep);
            player.Play();

        }

        private void code_PasswordChanged(object sender, RoutedEventArgs e)
        {
            beep();
        }

        private void cardNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            beep();
        }
    }
}
