using MediatR;
using Application.Files.Common.UploadSupabaseFile;

namespace Application.Files.Command.UploadSupabaseFile
{
    public class UploadSupabaseFileCommandHandler(SupabaseFileUploader supabaseFileUploader)
        : IRequestHandler<UploadSupabaseFileCommand, UploadSupabaseFileResponse>
    {
        public async Task<UploadSupabaseFileResponse> Handle(UploadSupabaseFileCommand request, CancellationToken cancellationToken)
        {
            var file = request.Request.File;

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");
            
            var fileData = new byte[file.Length];
            var bytesRead = 0;
            await using (var fileStream = file.OpenReadStream())
            {
                bytesRead = await fileStream.ReadAsync(fileData.AsMemory(0, (int)file.Length), cancellationToken);
            }
            
            if (bytesRead != file.Length)
            {
                throw new InvalidOperationException("The number of bytes read does not match the expected file size.");
            }
            
            var filePath = await supabaseFileUploader.UploadFileAsync(fileData, file.FileName);

            return new UploadSupabaseFileResponse
            {
                FilePath = filePath
            };
        }
    }
}