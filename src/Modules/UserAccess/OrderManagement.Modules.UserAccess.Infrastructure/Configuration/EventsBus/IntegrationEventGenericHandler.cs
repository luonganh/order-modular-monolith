﻿namespace OrderManagement.Modules.UserAccess.Infrastructure.Configuration.EventsBus
{
    internal class IntegrationEventGenericHandler<T> : IIntegrationEventHandler<T>
        where T : IntegrationEvent
    {
        public async Task Handle(T @event)
        {
			using (var scope = UserAccessCompositionRoot.BeginLifetimeScope())
			{
				using (var connection = scope.Resolve<ISqlConnectionFactory>().GetOpenConnection())
				{
					string type = @event.GetType().FullName;
					var data = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
					{
						ContractResolver = new AllPropertiesContractResolver()
					});

					var sql = "INSERT INTO [users].[InboxMessages] (Id, OccurredOn, Type, Data) " +
							  "VALUES (@Id, @OccurredOn, @Type, @Data)";

					await connection.ExecuteScalarAsync(sql, new
					{
						@event.Id,
						@event.OccurredOn,
						type,
						data
					});
				}
			}
		}
    }
}