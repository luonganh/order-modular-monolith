namespace OrderManagement.BuildingBlocks.Infrastructure.DomainEventsDispatching
{
	public interface IDomainEventsDispatcher
	{
		Task DispatchEventsAsync();
	}
}
