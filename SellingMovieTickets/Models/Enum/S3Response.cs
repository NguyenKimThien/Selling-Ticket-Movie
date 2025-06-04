namespace SellingMovieTickets.Models.Enum
{
    public class S3Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = "";
        public string? PresignedUrl { get; set; }
    }
}
