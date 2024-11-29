namespace PieHandlerService.Core.Interfaces;

public interface IRepositoryModifier<in T>
{
    Task Update(T item, CancellationToken cancellationToken);
    Task Add(T item, CancellationToken cancellationToken);
    Task Delete(T item, CancellationToken cancellationToken);
}
