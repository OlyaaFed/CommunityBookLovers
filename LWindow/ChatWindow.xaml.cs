using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityBookLovers.LWindow
{
    public partial class ChatWindow : Window
    {
        public User CurrentUser { get; internal set; }
        public User Friend { get; internal set; }

        public ChatWindow()
        {
            InitializeComponent();

            // Пример сообщений (можно удалить)
            AddMessage("Ты читала книгу толстого? Война и мир", false);
            AddMessage("Да", true);
            AddMessage("ого", false);
            AddMessage("но мне не зашла книга", true);
            
        }

        private void AddMessage(string text, bool isMyMessage)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = isMyMessage ?
                    System.Windows.Media.Brushes.LightGreen :
                    System.Windows.Media.Brushes.White,
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                HorizontalAlignment = isMyMessage ?
                    HorizontalAlignment.Right :
                    HorizontalAlignment.Left,
                MaxWidth = 300
            };

            var textBlock = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = textBlock;
            MessagesPanel.Children.Add(border);

            // Прокрутка вниз (используем ScrollViewer)
            var scrollViewer = GetScrollViewer(MessagesPanel);
            scrollViewer?.ScrollToBottom();
        }

        // Вспомогательный метод для поиска ScrollViewer
        private static ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) return depObj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(MessageBox.Text))
            {
                AddMessage(MessageBox.Text, true);
                MessageBox.Clear();

                // Здесь можно добавить сохранение в базу данных
                // и отправку другому пользователю
            }
        }
    }
}