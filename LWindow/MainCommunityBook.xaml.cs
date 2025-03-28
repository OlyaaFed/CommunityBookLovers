using CommunityBookLovers.LWindow;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommunityBookLovers
{
    public partial class MainCommunityBook : Window
    {
        private readonly User currentUser;
        private readonly LibraryContext _context;

        public MainCommunityBook(User user)
        {
            InitializeComponent();
            currentUser = user;
            _context = new LibraryContext();

            LoadUserData();
            LoadStatistics();
            LoadRecommendedBooks();
            LoadPopularAuthors();
        }

        private void LoadUserData()
        {
            UserNameRun.Text = currentUser.Name;
        }

        private void LoadStatistics()
        {
            BooksCount.Text = _context.Bookshelves
                .Count(b => b.UserId == currentUser.UserId)
                .ToString();

            FriendsCount.Text = _context.Friendships
                .Count(f => (f.UserId == currentUser.UserId || f.FriendId == currentUser.UserId) && f.IsAccepted)
                .ToString();

            ReviewsCount.Text = _context.Reviews
                .Count(r => r.UserId == currentUser.UserId)
                .ToString();
        }

        private void LoadRecommendedBooks()
        {
            var recommendedBooks = _context.Books
                .Include(b => b.Author)
                .OrderByDescending(b => b.Reviews.Count)
                .Take(5)
                .ToList();

            RecommendedBooks.ItemsSource = recommendedBooks;
        }

        private void LoadPopularAuthors()
        {
            var popularAuthors = _context.Authors
                .Include(a => a.Books)
                .OrderByDescending(a => a.Books.Sum(b => b.Reviews.Count))
                .Take(5)
                .ToList();

            PopularAuthorsList.ItemsSource = popularAuthors;
        }

        // Обработчики меню
        private void MenuItem_Home_Click(object sender, RoutedEventArgs e)
        {
            // Уже на главной
        }

        private void MenuItem_Books_Click(object sender, RoutedEventArgs e)
        {
            new BookWindow(currentUser).Show();
            Close();
        }

        private void MenuItem_PopularAuthors_Click(object sender, RoutedEventArgs e)
        {
            LoadPopularAuthors();
        }

        

        private void MenuItem_MyShelf_Click(object sender, RoutedEventArgs e)
        {
            new BookshelfWindow(currentUser).Show();
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

        private void MenuItem_AboutMe_Click(object sender, RoutedEventArgs e)
        {
            new AboutMeWindow(currentUser).Show();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
        }
    }
}