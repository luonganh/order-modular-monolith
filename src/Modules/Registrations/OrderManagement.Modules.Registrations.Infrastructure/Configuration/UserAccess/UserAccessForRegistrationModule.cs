namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.UserAccess
{
	public class UserAccessAutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<UserAccessModule>()
				.As<IUserAccessModule>()
				.InstancePerLifetimeScope();
		}
	}
}