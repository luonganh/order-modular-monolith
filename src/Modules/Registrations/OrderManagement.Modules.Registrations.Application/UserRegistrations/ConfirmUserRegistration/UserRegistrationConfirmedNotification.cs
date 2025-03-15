namespace OrderManagement.Modules.Registrations.Application.UserRegistrations.ConfirmUserRegistration;

public class UserRegistrationConfirmedNotification : DomainNotificationBase<UserRegistrationConfirmedDomainEvent>
{
    public UserRegistrationConfirmedNotification(UserRegistrationConfirmedDomainEvent domainEvent, Guid id)
        : base(domainEvent, id)
    {
    }
}