using CommunityBookLovers.LWindow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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
using CommunityBookLovers.MessegeBoxes;

namespace CommunityBookLovers
{
    /// <summary>
    /// Логика взаимодействия для AboutMeWindow.xaml
    /// </summary>
    public partial class AboutMeWindow : Window
    {
        private User currentUser;
        private bool isEditing = false;
        private LibraryContext _context;

        public AboutMeWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            DisplayUserInfo();
            _context = new LibraryContext();
            LoadRecentBooks();
        }

        private void LoadRecentBooks()
        {
            // Получаем последние прочитанные книги
            var recentBooks = _context.Bookshelves
                .Where(bs => bs.UserId == currentUser.UserId && bs.State == "Прочитал") // Фильтруем по состоянию "Прочитано"
                .OrderByDescending(bs => bs.DataAdded) // Сортировка по дате добавления
                .Take(5) // Ограничиваем количество книг
                .Select(bs => new
                {
                    Title = bs.Book.Title,
                    ImagePath = bs.Book.ImagePath // Извлекаем путь к изображению книги
                })
                .ToList();

            // Привязываем список книг к ItemsControl
            RecentBooksList.ItemsSource = recentBooks;
        }

        private void DisplayUserInfo()
        {
            if (!isEditing)
            {
                txtName.Text = currentUser.Name;
                txtEmail.Text = currentUser.Email;

                if (!string.IsNullOrEmpty(currentUser.ProfileImagePath))
                {
                    try
                    {
                        Uri imageUri;
                        if (Uri.TryCreate(currentUser.ProfileImagePath, UriKind.Absolute, out imageUri) && imageUri.IsFile)
                        {
                            if (File.Exists(currentUser.ProfileImagePath))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = imageUri;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                                ProfileImage.Source = bitmap;
                            }
                            else
                            {
                                MessageBox.Show("Файл изображения не найден.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректный путь к изображению.");
                        }
                    }
                    catch (Exception ex) when (
                        ex is ArgumentException ||
                        ex is NotSupportedException ||
                        ex is SecurityException ||
                        ex is IOException)
                    {
                        MessageBox.Show($"Произошла ошибка при загрузке изображения: {ex.Message}");
                    }
                }

                btnEdit.Visibility = Visibility.Visible;
                txtName.IsReadOnly = true;
                txtEmail.IsReadOnly = true;
                btnSave.Visibility = Visibility.Collapsed;
                
            }
            else
            {
                txtName.IsReadOnly = false;
                txtEmail.IsReadOnly = false;
                btnSave.Visibility = Visibility.Visible;
                ;
                btnEdit.Visibility = Visibility.Collapsed;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Включаем режим редактирования
            txtName.IsReadOnly = false;
            txtEmail.IsReadOnly = false;
            btnUploadImage.Visibility = Visibility.Visible; // Показываем кнопку загрузки фото
            btnSave.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Collapsed;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Поле 'Имя' не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Поле 'Email' не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Введите корректный email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Сохраняем изменения
            currentUser.Name = txtName.Text;
            currentUser.Email = txtEmail.Text;
            SaveUserData();

            // Выключаем режим редактирования
            txtName.IsReadOnly = true;
            txtEmail.IsReadOnly = true;
            btnUploadImage.Visibility = Visibility.Collapsed; // Скрываем кнопку загрузки фото
            btnSave.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Visible;

            // Уведомление об успешном сохранении
            MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалог выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Загружаем новое изображение
                string imagePath = openFileDialog.FileName;
                currentUser.ProfileImagePath = imagePath; // Сохраняем путь к изображению
                Uri imageUri = new Uri(imagePath);
                BitmapImage bitmap = new BitmapImage(imageUri);
                ProfileImage.Source = bitmap;

                // Уведомление об успешной загрузке фото
                MessageBox.Show("Фото успешно загружено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void MenuItem_Home_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новое окно главной страницы
            var mainWindow = new MainCommunityBook(currentUser);
            mainWindow.Show();

            // Закрываем текущее окно
            this.Close();
        }
        private void SaveUserData()
        {
            // Сохраняем данные пользователя в базу данных
            using (LibraryContext db = new LibraryContext())
            {
                db.Users.Update(currentUser);
                db.SaveChanges();
            }
        }

        // Метод для проверки корректности email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainCommunityBook mainCommunityBook = new MainCommunityBook(currentUser);
            mainCommunityBook.Show();
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var customMessageBox = new CustomWin();
            customMessageBox.ShowDialog();

            if (customMessageBox.Result)
            {
                foreach (var window in Application.Current.Windows)
                {
                    if (window is AboutMeWindow || window is CustomWin)
                    {
                        continue;
                    }

                    (window as Window)?.Close();
                }
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void MenuItem_AboutMe_Click(object sender, RoutedEventArgs e)
        {
            MessegeAboutme messegeAboutme = new MessegeAboutme();
            messegeAboutme.Show();
        }

        private void MenuItem_MyShelf_Click(object sender, RoutedEventArgs e)
        {
            BookshelfWindow bookshelfWindow = new BookshelfWindow(currentUser);
            bookshelfWindow.Show();
            this.Close();
        }

        private void MenuItem_Books_Click(object sender, RoutedEventArgs e)
        {
            BookWindow bookWindow = new BookWindow(currentUser);
            bookWindow.Show();
            this.Close();
        }

        private void MenuItem_Friends_Click(object sender, RoutedEventArgs e)
        {
            var friendsWindow = new CommunityBook(currentUser);
            friendsWindow.Show();
            this.Close();
        }

        private void MenuItem_Review_Click(object sender, RoutedEventArgs e)
        {
            ReviewWindow reviewWindow = new ReviewWindow(currentUser, _context);
            reviewWindow.Show();
            this.Close();
        }
    }
}

//using CommunityBookLovers.LWindow;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Win32;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using CommunityBookLovers.MessegeBoxes;
//namespace CommunityBookLovers
//{
//    /// <summary>
//    /// Логика взаимодействия для AboutMeWindow.xaml
//    /// </summary>
//    public partial class AboutMeWindow : Window
//    {
//        private User currentUser;
//        private bool isEditing = false;
//        private LibraryContext _context;
//        public AboutMeWindow(User user)
//        {
//            InitializeComponent();
//            currentUser = user;
//            DisplayUserInfo();
//        }

//        private void DisplayUserInfo()
//        {
//            if (!isEditing)
//            {
//                // Отображаем данные пользователя
//                txtName.Text = currentUser.Name;
//                txtEmail.Text = currentUser.Email;

//                if (!string.IsNullOrEmpty(currentUser.ProfileImagePath))
//                {
//                    try
//                    {
//                        Uri imageUri = new Uri(currentUser.ProfileImagePath);
//                        if (File.Exists(currentUser.ProfileImagePath))
//                        {
//                            BitmapImage bitmap = new BitmapImage(imageUri);
//                            ProfileImage.Source = bitmap;
//                        }
//                        else
//                        {
//                            MessageBox.Show("Файл изображения не найден.");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Произошла ошибка при загрузке изображения: {ex.Message}");
//                    }
//                }

//                // Скрываем кнопку редактирования и показываем кнопку сохранения
//                btnEdit.Visibility = Visibility.Visible;
//                txtName.IsReadOnly = true;
//                txtEmail.IsReadOnly = true;
//                btnSave.Visibility = Visibility.Collapsed;
//                btnUploadImage.Visibility = Visibility.Collapsed;
//            }
//            else
//            {
//                // Включаем редактирование
//                txtName.IsReadOnly = false;
//                txtEmail.IsReadOnly = false;
//                btnSave.Visibility = Visibility.Visible;
//                btnUploadImage.Visibility = Visibility.Visible;
//                btnEdit.Visibility = Visibility.Collapsed;
//            }

//            // Отображаем последние прочитанные книги
//            DisplayLastReadBooks();
//        }

//        private void DisplayLastReadBooks()
//        {
//            BooksPanel.Children.Clear(); // Очистить панель перед добавлением новых элементов

//            // Получаем последние прочитанные книги пользователя
//            var lastReadBooks = currentUser.Bookshelves
//                .Where(b => b.State == "Read") // Фильтруем по статусу "Read" (прочитано)
//                .OrderByDescending(b => b.DataAdded) // Сортируем по дате добавления, от самых новых
//                .Select(b => b.Book) // Получаем книги из полок
//                .Take(5) // Ограничиваем количество книг до 5 (или другого числа)
//                .ToList();

//            if (lastReadBooks == null || !lastReadBooks.Any())
//            {
//                // Если нет прочитанных книг, показываем сообщение
//                TextBlock noBooksText = new TextBlock
//                {
//                    Text = "У вас нет прочитанных книг.",
//                    FontSize = 14,
//                    Foreground = Brushes.Gray,
//                    HorizontalAlignment = HorizontalAlignment.Center
//                };
//                BooksPanel.Children.Add(noBooksText);
//                return;
//            }

//            // Для каждой книги, найденной в коллекции lastReadBooks, создаем элемент интерфейса
//            foreach (var book in lastReadBooks)
//            {
//                var bookPanel = new StackPanel
//                {
//                    Orientation = Orientation.Vertical,
//                    HorizontalAlignment = HorizontalAlignment.Left,
//                    Margin = new Thickness(10)
//                };

//                var bookImage = new Image
//                {
//                    Width = 60,
//                    Height = 90,
//                    Margin = new Thickness(5),
//                    Source = new BitmapImage(new Uri(book.ImagePath)) // Путь к изображению книги
//                };

//                var bookTitle = new TextBlock
//                {
//                    Text = book.Title, // Название книги
//                    FontSize = 14,
//                    HorizontalAlignment = HorizontalAlignment.Center,
//                    Width = 60,
//                    TextWrapping = TextWrapping.Wrap
//                };

//                // Добавляем изображение и название в панель книги
//                bookPanel.Children.Add(bookImage);
//                bookPanel.Children.Add(bookTitle);

//                // Добавляем панель книги на основную панель
//                BooksPanel.Children.Add(bookPanel);
//            }
//        }


//        private void btnEdit_Click(object sender, RoutedEventArgs e)
//        {
//            isEditing = true;
//            DisplayUserInfo();
//        }
//        private void btnSave_Click(object sender, RoutedEventArgs e)
//        {
//            currentUser.Name = txtName.Text;
//            currentUser.Email = txtEmail.Text;
//            SaveUserData();
//            isEditing = false;
//            DisplayUserInfo();
//        }


//        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
//        {
//            OpenFileDialog openFileDialog = new OpenFileDialog
//            {
//                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
//            };

//            if (openFileDialog.ShowDialog() == true)
//            {
//                string imagePath = openFileDialog.FileName;
//                currentUser.ProfileImagePath = imagePath;
//                Uri imageUri = new Uri(imagePath);
//                BitmapImage bitmap = new BitmapImage(imageUri);
//                ProfileImage.Source = bitmap;
//            }
//        }

//        private void SaveUserData()
//        {
//            using (LibraryContext db = new LibraryContext())
//            {
//                db.Users.Update(currentUser);
//                db.SaveChanges();
//            }
//        }

//        private void btnBack_Click(object sender, RoutedEventArgs e)
//        {
//            MainCommunityBook mainCommunityBook = new MainCommunityBook(currentUser);
//            mainCommunityBook.Show();
//            this.Close();
//        }

//        private void LogoutButton_Click(object sender, RoutedEventArgs e)
//        {
//            var customMessageBox = new CustomWin();
//            customMessageBox.ShowDialog();

//            if (customMessageBox.Result)
//            {

//                foreach (var window in Application.Current.Windows)
//                {
//                    if (window is AboutMeWindow || window is CustomWin)
//                    {
//                        continue;
//                    }

//                    (window as Window)?.Close();
//                }
//                MainWindow mainWindow = new MainWindow();
//                mainWindow.Show();
//                this.Close();
//            }
//        }
//        private void MenuItem_AboutMe_Click(object sender, RoutedEventArgs e)
//        {
//            MessegeAboutme messegeAboutme = new MessegeAboutme();
//            messegeAboutme.Show();

//        }

//        private void MenuItem_MyShelf_Click(object sender, RoutedEventArgs e)
//        {
//            BookshelfWindow bookshelfWindow = new BookshelfWindow(currentUser);
//            bookshelfWindow.Show();
//            this.Close();
//        }

//        private void MenuItem_Books_Click(object sender, RoutedEventArgs e)
//        {
//            BookWindow bookWindow = new BookWindow(currentUser);
//            bookWindow.Show();
//            this.Close();
//        }

//        private void MenuItem_Friends_Click(object sender, RoutedEventArgs e)
//        {
//            var friendsWindow = new CommunityBook(currentUser);
//            friendsWindow.Show();
//            this.Close();
//        }

//        private void MenuItem_Review_Click(object sender, RoutedEventArgs e)
//        {
//            ReviewWindow reviewWindow = new ReviewWindow(currentUser, _context);
//            reviewWindow.Show();
//            this.Close();
//        }
//    }
//}
