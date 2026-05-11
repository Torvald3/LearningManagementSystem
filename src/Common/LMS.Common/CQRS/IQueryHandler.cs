using LMS.Common.Results;

namespace LMS.Common.CQRS;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery
{
    Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken = default);
}