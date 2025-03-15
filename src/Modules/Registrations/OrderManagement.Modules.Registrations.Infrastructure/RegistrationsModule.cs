namespace OrderManagement.Modules.Registrations.Infrastructure
{
    public class RegistrationsModule : IRegistrationsModule
    {
        public async Task<TResult> ExecuteCommandAsync<TResult>(Application.Contracts.ICommand<TResult> command)
        {
            return await CommandsExecutor.Execute(command);
        }

        public async Task ExecuteCommandAsync(ICommand command)
        {
            await CommandsExecutor.Execute(command);
        }

		public async Task<TResult> ExecuteQueryAsync<TResult>(Application.Contracts.IQuery<TResult> query)
		{
			using (var scope = RegistrationsCompositionRoot.BeginLifetimeScope())
			{
				var mediator = scope.Resolve<IMediator>();

				return await mediator.Send(query);
			}
		}		
    }
}