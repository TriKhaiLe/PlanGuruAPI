using Application.Files.Common.SaveFile;
using MediatR;

namespace Application.Files.Command.SaveFile
{
    public class SaveFileCommandHandler : IRequestHandler<SaveFileCommand, SaveFileResponse>
    {
        public async Task<SaveFileResponse> Handle(SaveFileCommand request, CancellationToken cancellationToken)
        {
            var file = request.Request.File;
            var folderPath = request.Request.Folder;

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            var baseFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "files");
            if (!string.IsNullOrEmpty(folderPath))
            {
                baseFolderPath = Path.Combine(baseFolderPath, folderPath);
            }
            
            if (!Directory.Exists(baseFolderPath))
            {
                Directory.CreateDirectory(baseFolderPath); 
            }
            
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(baseFolderPath, uniqueFileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }
            
            return new SaveFileResponse
            {
                FilePath = fullPath,
            };
        }
    }
}