namespace OrderManagement.Modules.Registrations.Application.Contracts
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}