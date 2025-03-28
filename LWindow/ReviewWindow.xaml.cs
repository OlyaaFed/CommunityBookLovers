using Microsoft.EntityFrameworkCore;
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
using CommunityBookLovers.MessegeBoxes;
namespace CommunityBookLovers.LWindow
{
    /// <summary>
    /// Логика взаимодействия для ReviewWindow.xaml
    /// </summary>
    public partial class ReviewWindow : Window
    {
        private readonly LibraryContext _context;
        private readonly User currentUser;

        public ReviewWindow(User user, LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            currentUser = user;

            LoadBooks(); // Загружаем книги для обоих ComboBox
            LoadReviews();
        }



        private void LoadBooks()
        {
            try
            {
                var books = _context.Books.ToList();
                BookComboBox.ItemsSource = books;
                FilterBookComboBox.ItemsSource = books; // Загружаем книги и в фильтр
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке книг: {ex.Message}");
            }
        }

        private void LoadReviews()
        {
            if (_context == null)
            {
                MessageBox.Show("");
                return;
            }

            var reviews = _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .ToList();
            ReviewListBox.ItemsSource = reviews;
        }

        

        private void ReviewListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReviewListBox.SelectedItem is Review selectedReview)
            {
                DisplayReviewDetails(selectedReview);
            }
        }

        private void DisplayReviewDetails(Review review)
        {
            if (review == null) return;

            BookComboBox.Text = $"Книга: {review.Book?.Title ?? "Неизвестно"}";

            RatingComboBox.Text = $"Рейтинг: {review.Rating}/5";
            ReviewTextBox.Text = review.Text ?? "Отзыв отсутствует";
        }
        private void SaveReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (BookComboBox.SelectedItem == null || RatingComboBox.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ReviewTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля перед сохранением.");
                return;
            }

            var selectedBook = (Book)BookComboBox.SelectedItem;
            var rating = RatingComboBox.SelectedIndex + 1;
            var text = ReviewTextBox.Text;

            var newReview = new Review
            {
                UserId = currentUser.UserId,
                BookId = selectedBook.BookId,
                Rating = rating,
                Text = text,
                Date = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Reviews.Add(newReview);
            _context.SaveChanges();

            MessageBox.Show("Ваш отзыв был успешно сохранён!");
            LoadReviews();
            ReviewListBox.ScrollIntoView(ReviewListBox.Items[ReviewListBox.Items.Count - 1]);

            
            ReviewTextBox.Clear();
            RatingComboBox.SelectedIndex = -1;
            BookComboBox.SelectedIndex = -1;
        }
        private void MenuItem_Home_Click(object sender, RoutedEventArgs e)
{
    // Создаем новое окно главной страницы
    var mainWindow = new MainCommunityBook(currentUser);
    mainWindow.Show();

    // Закрываем текущее окно
    this.Close();
}
        private void FilterReviewsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedBookId = (int?)FilterBookComboBox.SelectedValue;
                var selectedDate = FilterDatePicker.SelectedDate;

                var reviews = _context.Reviews
                    .Include(r => r.Book)
                    .Include(r => r.User)
                    .AsQueryable();

                if (selectedBookId.HasValue)
                {
                    reviews = reviews.Where(r => r.BookId == selectedBookId.Value);
                }

                if (selectedDate.HasValue)
                {
                    reviews = reviews.Where(r => r.Date == DateOnly.FromDateTime(selectedDate.Value));
                }

                ReviewListBox.ItemsSource = reviews.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации отзывов: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterBookComboBox.SelectedIndex = -1;
            FilterDatePicker.SelectedDate = null;
            LoadReviews(); // Перезагружаем все отзывы
        }
        private void MenuItem_AboutMe_Click(object sender, RoutedEventArgs e)
        {
            var aboutMeWindow = new AboutMeWindow(currentUser);
            aboutMeWindow.Show();
            this.Close();
        }

        private void MenuItem_MyShelf_Click(object sender, RoutedEventArgs e)
        {
            var bookshelfWindow = new BookshelfWindow(currentUser);
            bookshelfWindow.Show();
            this.Close();
        }

        private void MenuItem_Books_Click(object sender, RoutedEventArgs e)
        {
            var bookWindow = new BookWindow(currentUser);
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
            MessegeReview messegeReview = new MessegeReview();
            messegeReview.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var customMessageBox = new CustomWin();
            customMessageBox.ShowDialog();

            if (customMessageBox.Result)
            {

                foreach (var window in Application.Current.Windows)
                {
                    if (window is ReviewWindow || window is CustomWin)
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
    }
}
