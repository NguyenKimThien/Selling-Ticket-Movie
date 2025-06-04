using SellingMovieTickets.Models.Enum;

namespace SellingMovieTickets.Services.Interfaces
{
    public interface IAwsS3Service
    {
        Task<S3Response> UploadFile(IFormFile file, string? prefix, string namefile);
        Task<ResponseMessage> DeleteFileAsync(string key);
    }
}
