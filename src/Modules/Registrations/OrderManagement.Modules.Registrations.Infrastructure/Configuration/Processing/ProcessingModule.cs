namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Processing
{
	// Define the ProcessingModule class, which inherits from the Autofac 'Module' class.
	// This module is responsible for registering components related to processing in the Autofac container.
	// The 'Load' method is overridden to configure the registrations during the container setup.
	internal class ProcessingModule : Module
	{
        protected override void Load(ContainerBuilder builder)
        {
			// Register the 'DomainEventsDispatcher' type as an implementation of 'IDomainEventsDispatcher',
			// with an 'InstancePerLifetimeScope' lifetime, ensuring a single instance is used within 
			// the same lifetime scope (e.g., for the duration of a request or a unit of work).
			builder.RegisterType<DomainEventsDispatcher>()
                .As<IDomainEventsDispatcher>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DomainNotificationsMapper>()
                .As<IDomainNotificationsMapper>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DomainEventsAccessor>()
                .As<IDomainEventsAccessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommandsScheduler>()
                .As<ICommandsScheduler>()
                .InstancePerLifetimeScope();
			
			// Register a generic decorator 'UnitOfWorkCommandHandlerDecorator' for the 'ICommandHandler' interface,
			// ensuring that any ICommandHandler<> is wrapped by the decorator. This allows for additional behavior 
			// (such as unit of work) to be applied to command handlers during execution.
			builder.RegisterGenericDecorator(
                typeof(UnitOfWorkCommandHandlerDecorator<>),
                typeof(ICommandHandler<>));
			
			builder.RegisterGenericDecorator(
                typeof(UnitOfWorkCommandHandlerWithResultDecorator<,>),
                typeof(ICommandHandler<,>));
           
            builder.RegisterGenericDecorator(
                typeof(ValidationCommandHandlerDecorator<>),
                typeof(ICommandHandler<>));

            builder.RegisterGenericDecorator(
                typeof(ValidationCommandHandlerWithResultDecorator<,>),
                typeof(ICommandHandler<,>));

            builder.RegisterGenericDecorator(
                typeof(LoggingCommandHandlerDecorator<>),
                typeof(IRequestHandler<>));

            builder.RegisterGenericDecorator(
                typeof(LoggingCommandHandlerWithResultDecorator<,>),
                typeof(IRequestHandler<,>));

            builder.RegisterGenericDecorator(
                typeof(DomainEventsDispatcherNotificationHandlerDecorator<>),
                typeof(INotificationHandler<>));

			// Register all types from the 'Assemblies.Application' assembly that are closed types of 'IDomainEventNotification<>',
			// meaning types that implement 'IDomainEventNotification' with specific type arguments. 
			// These types will be registered with a per-dependency lifetime, meaning a new instance will be created 
			// each time they are requested. Additionally, all constructors for these types will be discovered 
			// using the 'AllConstructorFinder' to support dependency injection.
			builder.RegisterAssemblyTypes(Assemblies.Application)
                .AsClosedTypesOf(typeof(IDomainEventNotification<>))
                .InstancePerDependency()
                .FindConstructorsWith(new AllConstructorFinder());

			//// 🔴 Debug: Kiểm tra danh sách đăng ký trước khi Build()
			//builder.ComponentRegistryBuilder.Registered += (sender, args) =>
			//{
			//	Console.WriteLine($" (1) Autofac Registered: {args.ComponentRegistration.Activator.LimitType.Name}");
			//	Console.WriteLine($" (2) Autofac Registered: {args.ComponentRegistration.Activator.LimitType.FullName}");
			//	Console.WriteLine($"-----------------------------------------------------------------------------------");
   //             Console.WriteLine();
			//};
		}
    }
}