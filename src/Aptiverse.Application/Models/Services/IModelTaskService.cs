using Aptiverse.Application.AI.Dtos;

namespace Aptiverse.Application.AI.Services
{
    public interface IModelTaskService
    {
        Task SendTaskToQueueAsync(ModelTaskPayloadDto taskPayload);
    }
}
