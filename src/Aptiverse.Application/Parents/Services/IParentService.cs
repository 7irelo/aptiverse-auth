using Aptiverse.Application.Parents.Dtos;

namespace Aptiverse.Core.Services.Services
{
    public interface IParentService
    {
        Task<ParentDto> CreateParentAsync(ParentDto parentDto);
        Task<ParentDto> GetOneParentAsync(long id);
        Task<IEnumerable<ParentDto>> GetManyParentAsync(string filter);
        Task<ParentDto> UpdateParentAsync(long id, ParentDto parentDto);
        Task DeleteParentAsync(long id);
    }
}
