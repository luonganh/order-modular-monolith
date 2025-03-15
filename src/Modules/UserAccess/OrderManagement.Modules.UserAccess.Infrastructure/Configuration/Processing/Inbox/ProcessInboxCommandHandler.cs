namespace OrderManagement.Modules.UserAccess.Infrastructure.Configuration.Processing.Inbox
{
    internal class ProcessInboxCommandHandler : ICommandHandler<ProcessInboxCommand>
    {
        private readonly IMediator _mediator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ProcessInboxCommandHandler(IMediator mediator, ISqlConnectionFactory sqlConnectionFactory)
        {
            _mediator = mediator;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(ProcessInboxCommand command, CancellationToken cancellationToken)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var schemaExists = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sys.schemas WHERE name = 'users'");
            Console.WriteLine(schemaExists > 0 ? "ProcessInboxCommand users exists!" : "ProcessInboxCommand users NOT found!");
            const string sql = $"""
                               SELECT 
                                  [InboxMessage].[Id] AS [{nameof(InboxMessageDto.Id)}], 
                                  [InboxMessage].[Type] AS [{nameof(InboxMessageDto.Type)}], 
                                  [InboxMessage].[Data] AS [{nameof(InboxMessageDto.Data)}] 
                               FROM [users].[InboxMessages] AS [InboxMessage] 
                               WHERE [InboxMessage].[ProcessedDate] IS NULL 
                               ORDER BY [InboxMessage].[OccurredOn]
                               """;

            var messages = await connection.QueryAsync<InboxMessageDto>(sql);

            const string sqlUpdateProcessedDate = """
                                                  UPDATE [users].[InboxMessages] 
                                                  SET [ProcessedDate] = @Date 
                                                  WHERE [Id] = @Id
                                                  """;

            foreach (var message in messages)
            {
                var messageAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(assembly => message.Type.Contains(assembly.GetName().Name));

                Type type = messageAssembly.GetType(message.Type);
                if (type == null)
                {
					continue; // Bỏ qua nếu không tìm thấy kiểu
				}

				// Dùng System.Text.Json nếu không cần hỗ trợ Type động
				object request = null;
				try
				{
					request = System.Text.Json.JsonSerializer.Deserialize(message.Data, type);
				}
				catch
				{
					// Nếu lỗi, fallback sang Newtonsoft.Json
					request = JsonConvert.DeserializeObject(message.Data, type);
				}

				//var request = JsonSerializer.DeserializeObject(message.Data, type);

                try
                {
                    await _mediator.Publish((INotification?)request, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                await connection.ExecuteAsync(sqlUpdateProcessedDate, new
                {
                    Date = DateTime.UtcNow,
                    message.Id
                });
            }
        }
    }
}