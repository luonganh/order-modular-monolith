namespace OrderManagement.Modules.UserAccess.Infrastructure.Configuration.EventsBus
{
    internal class EventsBusModule : Module
	{
		private readonly IEventsBus _eventsBus;

		public EventsBusModule(IEventsBus eventsBus)
		{
			_eventsBus = eventsBus;
		}

		protected override void Load(ContainerBuilder builder)
		{
			if (_eventsBus != null)
			{
				builder.RegisterInstance(_eventsBus).SingleInstance();
			}
			else
			{
				builder.RegisterType<InMemoryEventBusClient>()
					.As<IEventsBus>()
					.SingleInstance();
			}
		}
	}
}