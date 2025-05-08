using Microsoft.AspNetCore.Http;

namespace Application.Files.Common.UploadSupabaseFile
{
    public class UploadSupabaseFileRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}