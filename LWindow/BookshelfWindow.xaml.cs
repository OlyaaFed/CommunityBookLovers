using CommunityBookLovers.LWindow;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommunityBookLovers
{
    public partial class BookshelfWindow : Window
    {
        private readonly User currentUser;
        private readonly LibraryContext _context;
        private ObservableCollection<Bookshelf> bookshelf;

        public BookshelfWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            _context = new LibraryContext();
            LoadBookshelf();
        }
        private void MenuItem_Home_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новое окно главной страницы
            var mainWindow = new MainCommunityBook(currentUser);
            mainWindow.Show();

            // Закрываем текущее окно
            this.Close();
        }
        private void LoadBookshelf()
        {
            try
            {
                bookshelf = new ObservableCollection<Bookshelf>(
                    _context.Bookshelves
                        .Include(bs => bs.Book)
                        .ThenInclude(b => b.Author)
                        .Where(bs => bs.UserId == currentUser.UserId)
                        .OrderBy(bs => bs.Book.Title)
                        .ToList());

                lstBookshelf.ItemsSource = bookshelf;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке полки: {ex.Message}");
            }
        }

        private void StatusSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ComboBoxItem item)
            {
                FilterBooksByStatus(item.Content.ToString());
            }
        }

        private void FilterBooksByStatus(string status)
        {
            if (bookshelf == null || bookshelf.Count == 0) return;

            var filtered = status switch
            {
                "Прочитал" => bookshelf.Where(b => b.State == "Прочитал"),
                "Читаю" => bookshelf.Where(b => b.State == "Читаю"),
                "Хочу прочитать" => bookshelf.Where(b => b.State == "Хочу прочитать"),
                _ => bookshelf.AsEnumerable()
            };

            lstBookshelf.ItemsSource = new ObservableCollection<Bookshelf>(filtered);
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstBookshelf.SelectedItem is Bookshelf selectedBook)
            {
                try
                {
                    _context.Bookshelves.Remove(selectedBook);
                    _context.SaveChanges();
                    bookshelf.Remove(selectedBook);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении книги: {ex.Message}");
                }
            }
        }

        private void MenuItem_AboutMe_Click(object sender, RoutedEventArgs e)
        {
            new AboutMeWindow(currentUser).Show();
            Close();
        }

        private void MenuItem_MyShelf_Click(object sender, RoutedEventArgs e)
        {
            // Уже находимся в окне полки
        }
        
        private void MenuItem_Books_Click(object sender, RoutedEventArgs e)
        {
            new BookWindow(currentUser).Show();
            Close();
        }

        private void MenuItem_Friends_Click(object sender, RoutedEventArgs e)
        {
            new CommunityBook(currentUser).Show();
            Close();
        }

        private void MenuItem_Review_Click(object sender, RoutedEventArgs e)
        {
            new ReviewWindow(currentUser, _context).Show();
            Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}