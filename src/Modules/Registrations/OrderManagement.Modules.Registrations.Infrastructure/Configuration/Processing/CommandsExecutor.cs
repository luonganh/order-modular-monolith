namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Processing
{
    internal static class CommandsExecutor
    {
        internal static async Task Execute(ICommand command)
        {
			using (var scope = RegistrationsCompositionRoot.BeginLifetimeScope())
			{
				var mediator = scope.Resolve<IMediator>();
				await mediator.Send(command);
			}
		}

        internal static async Task<TResult> Execute<TResult>(Application.Contracts.ICommand<TResult> command)
        {
			using (var scope = RegistrationsCompositionRoot.BeginLifetimeScope())
			{
				var mediator = scope.Resolve<IMediator>();
				return await mediator.Send(command);
			}
		}
    }
}