namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Processing
{
    internal class LoggingCommandHandlerWithResultDecorator<T, TResult> : ICommandHandler<T, TResult>
        where T : Application.Contracts.ICommand<TResult>
    {
        private readonly Serilog.ILogger _logger;
        private readonly IExecutionContextAccessor _executionContextAccessor;
        private readonly ICommandHandler<T, TResult> _decorated;

        public LoggingCommandHandlerWithResultDecorator(
			Serilog.ILogger logger,
            IExecutionContextAccessor executionContextAccessor,
            ICommandHandler<T, TResult> decorated)
        {
            _logger = logger;
            _executionContextAccessor = executionContextAccessor;
            _decorated = decorated;             
		}

        public async Task<TResult> Handle(T command, CancellationToken cancellationToken)
        {			
			if (command is IRecurringCommand)
            {
                return await _decorated.Handle(command, cancellationToken);
            }

            using (
                Serilog.Context.LogContext.Push(
                    new RequestLogEnricher(_executionContextAccessor),
                    new CommandLogEnricher(command)))
            {
                try
                {
                    this._logger.Information(
                        "Executing command {@Command}",
                        command);

                    var result = await _decorated.Handle(command, cancellationToken);

                    this._logger.Information("Command processed successful, result {Result}", result);

                    return result;
                }
                catch (Exception exception)
                {
                    this._logger.Error(exception, "Command processing failed");
                    throw;
                }
            }
        }

        private class CommandLogEnricher : ILogEventEnricher
        {
            private readonly Application.Contracts.ICommand<TResult> _command;

            public CommandLogEnricher(Application.Contracts.ICommand<TResult> command)
            {
                _command = command;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty(
                    "Context",
                    new ScalarValue($"Command:{_command.Id.ToString()}")));
            }
        }

        private class RequestLogEnricher : ILogEventEnricher
        {
            private readonly IExecutionContextAccessor _executionContextAccessor;

            public RequestLogEnricher(IExecutionContextAccessor executionContextAccessor)
            {
                _executionContextAccessor = executionContextAccessor;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                if (_executionContextAccessor.IsAvailable)
                {
                    logEvent.AddOrUpdateProperty(new LogEventProperty(
                        "CorrelationId",
                        new ScalarValue(_executionContextAccessor.CorrelationId)));
                }
            }
        }
    }
}