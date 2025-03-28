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
    /// Логика взаимодействия для PdfReaderWindow.xaml
    /// </summary>
    public partial class PdfReaderWindow : Window
    {
        public PdfReaderWindow(string pdfPath)
        {
            InitializeComponent();
            LoadPdf(pdfPath);
        }

        private void LoadPdf(string pdfPath)
        {
            try
            {
                pdfViewer.Navigate(new Uri(pdfPath)); // Открытие PDF в браузере
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки PDF: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
}
