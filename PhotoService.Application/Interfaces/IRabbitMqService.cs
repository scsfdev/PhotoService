
namespace PhotoService.Application.Interfaces
{
    public interface IRabbitMqService
    {
        Task<bool> ValidateCategoryAsync(Guid categoryGuid, CancellationToken cancellationToken = default);

        Task InitializeAsync();
    }
}
