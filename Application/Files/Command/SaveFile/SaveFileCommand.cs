using Application.Files.Common.SaveFile;
using MediatR;

namespace Application.Files.Command.SaveFile
{
    public class SaveFileCommand : IRequest<SaveFileResponse>
    {
        public SaveFileRequest Request { get; set; }

        public SaveFileCommand(SaveFileRequest request)
        {
            Request = request;
        }
    }
}