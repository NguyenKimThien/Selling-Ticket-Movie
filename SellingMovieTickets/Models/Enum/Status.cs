using System.Text.Json.Serialization;

namespace SellingMovieTickets.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Show = 1,
        Hidden = 0
    }
}
