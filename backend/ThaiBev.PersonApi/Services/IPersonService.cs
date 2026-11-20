using ThaiBev.PersonApi.Dtos;

namespace ThaiBev.PersonApi.Services;

public interface IPersonService
{
    Task<IReadOnlyList<PersonListItem>> GetAllAsync(CancellationToken ct = default);
    Task<PersonDetail?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PersonDetail> CreateAsync(Dtos.CreatePersonRequest request, CancellationToken ct = default);
}
