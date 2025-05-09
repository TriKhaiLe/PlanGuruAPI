using Microsoft.AspNetCore.Http;

namespace Application.Files.Common.SaveFile
{
    public class SaveFileRequest
    {
        public IFormFile File { get; set; } = null!;
        public string Folder { get; set; } = "Files";
    }
}