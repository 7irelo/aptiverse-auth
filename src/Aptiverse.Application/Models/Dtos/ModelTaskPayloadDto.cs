namespace Aptiverse.Application.AI.Dtos
{
    public class ModelTaskPayloadDto
    {
        public string TaskId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
        public string InputText { get; set; } = string.Empty;
    }
}
