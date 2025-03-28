using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    /// Логика взаимодействия для ReadBookWindow.xaml
    /// </summary>
    public partial class ReadBookWindow : Window
    {
        public ReadBookWindow(string filePath)
        {
            InitializeComponent();

            if (File.Exists(filePath))
            {
                pdfViewer.Navigate(new Uri(filePath)); // Открытие PDF в WebBrowser
            }
            else
            {
                MessageBox.Show("Файл книги не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
