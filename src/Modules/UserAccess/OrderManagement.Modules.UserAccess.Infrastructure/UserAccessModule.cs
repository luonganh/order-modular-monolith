﻿namespace OrderManagement.Modules.UserAccess.Infrastructure
{
    public class UserAccessModule : IUserAccessModule
    {
        public async Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command)
        {
            return await CommandsExecutor.Execute(command);
        }

        public async Task ExecuteCommandAsync(ICommand command)
        {
            await CommandsExecutor.Execute(command);
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
			using (var scope = UserAccessCompositionRoot.BeginLifetimeScope())
			{
				var mediator = scope.Resolve<IMediator>();

				return await mediator.Send(query);
			}
		}
    }
}