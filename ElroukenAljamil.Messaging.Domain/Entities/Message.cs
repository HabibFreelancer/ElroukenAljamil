using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Messaging.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; private set; }
        public Guid ConversationId { get; private set; }
        public Guid SenderId { get; private set; }
        public string Content { get; private set; } = default!;
        public DateTime SentAt { get; private set; }
        public bool IsRead { get; private set; }


        private Message() { }


        public Message(Guid id, Guid conversationId, Guid senderId, string content, DateTime sentAt)
        {
            Id = id;
            ConversationId = conversationId;
            SenderId = senderId;
            Content = content;
            SentAt = sentAt;
            IsRead = false;
        }


        public void MarkAsRead() => IsRead = true;
    }

}
