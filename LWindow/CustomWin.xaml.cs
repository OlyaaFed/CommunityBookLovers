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
using System.Windows.Shapes;

namespace CommunityBookLovers.LWindow
{
    /// <summary>
    /// Логика взаимодействия для CustomWin.xaml
    /// </summary>
    public partial class CustomWin : Window
    {
        public bool Result { get; private set; } = false;

        public CustomWin()
        {
            InitializeComponent();
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем флаг на "истину" и закрываем окно
            Result = true;

            // Проверяем, существует ли уже окно MainWindow
            var existingMainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (existingMainWindow == null)
            {
                // Если окно не существует, создаем и показываем его
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }

            // Закрываем текущее окно
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем флаг на "ложь" и закрываем окно
            Result = false;
            this.Close();
        }



    }
}
