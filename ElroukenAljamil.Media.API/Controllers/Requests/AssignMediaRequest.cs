namespace ElroukenAljamil.Media.API.Controllers.Requests
{
    public class AssignMediaRequest
    {
        public int ListingId { get; set; }
        public List<Guid> MediaIds { get; set; } = new();
    }
}
