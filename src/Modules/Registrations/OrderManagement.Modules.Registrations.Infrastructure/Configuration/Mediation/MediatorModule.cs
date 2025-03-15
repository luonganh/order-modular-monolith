namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Mediation
{
	// Define the MediatorModule class, which inherits from the Autofac 'Module' class.
	// This class is responsible for registering components related to the Mediator pattern 
	// in the Autofac container. The 'Load' method is overridden to perform the actual registration.	
	public class MediatorModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			// Register the 'ServiceProviderWrapper' type as an implementation of 'IServiceProvider',
			// ensuring a new instance is created each time it is requested (InstancePerDependency).
			// This registration is conditional and will only occur if 'IServiceProvider' has not already been registered.
			builder.RegisterType<ServiceProviderWrapper>()
			.As<IServiceProvider>()
			.InstancePerDependency()
			.IfNotRegistered(typeof(IServiceProvider));

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			// Register all types in the assembly containing IMediator as their implemented interfaces,
			// and configure them to be created with a new instance for each lifetime scope (e.g., per HTTP request or other scope).
			//builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
			builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
			
			//builder.RegisterAssemblyTypes(assemblies)
			//	   .AsClosedTypesOf(typeof(ICommandHandler<>))
			//	   .AsImplementedInterfaces();

			//builder.RegisterAssemblyTypes(assemblies)
			//	   .AsClosedTypesOf(typeof(ICommandHandler<,>))
			//	   .AsImplementedInterfaces();

			// Define an array of open generic types related to MediatR and validation, including request handlers, 
			// notification handlers, validators, processors, exception handlers, and command handlers.
			// These types are used for registering different handler interfaces for various MediatR patterns.
			var mediatorOpenTypes = new[]
			{
				typeof(IRequestHandler<,>),
				typeof(INotificationHandler<>),
				typeof(IValidator<>),
				typeof(IRequestPreProcessor<>),
				typeof(IRequestHandler<>),
				typeof(IStreamRequestHandler<,>),
				typeof(IRequestPostProcessor<,>),
				typeof(IRequestExceptionHandler<,,>),
				typeof(IRequestExceptionAction<,>),
				typeof(ICommandHandler<>),
				typeof(ICommandHandler<,>),
			};

			// Register a custom source for resolving open generic types with contravariant support 
			// for the specified MediatR-related types, ensuring that the correct implementations 
			// are used for handlers, validators, and processors based on their dependencies.
			builder.RegisterSource(new ScopedContravariantRegistrationSource(
				mediatorOpenTypes));

			// Iterate through the defined open generic types (e.g., IRequestHandler, INotificationHandler, etc.)
			// and register all matching closed types in the specified assemblies (Application and ThisAssembly).
			// The types are registered as their implemented interfaces, and constructors are found using the AllConstructorFinder,
			// ensuring proper dependency injection for each type.
			foreach (var mediatorOpenType in mediatorOpenTypes)
			{
				builder
					.RegisterAssemblyTypes(Assemblies.Application, ThisAssembly)
					.AsClosedTypesOf(mediatorOpenType)
					.AsImplementedInterfaces()
					.FindConstructorsWith(new AllConstructorFinder());

				//builder
				//.RegisterAssemblyTypes(Assemblies.Application, ThisAssembly)
				//.AsClosedTypesOf(mediatorOpenType)
				//.AsImplementedInterfaces()
				//.FindConstructorsWith(new AllConstructorFinder())
				//.OnActivated(e =>
				//{
				//	var implementedInterfaces = e.Instance.GetType().GetInterfaces();
				//	var commandHandlerInterface = implementedInterfaces
				//		.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

				//	if (commandHandlerInterface != null)
				//	{
				//		var genericArgs = commandHandlerInterface.GetGenericArguments();
				//		var iRequestHandlerType = typeof(IRequestHandler<,>).MakeGenericType(genericArgs);

				//		var context = e.Context.Resolve<ILifetimeScope>();

				//		// Đảm bảo kiểu này không bị lỗi khi resolve
				//		context.ResolveOptional(iRequestHandlerType);

				//		// Thay thế TryAddRegistration bằng RegisterInstance
				//		builder.RegisterInstance(e.Instance)
				//			.As(iRequestHandlerType)
				//			.InstancePerLifetimeScope();
				//	}

				//	//if (typeof(ICommandHandler<,>).IsAssignableFrom(e.Instance.GetType()))
				//	//{
				//	//	var genericArgs = e.Instance.GetType().GetInterfaces()
				//	//		.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
				//	//		.GetGenericArguments();

				//	//	var iRequestHandlerType = typeof(IRequestHandler<,>).MakeGenericType(genericArgs);
				//	//	e.Context.ComponentRegistry.Register(
				//	//		RegistrationBuilder.ForDelegate((c, p) => e.Instance)
				//	//			.As(iRequestHandlerType)
				//	//			.InstancePerLifetimeScope()
				//	//			.CreateRegistration());
				//	//}
				//});

				//// 💡 Nếu là ICommandHandler<> thì đăng ký thêm như IRequestHandler<>
				//builder
				//	.RegisterAssemblyTypes(Assemblies.Application, ThisAssembly)
				//	.AsClosedTypesOf(mediatorOpenType)
				//	.As(t =>
				//	{
				//		var interfaces = new List<Type>();

				//		var commandHandlerInterface = t.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
				//		if (commandHandlerInterface != null)
				//		{
				//			var genericArgs = commandHandlerInterface.GetGenericArguments();
				//			interfaces.Add(typeof(IRequestHandler<>).MakeGenericType(genericArgs));
				//		}

				//		var commandHandlerWithResultInterface = t.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));
				//		if (commandHandlerWithResultInterface != null)
				//		{
				//			var genericArgs = commandHandlerWithResultInterface.GetGenericArguments();
				//			interfaces.Add(typeof(IRequestHandler<,>).MakeGenericType(genericArgs));
				//		}

				//		return interfaces.ToArray();
				//	})

				//	.AsImplementedInterfaces()
				//	.FindConstructorsWith(new AllConstructorFinder());
			}

			//		builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
			//.AsClosedTypesOf(typeof(ICommandHandler<,>)) // Đăng ký ICommandHandler<,>
			//.AsImplementedInterfaces();

			//builder.RegisterType<Mediator>()
			//.As<IMediator>()
			//.InstancePerLifetimeScope();

			//		builder.RegisterAssemblyTypes(typeof(RegisterNewUserCommandHandler).Assembly)
			//.AsClosedTypesOf(typeof(ICommandHandler<>))
			//.OnRegistered(e => Console.WriteLine($"📌 Đã đăng ký handler: {e.ComponentRegistration.Activator.LimitType.FullName}"))
			//.InstancePerLifetimeScope();

			////builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
			////builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly) // assembly của thư viện MediatR
			//builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()) // Register all handlers in the Application module
			//.AsClosedTypesOf(typeof(ICommandHandler<>))       // Find classes implementing ICommandHandler<T>
			//.As(typeof(IRequestHandler<>))                    // Map them to IRequestHandler<T> for MediatR recognition
			//.AsImplementedInterfaces()                        // Ensure all relevant interfaces are registered
			//.InstancePerLifetimeScope();

			//builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()) // Register all handlers in the Application module
			//.AsClosedTypesOf(typeof(ICommandHandler<,>))       // Find classes implementing ICommandHandler<T>
			//.As(typeof(IRequestHandler<,>))                    // Map them to IRequestHandler<T> for MediatR recognition
			//.AsImplementedInterfaces()                        // Ensure all relevant interfaces are registered
			//.InstancePerLifetimeScope();

			//// 👉 Thêm ánh xạ ICommandHandler<,> -> IRequestHandler<,> ngay sau khi đã đăng ký ICommandHandler<,>
			//builder.RegisterGeneric(typeof(ICommandHandler<,>))
			//	.As(typeof(IRequestHandler<,>))
			//	.InstancePerLifetimeScope();

			// Register the generic RequestPostProcessorBehavior and RequestPreProcessorBehavior as implementations 
			// of the IPipelineBehavior interface, allowing them to participate in the MediatR pipeline 
			// for request processing and pre-processing behaviors.
			builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
			builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

			//// 🔴 Debug: Kiểm tra danh sách đăng ký trước khi Build()
			//builder.ComponentRegistryBuilder.Registered += (sender, args) =>
			//{
			//	Console.WriteLine($"Mediator Registered: {args.ComponentRegistration.Activator.LimitType.FullName}");
			//};
		}

		// Define a custom registration source that implements IRegistrationSource,
		// which allows for contravariant registration of types. This class is used 
		// to handle the registration of types with a contravariant relationship
		// (i.e., types that allow for type substitution in a way that can resolve 
		// dependencies for more general types).
		private class ScopedContravariantRegistrationSource : IRegistrationSource
		{
			// Create a private readonly instance of ContravariantRegistrationSource, 
			// which will be used for handling contravariant registrations. This instance 
			// is likely used within the class to register and resolve types with contravariant dependencies.
			private readonly ContravariantRegistrationSource _source = new();

			// Create a private readonly list to store types. This list is likely used to hold 
			// a collection of types that will be registered or resolved within the class.
			private readonly List<Type> _types = new();

			public ScopedContravariantRegistrationSource(params Type[] types)
			{
				// Throw an ArgumentNullException if the 'types' argument is null, ensuring that 
				// the method receives a valid non-null collection of types. This helps prevent 
				// null reference errors and ensures correct behavior of the method.
				ArgumentNullException.ThrowIfNull(types);

				// Check if all types in the 'types' collection are generic type definitions. 
				// If any type is not a generic type definition, throw an ArgumentException 
				// with a message indicating that only generic type definitions are allowed.
				if (!types.All(x => x.IsGenericTypeDefinition))
				{
					throw new ArgumentException("Supplied types should be generic type definitions");
				}

				// Add the provided types to the '_types' list. This allows the collection to 
				// grow by including the types passed into the method, assuming they have been validated.
				_types.AddRange(types);
			}

			// Define a method that returns a collection of IComponentRegistration objects for a given service.
			// The method takes a 'service' parameter and a 'registrationAccessor' function that is used to access 
			// the registrations for the specified service. This method is likely used for resolving or retrieving 
			// component registrations based on the service type.
			public IEnumerable<IComponentRegistration> RegistrationsFor(
				Service service,
				Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
			{
				// Retrieve the component registrations for the specified service using the '_source'.
				// The 'registrationAccessor' function is used to access the relevant service registrations, 
				// and the result is stored in the 'components' variable for further processing.
				var components = _source.RegistrationsFor(service, registrationAccessor);

				foreach (var c in components)
				{
					// Filter the services of the target component 'c' to get only those of type 'TypedService'.
					// Then, for each service, retrieve the generic type definition (the open generic type) 
					// by calling 'GetGenericTypeDefinition' on the 'ServiceType' of each 'TypedService'.
					// The result is stored in 'defs', which contains the open generic types of the services.
					var defs = c.Target.Services
						.OfType<TypedService>()
						.Select(x => x.ServiceType.GetGenericTypeDefinition());

					// Check if any of the generic type definitions in 'defs' are contained in the '_types' list.
					// If there is a match, yield return the current component 'c', meaning it satisfies the condition 
					// and will be included in the result.
					if (defs.Any(_types.Contains))
					{
						yield return c;
					}
				}
			}

			// Property that returns the value of 'IsAdapterForIndividualComponents' from the '_source' object.
			// This indicates whether the source is an adapter for individual components, 
			// based on the underlying value in the '_source' instance.
			public bool IsAdapterForIndividualComponents => _source.IsAdapterForIndividualComponents;
		}
	}
}