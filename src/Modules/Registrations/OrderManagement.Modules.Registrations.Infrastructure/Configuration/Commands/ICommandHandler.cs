namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Commands
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }

    public interface ICommandHandler<in TCommand, TResult> :
        IRequestHandler<TCommand, TResult>
        where TCommand : OrderManagement.Modules.Registrations.Application.Contracts.ICommand<TResult>
    {
    }
}