namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Commands
{
    public interface ICommandsScheduler
    {
        Task EnqueueAsync(ICommand command);

        Task EnqueueAsync<T>(OrderManagement.Modules.Registrations.Application.Contracts.ICommand<T> command);
    }
}