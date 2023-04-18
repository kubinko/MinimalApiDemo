using MediatR;

namespace MinimalApi.Queries
{
    public record InvoiceQuery(long AttendeeId, string? Code) : IRequest<IResult>;
}
