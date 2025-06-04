using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.Entities
{
    // bảng các loại đồ ăn/ đồ uống đi kèm vé
    public class OtherServicesModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên dịch vụ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập giá")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(10, 3)")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        [FileExtension]
        public Status Status { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<OtherServicesOrderModel> OtherServicesOrders { get; set; }
    }
}
