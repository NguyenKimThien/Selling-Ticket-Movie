using System.Text.Json.Serialization;

namespace SellingMovieTickets.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PointChangeStatus
    {
        BuyTicket = 1,
        UsePoint = 0
    }
}
