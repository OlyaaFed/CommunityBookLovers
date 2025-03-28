using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CommunityBookLovers
{
    public partial class AddBookWindow : Window
    {
        private readonly LibraryContext _context;
        private string _coverImagePath;
        private string _bookFilePath;
        private User currentUser;

        public AddBookWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            LoadAuthorsAndGenres();
            StateComboBox.SelectedIndex = 0;
        }

        private void LoadAuthorsAndGenres()
        {
            try
            {
                AuthorComboBox.ItemsSource = _context.Authors.ToList();
                GenreComboBox.ItemsSource = _context.Genres.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите обложку книги"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _coverImagePath = openFileDialog.FileName;
                CoverImage.Source = new BitmapImage(new Uri(_coverImagePath));
            }
        }

        private void SelectBookFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы книг (*.pdf;*.epub;*.fb2)|*.pdf;*.epub;*.fb2",
                Title = "Выберите файл книги"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _bookFilePath = openFileDialog.FileName;
                BookFilePathText.Text = Path.GetFileName(_bookFilePath);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация обязательных полей
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Введите название книги", "Обязательное поле",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (AuthorComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите автора", "Обязательное поле",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (GenreComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите жанр", "Обязательное поле",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создание новой книги
                var newBook = new Book
                {
                    Title = TitleTextBox.Text,
                    AuthorId = (int)AuthorComboBox.SelectedValue,
                    GenreId = (int)GenreComboBox.SelectedValue,
                    PublicationYear = int.TryParse(PublicationYearTextBox.Text, out var year) ? year : null,
                    Pages = int.TryParse(PagesTextBox.Text, out var pages) ? pages : null,
                    Description = DescriptionTextBox.Text,
                    ImagePath = _coverImagePath,
                    PathToFile = _bookFilePath,
                    State = (StateComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Без статуса"
                };

                _context.Books.Add(newBook);
                _context.SaveChanges();

                MessageBox.Show("Книга успешно добавлена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении книги: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BookWindow bookWindow = new BookWindow(currentUser);
            bookWindow.Show();
            this.Close();
        }
    }
}