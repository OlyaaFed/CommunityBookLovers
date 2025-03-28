using CommunityBookLovers.MessegeBoxes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommunityBookLovers.LWindow
{
    public partial class CommunityBook : Window
    {
        private readonly User currentUser;
        private readonly LibraryContext dbContext;

        public CommunityBook(User user)
        {
            InitializeComponent();
            currentUser = user;
            dbContext = new LibraryContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                LoadFriendsList();
                LoadFriendshipRequests();
            }
            catch (Exception ex) when (ex.Message.Contains("RequestDate"))
            {
                // Игнорируем только ошибки связанные с RequestDate
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void LoadFriendsList()
        {
            var friends = dbContext.Friendships
                .Where(f => (f.UserId == currentUser.UserId || f.FriendId == currentUser.UserId) && f.IsAccepted)
                .Select(f => f.UserId == currentUser.UserId ? f.Friend : f.User)
                .Distinct()
                .ToList();

            lstFriends.ItemsSource = friends;
        }

        private void LoadFriendshipRequests()
        {
            var pendingRequests = dbContext.Friendships
                .Where(f => f.FriendId == currentUser.UserId && !f.IsAccepted)
                .Include(f => f.User)
                .ToList();

            lstFriendRequests.ItemsSource = pendingRequests;
        }

        private void AcceptRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
                try
                {
                    var request = dbContext.Friendships.FirstOrDefault(f => f.FriendshipId == friendshipId);
                    if (request != null)
                    {
                        request.IsAccepted = true;
                        dbContext.SaveChanges();
                        LoadData();
                        MessageBox.Show("Заявка принята!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка принятия заявки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RejectRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
                try
                {
                    var request = dbContext.Friendships.FirstOrDefault(f => f.FriendshipId == friendshipId);
                    if (request != null)
                    {
                        dbContext.Friendships.Remove(request);
                        dbContext.SaveChanges();
                        LoadFriendshipRequests();
                        MessageBox.Show("Заявка отклонена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка отклонения заявки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveFriend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendId)
            {
                try
                {
                    var friendship = dbContext.Friendships.FirstOrDefault(f =>
                        (f.UserId == currentUser.UserId && f.FriendId == friendId) ||
                        (f.UserId == friendId && f.FriendId == currentUser.UserId));

                    if (friendship != null)
                    {
                        dbContext.Friendships.Remove(friendship);
                        dbContext.SaveChanges();
                        LoadFriendsList();
                        MessageBox.Show("Друг удалён", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления друга: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendId)
            {
                try
                {
                    // Проверка существующей дружбы
                    bool alreadyFriends = dbContext.Friendships
                        .Any(f => (f.UserId == currentUser.UserId && f.FriendId == friendId && f.IsAccepted) ||
                                 (f.UserId == friendId && f.FriendId == currentUser.UserId && f.IsAccepted));

                    if (alreadyFriends)
                    {
                        MessageBox.Show("Вы уже друзья с этим пользователем", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Проверка существующей заявки
                    bool existingRequest = dbContext.Friendships
                        .Any(f => (f.UserId == currentUser.UserId && f.FriendId == friendId && !f.IsAccepted) ||
                                 (f.UserId == friendId && f.FriendId == currentUser.UserId && !f.IsAccepted));

                    if (existingRequest)
                    {
                        MessageBox.Show("Запрос на дружбу уже отправлен", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Создание новой заявки
                    var newFriendship = new Friendship
                    {
                        UserId = currentUser.UserId,
                        FriendId = friendId,
                        IsAccepted = false,
                        RequestDate = DateTime.Now
                    };

                    dbContext.Friendships.Add(newFriendship);
                    dbContext.SaveChanges();

                    MessageBox.Show("Заявка на дружбу отправлена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadFriendshipRequests();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка отправки заявки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Введите имя для поиска", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var results = dbContext.Users
                    .Where(u => u.Name.Contains(query) && u.UserId != currentUser.UserId)
                    .ToList();

                lstSearchResults.ItemsSource = results;

                if (!results.Any())
                {
                    MessageBox.Show("Пользователи не найдены", "Результаты поиска",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int friendId)
            {
                var friend = dbContext.Users.FirstOrDefault(u => u.UserId == friendId);
                if (friend != null)
                {
                    // Альтернативный вариант, если нужно передавать данные:
                    var chatWindow = new ChatWindow();
                    chatWindow.CurrentUser = currentUser;
                    chatWindow.Friend = friend;
                    chatWindow.Owner = this;
                    chatWindow.Show();
                }
            }
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Используйте поиск для добавления друзей", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NavigateToWindow(Window window)
        {
            window.Show();
            this.Close();
        }

        private void MenuItem_AboutMe_Click(object sender, RoutedEventArgs e)
            => NavigateToWindow(new AboutMeWindow(currentUser));

        private void MenuItem_MyShelf_Click(object sender, RoutedEventArgs e)
            => NavigateToWindow(new BookshelfWindow(currentUser));

        private void MenuItem_Books_Click(object sender, RoutedEventArgs e)
            => NavigateToWindow(new BookWindow(currentUser));

        private void MenuItem_Review_Click(object sender, RoutedEventArgs e)
            => NavigateToWindow(new ReviewWindow(currentUser, dbContext));

        private void MenuItem_Friends_Click(object sender, RoutedEventArgs e)
        {
            // Уже находимся в окне друзей
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                new MainWindow().Show();
                this.Close();
            }
        }
    }
}