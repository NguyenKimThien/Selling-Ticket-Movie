using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    // bảng khuyến mãi
    public class PromotionModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu tạo mã")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập phần trăm giảm giá")]
        public decimal DiscountPercentage { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn ngày bắt đầu khuyến mãi")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn ngày kết thúc khuyến mãi")]
        public DateTime EndDate { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<OrderModel>? Orders { get; set; }
    }
}
