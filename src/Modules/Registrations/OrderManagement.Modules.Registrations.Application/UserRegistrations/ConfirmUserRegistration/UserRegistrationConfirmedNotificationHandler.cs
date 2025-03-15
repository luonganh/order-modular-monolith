namespace OrderManagement.Modules.Registrations.Application.UserRegistrations.ConfirmUserRegistration;

public class UserRegistrationConfirmedNotificationHandler : INotificationHandler<UserRegistrationConfirmedNotification>
{
    private readonly IUserCreator _userCreator;

    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public UserRegistrationConfirmedNotificationHandler(IUserCreator userCreator, ISqlConnectionFactory sqlConnectionFactory)
    {
        _userCreator = userCreator;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task Handle(UserRegistrationConfirmedNotification notification, CancellationToken cancellationToken)
    {
        var connection = _sqlConnectionFactory.GetOpenConnection();

        var registration = await UserRegistrationProvider.GetById(
            connection,
            notification.DomainEvent.UserRegistrationId.Value);

        await _userCreator.Create(
            registration.Id,
            registration.Login,
            registration.Password,
            registration.Email,
            registration.FirstName,
            registration.LastName);
    }
}