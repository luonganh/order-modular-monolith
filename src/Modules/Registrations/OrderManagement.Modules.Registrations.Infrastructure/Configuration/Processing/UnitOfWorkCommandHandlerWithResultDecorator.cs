namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Processing
{
    internal class UnitOfWorkCommandHandlerWithResultDecorator<T, TResult> : ICommandHandler<T, TResult>
        where T : Application.Contracts.ICommand<TResult>
    {
        private readonly ICommandHandler<T, TResult> _decorated;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RegistrationsContext _registrationsContext;

        public UnitOfWorkCommandHandlerWithResultDecorator(
            ICommandHandler<T, TResult> decorated,
            IUnitOfWork unitOfWork,
            RegistrationsContext registrationsContext)
        {
            _decorated = decorated;
            _unitOfWork = unitOfWork;
            _registrationsContext = registrationsContext;
		}

        public async Task<TResult> Handle(T command, CancellationToken cancellationToken)
        {			
			var result = await this._decorated.Handle(command, cancellationToken);

            if (command is InternalCommandBase<TResult>)
            {
                var internalCommand = await _registrationsContext.InternalCommands.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

                if (internalCommand != null)
                {
                    internalCommand.ProcessedDate = DateTime.UtcNow;
                }
            }
			
			await this._unitOfWork.CommitAsync(cancellationToken);

            return result;
        }
    }
}