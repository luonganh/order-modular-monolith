namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration
{
	public class RegistrationsStartup
	{
		private static IContainer _container;

		public static void Initialize(
			string connectionString,
			IExecutionContextAccessor executionContextAccessor,
			ILogger logger,
			EmailsConfiguration emailsConfiguration,
			string textEncryptionKey,
			IEmailSender emailSender,
			IEventsBus eventsBus,
			long? internalProcessingPoolingInterval = null)
		{
			var moduleLogger = logger.ForContext("Module", "Registrations");

			ConfigureCompositionRoot(
				connectionString,
				executionContextAccessor,
				logger,
				emailsConfiguration,
				textEncryptionKey,
				emailSender,
				eventsBus);

			QuartzStartup.Initialize(moduleLogger, internalProcessingPoolingInterval);

			EventsBusStartup.Initialize(moduleLogger);
		}

		private static void ConfigureCompositionRoot(
			string connectionString,
			IExecutionContextAccessor executionContextAccessor,
			ILogger logger,
			EmailsConfiguration emailsConfiguration,
			string textEncryptionKey,
			IEmailSender emailSender,
			IEventsBus eventsBus)
		{
			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterModule(new LoggingModule(logger.ForContext("Module", "Registrations")));

			var loggerFactory = new Serilog.Extensions.Logging.SerilogLoggerFactory(logger);
			containerBuilder.RegisterModule(new DataAccessModule(connectionString, loggerFactory));
			containerBuilder.RegisterModule(new ProcessingModule());		

			containerBuilder.RegisterModule(new EventsBusModule(eventsBus));
			containerBuilder.RegisterModule(new MediatorModule());
			containerBuilder.RegisterModule(new UserAccessAutofacModule());

			var domainNotificationsMap = new BiDictionary<string, Type>();
			domainNotificationsMap.Add("NewUserRegisteredNotification", typeof(NewUserRegisteredNotification));
			domainNotificationsMap.Add("UserRegistrationConfirmedNotification", typeof(UserRegistrationConfirmedNotification));
			containerBuilder.RegisterModule(new OutboxModule(domainNotificationsMap));

			containerBuilder.RegisterModule(new QuartzModule());
			containerBuilder.RegisterModule(new DomainModule());
			containerBuilder.RegisterModule(new EmailModule(emailsConfiguration, emailSender));
			//// containerBuilder.RegisterModule(new SecurityModule(textEncryptionKey));

			containerBuilder.RegisterInstance(executionContextAccessor);
			
			//// 🔴 Debug: Kiểm tra danh sách đăng ký trước khi Build()
			//containerBuilder.ComponentRegistryBuilder.Registered += (sender, args) =>
			//{
			//	Console.WriteLine($"Registered: {args.ComponentRegistration.Activator.LimitType.FullName}");				
			//};

			_container = containerBuilder.Build();

			//// Kiểm tra việc đăng ký decorator bằng cách nào, viết giúp tôi ở dòng kế tiếp
			//var decorator = _container.Resolve<ICommandHandler<RegisterNewUserCommand>>(); // hoặc bạn có thể dùng một handler cụ thể
			//if (decorator is UnitOfWorkCommandHandlerWithResultDecorator<MyCommand, MyResult>)
			//{
			//	Console.WriteLine("Decorator đã được đăng ký thành công!");
			//}
			//else
			//{
			//	Console.WriteLine("Decorator chưa được đăng ký hoặc không áp dụng đúng cách.");
			//}

			//using (var scope = _container.BeginLifetimeScope())
			//{
			//	var handlers = scope.ComponentRegistry.Registrations;
			//	foreach (var handler in handlers)
			//	{
			//		Console.WriteLine($"Resolved: {handler.GetType().FullName}");
			//	}
			//}
			RegistrationsCompositionRoot.SetContainer(_container);
		}
	}
}