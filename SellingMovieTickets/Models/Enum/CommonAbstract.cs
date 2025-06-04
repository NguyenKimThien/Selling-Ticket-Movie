namespace SellingMovieTickets.Models.Entities
{
    public interface CommonAbstract
    {
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
