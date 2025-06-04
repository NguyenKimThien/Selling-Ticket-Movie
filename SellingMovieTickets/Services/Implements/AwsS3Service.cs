using Amazon.S3;
using Amazon.S3.Model;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Services.Interfaces;

namespace SellingMovieTickets.Services.Implements
{
    public class AwsS3Service : IAwsS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private IConfiguration _configuration;

        public AwsS3Service(IAmazonS3 amazonS3, IConfiguration configuration)
        {
            _s3Client = amazonS3;
            _configuration = configuration;
        }

        public async Task<S3Response> UploadFile(IFormFile file, string? prefix, string namefile)
        {
            string bucketName = _configuration["Bucket:Name"];
            var result = new S3Response();
            try
            {
                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
                if (!bucketExists)
                {
                    result.StatusCode = 404;
                    result.Message = $"Bucket {bucketName} does not exist.";
                    return result;
                }
                else
                {
                    string key = string.IsNullOrEmpty(namefile) ? file.FileName : $"{prefix}/{namefile}";
                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        Key = key,
                        InputStream = file.OpenReadStream()
                    };
                    request.Metadata.Add("Content-Type", file.ContentType);
                    await _s3Client.PutObjectAsync(request);

                    result.StatusCode = 200;
                    result.Message = $"File {key} uploaded to S3 successfully!";
                    result.PresignedUrl = $"https://{bucketName}.s3.amazonaws.com/{key}";
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResponseMessage> DeleteFileAsync(string? key)
        {
            string bucketName = _configuration["Bucket:Name"];
            var result = new ResponseMessage();
            try
            {
                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
                if (!bucketExists)
                {
                    result.StatusCode = 404;
                    result.Message = $"Bucket {bucketName} does not exist";
                    return result;
                }
                await _s3Client.DeleteObjectAsync(bucketName, key);
                result.StatusCode = 200;
                result.Message = "Delete success";
            }
            catch (Exception ex)
            {
                result.StatusCode = 400;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
