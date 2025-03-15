namespace OrderManagement.Modules.UserAccess.Application.Users.GetAuthenticatedUser
{
    internal class GetAuthenticatedUserQueryHandler : IQueryHandler<GetAuthenticatedUserQuery, GetUser.UserDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        private readonly IExecutionContextAccessor _executionContextAccessor;

        public GetAuthenticatedUserQueryHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            IExecutionContextAccessor executionContextAccessor)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _executionContextAccessor = executionContextAccessor;
        }

        public async Task<GetUser.UserDto> Handle(GetAuthenticatedUserQuery request, CancellationToken cancellationToken)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();

            const string sql = $"""
                                SELECT 
                                    [User].[Id] as [{nameof(GetUser.UserDto.Id)}], 
                                    [User].[IsActive] as [{nameof(GetUser.UserDto.IsActive)}],
                                    [User].[Login] as [{nameof(GetUser.UserDto.Login)}],
                                    [User].[Email] as [{nameof(GetUser.UserDto.Email)}],
                                    [User].[Name] as [{nameof(GetUser.UserDto.Name)}]
                                FROM [users].[v_Users] AS [User]
                                WHERE [User].[Id] = @UserId
                                """;

            return await connection.QuerySingleAsync<GetUser.UserDto>(sql, new
            {
                _executionContextAccessor.UserId
            });
        }
    }
}