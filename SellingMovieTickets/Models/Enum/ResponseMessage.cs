namespace SellingMovieTickets.Models.Enum
{
    public class ResponseMessage
    {
        public string Message { get; set; }
        public int? StatusCode { get; set; }

        public ResponseMessage()
        {

        }

        public ResponseMessage(string mess)
        {
            Message = mess;
        }
    }
}
