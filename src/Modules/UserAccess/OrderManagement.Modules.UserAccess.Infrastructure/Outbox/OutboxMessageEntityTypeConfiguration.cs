namespace OrderManagement.Modules.UserAccess.Infrastructure.Outbox
{
	internal class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
	{
		public void Configure(EntityTypeBuilder<OutboxMessage> builder)
		{
			builder.ToTable("OutboxMessages", "users");

			builder.HasKey(b => b.Id);
			builder.Property(b => b.Id).ValueGeneratedNever();
		}
	}
}
