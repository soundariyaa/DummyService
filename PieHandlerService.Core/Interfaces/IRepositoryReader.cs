using System.Linq.Expressions;

namespace PieHandlerService.Core.Interfaces;

public interface IRepositoryReader<T>
{
    Task<IEnumerable<T>> Fetch();
}

public interface IRepositoryReader<TIn, TOut>
{
    Task<TOut> Fetch(Expression<Func<TIn, bool>> filter);
    Task<IEnumerable<TOut>> FetchAll(Expression<Func<TIn, bool>> filter);
    Task<bool> Evaluate(Expression<Func<TIn, bool>> filter);
}

