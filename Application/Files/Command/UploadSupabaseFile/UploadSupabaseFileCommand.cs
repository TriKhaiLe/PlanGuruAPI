using Application.Files.Common.UploadSupabaseFile;
using MediatR;

namespace Application.Files.Command.UploadSupabaseFile
{
    public class UploadSupabaseFileCommand(UploadSupabaseFileRequest request) : IRequest<UploadSupabaseFileResponse>
    {
        public UploadSupabaseFileRequest Request { get; set; } = request;
    }
}