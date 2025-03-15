using OrderManagement.Modules.Registrations.Infrastructure;

namespace OrderManagement.API.Modules.UserAccess
{
    public class UserAccessAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAccessModule>()
                .As<IUserAccessModule>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RegistrationsModule>()
                .As<IRegistrationsModule>()
                .InstancePerLifetimeScope();
        }
    }
}