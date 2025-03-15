namespace OrderManagement.Modules.Registrations.Infrastructure.Configuration.Processing
{
    internal class UnitOfWorkCommandHandlerDecorator<T> : ICommandHandler<T>
        where T : ICommand
    {
        private readonly ICommandHandler<T> _decorated;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RegistrationsContext _registrationsContext;

        public UnitOfWorkCommandHandlerDecorator(
            ICommandHandler<T> decorated,
            IUnitOfWork unitOfWork,
            RegistrationsContext registrationsContext)
        {
            _decorated = decorated;
            _unitOfWork = unitOfWork;
            _registrationsContext = registrationsContext;
		}

        public async Task Handle(T command, CancellationToken cancellationToken)
        {			
			await this._decorated.Handle(command, cancellationToken);

            if (command is InternalCommandBase)
            {
                var internalCommand = await _registrationsContext.InternalCommands.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

                if (internalCommand != null)
                {
                    internalCommand.ProcessedDate = DateTime.UtcNow;
                }
            }
			
			await this._unitOfWork.CommitAsync(cancellationToken);
        }
    }
}