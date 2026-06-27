using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Messaging.Domain.Entities
{
    /// <summary>
    /// Aggregate Root : conversation entre acheteur et vendeur au sujet d'une annonce.
    /// </summary>
    public class Conversation
    {
        public Guid Id { get; private set; }
        public Guid ListingId { get; private set; }
        public Guid BuyerId { get; private set; }
        public Guid SellerId { get; private set; }
        public DateTime CreatedAt { get; private set; }


        private readonly List<Message> _messages = new();
        public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();


        private Conversation() { }


        public static Conversation Start(Guid listingId, Guid buyerId, Guid sellerId, string initialMessage)
        {
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                ListingId = listingId,
                BuyerId = buyerId,
                SellerId = sellerId,
                CreatedAt = DateTime.UtcNow
            };


            conversation.AddMessage(buyerId, initialMessage);
            return conversation;
        }


        public void AddMessage(Guid senderId, string content)
        {
            if (senderId != BuyerId && senderId != SellerId)
                throw new InvalidOperationException("Seuls les participants peuvent envoyer un message.");


            _messages.Add(new Message(Guid.NewGuid(), Id, senderId, content, DateTime.UtcNow));
        }
    }

}
