namespace OrderManagement.Modules.UserAccess.Infrastructure.Configuration.EventsBus
{
    public static class EventsBusStartup
    {
        public static void Initialize(
            Serilog.ILogger logger)
        {
            SubscribeToIntegrationEvents(logger);
        }

        private static void SubscribeToIntegrationEvents(Serilog.ILogger logger)
        {
			var eventBus = UserAccessCompositionRoot.BeginLifetimeScope().Resolve<IEventsBus>();
			//SubscribeToIntegrationEvent<MemberCreatedIntegrationEvent>(eventBus, logger);
		}

        private static void SubscribeToIntegrationEvent<T>(IEventsBus eventBus, Serilog.ILogger logger)
            where T : IntegrationEvent
        {
            logger.Information("Subscribe to {@IntegrationEvent}", typeof(T).FullName);
            eventBus.Subscribe(
                new IntegrationEventGenericHandler<T>());
        }
    }
}