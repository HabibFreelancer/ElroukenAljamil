using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Messaging.Domain.Enums;
using ElroukenAljamil.Messaging.Domain.Events;

namespace ElroukenAljamil.Messaging.Domain.Entities
{
    /// <summary>
    /// Agrégat racine représentant une conversation entre deux utilisateurs.
    /// Une conversation est liée à une annonce spécifique (le contexte de l'échange).
    /// </summary>
    public class Conversation : AggregateRoot
    {
        public Guid BuyerId { get; private set; }
        public string BuyerName { get; private set; } = string.Empty;
        public Guid SellerId { get; private set; }
        public string SellerName { get; private set; } = string.Empty;
        public Guid ListingId { get; private set; }
        public string ListingTitle { get; private set; } = string.Empty;
        public ConversationStatus Status { get; private set; } = ConversationStatus.Active;
        public DateTime? LastMessageAt { get; private set; }
        public int UnreadCountBuyer { get; private set; }
        public int UnreadCountSeller { get; private set; }

        private readonly List<Message> _messages = new();
        public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

        private Conversation() { } // EF Core

        /// <summary>
        /// Crée une nouvelle conversation. Initiée par l'acheteur qui contacte le vendeur.
        /// </summary>
        public static Conversation Create(
            Guid buyerId,
            string buyerName,
            Guid sellerId,
            string sellerName,
            Guid listingId,
            string listingTitle,
            string initialMessage)
        {
            if (buyerId == Guid.Empty)
                throw new ArgumentException("L'acheteur est obligatoire.", nameof(buyerId));
            if (sellerId == Guid.Empty)
                throw new ArgumentException("Le vendeur est obligatoire.", nameof(sellerId));
            if (buyerId == sellerId)
                throw new ArgumentException("Un utilisateur ne peut pas se contacter lui-même.");
            if (listingId == Guid.Empty)
                throw new ArgumentException("L'annonce est obligatoire.", nameof(listingId));
            if (string.IsNullOrWhiteSpace(initialMessage))
                throw new ArgumentException("Le premier message ne peut pas être vide.", nameof(initialMessage));

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId,
                BuyerName = buyerName.Trim(),
                SellerId = sellerId,
                SellerName = sellerName.Trim(),
                ListingId = listingId,
                ListingTitle = listingTitle.Trim(),
                Status = ConversationStatus.Active,
                UnreadCountBuyer = 0,
                UnreadCountSeller = 1,
                CreatedAt = DateTime.UtcNow
            };

            // Ajouter le premier message
            var message = Message.Create(
                conversationId: conversation.Id,
                senderId: buyerId,
                senderName: buyerName,
                content: initialMessage);

            conversation._messages.Add(message);
            conversation.LastMessageAt = message.SentAt;

            conversation.AddDomainEvent(new ConversationCreatedDomainEvent(
                conversation.Id, buyerId, sellerId, listingId, listingTitle));

            conversation.AddDomainEvent(new MessageSentDomainEvent(
                message.Id, conversation.Id, buyerId, buyerName, sellerId, initialMessage, listingId, listingTitle));

            return conversation;
        }

        /// <summary>
        /// Envoie un nouveau message dans la conversation.
        /// </summary>
        public Message SendMessage(Guid senderId, string senderName, string content)
        {
            if (Status != ConversationStatus.Active)
                throw new InvalidOperationException("Impossible d'envoyer un message dans une conversation archivée.");

            if (!IsParticipant(senderId))
                throw new InvalidOperationException("Seuls les participants peuvent envoyer des messages.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Le message ne peut pas être vide.", nameof(content));

            if (content.Length > 2000)
                throw new ArgumentException("Le message ne peut pas dépasser 2000 caractères.", nameof(content));

            var message = Message.Create(
                conversationId: Id,
                senderId: senderId,
                senderName: senderName,
                content: content);

            _messages.Add(message);
            LastMessageAt = message.SentAt;
            UpdatedAt = DateTime.UtcNow;

            // Incrémenter le compteur de non-lus pour le destinataire
            var recipientId = GetRecipientId(senderId);
            if (recipientId == BuyerId)
                UnreadCountBuyer++;
            else
                UnreadCountSeller++;

            AddDomainEvent(new MessageSentDomainEvent(
                message.Id, Id, senderId, senderName, recipientId,
                content, ListingId, ListingTitle));

            return message;
        }

        /// <summary>
        /// Marque tous les messages comme lus pour un participant.
        /// </summary>
        public void MarkAsRead(Guid userId)
        {
            if (!IsParticipant(userId))
                throw new InvalidOperationException("Seuls les participants peuvent marquer la conversation comme lue.");

            if (userId == BuyerId)
            {
                UnreadCountBuyer = 0;
            }
            else
            {
                UnreadCountSeller = 0;
            }

            // Marquer les messages individuels comme lus
            var unreadMessages = _messages
                .Where(m => m.SenderId != userId && !m.IsRead)
                .ToList();

            foreach (var message in unreadMessages)
            {
                message.MarkAsRead();
            }

            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MessagesReadDomainEvent(Id, userId, unreadMessages.Count));
        }

        /// <summary>
        /// Archive la conversation pour un participant.
        /// </summary>
        public void Archive(Guid userId)
        {
            if (!IsParticipant(userId))
                throw new InvalidOperationException("Seuls les participants peuvent archiver.");

            // On ne change le statut que si les deux ont archivé
            // Pour simplifier ici, on passe à Archived
            Status = ConversationStatus.Archived;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ConversationArchivedDomainEvent(Id, userId));
        }

        /// <summary>
        /// Vérifie si un utilisateur est participant à la conversation.
        /// </summary>
        public bool IsParticipant(Guid userId) => userId == BuyerId || userId == SellerId;

        /// <summary>
        /// Récupère l'identifiant du destinataire à partir de l'expéditeur.
        /// </summary>
        public Guid GetRecipientId(Guid senderId)
        {
            if (senderId == BuyerId) return SellerId;
            if (senderId == SellerId) return BuyerId;
            throw new InvalidOperationException("L'expéditeur n'est pas un participant.");
        }

        /// <summary>
        /// Récupère le nombre de messages non lus pour un participant.
        /// </summary>
        public int GetUnreadCount(Guid userId)
        {
            if (userId == BuyerId) return UnreadCountBuyer;
            if (userId == SellerId) return UnreadCountSeller;
            return 0;
        }

        /// <summary>
        /// Récupère le dernier message de la conversation.
        /// </summary>
        public Message? LastMessage => _messages
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefault();
    }

}
