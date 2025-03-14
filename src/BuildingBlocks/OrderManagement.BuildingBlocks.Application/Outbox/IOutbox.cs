namespace OrderManagement.BuildingBlocks.Application.Outbox
{
	public interface IOutbox
	{
		void Add(OutboxMessage message);

		Task Save();
	}
}
