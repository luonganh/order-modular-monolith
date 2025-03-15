namespace OrderManagement.Modules.Registrations.Infrastructure.Users;

public class UserAccessGateway : IUserCreator
{
    private readonly IUserAccessModule _userAccessModule;

    public UserAccessGateway(IUserAccessModule userAccessModule)
    {
        _userAccessModule = userAccessModule;
    }

    public async Task Create(
        Guid userRegistrationId,
        string login,
        string password,
        string email,
        string firstName,
        string lastName)
    {
        await _userAccessModule.ExecuteCommandAsync(new CreateUserCommand(
            userRegistrationId,
            login,
            email,
            firstName,
            lastName,
            password));
    }
}