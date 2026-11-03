namespace Edible.Application.Abstractions;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}