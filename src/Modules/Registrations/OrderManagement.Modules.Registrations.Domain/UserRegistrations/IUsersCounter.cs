﻿namespace OrderManagement.Modules.Registrations.Domain.UserRegistrations
{
    public interface IUsersCounter
    {
        int CountUsersWithLogin(string login);
    }
}