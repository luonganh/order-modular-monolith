namespace OrderManagement.API.Modules.UserAccess
{
    [Route("api/userAccess/authenticatedUser")]
    [ApiController]
    public class AuthenticatedUserController : ControllerBase
    {
        private readonly IUserAccessModule _userAccessModule;

        public AuthenticatedUserController(IUserAccessModule userAccessModule)
        {
            _userAccessModule = userAccessModule;
        }

        [NoPermissionRequired]
        [HttpGet("")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            var user = await _userAccessModule.ExecuteQueryAsync(new GetAuthenticatedUserQuery());

            return Ok(user);
        }        
    }
}