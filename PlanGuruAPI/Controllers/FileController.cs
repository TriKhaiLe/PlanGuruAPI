using Application.Files.Command;
using Application.Files.Command.SaveFile;
using Application.Files.Common;
using Application.Files.Common.SaveFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PlanGuruAPI.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileController(ISender mediator) : ControllerBase
    {
        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromForm] SaveFileRequest request)
        {
            var result = await mediator.Send(new SaveFileCommand(request));
            return Ok(result);
        }
    }
}