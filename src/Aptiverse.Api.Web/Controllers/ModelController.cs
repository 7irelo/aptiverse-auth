using Aptiverse.Application.AI.Dtos;
using Aptiverse.Application.AI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Api.Web.Controllers
{
    [Route("api/models")]
    [ApiController]
    public class ModelController(IModelTaskService aiTaskService) : ControllerBase
    {
        private readonly IModelTaskService _aiTaskService = aiTaskService;
        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] string inputText)
        {
            var task = new ModelTaskPayloadDto
            {
                TaskType = "summarization",
                InputText = inputText,
                UserId = User.Identity?.Name ?? "anonymous"
            };

            await _aiTaskService.SendTaskToQueueAsync(task);
            return Accepted(new { message = "Task queued" });
        }
    }
}
