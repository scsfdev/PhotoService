using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoService.Application.Interfaces
{
    public interface IRabbitMqService
    {
        Task<bool> ValidateCategoryAsync(Guid categoryGuid, CancellationToken cancellationToken = default);
    }
}
