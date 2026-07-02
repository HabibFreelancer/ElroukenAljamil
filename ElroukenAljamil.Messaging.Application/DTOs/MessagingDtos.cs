using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Messaging.Application.DTOs
{
    public record ConversationDto
    {
        public Guid Id { get; init; }
        public Guid BuyerId { get; init; }
        public string BuyerName { get; init; } = string.Empty;
        public Guid SellerId { get; init; }
        public string SellerName { get; init; } = string.Empty;
        public Guid ListingId { get; init; }
        public string ListingTitle { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public int UnreadCount { get; init; }
        public MessageDto? LastMessage { get; init; }
        public DateTime? LastMessageAt { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record ConversationDetailDto
    {
        public Guid Id { get; init; }
        public Guid BuyerId { get; init; }
        public string BuyerName { get; init; } = string.Empty;
        public Guid SellerId { get; init; }
        public string SellerName { get; init; } = string.Empty;
        public Guid ListingId { get; init; }
        public string ListingTitle { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public List<MessageDto> Messages { get; init; } = new();
        public DateTime CreatedAt { get; init; }
    }

    public record MessageDto
    {
        public Guid Id { get; init; }
        public Guid SenderId { get; init; }
        public string SenderName { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public DateTime SentAt { get; init; }
        public bool IsRead { get; init; }
        public DateTime? ReadAt { get; init; }
        public bool IsEdited { get; init; }
        public bool IsDeleted { get; init; }
        public bool IsMine { get; init; }
    }

    public record ConversationListDto
    {
        public List<ConversationDto> Conversations { get; init; } = new();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalUnread { get; init; }
    }

}
