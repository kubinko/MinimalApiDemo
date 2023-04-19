using MediatR;
using Microsoft.Extensions.Options;
using MinimalApi.Common.Options;

namespace MinimalApi.Queries
{
    public class WorkshopInfoQueryHandler : IRequestHandler<WorkshopInfoQuery, IResult>
    {
        private readonly WorkshopSettings _settings;

        public WorkshopInfoQueryHandler(IOptions<WorkshopSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task<IResult> Handle(WorkshopInfoQuery request, CancellationToken cancellationToken)
            => Task.FromResult(Results.Ok(_settings));
    }
}
