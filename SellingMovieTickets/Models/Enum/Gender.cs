using System.Text.Json.Serialization;

namespace SellingMovieTickets.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2
    }
}
