namespace OrderManagement.Modules.Registrations.Infrastructure
{
    public class RegistrationsContext : DbContext, IDomainDbContext
	{
        public DbSet<UserRegistration> UserRegistrations { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<InternalCommand> InternalCommands { get; set; }

        private readonly ILoggerFactory _loggerFactory;

        public RegistrationsContext(DbContextOptions<RegistrationsContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserRegistrationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InternalCommandEntityTypeConfiguration());
        }
    }
}