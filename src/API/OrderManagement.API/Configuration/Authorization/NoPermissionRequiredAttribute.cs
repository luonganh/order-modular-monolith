﻿namespace OrderManagement.API.Configuration.Authorization
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class NoPermissionRequiredAttribute : Attribute
	{
	}
}
