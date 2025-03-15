namespace OrderManagement.Modules.UserAccess.Infrastructure.Configuration.Commands
{
	public interface ICommandsScheduler
	{
		Task EnqueueAsync(ICommand command);

		Task EnqueueAsync<T>(ICommand<T> command);
	}
}
