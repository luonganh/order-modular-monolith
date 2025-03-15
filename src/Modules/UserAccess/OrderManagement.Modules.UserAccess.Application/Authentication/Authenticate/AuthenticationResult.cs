﻿namespace OrderManagement.Modules.UserAccess.Application.Authentication.Authenticate
{
    public class AuthenticationResult
    {
        public AuthenticationResult(string authenticationError)
        {
            IsAuthenticated = false;
            AuthenticationError = authenticationError;
        }

        public AuthenticationResult(UserDto user)
        {
			IsAuthenticated = true;
			User = user;
        }

        public bool IsAuthenticated { get; }

        public string AuthenticationError { get; }

        public UserDto User { get; }
    }
}