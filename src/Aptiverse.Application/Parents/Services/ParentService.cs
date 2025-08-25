using Aptiverse.Application.Parents.Dtos;

namespace Aptiverse.Application.Parents.Services
{
    public class ParentService : IParentService
    {
        public Task<ParentDto> CreateParentAsync(ParentDto parentDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteParentAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ParentDto>> GetManyParentAsync(string filter)
        {
            throw new NotImplementedException();
        }

        public Task<ParentDto> GetOneParentAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<ParentDto> UpdateParentAsync(long id, ParentDto parentDto)
        {
            throw new NotImplementedException();
        }
    }
}
