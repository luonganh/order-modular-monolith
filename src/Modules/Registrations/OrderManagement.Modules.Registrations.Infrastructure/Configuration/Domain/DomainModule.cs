namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Domain
{
	internal class DomainModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<UsersCounter>()
				.As<IUsersCounter>()
				.InstancePerLifetimeScope();

			builder.RegisterType<UserAccessGateway>()
				.As<IUserCreator>()
				.InstancePerLifetimeScope();
		}
	}
}