namespace EventDrivenArchDemo.Api.Models.Responses
{
    public class RentDetailResponse
    {
        public int RentId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
    }
}
