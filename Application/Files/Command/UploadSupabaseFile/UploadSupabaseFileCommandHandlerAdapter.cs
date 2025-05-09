using Application.Files.Command.SaveFile;
using Application.Files.Common.SaveFile;
using Application.Files.Common.UploadSupabaseFile;
using MediatR;

namespace Application.Files.Command.UploadSupabaseFile
{
    public class UploadSupabaseFileCommandHandlerAdapter : IRequestHandler<SaveFileCommand, SaveFileResponse>
    {
        private readonly UploadSupabaseFileCommandHandler _supabaseHandler;

        public UploadSupabaseFileCommandHandlerAdapter(UploadSupabaseFileCommandHandler supabaseHandler)
        {
            _supabaseHandler = supabaseHandler;
        }

        public async Task<SaveFileResponse> Handle(SaveFileCommand request, CancellationToken cancellationToken)
        {
            var uploadRequest = new UploadSupabaseFileCommand(
                new UploadSupabaseFileRequest
                {
                    File = request.Request.File
                }
            );
            
            var response = await _supabaseHandler.Handle(uploadRequest, cancellationToken);

            return new SaveFileResponse
            {
                FilePath = response.FilePath
            };
        }
    }
}