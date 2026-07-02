using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Messaging.Domain.Entities
{
    /// <summary>
    /// Entité représentant un message dans une conversation.
    /// Fait partie de l'agrégat Conversation.
    /// </summary>
    public class Message : BaseEntity
    {
        public Guid ConversationId { get; private set; }
        public Guid SenderId { get; private set; }
        public string SenderName { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public DateTime SentAt { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public bool IsEdited { get; private set; }
        public DateTime? EditedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private Message() { } // EF Core

        /// <summary>
        /// Factory pour créer un nouveau message.
        /// </summary>
        public static Message Create(Guid conversationId, Guid senderId, string senderName, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Le contenu du message est obligatoire.", nameof(content));
            if (content.Length > 2000)
                throw new ArgumentException("Le message ne peut pas dépasser 2000 caractères.", nameof(content));

            return new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = senderId,
                SenderName = senderName.Trim(),
                Content = content.Trim(),
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsEdited = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Marque le message comme lu.
        /// </summary>
        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Modifie le contenu du message (dans les 15 minutes après envoi).
        /// </summary>
        public void Edit(string newContent, Guid editorId)
        {
            if (editorId != SenderId)
                throw new InvalidOperationException("Seul l'expéditeur peut modifier son message.");

            if (IsDeleted)
                throw new InvalidOperationException("Impossible de modifier un message supprimé.");

            var editWindow = TimeSpan.FromMinutes(15);
            if (DateTime.UtcNow - SentAt > editWindow)
                throw new InvalidOperationException("Le message ne peut plus être modifié (délai de 15 minutes dépassé).");

            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("Le contenu ne peut pas être vide.", nameof(newContent));

            if (newContent.Length > 2000)
                throw new ArgumentException("Le message ne peut pas dépasser 2000 caractères.", nameof(newContent));

            Content = newContent.Trim();
            IsEdited = true;
            EditedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Supprime le message (soft delete — le contenu est remplacé).
        /// </summary>
        public void Delete(Guid deleterId)
        {
            if (deleterId != SenderId)
                throw new InvalidOperationException("Seul l'expéditeur peut supprimer son message.");

            if (IsDeleted)
                throw new InvalidOperationException("Le message est déjà supprimé.");

            Content = "[Message supprimé]";
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
