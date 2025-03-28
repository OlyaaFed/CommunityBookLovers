using CommunityBookLovers.MessegeBoxes;
using System.Linq;
using System.Windows;

namespace CommunityBookLovers
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtEmail.Focus();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = psw.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите email и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            User user = AuthenticateUser(email, password);

            if (user != null)
            {
                // Показываем сообщение об успешной авторизации
                new MessegeAvtoriz().Show();

                // Открываем главное окно
                new MainCommunityBook(user).Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неправильный email или пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User AuthenticateUser(string email, string password)
        {
            using (var db = new LibraryContext())
            {
                return db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            }
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().Show();
            this.Close();
        }
    }
}