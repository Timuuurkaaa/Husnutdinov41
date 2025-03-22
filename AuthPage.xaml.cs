using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Husnutdinov41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public int AuthFailscount = 0;
        public string Capchacharacters = "";
        public AuthPage()
        {
            InitializeComponent();
        }
        private void LoginGuest_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ShoesPage(null));
            LoginBox.Text = "";
            PasswordBox.Text = "";
            CapchaStackPanel.Visibility = Visibility.Hidden;
            CapchaTB.Visibility = Visibility.Hidden;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Text;
            if(login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля!");
                return;
            }
            else
            {
                if (AuthFailscount > 0 && CapchaTB.Text == "")
                {
                    AuthFailscount = 0;
                    MessageBox.Show("Введите капчу!");
                }
                    
                else
                {
                    User user = Husnutdinov41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);

                    if (user != null && (AuthFailscount == 0 || Capchacharacters == CapchaTB.Text))
                    {
                        Manager.MainFrame.Navigate(new ShoesPage(user));
                        login = "";
                        password = "";
                        Capchacharacters = "";
                        captchaOneWord.Text = "";
                        captchaTwoWord.Text = "";
                        captchaThreeWord.Text = "";
                        captchaFourWord.Text = "";
                        CapchaTB.Text = "";
                        CapchaStackPanel.Visibility = Visibility.Hidden;
                        CapchaTB.Visibility = Visibility.Hidden;
                        LoginBox.Text = "";
                        PasswordBox.Text = "";

                    }
                    else
                    {
                        if (AuthFailscount > 0)
                            MessageBox.Show("Логин, пароль или капча введены неверно!");
                        else
                            MessageBox.Show("Логин или пароль введены неверно!");
                        CapchaStackPanel.Visibility = Visibility.Visible;
                        CapchaTB.Visibility = Visibility.Visible;
                        Random random = new Random();
                        string symbols = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
                        Capchacharacters = (symbols[random.Next(symbols.Length)].ToString() + symbols[random.Next(symbols.Length)].ToString() + symbols[random.Next(symbols.Length)].ToString() + symbols[random.Next(symbols.Length)].ToString());
                        captchaOneWord.Text = Capchacharacters[0].ToString();
                        captchaTwoWord.Text = Capchacharacters[1].ToString();
                        captchaThreeWord.Text = Capchacharacters[2].ToString();
                        captchaFourWord.Text = Capchacharacters[3].ToString();

                        captchaOneWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);
                        captchaTwoWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);
                        captchaThreeWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);
                        captchaFourWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);

                        AuthFailscount++;

                        if (AuthFailscount >= 2)
                        {
                            Login.IsEnabled = false;
                            MessageBox.Show("Данные введены неверно, подождите 10 секунд!");
                            await Task.Delay(10000);
                            Login.IsEnabled = true;
                        }
                    }
                }
            }
        }
        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}