using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBookLovers.Models
{
    public class ChatMessage
    {
        public int Id { get; set; } // Идентификатор сообщения
        public string Sender { get; set; } // Имя отправителя
        public string Text { get; set; } // Текст сообщения
        public DateTime Timestamp { get; set; } // Время отправки
        public bool IsMyMessage { get; set; } // Является ли сообщение отправленным пользователем
    }

}
