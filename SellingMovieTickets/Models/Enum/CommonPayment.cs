namespace SellingMovieTickets.Models.Enum
{
    public interface CommonPayment
    {

        public decimal ConcessionAmount { get; set; } // Số tiền phụ phí bỏng nước
        public decimal TotalAmount { get; set; }      // Tổng tiền
        public decimal DiscountAmount { get; set; }   // Số tiền giảm giá
        public decimal PaymentAmount { get; set; }    // Số tiền phải thanh toán
    }
}
