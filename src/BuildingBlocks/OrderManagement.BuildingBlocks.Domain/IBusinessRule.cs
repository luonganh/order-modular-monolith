namespace OrderManagement.BuildingBlocks.Domain
{
	public interface IBusinessRule
    {
		bool IsBroken();
		string Message { get; }
	}
}
